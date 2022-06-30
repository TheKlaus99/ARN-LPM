using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ARUnit
{
	[CustomEditor(typeof(ARMap))]
	public class ARMapEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			ARMap myScript = (ARMap) target;
		}
	}
}
