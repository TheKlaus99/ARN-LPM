using System;
using System.Collections;
using System.Collections.Generic;
using ARUnit;
using GPSUnit;
using PositionUnit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PosLPMHotPoint : MonoBehaviour
{
    public RectTransform pivotRT, a1RT, a2RT, gpsPoint;
    public PositionUnit.Test.EventRecorderPOSU EventRecorder;
    public GameObject fakeGPSPoint;
    public InputField accuracyByMeterIF, maxHorizontalAccuracyIF;
    public Text correctAngleT, angleAccuracyT, camPos;

    Vector2 rectScale; //TODO: Debug

    GPSMap mapGPS;

    public MapController MapController;

    void Start()
    {
        Application.targetFrameRate = 60;
        PositionUnit.PositionInterface.onARCameraTramsformUpdate += OnARCameraTramsformUpdate;
        PositionUnit.PositionInterface.onARRAWCameraTramsformUpdate += OnARCameraRAWTramsformUpdate;
        GPSInterface.onGPSUpdate += onGPSUpdate;

        accuracyByMeterIF.onEndEdit.AddListener(OnAccuracyByMeterIFChange);
        accuracyByMeterIF.text = ARNSettings.settings.accuracyByMeter.ToString();
        maxHorizontalAccuracyIF.onEndEdit.AddListener(OnMaxHorizontalAccuracyIFChange);
        maxHorizontalAccuracyIF.text = PositionInterface.PositionController.maxHorizontalAccuracy.ToString();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData)data); });
        fakeGPSPoint.GetComponent<EventTrigger>().triggers.Add(entry);


        rectScale = ARNSettings.settings.pixelsInMeter;
        mapGPS = ARNSettings.settings.GPSMap;
    }

    public void ChangeCorrectTypeDD(int val)
    {
        //TODO: remove this
        //PositionUnit.PositionInterface.PositionController.correctType = val;
    }

    private void onGPSUpdate(GPSInfo info)
    {
        Vector3 p = GPSUtility.GPSToVector(new GPSInfo(mapGPS.latitude, mapGPS.longitude, 0), info);

        MapController.UpdateGPS(mapGPS.localPos + p, info.horizontalAccuracy);
        /*gpsPoint.anchoredPosition = new Vector2((mapGPS.localPos.x + p.x) * rectScale.x, (mapGPS.localPos.z + p.z) * rectScale.y);
		gpsPoint.GetChild(0).transform.localScale = Vector3.one * info.horizontalAccuracy * 2;*/
    }

    void OnAccuracyByMeterIFChange(string text)
    {
        ARNSettings.settings.accuracyByMeter = (float)System.Convert.ToDouble(text);
    }

    void OnMaxHorizontalAccuracyIFChange(string text)
    {
        PositionInterface.PositionController.maxHorizontalAccuracy = (float)System.Convert.ToDouble(text);
    }

    public void OnDragDelegate(PointerEventData data)
    {
        fakeGPSPoint.transform.position = data.position;
    }

    public void FakeGPS(int meters)
    {
        GPSInterface.UpdateGPSStatus(GPSServiceStatus.Running);
        Vector2 pos = fakeGPSPoint.GetComponent<RectTransform>().anchoredPosition / rectScale;
        GPSInfo fakeInfo = GPSUtility.VectorToGPS(new GPSInfo(mapGPS.latitude, mapGPS.longitude, 0), new Vector3(pos.x, 0, pos.y) - mapGPS.localPos);
        fakeInfo.horizontalAccuracy = meters;
        GPSInterface.GPSUpdate(fakeInfo);
    }

    private void Update()
    {
        correctAngleT.text = "correct angle = " + PositionInterface.PositionController.currentAngle.ToString();
        angleAccuracyT.text = "angle accuracy = " + PositionInterface.PositionController.currentAngleAccuracy.ToString();

        pivotRT.anchoredPosition = new Vector2(PositionInterface.PositionController.pivot.mapPos.x * rectScale.x,
            PositionInterface.PositionController.pivot.mapPos.z * rectScale.y);

        a1RT.anchoredPosition = new Vector2(PositionInterface.PositionController.a1.mapPos.x * rectScale.x,
            PositionInterface.PositionController.a1.mapPos.z * rectScale.y);

        a2RT.anchoredPosition = new Vector2(PositionInterface.PositionController.a2.mapPos.x * rectScale.x,
            PositionInterface.PositionController.a2.mapPos.z * rectScale.y);


    }

    void OnARCameraRAWTramsformUpdate(Vector3 pos, Quaternion rot)
    {
        camPos.text = "camPos = (" + (int)pos.x + "; " + (int)pos.y + "; " + (int)pos.z + ")";
    }

    void OnARCameraTramsformUpdate(Vector3 pos, Quaternion rot)
    {
        MapController.UpdateTransform(pos, rot);
    }

    public void OnStartGPSTap()
    {
        GPSInterface.StartGPS(1, 1);
        GPSInterface.StartCompass();
    }

    public void OnStartARTap()
    {
        ARInterface.StartARSession();
    }

    public void OnShareTap()
    {
        if (EventRecorder != null && EventRecorder.e != null)
        {
            BinarySaver.Save(EventRecorder.e, Application.persistentDataPath + "/eventsData");
            AS.IOS.Native.ShareDialog(Application.persistentDataPath + "/eventsData", "eventsData", "eventsData", 0, 0);
        }
    }
}
