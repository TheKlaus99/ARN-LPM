using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{

	public class ARPlane
	{
		public string identifier;

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 extent;

		public ARPlane(string identifier, Vector3 position, Quaternion rotation, Vector3 extent)
		{
			this.identifier = identifier;
			this.position = position;
			this.rotation = rotation;
			this.extent = extent;
		}

		public override string ToString()
		{
			return string.Format("id: {0}, pos: {1}, rot: {2}, extent: {3}", identifier, position, rotation.eulerAngles, extent);
		}
	}
}
