using System.Collections;
using System.Collections.Generic;
using ARUnit;
using GPSUnit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PositionUnit.Test
{

	public class PosFullTest : MonoBehaviour
	{
		public RectTransform camRT, pivotRT, a1RT, a2RT, contentRT;
		public EventRecorderPOSU EventRecorder;
		public Transform mapParent;
		public GameObject estimate, fakeGPSPoint;
		public InputField accuracyByMeterIF, maxHorizontalAccuracyIF;
		public Text correctAngleT, angleAccuracyT;
		public Vector2 rectScale;
		public GPSMap mapGPS;

		// Use this for initialization
		void Start()
		{
			PositionUnit.PositionInterface.onARCameraTramsformUpdate += OnARCameraTramsformUpdate;

			accuracyByMeterIF.onEndEdit.AddListener(OnAccuracyByMeterIFChange);
			accuracyByMeterIF.text = ARNSettings.settings.accuracyByMeter.ToString();
			maxHorizontalAccuracyIF.onEndEdit.AddListener(OnMaxHorizontalAccuracyIFChange);
			maxHorizontalAccuracyIF.text = PositionInterface.PositionController.maxHorizontalAccuracy.ToString();

			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener((data) => { OnDragDelegate((PointerEventData) data); });
			fakeGPSPoint.GetComponent<EventTrigger>().triggers.Add(entry);
		}

		void OnAccuracyByMeterIFChange(string text)
		{
			ARNSettings.settings.accuracyByMeter = (float) System.Convert.ToDouble(text);
		}

		void OnMaxHorizontalAccuracyIFChange(string text)
		{
			PositionInterface.PositionController.maxHorizontalAccuracy = (float) System.Convert.ToDouble(text);
		}

		public void OnDragDelegate(PointerEventData data)
		{
			fakeGPSPoint.transform.position = data.position;
		}

		public void FakeGPS(int meters)
		{
			Vector2 pos = fakeGPSPoint.GetComponent<RectTransform>().anchoredPosition / rectScale;
			GPSInfo fakeInfo = GPSUtility.VectorToGPS(new GPSInfo(mapGPS.latitude, mapGPS.longitude, 0), new Vector3(pos.x, 0, pos.y) - mapGPS.localPos);
			fakeInfo.horizontalAccuracy = meters;
			GPSInterface.GPSUpdate(fakeInfo);
		}

		void OnARCameraTramsformUpdate(Vector3 pos, Quaternion rot)
		{
			camRT.anchoredPosition = new Vector2(rectScale.x * pos.x, rectScale.y * pos.z);
			camRT.localRotation = Quaternion.Euler(0, 0, -rot.eulerAngles.y);
		}

		List<RectTransform> estimates = new List<RectTransform>();
		private void Update()
		{
			correctAngleT.text = "correct angle = " + PositionInterface.PositionController.currentAngle.ToString();
			angleAccuracyT.text = "angle accuracy = " + PositionInterface.PositionController.currentAngleAccuracy.ToString();


			Vector3 scale = Vector3.one / contentRT.localScale.x;

			camRT.localScale = scale;
			pivotRT.localScale = scale;
			a1RT.localScale = scale;
			a2RT.localScale = scale;
			fakeGPSPoint.transform.localScale = scale;



			pivotRT.anchoredPosition = new Vector2(PositionInterface.PositionController.currentEstimate.mapPos.x * rectScale.x,
				PositionInterface.PositionController.currentEstimate.mapPos.z * rectScale.y);

			a1RT.anchoredPosition = new Vector2(PositionInterface.PositionController.a1.mapPos.x * rectScale.x,
				PositionInterface.PositionController.a1.mapPos.z * rectScale.y);

			a2RT.anchoredPosition = new Vector2(PositionInterface.PositionController.a2.mapPos.x * rectScale.x,
				PositionInterface.PositionController.a2.mapPos.z * rectScale.y);


			for (int i = 0; i < PositionInterface.PositionController.estimates.Count && i < estimates.Count; i++)
			{
				estimates[i].gameObject.SetActive(true);
				Vector3 pos = PositionInterface.PositionController.estimates[i].mapPos.ToVector3();
				estimates[i].anchoredPosition = new Vector2(rectScale.x * pos.x, rectScale.y * pos.z);
				estimates[i].localScale = scale;
				estimates[i].GetChild(0).localScale = Vector3.one * PositionInterface.PositionController.estimates[i].horizontalAccuracy * 2 / scale.x;
			}

			for (int i = estimates.Count; i < PositionInterface.PositionController.estimates.Count; i++)
			{
				estimates.Add(Instantiate(estimate, mapParent).GetComponent<RectTransform>());
				Vector3 pos = PositionInterface.PositionController.estimates[i].mapPos.ToVector3();
				estimates[i].anchoredPosition = new Vector2(rectScale.x * pos.x, rectScale.y * pos.z);
				estimates[i].localScale = scale;
				estimates[i].GetChild(0).localScale = Vector3.one * PositionInterface.PositionController.estimates[i].horizontalAccuracy * 2 / scale.x;
			}

			for (int i = PositionInterface.PositionController.estimates.Count; i < estimates.Count; i++)
			{
				estimates[i].gameObject.SetActive(false);
			}

		}

		public void OnStartTap()
		{
			ARInterface.StartARSession();
			GPSInterface.StartGPS(1, 1);
			GPSInterface.StartCompass();
		}

		public void OnReStartTap()
		{
			ARInterface.ReStartARSession();
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
}
