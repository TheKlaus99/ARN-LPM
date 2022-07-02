using System.Collections;
using System.Collections.Generic;
using PositionUnit;
using UnityEngine;
using UnityEngine.UI;


public class GPSHelpTextController : MonoBehaviour
{

    public Text startARHelpT;

    [TextArea(0, 5)]
    public string waitGPSAcc, waitGPAAngle, GPSDisable, GPSTooFar;

    public void ChangeGPSTextWhenARStarted(float gpsAcc = 100)
    {

        if (PrivacyAcceser.CheckLocationAccess())
        {
            if (gpsAcc > 25)
            {
                startARHelpT.text = string.Format(waitGPSAcc, (int)gpsAcc);
            }
            else if (gpsAcc == -1)
            {
                startARHelpT.text = GPSTooFar;
            }
            else if (PositionInterface.PositionController.currentAngleAccuracy > ARNSettings.settings.minAngleAccuracyToStart)
            {
                startARHelpT.text = string.Format(waitGPAAngle, (int)(PositionInterface.PositionController.currentAngleAccuracy - ARNSettings.settings.minAngleAccuracyToStart));
            }
        }
        else
        {
            startARHelpT.text = GPSDisable;
        }
    }
}
