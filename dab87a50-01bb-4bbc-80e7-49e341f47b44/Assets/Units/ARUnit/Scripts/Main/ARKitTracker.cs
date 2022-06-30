using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;


namespace ARUnit
{

	public class ARKitTracker : MonoBehaviour
	{
		private static ARKitTracker instance;
		public static ARKitTracker getInstance()
		{
			if (instance == null)
			{
				Debug.Log("Create Obj: ARKitTracker");
				UIDebug.Log("Create Obj: ARKitTracker");  
				instance = new GameObject("ARKitTracker", typeof(ARKitTracker)).GetComponent<ARKitTracker>();
			}
			return instance;
		}


		UnityARSessionNativeInterface m_session;

		public GameObject ARCamera;
		public Material m_ClearMaterial;

		public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravityAndHeading;

		[Header("AR Config Options")]
		public bool getPointCloud = true;

		[Header("Image Tracking")]
		public ARReferenceImagesSet detectionImages = null;
		public int maximumNumberOfTrackedImages = 0;

		public ARKitWorldTrackingSessionConfiguration sessionConfiguration(bool planeDetection = true)
		{
			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
			config.planeDetection = (planeDetection) ? UnityARPlaneDetection.Horizontal : UnityARPlaneDetection.None;
			config.alignment = startAlignment;
			config.getPointCloudData = getPointCloud;
			config.enableLightEstimation = false;
			config.enableAutoFocus = true;
			config.maximumNumberOfTrackedImages = maximumNumberOfTrackedImages;
			config.environmentTexturing = UnityAREnvironmentTexturing.UnityAREnvironmentTexturingNone;
			if (UnityARVideoFormat.SupportedVideoFormats().Count > 0)
				config.videoFormat = UnityARVideoFormat.SupportedVideoFormats() [0].videoFormatPtr;
			if (detectionImages != null)
				config.referenceImagesGroupName = detectionImages.resourceGroupName;

			return config;
		}

		private void Start()
		{
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = this;
			}
			//DontDestroyOnLoad(gameObject);
			UnityEngine.XR.iOS.UnityARVideo video = ARCamera.AddComponent<UnityEngine.XR.iOS.UnityARVideo>();
			video.m_ClearMaterial = m_ClearMaterial;

			m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

			Subcribe();
		}

		void Subcribe()
		{
			ARInterface.onStartSession += OnStartSession;
			ARInterface.onReStartSession += OnRestartSession;
			ARInterface.onStopSession += OnStopSession;
			ARInterface.onChangePaneMode += OnChangePaneMode;

			UnityARSessionNativeInterface.ARImageAnchorAddedEvent += OnImageAdd;
			UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += OnImageUpdate;
			UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += OnImageRemove;

			UnityARSessionNativeInterface.ARAnchorAddedEvent += OnPlaneAdd;
			UnityARSessionNativeInterface.ARAnchorRemovedEvent += OnPlaneRemove;
			UnityARSessionNativeInterface.ARAnchorUpdatedEvent += OnPlaneUpdate;
		}

		void UnSubscribe()
		{
			DeviceChange.OnOrientationChange -= OnOrientationChange;
			if (ARInterface.ARStatus == ARStatus.Running)
			{
				UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FrameUpdate;
			}
			else if (ARInterface.ARStatus == ARStatus.Initializing)
			{
				UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
			}
		}

