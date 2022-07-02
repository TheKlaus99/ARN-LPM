using System;
using UnityEngine;

public class AndroidPermissionCallback : AndroidJavaProxy
{
    private event Action<string> OnPermissionGrantedAction;
    private event Action<string> OnPermissionDeniedAction;
    private event Action<string> OnPermissionDeniedAndDontAskAgainAction;

    public AndroidPermissionCallback(Action<string> onGrantedCallback, Action<string> onDeniedCallback, Action<string> onDeniedAndDontAskAgainCallback)
        : base("com.unity3d.plugin.NinsarAndroidPermissions$IPermissionRequestResult2")
    {
        if (onGrantedCallback != null)
        {
            OnPermissionGrantedAction += onGrantedCallback;
        }
        if (onDeniedCallback != null)
        {
            OnPermissionDeniedAction += onDeniedCallback;
        }
        if (onDeniedAndDontAskAgainCallback != null)
        {
            OnPermissionDeniedAndDontAskAgainAction += onDeniedAndDontAskAgainCallback;
        }
    }

    // Handle permission granted
    public virtual void OnPermissionGranted(string permissionName)
    {
        //Debug.Log("Permission " + permissionName + " GRANTED");
        if (OnPermissionGrantedAction != null)
        {
            OnPermissionGrantedAction(permissionName);
        }
    }

    // Handle permission denied
    public virtual void OnPermissionDenied(string permissionName)
    {
        //Debug.Log("Permission " + permissionName + " DENIED!");
        if (OnPermissionDeniedAction != null)
        {
            OnPermissionDeniedAction(permissionName);
        }
    }

    // Handle permission denied and 'Dont ask again' selected
    // Note: falls back to OnPermissionDenied() if action not registered
    public virtual void OnPermissionDeniedAndDontAskAgain(string permissionName)
    {
        //Debug.Log("Permission " + permissionName + " DENIED and 'Dont ask again' was selected!");
        if (OnPermissionDeniedAndDontAskAgainAction != null)
        {
            OnPermissionDeniedAndDontAskAgainAction(permissionName);
        }
        else if (OnPermissionDeniedAction != null)
        {
            // Fall back to OnPermissionDeniedAction
            OnPermissionDeniedAction(permissionName);
        }
    }
}

public class AndroidPermissionsManager
{
    private static AndroidJavaObject m_Activity;
    private static AndroidJavaObject m_PermissionService;

    private static AndroidJavaObject GetActivity()
    {
        if (m_Activity == null)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_Activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
        return m_Activity;
    }

    private static AndroidJavaObject GetPermissionsService()
    {
        return m_PermissionService ??
            (m_PermissionService = new AndroidJavaObject("com.unity3d.plugin.NinsarAndroidPermissions"));
    }

    public static bool IsPermissionGranted(AndroidPermission permission)
    {
        return GetPermissionsService().Call<bool>("IsPermissionGranted", GetActivity(), GetPermissionStr(permission));
    }

    public static void RequestPermission(AndroidPermission permission, AndroidPermissionCallback callback)
    {
        RequestPermission(new[] { GetPermissionStr(permission) }, callback);
    }

    public static void RequestPermission(string[] permissionNames, AndroidPermissionCallback callback)
    {
        GetPermissionsService().Call("RequestPermissionAsync", GetActivity(), permissionNames, callback);
    }

    private static string GetPermissionStr(AndroidPermission permission)
    {
        return "android.permission." + permission.ToString();
    }

    public enum AndroidPermission
    {
        ACCESS_COARSE_LOCATION,
        ACCESS_FINE_LOCATION,
        ADD_VOICEMAIL,
        BODY_SENSORS,
        CALL_PHONE,
        CAMERA,
        GET_ACCOUNTS,
        PROCESS_OUTGOING_CALLS,
        READ_CALENDAR,
        READ_CALL_LOG,
        READ_CONTACTS,
        READ_EXTERNAL_STORAGE,
        READ_PHONE_STATE,
        READ_SMS,
        RECEIVE_MMS,
        RECEIVE_SMS,
        RECEIVE_WAP_PUSH,
        RECORD_AUDIO,
        SEND_SMS,
        USE_SIP,
        WRITE_CALENDAR,
        WRITE_CALL_LOG,
        WRITE_CONTACTS,
        WRITE_EXTERNAL_STORAGE
    }
}
