using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
	public class ARCamMover : MonoBehaviour
	{
		// Use this for initialization
		void Start()
		{
			ARInterface.onARTransformUpdate += TransformUpdate;
			ARInterface.onARCameraProjectionMatrixUpdate += ProjUpdate;
		}

		ARTransform ARTransform = new ARTransform();
		void TransformUpdate(ARTransform ARTransform)
		{
			this.ARTransform = ARTransform;
		}

		void ProjUpdate(Matrix4x4 ProjectionMatrix)
		{
			GetComponent<Camera>().projectionMatrix = ProjectionMatrix;
		}


		// Update is called once per frame
		void Update()
		{
			transform.localPosition = ARTransform.position;
			transform.localRotation = ARTransform.rotation;
		}
	}
}
