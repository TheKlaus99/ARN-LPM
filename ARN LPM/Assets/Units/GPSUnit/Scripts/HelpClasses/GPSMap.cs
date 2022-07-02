using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPSUnit
{
	[CreateAssetMenu(fileName = "GPSMap", menuName = "Map/GPSMap", order = 0)]
	public class GPSMap : ScriptableObject
	{
		public float latitude, longitude, altitude;
		public Vector3 localPos;

		public float width, height;
		public Texture2D filter;
	}
}
