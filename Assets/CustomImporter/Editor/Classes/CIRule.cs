using UnityEngine;
using UnityEditor.Presets;
using UnityEditor;


namespace CustomImporter
{
	/// <summary>
	/// Generic Rules applying for all kind of importer
	/// </summary>
	[System.Serializable]
	public class CIGenericRule
	{
		public string _s_visible_name = "Filter";
		[SerializeField]
		protected string _s_label;
		public string LABEL { get { return _s_label; } }

		[SerializeField]
		protected Preset _Preset;
		public Preset PRESET { get { return _Preset; } }

		[SerializeField]
		protected int _i_priority;
		public int PRIORITY { get { return _i_priority; } }

		[SerializeField]
		protected bool _b_filter_name = true;
		[SerializeField]
		protected string _s_test_name = "";
		[SerializeField]
		protected bool _b_name_exact = true;

		[SerializeField]
		protected bool _b_filter_path = true;
		[SerializeField]
		protected string _s_test_path = "";
		[SerializeField]
		protected bool _b_path_exact = true;


		private int _i_id = 120;



		public static string GetFilter (string type)
		{
			if (type == "CITextureRule")
			{
				return "t:Texture";
			}
			else if (type == "CIModelRule")
			{
				return "t:Model";
			}
			else if (type == "CIAudioRule")
			{
				return "t:AudioClip";
			}
			else
			{
				return "";
			}
		}/*GetFilter*/


		public virtual bool TestCondition(string path, string name)
		{
			bool name_test = true;
			bool path_test = true;

			if (_b_filter_name)
			{
				if (_b_name_exact)
				{
					name_test = name.Equals(_s_test_name);
				}
				else
				{
					name_test = name.Contains(_s_test_name);
				}
			}

			if (_b_filter_path)
			{
				if(_b_path_exact)
				{
					path_test = path.Equals(_s_test_path);
				}
				else
				{
					path_test = path.Contains(_s_test_path);
				}
			}

			return name_test && path_test;
		}/*TestCondition*/

	}/*CIGenericRule*/


	/// <summary>
	/// Rules for a specified Preset of texture importer
	/// </summary>
	[System.Serializable]
	public class CITextureRule : CIGenericRule
	{
		 //TODO check how to get metadata info of the file to add filter like size
	}/*CITextureRule*/


	/// <summary>
	/// Rules for a specified Preset of model importer
	/// </summary>
	[System.Serializable]
	public class CIModelRule : CIGenericRule
	{
		 //TODO check how to get metadata info of the file
	}/*CIModelRule*/


	/// <summary>
	/// Rules for a specified Preset of audio importer
	/// </summary>
	[System.Serializable]
	public class CIAudioRule : CIGenericRule
	{
		 //TODO check how to get metadata info of the file
	}/*CIAudioRule*/


	[CustomPropertyDrawer(typeof(CIGenericRule))]
	public class CIGenericRuleDrawer : PropertyDrawer
	{
		private GUIContent guiContentFilterLabel = new GUIContent("Filter label");
		private GUIContent guiContentPriority = new GUIContent("Priority");
		private GUIContent guiContentNameTest = new GUIContent("Name to test");
		private GUIContent guiContentPathTest = new GUIContent("Path to test");
		private GUIContent guiContentIsExact = new GUIContent("Is exact");

		private SerializedProperty _Preset;
		private SerializedProperty _s_label;
		private SerializedProperty _i_priority;
		private SerializedProperty _s_visible_name;
		private SerializedProperty _b_filter_name;
		private SerializedProperty _s_test_name;
		private SerializedProperty _b_name_exact;
		private SerializedProperty _b_filter_path;
		private SerializedProperty _s_test_path;
		private SerializedProperty _b_path_exact;


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_Preset = property.FindPropertyRelative("_Preset");
			_s_label = property.FindPropertyRelative("_s_label");
			_i_priority = property.FindPropertyRelative("_i_priority");
			_s_visible_name = property.FindPropertyRelative("_s_visible_name");
			_b_filter_name = property.FindPropertyRelative("_b_filter_name");
			_s_test_name = property.FindPropertyRelative("_s_test_name");
			_b_name_exact = property.FindPropertyRelative("_b_name_exact");
			_b_filter_path = property.FindPropertyRelative("_b_filter_path");
			_s_test_path = property.FindPropertyRelative("_s_test_path");
			_b_path_exact = property.FindPropertyRelative("_b_path_exact");

			GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
			//TODO FieldInfo;
			bool rule_open = EditorGUILayout.PropertyField(property, label);
			if (rule_open)
			{
				EditorGUILayout.Separator();
				int indent = EditorGUI.indentLevel;
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_s_label, guiContentFilterLabel);
				EditorGUILayout.PropertyField(_i_priority, guiContentPriority);
				_s_visible_name.stringValue = string.Format("Filter {0} | Priority {1}", _s_label.stringValue, _i_priority.intValue);

				EditorGUILayout.PropertyField(_Preset);

				_b_filter_name.boolValue = EditorGUILayout.ToggleLeft(" Filter by name", _b_filter_name.boolValue);
				if (_b_filter_name.boolValue)
				{
					EditorGUI.indentLevel += 2;
					EditorGUILayout.PropertyField(_s_test_name, guiContentNameTest);
					EditorGUILayout.PropertyField(_b_name_exact, guiContentIsExact);
					EditorGUI.indentLevel -= 2;
				}

				_b_filter_path.boolValue = EditorGUILayout.ToggleLeft(" Filter by path", _b_filter_path.boolValue);
				if(_b_filter_path.boolValue)
				{
					EditorGUI.indentLevel += 2;
					EditorGUILayout.PropertyField(_s_test_path, guiContentPathTest);
					EditorGUILayout.PropertyField(_b_path_exact, guiContentIsExact);
					EditorGUI.indentLevel -= 2;
				}
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();

				EditorGUILayout.LabelField("For assets using this filter : ", GUILayout.ExpandWidth(true));

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Separator();
				if(GUILayout.Button("Reapply for unmodified assets"))
				{
					CISettingsGeneric<CIGenericRule>.ApplyNewPresets(false, CIGenericRule.GetFilter(property.type), _s_label.stringValue, _Preset.objectReferenceValue as Preset);
				}
				EditorGUILayout.Separator();
				if(GUILayout.Button("Reapply for all assets"))
				{
					CISettingsGeneric<CIGenericRule>.ApplyNewPresets(true, CIGenericRule.GetFilter(property.type), _s_label.stringValue, _Preset.objectReferenceValue as Preset);
				}
				EditorGUILayout.Separator();
				EditorGUILayout.EndHorizontal();

				EditorGUI.indentLevel = indent;
				EditorGUILayout.Separator();
				EditorGUILayout.Separator();
			}
		}/*OnGUI*/

	}/*CIGenericRuleDrawer*/


	[CustomPropertyDrawer(typeof(CITextureRule))]
	public class CITextureRuleDrawer : CIGenericRuleDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}/*OnGUI*/

	}/*CITextureRuleDrawer*/


	[CustomPropertyDrawer(typeof(CIModelRule))]
	public class CIModelRuleDrawer : CIGenericRuleDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}/*OnGUI*/

	}/*CIModelRuleDrawer*/


	[CustomPropertyDrawer(typeof(CIAudioRule))]
	public class CIAudioRuleDrawer : CIGenericRuleDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}/*OnGUI*/

	}/*CIAudioRuleDrawer*/
}