using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Presets;


namespace CustomImporter
{
	/// <summary>
	/// Generic Rules applying for all kind of importer
	/// </summary>
	[System.Serializable]
	public class CIGenericRule
	{
		[SerializeField]
		protected Preset _Preset;
		public Preset PRESET { get { return _Preset; } }

		[SerializeField]
		protected int _i_priority;
		public int PRIORITY { get { return _i_priority; } }

		[SerializeField]
		protected string _s_test_name = "";
		[SerializeField]
		protected bool _b_name_contains = true;
		[SerializeField]
		protected bool _b_name_exact = true;

		[SerializeField]
		protected string _s_test_path = "";
		[SerializeField]
		protected bool _b_path_contains = true;
		[SerializeField]
		protected bool _b_path_exact = true;

		public virtual bool TestCondition(string path, string name)
		{
			return (!_b_path_contains || path.Contains(_s_test_path))
				&& (!_b_path_exact || path.Equals(_s_test_path))
				&& (!_b_name_contains || name.Contains(_s_test_name))
				&& (!_b_name_exact || name.Equals(_s_test_name));
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
}