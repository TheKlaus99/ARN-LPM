using System.Collections;
using UnityEditor;
using UnityEngine;

namespace PositionUnit.Test
{
	[CustomEditor(typeof(FakeEvents))]
	public class FakeEventsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			FakeEvents myScript = (FakeEvents) target;

			if (GUILayout.Button("Fake GPS update"))
			{
				myScript.FakeGPS();
			}

			if (GUILayout.Button("Fake ARImage add"))
			{
				myScript.FakeARImageAdd();
			}

			if (GUILayout.Button("Fake ARImage update"))
			{
				myScript.FakeARImageUpdate();
			}

			if (GUILayout.Button("Fake Event record Play"))
			{
				myScript.FakeEventsFromRecord();
			}
		}
	}
}
