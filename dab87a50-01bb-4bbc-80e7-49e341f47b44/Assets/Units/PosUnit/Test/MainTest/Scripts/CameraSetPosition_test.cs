using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit.Test
{
	public class CameraSetPosition_test : MonoBehaviour
	{
		public Transform cam;

		// Use this for initialization
		void Start()
		{
			PositionInterface.onARCameraTramsformUpdate += UpdateCam;
		}

		void UpdateCam(Vector3 position, Quaternion rotation)
		{
			cam.localPosition = position;
			cam.localRotation = rotation;
		}
	}
}
