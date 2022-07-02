using System;
using System.Collections;
using System.Collections.Generic;
using GPSUnit;
using UnityEngine;


public class ErrorUIController : MonoBehaviour
{
    public GameObject content, info, cameraContent, locationContent, cameraButton, locationButton;

    public GameObject[] cameraText;
    public GameObject[] locationText;
    public Animator animator;

    public MainUIController mainUIController;


    [SerializeField] bool locationDetermined;
    [SerializeField] bool locationAccesed;
    [SerializeField] bool cameraDetermined;
    [SerializeField] bool cameraAccesed;
    // Start is called before the first frame update
    void Awake()
    {
        UpdateStatus();
    }

    void UpdateStatus()
    {
#if !UNITY_EDITOR
        locationDetermined = !PrivacyAcceser.CheckLocationNotDetermined(); //был ли запрос
        locationAccesed = locationDetermined && PrivacyAcceser.CheckLocationAccess();
        cameraDetermined = !PrivacyAcceser.CheckCameraNotDetermined(); //был ли запрос
        cameraAccesed = cameraDetermined && PrivacyAcceser.CheckCameraAccess();
#endif
        mainUIController.SetStateByPrivacy();

        content.SetActive(!(cameraAccesed && locationAccesed));

    }


    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            UpdateInfo();
            //PrivacyAcceser.ChackCameraAcces(result => cameraFail = result);
            // content.SetActive(locationFail || cameraFail);
        }
    }

    void UpdateInfo()
    {
        UpdateStatus();
        if (cameraAccesed && locationAccesed)
        {
            info.SetActive(false);
        }
        else if (info.activeSelf)
        {
            UpdateInfoContent();
        }
    }

    void UpdateInfoContent()
    {
        cameraButton.SetActive(!cameraDetermined);
        cameraText[cameraDetermined ? 0 : 1].SetActive(false);
        cameraText[cameraDetermined ? 1 : 0].SetActive(true);
        cameraContent.SetActive(!cameraAccesed);

        locationButton.SetActive(!locationDetermined);
        locationText[locationDetermined ? 0 : 1].SetActive(false);
        locationText[locationDetermined ? 1 : 0].SetActive(true);
        locationContent.SetActive(!locationAccesed);
    }

    public void OnTap()
    {
        UpdateInfoContent();
        info.SetActive(true);
    }

    public void OnTapCloseInfo()
    {
        info.SetActive(false);
        UpdateStatus();
    }

    public void OnTapCamAccess()
    {
        PrivacyAcceser.RequestCameraAccess(result =>
        {
            UpdateInfo();
        });
    }

    public void OnTapLocationAccess()
    {
        PrivacyAcceser.RequestLocationAccess(result =>
        {
            UpdateInfo();
        });
    }

    void OnAlertCompleate(int result)
    {
        Debug.Log("OnAlertCompleate " + result);
    }
}
