using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit.Test
{
	public class GenerateARCameraTransform_test : MonoBehaviour
	{
		public Transform cam;
		public bool simulateAR;

		// Update is called once per frame
		void Update()
		{
			if (simulateAR)
			{
				ARUnit.ARInterface.UpdateARTransform(new ARUnit.ARTransform(cam.localPosition, cam.localRotation));
			}
			else
			{
				PositionInterface.UpdateARRAWCameraTransform(cam.localPosition, cam.localRotation);
			}
		}
	}
}