		#region status
		ARUnit.ARTrackingStateReason trackingStateReason(UnityEngine.XR.iOS.ARTrackingStateReason state)
		{
			switch (state)
			{
				case UnityEngine.XR.iOS.ARTrackingStateReason.ARTrackingStateReasonNone:
					return ARUnit.ARTrackingStateReason.ARTrackingStateReasonNone;

				case UnityEngine.XR.iOS.ARTrackingStateReason.ARTrackingStateReasonRelocalizing:
					return ARUnit.ARTrackingStateReason.ARTrackingStateReasonRelocalizing;

				case UnityEngine.XR.iOS.ARTrackingStateReason.ARTrackingStateReasonInsufficientFeatures:
					return ARUnit.ARTrackingStateReason.ARTrackingStateReasonInsufficientFeatures;

				case UnityEngine.XR.iOS.ARTrackingStateReason.ARTrackingStateReasonInitializing:
					return ARUnit.ARTrackingStateReason.ARTrackingStateReasonInitializing;

				case UnityEngine.XR.iOS.ARTrackingStateReason.ARTrackingStateReasonExcessiveMotion:
					return ARUnit.ARTrackingStateReason.ARTrackingStateReasonExcessiveMotion;
			}
			return ARUnit.ARTrackingStateReason.ARTrackingStateReasonUnSupported;
		}
		ARUnit.ARTrackingState trackingState(UnityEngine.XR.iOS.ARTrackingState state)
		{
			switch (state)
			{
				case UnityEngine.XR.iOS.ARTrackingState.ARTrackingStateLimited:
					return ARUnit.ARTrackingState.ARTrackingStateLimited;

				case UnityEngine.XR.iOS.ARTrackingState.ARTrackingStateNormal:
					return ARUnit.ARTrackingState.ARTrackingStateNormal;

				case UnityEngine.XR.iOS.ARTrackingState.ARTrackingStateNotAvailable:
					return ARUnit.ARTrackingState.ARTrackingStateNotAvailable;

			}
			return ARUnit.ARTrackingState.ARTrackingStateUnSupported;
		}
		UnityEngine.XR.iOS.ARTrackingState lastState;
		UnityEngine.XR.iOS.ARTrackingStateReason lastStateReason;
		void UpdateStates(UnityARCamera cam)
		{
			if (lastState != cam.trackingState)
			{
				lastState = cam.trackingState;
				ARInterface.ChangeTrackingState(trackingState(cam.trackingState));
			}
			if (lastStateReason != cam.trackingReason)
			{
				lastStateReason = cam.trackingReason;
				ARInterface.ChangeTrackingStateReason(trackingStateReason(cam.trackingReason));
			}
		}
		#endregion

		Matrix4x4 CamProjMatrix(UnityARCamera cam)
		{
			Matrix4x4 matrix = new Matrix4x4();
			matrix.SetColumn(0, cam.projectionMatrix.column0);
			matrix.SetColumn(1, cam.projectionMatrix.column1);
			matrix.SetColumn(2, cam.projectionMatrix.column2);
			matrix.SetColumn(3, cam.projectionMatrix.column3);
			return matrix;
		}

		void FirstFrameUpdate(UnityARCamera cam)
		{
			ARInterface.ChangeStatus(ARStatus.Running);
			UpdateStates(cam);
			UpdateCamProjMatrix(cam);
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdate;
		}

		bool ProjMatrixUpdated = false;
		void UpdateCamProjMatrix(UnityARCamera cam)
		{
			ProjMatrixUpdated = true;
			ARInterface.UpdateCameraProjectionMatrix(CamProjMatrix(cam));
		}

		void FrameUpdate(UnityARCamera cam)
		{
			UpdateStates(cam);

			if (!ProjMatrixUpdated)
			{
				UpdateCamProjMatrix(cam);
			}

			Matrix4x4 matrix = new Matrix4x4();
			matrix.SetColumn(0, cam.worldTransform.column0);
			matrix.SetColumn(1, cam.worldTransform.column1);
			matrix.SetColumn(2, cam.worldTransform.column2);
			matrix.SetColumn(3, cam.worldTransform.column3);
			ARTransform transform = new ARTransform(UnityARMatrixOps.GetPosition(matrix), UnityARMatrixOps.GetRotation(matrix));

			ARInterface.UpdateARTransform(transform);
		}

