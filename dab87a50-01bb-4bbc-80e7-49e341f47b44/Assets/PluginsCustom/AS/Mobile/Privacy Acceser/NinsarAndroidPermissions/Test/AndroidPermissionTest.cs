using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidPermissionTest : MonoBehaviour
{
    public Text text;
    public void OnTapGetPermission()
    {
        AndroidPermissionsManager.RequestPermission(AndroidPermissionsManager.AndroidPermission.CAMERA,
        new AndroidPermissionCallback
        (
            grantedPermission =>
            {
                OnAllow();
            },
            deniedPermission =>
            {
                OnDeny();
            },
            deniedPermissionAndDontAskAgain =>
            {
                OnDenyAndNeverAskAgain();
            }
        )
        );

    }

    private void OnDenyAndNeverAskAgain()
    {
        text.text = "DenyAndNeverAskAgain";
    }

    private void OnDeny()
    {
        text.text = "Deny";
    }

    private void OnAllow()
    {
        text.text = "Allow";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
