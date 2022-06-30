using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;


namespace ARUnit
{

    public class ARCoreTracker : MonoBehaviour
    {

        private static ARCoreTracker instance;
        public static ARCoreTracker getInstance()
        {
            if (instance == null)
            {
                Debug.Log("Create Obj: ARCoreTracker");
                UIDebug.Log("Create Obj: ARCoreTracker");  
                instance = new GameObject("ARCoreTracker", typeof(ARCoreTracker)).GetComponent<ARCoreTracker>();
            }
            return instance;
        }

        public static ARCoreSession aRCoreSession;
        public static ARCoreSession getARCoreSession()
        {
            if (aRCoreSession == null)
            {
                if (getInstance().GetComponent<ARCoreSessionOverride>() != null)
                {
                    aRCoreSession = getInstance().GetComponent<ARCoreSessionOverride>();
                }
                else
                {
                    aRCoreSession = getInstance().gameObject.AddComponent<ARCoreSessionOverride>();

                }
            }
            return aRCoreSession;
        }


        public GameObject ARCamera;

        public Material backgroundMaterial;

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

            ARCoreBackgroundRendererOverride video = ARCamera.AddComponent<ARCoreBackgroundRendererOverride>();
            video.SetMaterial(backgroundMaterial);

            Subcribe();
        }

        void Subcribe()
        {
            ARInterface.onStartSession += OnStartSession;
            ARInterface.onReStartSession += OnRestartSession;
            ARInterface.onStopSession += OnStopSession;
            ARInterface.onChangePaneMode += OnChangePaneMode;

        }

        void UnSubscribe()
        {
            DeviceChange.OnOrientationChange -= OnOrientationChange;
        }

        private void OnChangePaneMode(bool isActive)
        {
            Debug.Log("Unsupported runtime change pane mode");
        }

        bool arSessionIsStarted = false;
        private void OnStartSession()
        {
            if (!arSessionIsStarted)
            {
                LifecycleManager.Instance.CreateSession(getARCoreSession());
                LifecycleManager.Instance.EnableSession();
                arSessionIsStarted = true;
                StartCoroutine(StartARIE());
            }
        }


        private void OnStopSession()
        {
            if (arSessionIsStarted)
            {
                LifecycleManager.Instance.DisableSession();
                LifecycleManager.Instance.ResetSession();
                arSessionIsStarted = false;
                ARInterface.ChangeStatus(ARStatus.Stopped);
            }
        }

        private void OnRestartSession()
        {
            if (arSessionIsStarted)
            {
                LifecycleManager.Instance.DisableSession();
                LifecycleManager.Instance.ResetSession();
                LifecycleManager.Instance.CreateSession(getARCoreSession());
                LifecycleManager.Instance.EnableSession();
            }

        }

        private void OnOrientationChange(DeviceOrientation obj)
        {
            Debug.Log("OrientationChange");
        }

        #region status

        ARUnit.ARTrackingStateReason trackingStateReason(LostTrackingReason state)
        {
            switch (state)
            {
                case LostTrackingReason.None:
                    return ARUnit.ARTrackingStateReason.ARTrackingStateReasonNone;

                case LostTrackingReason.InsufficientLight:
                case LostTrackingReason.InsufficientFeatures:
                    return ARUnit.ARTrackingStateReason.ARTrackingStateReasonInsufficientFeatures;

                case LostTrackingReason.ExcessiveMotion:
                    return ARUnit.ARTrackingStateReason.ARTrackingStateReasonExcessiveMotion;

            }
            return ARUnit.ARTrackingStateReason.ARTrackingStateReasonUnSupported;
        }

        ARUnit.ARTrackingState trackingState(SessionStatus state)
        {

            if (state == SessionStatus.Tracking)
            {
                return ARUnit.ARTrackingState.ARTrackingStateNormal;
            }
            else if (state == SessionStatus.LostTracking)
            {
                return ARUnit.ARTrackingState.ARTrackingStateLimited;
            }
            else if (state == SessionStatus.FatalError || state == SessionStatus.ErrorSessionConfigurationNotSupported ||
                state == SessionStatus.ErrorPermissionNotGranted || state == SessionStatus.ErrorApkNotAvailable)
            {
                return ARUnit.ARTrackingState.ARTrackingStateUnSupported;
            }
            else
            {
                return ARUnit.ARTrackingState.ARTrackingStateNotAvailable;

            }

        }

