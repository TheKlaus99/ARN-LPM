using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit
{
	public class PosCamMover : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			PositionInterface.onARCameraTramsformUpdate += TransformUpdate;
			ARUnit.ARInterface.onARCameraProjectionMatrixUpdate += ProjUpdate;
		}

		Vector3 ARPosition = Vector3.zero;
		Quaternion ARRotation = new Quaternion();
		void TransformUpdate(Vector3 position, Quaternion rotation)
		{
			ARPosition = position;
			ARRotation = rotation;
		}

		void ProjUpdate(Matrix4x4 ProjectionMatrix)
		{
			GetComponent<Camera>().projectionMatrix = ProjectionMatrix;
#if UNITY_IOS
			UnityEngine.XR.iOS.UnityARSessionNativeInterface.GetARSessionNativeInterface().SetCameraClipPlanes(GetComponent<Camera>().nearClipPlane, GetComponent<Camera>().farClipPlane);
			Debug.Log("Set cameta Clip Planes" + GetComponent<Camera>().nearClipPlane + " " + GetComponent<Camera>().farClipPlane);
#endif
		}


		// Update is called once per frame
		void Update()
		{

			transform.localPosition = ARPosition;
			transform.localRotation = ARRotation;

		}
	}
}
