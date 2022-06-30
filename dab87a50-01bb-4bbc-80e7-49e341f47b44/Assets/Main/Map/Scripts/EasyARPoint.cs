using System;
using System.Collections;
using System.Collections.Generic;
using ARUnit;
using GPSUnit;
using UnityEngine;

public class EasyARPoint : MonoBehaviour
{
	Animator anim;
	MapMarkerController mapMarkerController;
	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();
		mapMarkerController = GetComponentInParent<MapMarkerController>();
		GPSInterface.onGPSUpdate += OnGPSUpdate;
		ARInterface.onStatusChange += OnARStatusChange;
	}

	private void OnARStatusChange(ARStatus ARStatus)
	{
		if (ARStatus == ARStatus.Running)
		{
			DiasbleEasyAR();
		}
	}

	private void OnGPSUpdate(GPSInfo info)
	{
		if (info.horizontalAccuracy < ARNSettings.settings.GPSAccuracyToSwitchFromEasyAR)
		{
			DiasbleEasyAR();
		}
	}

	void DiasbleEasyAR()
	{
		anim.SetBool("isHide", true);
		mapMarkerController.easyARHouseInfo.house = null;

	}
}
