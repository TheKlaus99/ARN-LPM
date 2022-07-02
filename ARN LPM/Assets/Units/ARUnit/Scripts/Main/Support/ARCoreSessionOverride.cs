using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class ARCoreSessionOverride : ARCoreSession
{
    public new void Awake()
    {
        if (SessionConfig != null &&
            SessionConfig.LightEstimationMode != LightEstimationMode.Disabled &&
            Object.FindObjectsOfType<EnvironmentalLight>().Length == 0)
        {
            Debug.Log("Light Estimation may not work properly when EnvironmentalLight is not" +
                " attached to the scene.");
        }
    }

    public new void OnEnable() { }

    public new void OnDisable() { }

    public new void OnDestroy() { }


}
