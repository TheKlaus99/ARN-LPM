using System;
using System.Collections;
using System.Collections.Generic;
using GPSUnit;
using PositionUnit;
using UnityEngine;

public class GPSPointMover : MonoBehaviour {

	public Transform accRound;
	public CanvasGroup canvasGroup;

	public float targetScale;
	public Vector2 targetPos = Vector3.zero;

	[HideInInspector]
	public RectTransform rt;
	float speed = 2f;

	void Start() {
		rt = GetComponent<RectTransform>();
		PositionUnit.PositionInterface.onStatusChange += OnStatusChange;
		GPSUnit.GPSInterface.onGPSStatusUpdate += OnGPSStatusUpdate;
	}

	private void OnGPSStatusUpdate(GPSServiceStatus status) {
		Debug.Log(status);
		if (status == GPSUnit.GPSServiceStatus.Running) {
			GPSUnit.GPSInterface.onGPSUpdate += OnGPSUpdate;
		} else {
			GetComponent<CanvasGroup>().alpha = 0;

		}
	}

	private void OnGPSUpdate(GPSInfo info) {
		Debug.Log(info.ToString());
		GPSInfo mapPos = new GPSInfo(ARNSettings.settings.GPSMap.latitude, ARNSettings.settings.GPSMap.longitude, 0);

		if (GPSUtility.distance(info, mapPos) < ARNSettings.settings.maxDistanceBetweenMapAndGPS) {
			GPSUnit.GPSInterface.onGPSUpdate -= OnGPSUpdate;
			GetComponent<CanvasGroup>().alpha = 1;
		}
	}

	private void OnStatusChange(PosStatus status) {
		if (status == PosStatus.normal) {
			canvasGroup.alpha = .3f;
		} else if (status == PosStatus.unknown) {
			canvasGroup.alpha = 1;
		}
	}

	public void SetTargets(Vector2 targetPos, float targetScale) {
		this.targetPos = targetPos;
		this.targetScale = targetScale;
	}

	public void SetTargetsForce(Vector2 targetPos, float targetScale) {
		this.targetPos = targetPos;
		this.targetScale = targetScale;
		rt.anchoredPosition = targetPos;
		accRound.localScale = Vector3.one * targetScale;
	}

	void Update() {
		rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, targetPos, Time.deltaTime * speed);
		accRound.localScale = Vector3.one * (accRound.localScale.x + (targetScale - accRound.localScale.x) * Time.deltaTime * speed);
	}
}