        SessionStatus lastState;

        LostTrackingReason lastStateReason;

        void UpdateStates()
        {
            if (lastState != Session.Status)
            {
                lastState = Session.Status;
                ARInterface.ChangeTrackingState(trackingState(lastState));
            }
            if (lastStateReason != Session.LostTrackingReason)
            {
                lastStateReason = Session.LostTrackingReason;
                ARInterface.ChangeTrackingStateReason(trackingStateReason(lastStateReason));
            }
        }

        #endregion

        IEnumerator StartARIE()
        {
            ARInterface.ChangeStatus(ARStatus.Initializing);
            while (Session.Status == SessionStatus.Initializing || Session.Status == SessionStatus.None)
            {
                Debug.Log("wait");
                yield return new WaitForEndOfFrame();
            }
            Debug.Log(Session.Status);
            if (Session.Status == SessionStatus.Tracking || Session.Status == SessionStatus.LostTracking)
            {
                ARInterface.ChangeStatus(ARStatus.Running);
                StartCoroutine(UpdateFrameIE());
            }
            else
            {
                ARInterface.ChangeStatus(ARStatus.Failed);
                LifecycleManager.Instance.DisableSession();
                LifecycleManager.Instance.ResetSession();
                arSessionIsStarted = false;
            }
        }

        IEnumerator UpdateFrameIE()
        {
            while (arSessionIsStarted)
            {
                yield return new WaitForEndOfFrame();
                UpdateStates();
                UpdateImages();
                if (ARInterface.ARPaneDetectionEnable)
                    UpdatePlanes();
                UpdateFrame();
            }
        }

        void UpdateFrame()
        {
            Pose pose = Frame.Pose;
            if (pose != null)
            {
                ARTransform transform = new ARTransform(pose.position, pose.rotation);
                ARInterface.UpdateARTransform(transform);
            }
        }

        private Dictionary<int, AugmentedImage> lastImages = new Dictionary<int, AugmentedImage>();
        void UpdateImages()
        {
            List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();
            Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);
            foreach (var image in m_TempAugmentedImages)
            {
                if (lastImages.ContainsKey(image.DatabaseIndex))
                {
                    if (image.TrackingState == TrackingState.Tracking)
                    {
                        ARInterface.ARImageUpdate(new ARImage(image.Name, image.CenterPose.position, image.CenterPose.rotation));
                    }
                }
                else
                {
                    if (image.TrackingState == TrackingState.Tracking)
                    {
                        lastImages.Add(image.DatabaseIndex, image);
                        ARInterface.ARImageAdd(new ARImage(image.Name, image.CenterPose.position, image.CenterPose.rotation));
                    }
                }
                Debug.Log(image.Name);
            }
        }

        void UpdatePlanes()
        {
            List<DetectedPlane> m_TempDetectedPlane = new List<DetectedPlane>();
            Session.GetTrackables<DetectedPlane>(m_TempDetectedPlane, TrackableQueryFilter.New);
            foreach (var pane in m_TempDetectedPlane)
            {
                ARInterface.ARPlaneAdd(new ARPlane(pane.GetHashCode().ToString(), pane.CenterPose.position, pane.CenterPose.rotation, new Vector3(pane.ExtentX, 0, pane.ExtentZ)));
            }
            Session.GetTrackables<DetectedPlane>(m_TempDetectedPlane, TrackableQueryFilter.Updated);
            foreach (var pane in m_TempDetectedPlane)
            {
                ARInterface.ARPlaneUpdate(new ARPlane(pane.GetHashCode().ToString(), pane.CenterPose.position, pane.CenterPose.rotation, new Vector3(pane.ExtentX, 0, pane.ExtentZ)));
            }
        }

    }
}
