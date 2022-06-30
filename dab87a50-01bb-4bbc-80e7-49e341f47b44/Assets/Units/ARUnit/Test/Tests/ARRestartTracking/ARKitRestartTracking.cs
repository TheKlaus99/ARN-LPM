using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace ARUnit.Test
{
	public class ARKitRestartTracking : MonoBehaviour
	{
		public Camera m_camera;
		public UnityARSessionNativeInterface m_session;

		public UnityEngine.UI.Toggle tesetTracking, removeExisting;
		bool sessionStarted;
		// Use this for initialization
		void Start()
		{
			m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
			Application.targetFrameRate = 60;
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
		}

		void FirstFrameUpdate(UnityARCamera cam)
		{
			sessionStarted = true;
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
		}

		UnityARSessionRunOption option
		{
			get
			{
				UnityARSessionRunOption option = new UnityARSessionRunOption();
				if (tesetTracking.isOn && removeExisting.isOn)
				{
					option = UnityARSessionRunOption.ARSessionRunOptionResetTracking | UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors;
				}
				else if (tesetTracking.isOn)
				{
					option = UnityARSessionRunOption.ARSessionRunOptionResetTracking;
				}
				else if (removeExisting.isOn)
				{
					option = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors;
				}
				else
				{
					option = 0;
				}
				Debug.Log(option);
				return option;
			}
		}

		void Update()
		{

			if (m_camera != null && sessionStarted)
			{
				// JUST WORKS!
				Matrix4x4 matrix = m_session.GetCameraPose();
				m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
				m_camera.transform.localRotation = UnityARMatrixOps.GetRotation(matrix);

				m_camera.projectionMatrix = m_session.GetCameraProjection();
			}

		}

		public void StartSession()
		{
			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();

			config.alignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;
			config.planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			config.enableAutoFocus = true;
			config.environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;
			config.maximumNumberOfTrackedImages = 1;
			m_session.RunWithConfigAndOptions(config, option);
		}

		public void PaneDetectionOff()
		{
			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();

			config.alignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;
			config.planeDetection = UnityARPlaneDetection.None;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			config.enableAutoFocus = true;
			config.environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;
			config.maximumNumberOfTrackedImages = 1;

			m_session.RunWithConfigAndOptions(config, option);
		}

		public void PaneDetectionOn()
		{
			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();

			config.alignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;
			config.planeDetection = UnityARPlaneDetection.HorizontalAndVertical;
			config.getPointCloudData = true;
			config.enableLightEstimation = true;
			config.enableAutoFocus = true;
			config.environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;
			config.maximumNumberOfTrackedImages = 1;
			m_session.RunWithConfigAndOptions(config, option);
		}


		public void StopSession()
		{
			m_session.Pause();
		}
	}
}
