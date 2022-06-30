using System.Collections;
using UnityEditor;
using UnityEngine;

namespace PositionUnit.Test
{
	[CustomEditor(typeof(GenerateEstimate_test))]
	public class GenerateEstimate_testEditor : Editor
	{

		GenerateEstimate_test generateEstimate_test;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GenerateEstimate_test myScript = (GenerateEstimate_test) target;

			if (GUILayout.Button("AddEstimate"))
			{
				myScript.AddEstimateButtonTap();
			}
		}
	}
}
