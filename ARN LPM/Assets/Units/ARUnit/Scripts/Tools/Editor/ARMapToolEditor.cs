using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ARUnit.Tool
{
	[CustomEditor(typeof(ARMapTool))]
	public class ARMapEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			ARMapTool myScript = (ARMapTool) target;

			if (GUILayout.Button("Set Anchors"))
			{
				myScript.SetAllTargets();
			}
			if (myScript.targetScriptable != null)
				GUILayout.Label("Count = " + ((myScript.targetScriptable.imageAnchors != null) ? myScript.targetScriptable.imageAnchors.Length.ToString() : "-"));

		}
	}
}
