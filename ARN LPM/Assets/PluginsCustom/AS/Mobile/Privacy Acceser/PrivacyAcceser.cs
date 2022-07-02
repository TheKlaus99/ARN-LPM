using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Android;



public class PrivacyAcceser : MonoBehaviour
{

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern bool _CheckPermission(int type);

    [DllImport("__Internal")]
    private static extern bool _CheckPermissionNotDetermined(int Type);

    [DllImport("__Internal")]
    private static extern void _RequestPermission(int type);
#endif

    public static bool CheckCameraAccess()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IOS
        return _CheckPermission(0);
#elif UNITY_ANDROID
        return AndroidPermissionsManager.IsPermissionGranted(AndroidPermissionsManager.AndroidPermission.CAMERA);

#endif
    }

    public static bool CheckCameraNotDetermined()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_IOS
        return _CheckPermissionNotDetermined(0);
#elif UNITY_ANDROID
        return !PlayerPrefs.HasKey("CheckCameraNotDeterminedAndroid");
#endif
    }

    static int requestCameraAccessResult = 0;
    public static void RequestCameraAccess(System.Action<bool> collback)
    {
        Debug.Log("RequestCameraAccess");
#if UNITY_EDITOR
        collback.Invoke(true);
#elif UNITY_IOS
        if (CheckCameraNotDetermined())
        {
            instance.StartCoroutine(instance.RequestCameraCallbackIE(collback));
            _RequestPermission(0);
        }
        else
        {
            collback.Invoke(CheckCameraAccess());
        }

#elif UNITY_ANDROID
        Debug.Log("CheckCameraNotDetermined = " + CheckCameraNotDetermined());
        requestCameraAccessResult = 0;
        if (CheckCameraNotDetermined())
        {
            instance.StartCoroutine(instance.RequestCameraCallbackIE(collback));
            AndroidPermissionsManager.RequestPermission(AndroidPermissionsManager.AndroidPermission.CAMERA,
                new AndroidPermissionCallback(
                    grantedPermission => requestCameraAccessResult = 1,
                    deniedPermission => requestCameraAccessResult = 2,
                    deniedPermissionAndDontAskAgain => requestCameraAccessResult = 3
                )
            );
        }
        else
        {
            collback.Invoke(CheckCameraAccess());
        }
#endif
    }


    public static bool CheckLocationAccess()
    {
#if UNITY_EDITOR
        return true;
#elif UNITY_IOS
        return _CheckPermission(2);
#elif UNITY_ANDROID
        return AndroidPermissionsManager.IsPermissionGranted(AndroidPermissionsManager.AndroidPermission.ACCESS_FINE_LOCATION);
#endif
    }

    public static bool CheckLocationNotDetermined()
    {
#if UNITY_EDITOR
        return false;
#elif UNITY_IOS
        return _CheckPermissionNotDetermined(2);
#elif UNITY_ANDROID
        return !PlayerPrefs.HasKey("CheckLocationNotDeterminedAndroid");
#endif
    }

    static int requestLocationAccessResult = 0;
    public static void RequestLocationAccess(System.Action<bool> collback)
    {
#if UNITY_EDITOR
        collback.Invoke(true);
#elif UNITY_IOS
        if (CheckLocationNotDetermined())
        {
            instance.StartCoroutine(instance.RequestLocationCallbackIE(collback));
            Input.location.Start();
            Input.location.Stop();
        }
        else
        {
            collback.Invoke(CheckLocationAccess());
        }
#elif UNITY_ANDROID
        if (CheckLocationNotDetermined())
        {
            requestLocationAccessResult = 0;
            instance.StartCoroutine(instance.RequestLocationCallbackIE(collback));
            AndroidPermissionsManager.RequestPermission(AndroidPermissionsManager.AndroidPermission.ACCESS_FINE_LOCATION,
                new AndroidPermissionCallback(
                    grantedPermission => requestLocationAccessResult = 1,
                    deniedPermission => requestLocationAccessResult = 2,
                    deniedPermissionAndDontAskAgain => requestLocationAccessResult = 3
                )
            );
        }
        else
        {
            collback.Invoke(CheckLocationAccess());
        }
#endif
    }


    private static PrivacyAcceser istance_p;
    private static PrivacyAcceser instance
    {
        get
        {
            if (istance_p == null)
            {
                GameObject go = new GameObject(typeof(PrivacyAcceser).Name);
                istance_p = go.AddComponent<PrivacyAcceser>();
            }
            return istance_p;
        }
    }

    private void Awake()
    {
        if (istance_p == null)
            istance_p = this;

        DontDestroyOnLoad(gameObject);
    }


    public IEnumerator RequestCameraCallbackIE(System.Action<bool> collback)
    {
#if UNITY_IOS
        while (CheckCameraNotDetermined())
        {
            yield return new WaitForSeconds(.1f);
        }
        collback.Invoke(CheckCameraAccess());
#elif UNITY_ANDROID
        while (requestCameraAccessResult == 0)
        {
            yield return new WaitForSeconds(.1f);
        }
        if (requestCameraAccessResult == 1)
        {
            PlayerPrefs.SetInt("CheckCameraNotDeterminedAndroid", 1);
            collback.Invoke(true);
        }
        else if (requestCameraAccessResult == 2)
        {
            if (PlayerPrefs.HasKey("CheckCameraNotDeterminedAndroid"))
                PlayerPrefs.DeleteKey("CheckCameraNotDeterminedAndroid");
            collback.Invoke(false);
        }
        else
        {
            PlayerPrefs.SetInt("CheckCameraNotDeterminedAndroid", 1);
            collback.Invoke(false);
        }
#else
        yield return null;
#endif
    }

    public IEnumerator RequestLocationCallbackIE(System.Action<bool> collback)
    {
#if UNITY_IOS
        while (CheckLocationNotDetermined())
        {
            yield return new WaitForSeconds(.1f);
        }
        collback.Invoke(CheckLocationAccess());
#elif UNITY_ANDROID
        while (requestLocationAccessResult == 0)
        {
            yield return new WaitForSeconds(.1f);
        }
        if (requestLocationAccessResult == 1)
        {
            PlayerPrefs.SetInt("CheckLocationNotDeterminedAndroid", 1);
            collback.Invoke(true);
        }
        else if (requestLocationAccessResult == 2)
        {
            if (PlayerPrefs.HasKey("CheckLocationNotDeterminedAndroid"))
                PlayerPrefs.DeleteKey("CheckLocationNotDeterminedAndroid");
            collback.Invoke(false);
        }
        else
        {
            PlayerPrefs.SetInt("CheckLocationNotDeterminedAndroid", 1);
            collback.Invoke(false);
        }
#else
        yield return null;
#endif
    }

}
