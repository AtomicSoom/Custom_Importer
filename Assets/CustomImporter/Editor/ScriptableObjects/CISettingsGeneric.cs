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
	public abstract class CIMenuItems : ScriptableObject
	{
		/// <summary>
		/// Use to call sorting function from Unity Inspector
		/// </summary>
		/// <param name="command"></param>
		[MenuItem("CONTEXT/CIMenuItems/Sort By Priority")]
		public static void SortByPriorityMenu(MenuCommand command)
		{
			CIMenuItems settings = command.context as CIMenuItems;
			settings.SortByPriority(settings);
		}/*SortByPriorityMenu*/

		/// <summary>
		/// abstract function to sort the list via mother class context
		/// </summary>
		/// <param name="mother"></param>
		public abstract void SortByPriority(CIMenuItems mother);

	}/*CIMenuItems*/


	/// <summary>
	/// ScriptableObject to store a set of importer rules
	/// </summary>
	[System.Serializable]
	public class CISettingsGeneric<T> : CIMenuItems where T : CIGenericRule
	{
		[SerializeField]
		protected List<T> _L_rules = new List<T>();
		public List<T> RULES { get { return _L_rules; } }


		/// <summary>
		/// Gets a texture importer preset from a name (name of the preset file)
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public virtual Preset GetPresetByName(string name)
		{
			CIGenericRule importer = _L_rules.Find(x => x.PRESET.name == name);
			return importer == null ? null : importer.PRESET;
		}/*GetPresetByName*/


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
		public override void SortByPriority(CIMenuItems mother)
		{
			CISettingsGeneric<T> settings = mother as CISettingsGeneric<T>;
			settings._L_rules.Sort(delegate (T x, T y)
			{
				return y.PRIORITY - x.PRIORITY;
			});
		}/*SortByPriority*/

	}/*CISettingsGeneric*/


	/// <summary>
	/// Custom Inspector to make a button to call the sorting function
	/// </summary>
	[CustomEditor(typeof(CIMenuItems))]
	public class CIMenuItemsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Sort by priority"))
			{
				CIMenuItems script = target as CIMenuItems;
				script.SortByPriority(script);
			}
			DrawDefaultInspector();
		}/*OnInspectorGUI*/

	}/*CIMenuItemsEditor*/


	[CustomEditor(typeof(CISettingsGeneric<CIGenericRule>))]
	public class CISettingsGengericEditor : CIMenuItemsEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}/*OnInspectorGUI*/

	}/*CISettingsGengericEditor*/
}