		void StartSession(bool planeDetection = false, bool restart = true)
		{
			UIDebug.Log("PaneDetection " + planeDetection);
			ARInterface.ChangeStatus(ARStatus.Initializing);
			ARKitWorldTrackingSessionConfiguration config = sessionConfiguration(planeDetection);
			if (config.IsSupported)
			{
				UnityARSessionRunOption option = new UnityARSessionRunOption();
				if (restart)
				{
					option = UnityARSessionRunOption.ARSessionRunOptionResetTracking | UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors;
				}
				else
				{
					option = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors;
				}
				m_session.RunWithConfigAndOptions(config, option);
				m_session.SetCameraClipPlanes(0.01f, 5000);
				UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
			}
			else
			{
				//UIDebug.Log("ARConfig unsupported");
				ARInterface.ChangeStatus(ARStatus.Unsupported);
			}
		}

		void OnStartSession()
		{
			if (ARInterface.ARStatus == ARStatus.Stopped || ARInterface.ARStatus == ARStatus.Failed)
			{
				//UIDebug.Log("Start AR Session");
				DeviceChange.OnOrientationChange += OnOrientationChange;
				UnityARSessionNativeInterface.ARSessionFailedEvent += OnSessionFaild;
				StartSession(ARInterface.ARPaneDetectionEnable, true);
			}
		}

		void OnOrientationChange(DeviceOrientation orientation)
		{
			ProjMatrixUpdated = false;
		}

		void OnSessionFaild(string error)
		{
			//UIDebug.Log("SessionFaild: " + error);
			UnityARSessionNativeInterface.ARSessionFailedEvent -= OnSessionFaild;
			UnSubscribe();
			ARInterface.SessionFaildEvent(error);
			ARInterface.ChangeStatus(ARStatus.Failed);
		}

		void OnChangePaneMode(bool isActive)
		{
			//UIDebug.Log("DetectPane set: " + isActive.ToString());
			StartSession(isActive, false);
		}

		void OnRestartSession()
		{
			//UIDebug.Log("Restart AR Session");
			StartSession(ARInterface.ARPaneDetectionEnable, true);
		}

		void OnStopSession()
		{
			if (ARInterface.ARStatus == ARStatus.Running)
			{
				//UIDebug.Log("Stop AR Session");
				m_session.Pause();
				UnSubscribe();
				ARInterface.ChangeStatus(ARStatus.Stopped);
			}
		}

		ARUnit.ARImage ARImageAnchorToARImage(ARImageAnchor anchorData)
		{
			return new ARUnit.ARImage(
				anchorData.referenceImageName,
				UnityARMatrixOps.GetPosition(anchorData.transform),
				UnityARMatrixOps.GetRotation(anchorData.transform)
			);
		}

		void OnImageAdd(ARImageAnchor anchorData)
		{
			ARInterface.ARImageAdd(ARImageAnchorToARImage(anchorData));
		}

		void OnImageUpdate(ARImageAnchor anchorData)
		{
			ARInterface.ARImageUpdate(ARImageAnchorToARImage(anchorData));
		}

		ARUnit.ARPlane ARPlaneAnchorToARPlane(ARPlaneAnchor anchorData)
		{
			return new ARUnit.ARPlane(
				anchorData.identifier,
				UnityARMatrixOps.GetPosition(anchorData.transform),
				UnityARMatrixOps.GetRotation(anchorData.transform),
				anchorData.extent
			);
		}

		void OnImageRemove(ARImageAnchor anchorData)
		{
			ARInterface.ARImageRemove(ARImageAnchorToARImage(anchorData));
		}

		void OnPlaneAdd(ARPlaneAnchor anchorData)
		{
			ARInterface.ARPlaneAdd(ARPlaneAnchorToARPlane(anchorData));
		}

		void OnPlaneUpdate(ARPlaneAnchor anchorData)
		{
			ARInterface.ARPlaneUpdate(ARPlaneAnchorToARPlane(anchorData));
		}

		void OnPlaneRemove(ARPlaneAnchor anchorData)
		{
			ARInterface.ARPlaneRemove(ARPlaneAnchorToARPlane(anchorData));
		}

	}
}
