using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;


namespace CustomImporter
{
	/// <summary>
	/// ScriptableObject to store a set of importer rules
	/// </summary>
	[CreateAssetMenu(fileName = "New Importer Texture Settings", menuName = "CustomImporter : Texture Settings", order = 9001)]
	[System.Serializable]
	public class CISettingsTexture : CISettingsGeneric<CITextureRule>
	{
		/// <summary>
		/// Gets the first texture importer preset with valid rules
		/// Default for the importer is returned if none is valid
		/// </summary>
		/// <param name="complete_path"></param>
		/// <param name="importer"></param>
		/// <returns></returns>
		public override Preset GetPresetByRule(string complete_path, AssetImporter importer)
		{
			int path_end = complete_path.LastIndexOfAny(new[] { '/', '\\' });
			string path = complete_path.Substring(0, path_end);
			string name = complete_path.Substring(path_end);

			for(int i = 0; i < _L_rules.Count; ++i)
			{
				if(_L_rules[i].TestCondition(path, name))
				{
					return _L_rules[i].PRESET;
				}
			}

			return Preset.GetDefaultForObject(importer as TextureImporter);
		}/*GetPresetByPath*/

	}/*CISettingsTexture*/


	[CustomEditor(typeof(CISettingsTexture))]
	public class CISettingsTextureEditor : CIMenuItemsEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}/*OnInspectorGUI*/

	}/*CISettingsTextureEditor*/
}
