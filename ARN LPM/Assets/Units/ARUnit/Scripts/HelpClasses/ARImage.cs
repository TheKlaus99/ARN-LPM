using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
	public class ARImage
	{
		public string name;
		public Vector3 position;
		public Quaternion rotation;

		public ARImage()
		{
			this.name = "";
			this.position = new Vector3();
			this.rotation = new Quaternion();
		}

		public ARImage(ARImage ARImage)
		{
			this.name = ARImage.name;
			this.position = new Vector3(ARImage.position.x, ARImage.position.y, ARImage.position.z);
			this.rotation = new Quaternion(ARImage.rotation.x, ARImage.rotation.y, ARImage.rotation.z, ARImage.rotation.w);
		}

		public ARImage(string name, Vector3 position, Quaternion rotation)
		{
			this.name = name;
			this.position = position;
			this.rotation = rotation;
		}

		public override string ToString()
		{
			return string.Format("name: {0}, pos: {1}, rot: {2}", name, position.ToString(), rotation.eulerAngles.ToString());
		}
	}
}
