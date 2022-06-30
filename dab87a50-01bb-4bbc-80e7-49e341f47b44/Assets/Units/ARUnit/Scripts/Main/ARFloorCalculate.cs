using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
	public class ARFloorCalculate : MonoBehaviour
	{

		float distanceToUpdate = 3;
		float areaToUpdate = 1;

		Vector3 currentPos;
		float currentArea = 0;

		void Start()
		{
			ARInterface.onPlaneAdd += onPlaneUpdate;
			ARInterface.onPlaneUpdate += onPlaneUpdate;
		}

		void OnDestroy()
		{
			ARInterface.onPlaneAdd -= onPlaneUpdate;
			ARInterface.onPlaneUpdate -= onPlaneUpdate;
		}

		float Area(ARPlane ARPlane)
		{
			return ARPlane.extent.x * ARPlane.extent.z;
		}

		private void onPlaneUpdate(ARPlane ARPlane)
		{
			float area = Area(ARPlane);
			if (ARPlane.position.y + .7f < ARInterface.rawARTransform.position.y && area > areaToUpdate)
			{
				if (area * .9f > currentArea || Vector3.Distance(ARPlane.position, currentPos) - Vector3.Distance(Vector3.zero, ARPlane.extent) > distanceToUpdate)
				{
					currentArea = area;
					currentPos = ARPlane.position;
					ARInterface.UpdateFloorLevel(currentPos.y);
				}
			}
		}
	}
}
