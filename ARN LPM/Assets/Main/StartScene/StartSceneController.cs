using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{

    public Animator accesRequester;
    bool checkRequestEnd = false;
    bool needToLoad = false;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        ARUnit.ARInterface.RequestCheckAndroidSupport(AndroidSupportCollback);
#endif
        if (!PlayerPrefs.HasKey("FirstStart"))
        {
            PlayerPrefs.SetInt("FirstStart", 1);
            accesRequester.SetTrigger("Open");
        }
        else
        {
            StartCoroutine(LoadIE());
        }
    }

    IEnumerator LoadIE()
    {
        yield return new WaitForSeconds(0.5f);
        GoToMain();
    }

#if UNITY_ANDROID
    private void AndroidSupportCollback()
    {
        checkRequestEnd = true;
        if (needToLoad)
        {
            GoToMain();
        }
    }
#endif


    public void OnTapCam()
    {
        Debug.Log("Tap request button");
        PrivacyAcceser.RequestCameraAccess(result =>
        {
            Debug.Log("start collBack. result = " + result);
            accesRequester.SetTrigger(result ? "CameraAccesed" : "CameraDenied");
            OnEnd();
        });
    }


    public void OnTapLoc()
    {
        Debug.Log("LocRequest");

        PrivacyAcceser.RequestLocationAccess(result =>
        {
            accesRequester.SetTrigger(result ? "LocationAccesed" : "LocationDenied");
            OnEnd();
        });
    }


    void OnEnd()
    {
        if (!(PrivacyAcceser.CheckCameraNotDetermined() || PrivacyAcceser.CheckLocationNotDetermined()))
        {
            if (PrivacyAcceser.CheckCameraAccess() && PrivacyAcceser.CheckLocationAccess())
            {
                GoToMain();
            }
            else
            {
                accesRequester.SetTrigger("NextButton");
            }
        }
    }

    public void OnTapSkip()
    {
        GoToMain();
    }

    void GoToMain()
    {
        needToLoad = true;
#if UNITY_ANDROID
        if (checkRequestEnd)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
#else
        SceneManager.LoadScene(1, LoadSceneMode.Single);
#endif
    }

    public void OnTapUpdate()
    {
        UIDebug.Log(PrivacyAcceser.CheckCameraAccess().ToString() + " - " + PrivacyAcceser.CheckLocationAccess().ToString());
    }

}
