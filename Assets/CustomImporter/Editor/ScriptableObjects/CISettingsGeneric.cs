using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;


namespace CustomImporter
{
	/// <summary>
	/// ScriptableObject to use as parent for importer settings
	/// This enables the generic class to have contextual menu items functions
	/// </summary>
	[System.Serializable]
	public abstract class CISettingsParent : ScriptableObject
	{
		/// <summary>
		/// abstract function to sort the list via mother class context
		/// </summary>
		/// <param name="mother"></param>
		public abstract void SortByPriority(CISettingsParent mother);


		/// <summary>
		/// abstract function to add a new rule via mother class context
		/// </summary>
		/// <param name="mother"></param>
		public abstract void AddRule(CISettingsParent mother);


		/// <summary>
		/// abstract function to clear all rules via mother class context
		/// </summary>
		/// <param name="mother"></param>
		public abstract void ClearRules(CISettingsParent mother);


		/// <summary>
		/// abstract function to delete a rule at "index" via mother class context
		/// </summary>
		/// <param name="mother"></param>
		/// <param name="rule"></param>
		public abstract void DeleteRule(CISettingsParent mother, int index);


		/// <summary>
		/// Function to reapply the preset to the assets linked to the rule,
		/// forcing allows to apply for all assets, non forcing applies only to unmodified assets
		/// </summary>
		/// <param name="force"></param>
		/// <param name="filter"></param>
		/// <param name="rule"></param>
		/// <param name="preset"></param>
		public static void ApplyNewPresets(bool force, string filter, string rule, Preset preset)
		{
			string[] assets = AssetDatabase.FindAssets(filter);
			for(int i = 0; i < assets.Length; ++i)
			{
				AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]));
				if(importer && !string.IsNullOrEmpty(importer.userData) && importer.userData.Contains(rule) && (force || !importer.userData.Contains(CustomImporter._s_differing)))
				{
					Debug.LogFormat("Asset reimported : {0}", importer.assetPath);
					string userdatas = importer.userData;
					preset.ApplyTo(importer);
					importer.userData = userdatas;
					importer.SaveAndReimport();
				}
			}
		}/*ApplyNewPresets*/


		/// <summary>
		/// Function to reset the rule,
		/// Works by removing all links to assets and reimporting them as if they were new assets
		/// </summary>
		/// <param name="force"></param>
		/// <param name="filter"></param>
		/// <param name="rule"></param>
		public static void ResetRule(bool force, string filter, string rule)
		{
			string[] assets = AssetDatabase.FindAssets(filter);
			for(int i = 0; i < assets.Length; ++i)
			{
				AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(assets[i]));
				if(importer && !string.IsNullOrEmpty(importer.userData) && importer.userData.Contains(rule) && (force || importer.userData.Contains(CustomImporter._s_differing)))
				{
					Debug.LogFormat("Asset reseted : {0}", importer.assetPath);
					importer.userData = importer.userData.Replace(string.Format("\n{0}\n{1}", CustomImporter._s_prefix, rule), "");
					importer.userData = importer.userData.Replace(string.Format("\n{0}", CustomImporter._s_differing), "");
					importer.SaveAndReimport();
				}
			}
		}/*UnlinkAssets*/

	}/*CISettingsParent*/


	/// <summary>
	/// ScriptableObject to store a set of importer rules
	/// </summary>
	[System.Serializable]
	public class CISettingsGeneric<T> : CISettingsParent where T : CIGenericRule
	{
		[SerializeField]
		protected List<T> _L_rules = new List<T>();
		public List<T> RULES { get { return _L_rules; } }

		public CILinks _CILinks;


		/// <summary>
		/// Gets an importer preset from a name (name of the preset file)
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual Preset GetPresetByName(string name)
		{
			CIGenericRule importer = _L_rules.Find(x => x.PRESET.name == name);
			return importer?.PRESET;
		}/*GetPresetByName*/


		/// <summary>
		/// Get an importer preset from the rule ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual Preset GetPresetByRuleID(string id)
		{
			CIGenericRule importer = _L_rules.Find(x => x.ID.ToString() == id);
			return importer?.PRESET;
		}/*GetPresetByRuleID*/


		/// <summary>
		/// Get the rule ID as a string from the preset used
		/// </summary>
		/// <param name="preset"></param>
		/// <returns></returns>
		public virtual string GetRuleIDstringFromPreset(Preset preset)
		{
			CIGenericRule importer = _L_rules.Find(x => x.PRESET == preset);
			return importer?.ID.ToString();
		}/*GetRuleIDstringFromPreset*/


		/// <summary>
		/// Gets the first importer preset with valid rules
		/// Default for the importer is returned if none is valid
		/// </summary>
		/// <param name="complete_path"></param>
		/// <param name="importer"></param>
		/// <returns></returns>
		public virtual Preset GetPresetByRule(string complete_path, AssetImporter importer)
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

			return Preset.GetDefaultForObject(importer);
		}/*GetPresetByRule*/


		/// <summary>
		/// Reorder the list using priority from high to low
		/// Using mother class to get context
		/// </summary>
		/// <param name="mother"></param>
		public override void SortByPriority(CISettingsParent mother)
		{
			CISettingsGeneric<T> settings = mother as CISettingsGeneric<T>;
			settings._L_rules.Sort(delegate (T x, T y)
			{
				return y.PRIORITY - x.PRIORITY;
			});
		}/*SortByPriority*/


		/// <summary>
		/// Add a new rule to the list, adding null because the object is recreated anyway via serialization
		/// </summary>
		/// <param name="mother"></param>
		public override void AddRule (CISettingsParent mother)
		{
			CISettingsGeneric<T> settings = mother as CISettingsGeneric<T>;
			settings._L_rules.Add(null);
		}/*AddRule*/


		/// <summary>
		/// Deleting all rules from the lists
		/// </summary>
		/// <param name="mother"></param>
		public override void ClearRules(CISettingsParent mother)
		{
			CISettingsGeneric<T> settings = mother as CISettingsGeneric<T>;
			settings._L_rules.Clear();
		}/*ClearRules*/


		/// <summary>
		/// Delete a specific rule based on it's index
		/// </summary>
		/// <param name="mother"></param>
		/// <param name="index"></param>
		public override void DeleteRule(CISettingsParent mother, int index)
		{
			CISettingsGeneric<T> settings = mother as CISettingsGeneric<T>;
			settings._L_rules.RemoveAt(index);
		}/*DeleteRule*/


		/// <summary>
		/// OnEnable we get the CILinks reference,
		/// this is just to make sure it's allocated when opening the inspector
		/// </summary>
		private void OnEnable()
		{
			_CILinks = CustomImporter.GetLinks();
		}/*OnEnable*/

	}/*CISettingsGeneric*/


	/// <summary>
	/// Custom Inspector to make a button to call the sorting function
	/// </summary>
	[CustomEditor(typeof(CISettingsParent))]
	public class CIMenuItemsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Separator();
			if (GUILayout.Button("Sort by priority"))
			{
				CISettingsParent script = target as CISettingsParent;
				script.SortByPriority(script);
			}

			EditorGUILayout.Separator();
			if(GUILayout.Button("Add new rule"))
			{
				CISettingsParent script = target as CISettingsParent;
				script.AddRule(script);
			}

			EditorGUILayout.Separator();
			if(GUILayout.Button("Clear Rules"))
			{
				CISettingsParent script = target as CISettingsParent;
				script.ClearRules(script);
			}
			EditorGUILayout.Separator();
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
			serializedObject.UpdateIfRequiredOrScript();
		}/*OnInspectorGUI*/

	}/*CIMenuItemsEditor*/


	[CustomEditor(typeof(CISettingsGeneric<CIGenericRule>))]
	public class CISettingsGenericEditor : CIMenuItemsEditor
	{
		protected SerializedProperty ruleList;
		protected CISettingsGeneric<CIGenericRule> script;

		public void OnEnable()
		{
			ruleList = serializedObject.FindProperty("_L_rules");
		}/*OnEnable*/

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(ruleList, new GUIContent("Filters"), false, GUILayout.MaxHeight(GUI.skin.textField.lineHeight));
			EditorGUILayout.Separator();
			int indent = EditorGUI.indentLevel;
			if(ruleList.isExpanded)
			{
				EditorGUI.indentLevel = indent + 1;
				for(int i = 0; i < ruleList.arraySize; i++)
				{

					GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(ruleList.GetArrayElementAtIndex(i), GUILayout.Height(0f), GUILayout.Width(-10f));
					if(GUILayout.Button("X", GUILayout.ExpandWidth(false)))
					{
						CISettingsParent script = target as CISettingsParent;
						script.DeleteRule(script, i);
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUI.indentLevel = indent;
			}
			
			serializedObject.ApplyModifiedProperties();
			serializedObject.UpdateIfRequiredOrScript();
		}/*OnInspectorGUI*/

	}/*CISettingsGenericEditor*/
}
