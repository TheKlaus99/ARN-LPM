using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARUnit
{
	[CreateAssetMenu(fileName = "ARMap", menuName = "Map/ARMap", order = 0)]
	public class ARMap : ScriptableObject
	{
		public ARImageTransform[] imageAnchors;
		[System.Serializable]
		public class ARImageTransform
		{
			public string name;
			public Vector3 position;
			public Quaternion rotation;
		}
	}
}
