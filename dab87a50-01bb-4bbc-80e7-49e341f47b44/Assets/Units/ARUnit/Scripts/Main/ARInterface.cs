using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
    public class ARInterface
    {

        private static ARTransform rawARTransform_p = new ARTransform();
        public static ARTransform rawARTransform
        {
            get
            {
                return rawARTransform_p;
            }
        }

        private static ARStatus ARStatus_p;
        public static ARStatus ARStatus
        {
            get
            {
                return ARStatus_p;
            }
        }

        private static float floorLevel_p = -1.5f;
        public static float floorLevel
        {
            get
            {
                return floorLevel_p;
            }
        }

        public static ARUnit.ARTrackingState ARTrackingState_p;
        public static ARUnit.ARTrackingState ARTrackingState
        {
            get
            {
                return ARTrackingState_p;
            }
        }

        public static ARUnit.ARTrackingStateReason ARTrackingStateReason_p;
        public static ARUnit.ARTrackingStateReason ARTrackingStateReason
        {
            get
            {
                return ARTrackingStateReason_p;
            }
        }

        private static bool ARPaneDetectionEnable_p = true;
        public static bool ARPaneDetectionEnable
        {
            get
            {
                return ARPaneDetectionEnable_p;
            }
        }

        #region NativeEvents

        public delegate void OnARTransformUpdate(ARTransform ARTransform);
        public delegate void OnARCameraProjectionMatrixUpdate(Matrix4x4 ProjectionMatrix);

        public delegate void OnImageAdd(ARImage ARImage);
        public delegate void OnImageUpdate(ARImage ARImage);
        public delegate void OnImageRemoved(ARImage ARImage);

        public delegate void OnPlaneAdd(ARPlane ARPlane);
        public delegate void OnPlaneUpdate(ARPlane ARPlane);
        public delegate void OnPlaneRemoved(ARPlane ARPlane);

        public delegate void OnStatusChange(ARStatus ARStatus);
        public delegate void OnTrackingStateChange(ARUnit.ARTrackingState ARTrackingState);
        public delegate void OnTrackingStateReasonChange(ARUnit.ARTrackingStateReason ARTrackingStateReason);
        public delegate void OnStartSession(); //TODO: add start arSession parametrs
        public delegate void OnReStartSession(); //TODO: add restart arSession parametrs
        public delegate void OnChangePaneMode(bool isActive);
        public delegate void OnStopSession();
        public delegate void OnSessionFaild(string error);



        public static event OnARTransformUpdate onARTransformUpdate;
        public static event OnARCameraProjectionMatrixUpdate onARCameraProjectionMatrixUpdate;

        public static event OnImageUpdate onImageUpdate;
        public static event OnImageAdd onImageAdd;
        public static event OnImageRemoved onImageRemoved;

        public static event OnPlaneUpdate onPlaneUpdate;
        public static event OnPlaneAdd onPlaneAdd;
        public static event OnPlaneRemoved onPlaneRemoved;

        public static event OnStatusChange onStatusChange;
        public static event OnTrackingStateChange onTrackingStateChange;
        public static event OnTrackingStateReasonChange onTrackingStateReasonChange;
        public static event OnStartSession onStartSession;
        public static event OnReStartSession onReStartSession;
        public static event OnChangePaneMode onChangePaneMode;
        public static event OnStopSession onStopSession;
        public static event OnSessionFaild onSessionFaild;

        #endregion

        public delegate void OnFloorLevelUpdate(float floorLevel);
        public static event OnFloorLevelUpdate onFloorLevelUpdate;



        public static void UpdateARTransform(ARTransform ARTransform)
        {
            rawARTransform_p = ARTransform;

            if (onARTransformUpdate != null)
                onARTransformUpdate.Invoke(ARTransform);
        }

        #region ARImage

        public static void ARImageUpdate(ARImage ARImage)
        {
            if (onImageUpdate != null && ARStatus_p == ARStatus.Running)
                onImageUpdate.Invoke(ARImage);
        }

        public static void ARImageAdd(ARImage ARImage)
        {
            if (onImageAdd != null && ARStatus_p == ARStatus.Running)
                onImageAdd.Invoke(ARImage);
        }

        public static void ARImageRemove(ARImage ARImage)
        {
            if (onImageRemoved != null && ARStatus_p == ARStatus.Running)
                onImageRemoved.Invoke(ARImage);
        }

        #endregion

        #region ARPlane

        public static void ARPlaneUpdate(ARPlane ARPlane)
        {
            if (onPlaneUpdate != null && ARStatus_p == ARStatus.Running)
                onPlaneUpdate.Invoke(ARPlane);
        }

        public static void ARPlaneAdd(ARPlane ARPlane)
        {
            if (onPlaneAdd != null && ARStatus_p == ARStatus.Running)
                onPlaneAdd.Invoke(ARPlane);
        }

        public static void ARPlaneRemove(ARPlane ARPlane)
        {
            if (onPlaneRemoved != null && ARStatus_p == ARStatus.Running)
                onPlaneRemoved.Invoke(ARPlane);
        }

        #endregion


        public static bool isSupport()
        {
#if UNITY_IOS
            return UnityEngine.XR.iOS.UnityARSessionNativeInterface.IsARKit_1_5_Supported();

#elif UNITY_ANDROID
			return PlayerPrefs.HasKey("ARCoreIsSupport") && PlayerPrefs.GetInt("ARCoreIsSupport") == 1;
#else
			return false;
#endif
        }

        public static void RequestCheckAndroidSupport(System.Action collback)
        {
#if UNITY_ANDROID
			ARCoreInstallChecker.Instance.Check(collback);
#else
            collback.Invoke();
#endif
        }

        public static void ChangeStatus(ARStatus ARStatus)
        {
            ARStatus_p = ARStatus;
            if (onStatusChange != null)
                onStatusChange.Invoke(ARStatus);
        }

        public static void ChangeTrackingState(ARUnit.ARTrackingState ARTrackingState)
        {
            ARTrackingState_p = ARTrackingState;
            if (onTrackingStateChange != null)
                onTrackingStateChange.Invoke(ARTrackingState);
        }

        public static void ChangeTrackingStateReason(ARUnit.ARTrackingStateReason ARTrackingStateReason)
        {
            ARTrackingStateReason_p = ARTrackingStateReason;
            if (onTrackingStateReasonChange != null)
                onTrackingStateReasonChange.Invoke(ARTrackingStateReason);
        }

        public static void StartARSession()
        {
            if (onStartSession != null)
                onStartSession.Invoke();
        }

        public static void ReStartARSession()
        {
            if (onReStartSession != null)
                onReStartSession.Invoke();
        }

        public static void ChangePaneDetectionMode(bool isActive)
        {
            ARPaneDetectionEnable_p = isActive;
            if (onChangePaneMode != null)
                onChangePaneMode.Invoke(isActive);
        }

        public static void StopARSession()
        {
            if (onStopSession != null)
                onStopSession.Invoke();
        }

        public static void SessionFaildEvent(string error)
        {
            if (onSessionFaild != null)
                onSessionFaild.Invoke(error);
        }

        public static void UpdateCameraProjectionMatrix(Matrix4x4 Matrix4x4)
        {
            if (onARCameraProjectionMatrixUpdate != null)
                onARCameraProjectionMatrixUpdate.Invoke(Matrix4x4);
        }

        public static void UpdateFloorLevel(float floorLevel)
        {
            if (onFloorLevelUpdate != null)
                onFloorLevelUpdate.Invoke(floorLevel);
        }
    }
}
