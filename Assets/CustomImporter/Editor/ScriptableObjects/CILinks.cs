using UnityEngine;
using UnityEditor;


namespace CustomImporter
{
	/// <summary>
	/// ScriptableObjects to link settings to the importer
	/// </summary>
	//[CreateAssetMenu(fileName = "CustomImporterLinks", menuName = "CustomImporterLinks", order = 9000)]
	public class CILinks : ScriptableObject
	{
		public CISettingsTexture _CISettings_texture;
		public CISettingsModel _CISettings_model;
		public CISettingsAudio _CISettings_audio;
		public bool _b_debug;
	}/*CILinks*/


	[CustomEditor(typeof(CILinks))]
	public class CILinksEditor : Editor
	{
		private SerializedProperty textureSettings;
		private SerializedProperty modelSettings;
		private SerializedProperty audioSettings;
		private SerializedProperty debugEnabled;

		private GUIContent guiContentSettings = new GUIContent("Settings");
		private GUIContent guiContentDebug = new GUIContent("Enable Debugging");

		private GUIStyle guiStyle;


		public void OnEnable()
		{
			textureSettings = serializedObject.FindProperty("_CISettings_texture");
			modelSettings = serializedObject.FindProperty("_CISettings_model");
			audioSettings = serializedObject.FindProperty("_CISettings_audio");
			debugEnabled = serializedObject.FindProperty("_b_debug");
		}/*OnEnable*/

		public override void OnInspectorGUI()
		{
			guiStyle = new GUIStyle(GUI.skin.label);
			guiStyle.fontStyle = FontStyle.Bold;

			EditorGUILayout.LabelField("Texture Importer Settings", guiStyle);
			EditorGUILayout.PropertyField(textureSettings, guiContentSettings);

			Separator();

			EditorGUILayout.LabelField("Model Importer Settings", guiStyle);
			EditorGUILayout.PropertyField(modelSettings, guiContentSettings);

			Separator();

			EditorGUILayout.LabelField("Audio Importer Settings", guiStyle);
			EditorGUILayout.PropertyField(audioSettings, guiContentSettings);

			Separator();

			EditorGUILayout.PropertyField(debugEnabled, guiContentDebug);
		}/*OnInspectorGUI*/


		private void Separator ()
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
			EditorGUILayout.Separator();
		}/*Separator*/

	}/*CILinksEditor*/
}
