using System.Collections;
using System.Collections.Generic;
using GPSUnit;
using UnityEngine;

namespace PositionUnit.Test
{
	public class FakeEvents : MonoBehaviour
	{


		[Header("ARInterface.UpdateARTransform")]
		public Transform Camera;

		[Header("GPSInterface.GPSUpdate")]
		public Transform pointReal, pointLocal;
		public GPSMap mapGPS;

		[Header("ARInterface.ARImageAdd")]
		public Transform pointImage;
		public string ARImageName;


		[Space]
		[Header("EventRecorder")]
		public string EventRecorderPath;
		public EventRecorderPOSU EventRecorder;


		public void FakeEventsFromRecord()
		{
			if (EventRecorder != null)
			{
				if (string.IsNullOrEmpty(EventRecorderPath))
					EventRecorderPath = Application.persistentDataPath + "/eventsData";
				EventRecorder.Play((EventRecord) BinarySaver.Load(EventRecorderPath));
			}
		}

		public void FakeGPS()
		{
			GPSInfo fakeInfo = GPSUtility.VectorToGPS(new GPSInfo(mapGPS.latitude, mapGPS.longitude, 0), pointReal.localPosition - mapGPS.localPos);
			GPSInterface.GPSUpdate(fakeInfo);
		}

		public void FakeARImageAdd()
		{
			ARUnit.ARImage ARImage = new ARUnit.ARImage(ARImageName, pointImage.localPosition, pointImage.localRotation);
			ARUnit.ARInterface.ARImageAdd(ARImage);
		}

		public void FakeARImageUpdate()
		{
			ARUnit.ARImage ARImage = new ARUnit.ARImage(ARImageName, pointImage.localPosition, pointImage.localRotation);
			ARUnit.ARInterface.ARImageUpdate(ARImage);
		}

		private void Update()
		{
			if (Camera != null)
			{
				ARUnit.ARInterface.UpdateARTransform(new ARUnit.ARTransform(Camera.localPosition, Camera.localRotation));
			}
		}
	}
}
