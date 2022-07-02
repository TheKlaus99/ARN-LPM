using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCoreInternal;
using UnityEngine;
using UnityEngine.UI;


public class TestController : MonoBehaviour
{
    public ARCoreSession ARCoreSession;
    public Text statusT, trackingReasonT, posT, rotT;

    bool statusStart = false;
    public void StartAR()
    {
        if (!statusStart)
        {
            LifecycleManager.Instance.CreateSession(ARCoreSession);
            LifecycleManager.Instance.EnableSession();
        }
        statusStart = true;

    }

    public void StopAR()
    {
        if (statusStart)
        {
            LifecycleManager.Instance.DisableSession();
            LifecycleManager.Instance.ResetSession();
        }
        statusStart = false;
    }

    public void RestartAR1()
    {
        StartCoroutine(RestartIE());
    }

    IEnumerator RestartIE()
    {
        LifecycleManager.Instance.DisableSession();
        yield return new WaitForEndOfFrame();
        LifecycleManager.Instance.EnableSession();
    }

    public void RestartAR2()
    {
        StopAR();
        StartAR();
    }


    private void Update()
    {
        statusT.text = string.Format("Status: {0}", Session.Status.ToString());
        trackingReasonT.text = string.Format("Tracking reason: {0}", Session.LostTrackingReason.ToString());
        posT.text = string.Format("pos: {0}", Frame.Pose.position.ToString("0.00"));
        rotT.text = string.Format("rot: {0}", Frame.Pose.rotation.eulerAngles.ToString("0.00"));
    }
}
