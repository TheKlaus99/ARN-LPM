using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

namespace ARUnit
{


    public class ARCoreInstallChecker : MonoBehaviour
    {
        static ARCoreInstallChecker instance = null;
        public static ARCoreInstallChecker Instance
        {
            get
            {
                if (!instance)
                {
                    instance = new GameObject("ARCoreInstallChecker").AddComponent<ARCoreInstallChecker>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        public void Check(System.Action collback)
        {
            StartCoroutine(CheckIE(collback));
        }

        IEnumerator CheckIE(System.Action collback)
        {
            AsyncTask<ApkAvailabilityStatus> checkTask = Session.CheckApkAvailability();
            CustomYieldInstruction customYield = checkTask.WaitForCompletion();
            yield return customYield;
            ApkAvailabilityStatus result = checkTask.Result;
            switch (result)
            {
                case ApkAvailabilityStatus.SupportedApkTooOld:
                    _ShowAndroidToastMessage("Supported apk too old");
                    SetSupported(false);
                    break;
                case ApkAvailabilityStatus.SupportedInstalled:
                    _ShowAndroidToastMessage("Supported and installed");
                    SetSupported(true);
                    break;
                case ApkAvailabilityStatus.SupportedNotInstalled:
                    _ShowAndroidToastMessage("Supported, not installed, requesting installation");
                    SetSupported(false);
                    break;
                case ApkAvailabilityStatus.UnknownChecking:
                    _ShowAndroidToastMessage("Unknown Checking");
                    SetSupported(false);
                    break;
                case ApkAvailabilityStatus.UnknownError:
                    _ShowAndroidToastMessage("Unknown Error");
                    SetSupported(false);
                    break;
                case ApkAvailabilityStatus.UnknownTimedOut:
                    _ShowAndroidToastMessage("Unknown Timed out");
                    SetSupported(false);
                    break;
                case ApkAvailabilityStatus.UnsupportedDeviceNotCapable:
                    _ShowAndroidToastMessage("Unsupported Device Not Capable");
                    SetSupported(false);
                    break;
            }
            collback.Invoke();
        }

        void SetSupported(bool supported)
        {
            PlayerPrefs.SetInt("ARCoreIsSupport", supported?1 : 0);
        }

        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject =
                        toastClass.CallStatic<AndroidJavaObject>(
                            "makeText", unityActivity, message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
