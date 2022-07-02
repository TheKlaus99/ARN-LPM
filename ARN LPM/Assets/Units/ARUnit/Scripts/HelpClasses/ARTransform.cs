using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
	public class ARTransform
	{
		public Vector3 position;
		public Quaternion rotation;

		public ARTransform()
		{
			position = new Vector3(0, 0, 0);
			rotation = Quaternion.Euler(0, 0, 0);
		}

		public ARTransform(Vector3 position, Quaternion rotration)
		{
			this.position = position;
			this.rotation = rotration;
		}

		public override string ToString()
		{
			return string.Format("pos: {0}, rot: {1}", position.ToString(), rotation.eulerAngles.ToString());
		}
	}
}
