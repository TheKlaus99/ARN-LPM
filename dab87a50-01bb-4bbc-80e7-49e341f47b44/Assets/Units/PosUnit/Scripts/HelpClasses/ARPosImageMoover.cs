using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit.Test
{
	public class ARPosImageMoover : ARUnit.ARImageMover
	{
		void Update()
		{
			transform.localPosition = PositionInterface.PositionController.TranslatePositionReleativeCam(ARImage.position);
			transform.localRotation = PositionInterface.PositionController.TranslateRotation(ARImage.rotation);
		}
	}
}
