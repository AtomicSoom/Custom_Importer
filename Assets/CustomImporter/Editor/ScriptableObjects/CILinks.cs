using UnityEngine;


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
}
