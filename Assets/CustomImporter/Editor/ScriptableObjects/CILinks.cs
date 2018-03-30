using System.Collections;
using System.Collections.Generic;
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
		public CISettingsTexture _SCO_settings;
		public bool _b_debug;
	}/*CILinks*/
}
