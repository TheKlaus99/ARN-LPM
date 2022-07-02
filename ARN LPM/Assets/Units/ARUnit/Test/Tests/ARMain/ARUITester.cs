using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARUnit.Test
{

	public class ARUITester : MonoBehaviour
	{
		public Text camPosT, statusT, trackingStatusT, paneDetectionT, trackingStatusReasonT, sessionFaildT;
		public Transform floorT;

		private void Awake()
		{
			ARInterface.onARTransformUpdate += OnCameraTransformChange;
			ARInterface.onStatusChange += OnStatusChange;
			ARInterface.onTrackingStateReasonChange += OnTrackingStateReasonChange;
			ARInterface.onTrackingStateChange += OnTrackingStateChange;
			ARInterface.onChangePaneMode += OnChangePaneMode;
			ARInterface.onSessionFaild += OnSessionFaild;

			ARInterface.onFloorLevelUpdate += OnFloorLevelUpdate;
		}

		private void OnFloorLevelUpdate(float floorLevel)
		{
			floorT.position = new Vector3(floorT.position.x, floorLevel, floorT.position.z);
		}

		public void OnStartTap()
		{
			ARInterface.StartARSession();
		}

		public void OnStopTap()
		{
			ARInterface.StopARSession();
		}

		public void OnReStartTap()
		{
			ARInterface.ReStartARSession();
		}

		public void OnPaneTap(bool isEnable)
		{
			ARInterface.ChangePaneDetectionMode(isEnable);
		}

		void OnStatusChange(ARStatus ARStatus)
		{
			statusT.text = "Status: " + ARStatus.ToString();
		}

		void OnChangePaneMode(bool isEnable)
		{
			paneDetectionT.text = "Pane detection: " + isEnable.ToString();
		}

		void OnSessionFaild(string error)
		{
			sessionFaildT.text = error;
		}

		void OnTrackingStateChange(ARUnit.ARTrackingState state)
		{
			trackingStatusT.text = "TrackingState: " + state.ToString();
		}

		void OnTrackingStateReasonChange(ARUnit.ARTrackingStateReason state)
		{
			trackingStatusReasonT.text = "TrackingState: " + state.ToString();
		}

		void OnCameraTransformChange(ARUnit.ARTransform transform)
		{
			camPosT.text = string.Format("pos: {0}\nrot: {1}", transform.position.ToString(), transform.rotation.eulerAngles.ToString());
		}

	}
}
