using System;
using System.Collections;
using System.Collections.Generic;
using PositionUnit;
using UnityEngine;

public class ARPosImageTrackProgress : ARImageTrackProgress
{
	private void Update()
	{
		transform.localPosition = PositionInterface.PositionController.TranslatePositionReleativeCam(position);
		transform.localRotation = PositionInterface.PositionController.TranslateRotation(rotation);

	}
}
