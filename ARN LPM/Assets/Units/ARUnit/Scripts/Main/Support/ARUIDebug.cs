using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit.Test
{
	public class ARUIDebug : MonoBehaviour
	{
		[Header("AR Session events")]
		public bool onStartSessionEnable = false;
		public bool onReStartSessionEnable = false;
		public bool onStopSessionEnable = false;
		public bool onSessionFaildEnable = false;

		[Header("AR Status events")]
		public bool onStatusChangeEnable = false;
		public bool onTrackingStateChangeEnable = false;
		public bool onTrackingStateReasonChangeEnable = false;
		public bool onChangePaneModeEnable = false;

		[Header("AR Image ebents")]
		public bool onImageAddedEnable = false;
		public bool onImageUpdateEnable = false;
		public bool onImageRemovedEnable = false;

		[Header("AR Plane ebents")]
		public bool onPlaneAddedEnable = false;
		public bool onPlaneUpdateEnable = false;
		public bool onPlaneRemovedEnable = false;

		[Header("AR Camera events")]
		public bool onARTransformUpdateEnable = false;
		public bool OnARCameraProjectionMatrixUpdateEnable = false;

		[Header("Calculated")]
		public bool onFloorLevelUpdateEnable = false;

		private void Awake()
		{

			ARInterface.onARTransformUpdate += onARTransformUpdate;
			ARInterface.onImageUpdate += onImageUpdate;
			ARInterface.onImageAdd += onImageAdded;
			ARInterface.onImageRemoved += onImageRemoved;

			ARInterface.onPlaneUpdate += onPlaneUpdate;
			ARInterface.onPlaneAdd += onPlaneAdded;
			ARInterface.onPlaneRemoved += onPlaneRemoved;

			ARInterface.onFloorLevelUpdate += onFloorLevelUpdate;

			ARInterface.onStatusChange += onStatusChange;
			ARInterface.onTrackingStateChange += onTrackingStateChange;
			ARInterface.onTrackingStateReasonChange += onTrackingStateReasonChange;
			ARInterface.onStartSession += onStartSession;
			ARInterface.onReStartSession += onReStartSession;
			ARInterface.onChangePaneMode += onChangePaneMode;
			ARInterface.onStopSession += onStopSession;
			ARInterface.onSessionFaild += onSessionFaild;
			ARInterface.onARCameraProjectionMatrixUpdate += OnARCameraProjectionMatrixUpdate;
		}

		private void onFloorLevelUpdate(float floorLevel)
		{
			if (onFloorLevelUpdateEnable)
				UIDebug.Log("onFloorLevelUpdate| " + floorLevel);
		}

		void onARTransformUpdate(ARTransform ARTransform)
		{
			if (onARTransformUpdateEnable)
				UIDebug.Log("onARTransformUpdate| " + ARTransform.ToString());
		}

		void onImageAdded(ARImage ARImage)
		{
			if (onImageAddedEnable)
				UIDebug.Log("onImageAdded| " + ARImage.ToString());
		}

		void onImageUpdate(ARImage ARImage)
		{
			if (onImageUpdateEnable)
				UIDebug.Log("onImageUpdate| " + ARImage.ToString());
		}

		void onImageRemoved(ARImage ARImage)
		{
			if (onImageRemovedEnable)
				UIDebug.Log("onImageRemoved| " + ARImage.ToString());
		}

		void onPlaneAdded(ARPlane ARPlane)
		{
			if (onPlaneAddedEnable)
				UIDebug.Log("onPlaneAdded| " + ARPlane.ToString());
		}

		void onPlaneUpdate(ARPlane ARPlane)
		{
			if (onPlaneUpdateEnable)
				UIDebug.Log("onPlaneUpdate| " + ARPlane.ToString());
		}

		void onPlaneRemoved(ARPlane ARPlane)
		{
			if (onPlaneRemovedEnable)
				UIDebug.Log("onPlaneRemoved| " + ARPlane.ToString());
		}

		void onStatusChange(ARStatus ARStatus)
		{
			if (onStatusChangeEnable)
				UIDebug.Log("onStatusChange| " + ARStatus.ToString());
		}

		void onTrackingStateChange(ARUnit.ARTrackingState ARTrackingState)
		{
			if (onTrackingStateChangeEnable)
				UIDebug.Log("onTrackingStateChange| " + ARTrackingState.ToString());
		}

		void onTrackingStateReasonChange(ARUnit.ARTrackingStateReason ARTrackingStateReason)
		{
			if (onTrackingStateReasonChangeEnable)
				UIDebug.Log("onTrackingStateReasonChange| " + ARTrackingStateReason.ToString());
		}

		void onStartSession()
		{
			if (onStartSessionEnable)
				UIDebug.Log("onStartSession");
		}

		void onReStartSession()
		{
			if (onReStartSessionEnable)
				UIDebug.Log("onReStartSession");
		}

		void onChangePaneMode(bool isActive)
		{
			if (onChangePaneModeEnable)
				UIDebug.Log("onChangePaneMode| " + isActive);
		}

		void onStopSession()
		{
			if (onStopSessionEnable)
				UIDebug.Log("onStopSesson");
		}

		void onSessionFaild(string error)
		{
			if (onSessionFaildEnable)
				UIDebug.Log("onSessionFaild| " + error);
		}

		void OnARCameraProjectionMatrixUpdate(Matrix4x4 ProjectionMatrix)
		{
			if (OnARCameraProjectionMatrixUpdateEnable)
				UIDebug.Log("OnARCameraProjectionMatrixUpdate");
		}

	}
}
