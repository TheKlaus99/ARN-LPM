using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace GPSUnit.Tool
{
	[CustomEditor(typeof(GPSMapTool))]
	public class GPSMapToolEditor : Editor
	{


		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GPSMapTool myScript = (GPSMapTool) target;

			if (GUILayout.Button("Set map"))
			{
				myScript.SetMap();
			}

			if (GUILayout.Button("Set size"))
			{
				myScript.SetSize();
			}
		}
	}
}
