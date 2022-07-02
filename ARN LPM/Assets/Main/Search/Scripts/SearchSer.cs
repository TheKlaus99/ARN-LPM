using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Search
{
	[System.Serializable]
	public class SearchSer
	{
		public string name;
		public string info;
		public Vector3 pos;

		public int sortingOrder;

		public SearchSer(string name, string info, Vector3 pos, int sortingOrder)
		{
			this.name = name;
			this.info = info;
			this.pos = pos;
			this.sortingOrder = sortingOrder;
		}
	}

	[System.Serializable]
	public class SearchEventArraySer
	{
		public SearchEventSer[] data;
	}

	[System.Serializable]
	public class SearchEventSer
	{
		public string name;
		public string info;
		public Vector3 pos;
		public string date;
		public string time;
		public int sortingOrder;

		public SearchSer GetSearchSer()
		{
			return new SearchSer(name, info, pos, sortingOrder);
		}
	}

	[System.Serializable]
	public class Vector3S
	{
		public float x;
		public float y;
		public float z;

		public Vector3S(Vector3 vector3)
		{
			x = vector3.x;
			y = vector3.y;
			z = vector3.z;
		}

		public Vector3S(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x, y, z);
		}
	}
}
