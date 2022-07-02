using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit.Test
{

	public class GenerateEstimate_test : MonoBehaviour
	{

		public Transform currentPoint, targetPoint;


		public Vector3 currentPos, targetPos;
		public float angleAccuracy = 3;
		// Update is called once per frame
		void Update()
		{
			currentPos = currentPoint.localPosition;
			targetPos = targetPoint.localPosition;
		}

		public void AddEstimateButtonTap()
		{
			float a = currentPoint.localRotation.eulerAngles.y - targetPoint.localRotation.eulerAngles.y;
			if (a > 180)
			{
				a = 360 - a;
			}

			Estimate estimate = new Estimate("EditorTest_" + Time.time.ToString(),
				new Vector3S(currentPos),
				new Vector3S(targetPos),
				a,
				15,
				angleAccuracy);
			PositionInterface.AddEstimate(estimate);
		}
	}

}
