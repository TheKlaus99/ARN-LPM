using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPSUnit.Test
{
	public class GPSStarter_test : MonoBehaviour
	{
		public UnityEngine.UI.Text infoText, statusText;
		public RectTransform compassImage;
		public GPSToEstimate GPSToEstimate;
		public Transform mapParent;
		public GameObject estimatePref;


		private void Awake()
		{
			GPSInterface.onGPSUpdate += GPSUpdate;
			GPSInterface.onGPSCompassUpdate += CompassUpdate;
			GPSToEstimate.onEstimateGenerate += GenerateEstimate;
			GPSInterface.onGPSStatusUpdate += UpdateGPSStatus;
			GPSInterface.onStartGPS += StartGPS;
			GPSInterface.onStopGPS += StopGPS;
			GPSInterface.onStartCompass += StartCompass;
			GPSInterface.onStopCompass += StopCompass;
		}

		#region UI
		public void StartGPSTap()
		{
			GPSInterface.StartGPS(1, 1);
			GPSInterface.StartCompass();
		}

		public void StopGPSTap()
		{
			GPSInterface.StopGPS();
			GPSInterface.StopCompass();
		}

		void printInfo(GPSInfo info)
		{
			infoText.text = info.ToString();
		}

		void printStatus(GPSServiceStatus status)
		{
			statusText.text = status.ToString();
		}
		#endregion


		void InstantiateEst(PositionUnit.Estimate estimate)
		{
			GameObject go = Instantiate(estimatePref, mapParent);
			go.transform.localPosition = estimate.realPos.ToVector3();
			go.transform.localScale = Vector3.one * estimate.horizontalAccuracy;
		}

		#region events

		void GPSUpdate(GPSInfo info)
		{
			printInfo(info);
			Debug.Log("GPSUpdate\n" + info.ToString());
		}

		void CompassUpdate(GPSCompassInfo info)
		{
			compassImage.localRotation = Quaternion.Euler(0, 0, -info.trueHeading);
		}

		void GenerateEstimate(PositionUnit.Estimate estimate)
		{
			InstantiateEst(estimate);
			Debug.Log("GenerateEstimate\n" + estimate.ToString());
		}

		void UpdateGPSStatus(GPSServiceStatus status)
		{
			printStatus(status);
			Debug.Log("UpdateGPSStatus\n" + status.ToString());
		}

		void StartGPS(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			Debug.Log("StartGPS\ndesiredAccuracyInMeters = " + desiredAccuracyInMeters + "\ndesiredAccuracyInMeters = " + desiredAccuracyInMeters);
		}

		void StopGPS()
		{
			Debug.Log("StopGPS");
		}

		void StartCompass()
		{
			Debug.Log("StartCompass");
		}

		void StopCompass()
		{
			Debug.Log("StopCompass");
		}
		#endregion
	}
}
