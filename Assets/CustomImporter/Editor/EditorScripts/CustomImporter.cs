using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;


namespace CustomImporter
{
	/// <summary>
	/// Editor script to apply importer settings.
	/// </summary>
	public class CustomImporter : AssetPostprocessor
	{
		//ScriptableObjects used to have a link between the assetpostprocessor
		//and the differents importer settings
		private CILinks _CILinks;

		//Active importers settings, if null importer will be set to Unity default
		private CISettingsTexture _CISettings_texture;
		private CISettingsModel _CISettings_model;
		private CISettingsAudio _CISettings_audio;


		//Debug on/off
		private bool _b_debug = false;

		//Strings used to add to importer userData to know
		//if it's a first import or if it derived from the preset
		/// <summary>
		/// Prefix used for userData to indicate that it uses a custom importer settings
		/// </summary>
		public const string _s_prefix = "CustomImporter used";
		/// <summary>
		/// Text added to userData if the settings has derived from original settings
		/// so we can keep it's value if we change the settings (or not if forced to refresh)
		/// </summary>
		public const string _s_differing = "Differ from preset";


		/// <summary>
		/// Function to set texture importer settings
		/// </summary>
		private void OnPreprocessTexture()
		{
			//Loading linked information
			SetLinks();
			OnPreProcessAsset(assetImporter, _CISettings_texture);
		}/*OnPreprocessTexture*/


		/// <summary>
		/// Function to set model importer settings
		/// </summary>
		private void OnPreprocessModel()
		{
			//Loading linked information
			SetLinks();
			OnPreProcessAsset(assetImporter, _CISettings_model);
		}/*OnPreprocessTexture*/


		/// <summary>
		/// Function to set audio importer settings
		/// </summary>
		private void OnPreprocessAudio()
		{
			//Loading linked information
			SetLinks();
			OnPreProcessAsset(assetImporter, _CISettings_audio);
		}/*OnPreprocessTexture*/


		private void OnPreProcessAsset<T> (AssetImporter importer, CISettingsGeneric<T> settings)
			where T : CIGenericRule
		{
			if(_b_debug)
			{
				Debug.LogFormat("CustomImporter active : {0} with type {1}",
					settings != null, typeof(T).ToString());
			}

			//If there are no settings sets we don't change anything (let the default stuff happen)
			if(settings != null)
			{
				bool usesPreset = assetImporter.userData.Contains(_s_prefix);
				bool isDiffering = assetImporter.userData.Contains(_s_differing);

				if(_b_debug)
				{
					Debug.LogFormat("Asset uses preset : {0} --- Asset differ from preset : {1}", usesPreset, isDiffering);
				}

				//If the data already uses the custom importer we'll check if it differs from the preset
				if(usesPreset)
				{
					string[] datas = assetImporter.userData.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
					string settings_used = "";

					//Looking for the settings name
					for(int i = 0; i < datas.Length - 1; ++i)
					{
						if(datas[i].Equals(_s_prefix))
						{
							settings_used = datas[i + 1];
							if (_b_debug)
							{
								Debug.LogFormat("Rule label is : {0}", settings_used);
							}
							break;
						}
					}

					Preset preset = settings.GetPresetByRuleLabel(settings_used);
					if(preset != null)
					{
						if(_b_debug)
						{
							Debug.LogFormat("Asset preset is : {0}", preset.name);
						}

						//if different, set customized_after_import
						//else remove customized_after_import if necessary
						Preset dummy = new Preset(assetImporter);
						if(dummy.Equals(preset) && isDiffering)
						{
							if(_b_debug)
							{
								Debug.Log("Removing customized flag");
							}
							assetImporter.userData.Replace(string.Format("\n{0}", _s_differing), "");
						}
						else if(!isDiffering)
						{
							if(_b_debug)
							{
								Debug.Log("Adding customized flag");
							}
							assetImporter.userData = string.Format("{0}\n{1}", assetImporter.userData, _s_differing);
						}
						//let the settings apply
					}
					else if(_b_debug)
					{
						Debug.Log("No preset found but uses CustomImporter");
					}
				}
				//If there is no settings applied we try to find a corresponding one
				else
				{
					if(_b_debug)
					{
						Debug.Log("First time import or never used a preset");
					}
					Preset preset = settings.GetPresetByRule(assetPath, assetImporter);
					//We found a preset -> apply it
					if(preset != null)
					{
						preset.ApplyTo(assetImporter);
						assetImporter.userData = string.Format("{0}\n{1}\n{2}", assetImporter.userData, _s_prefix, settings.GetRuleLabelFromPreset(preset));
						if(_b_debug)
						{
							Debug.LogFormat("Preset found : {0}", preset.name);
						}
					}
					else if(_b_debug)
					{
						Debug.Log("No preset found, don't uses CustomImporter");
					}
					//We didn't found anything, let Unity deal with it.
				}
			}
		}


		private void SetLinks()
		{
			_CILinks = AssetDatabase.LoadAssetAtPath<CILinks>("Assets/CustomImporter/CustomImporterLinks.asset");
			_CISettings_texture = _CILinks._CISettings_texture;
			_CISettings_model = _CILinks._CISettings_model;
			_CISettings_audio = _CILinks._CISettings_audio;
			_b_debug = _CILinks._b_debug;
		}/*SetLinks*/

	}/*CustomImporter*/
}
