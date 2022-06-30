using System.Collections;
using System.Collections.Generic;
using ARUnit;
using GPSUnit;
using UnityEngine;
using UnityEngine.UI;

namespace PositionUnit.Test
{

	public class PosGPSTest : MonoBehaviour
	{
		public RectTransform camRT;
		public Transform mapParent;
		public GameObject gps;
		public Vector2 rectScale;
		public Text infoText, statusText;
		public GPSMap GPSMap;

		// Use this for initialization
		void Start()
		{
			PositionUnit.PositionInterface.onARCameraTramsformUpdate += OnARCameraTramsformUpdate;
			GPSInterface.onGPSUpdate += GPSUpdate;
			GPSToEstimate.onEstimateGenerate += GenerateEstimate;
			GPSInterface.onGPSStatusUpdate += UpdateGPSStatus;
			GPSInterface.onStartGPS += StartGPS;
			GPSInterface.onStopGPS += StopGPS;
		}

		void OnARCameraTramsformUpdate(Vector3 pos, Quaternion rot)
		{
			camRT.anchoredPosition = new Vector2(rectScale.x * pos.x, rectScale.y * pos.z);
			camRT.localRotation = Quaternion.Euler(0, 0, -rot.eulerAngles.y);
		}

		void InstantiateEst(PositionUnit.Estimate estimate)
		{
			UIDebug.Log("Estimate :" + estimate.mapPos.ToVector3().ToString());
			GameObject go = Instantiate(gps, mapParent);
			go.transform.localPosition = new Vector2(rectScale.x * estimate.mapPos.x, rectScale.y * estimate.mapPos.z);
		}

		void printInfo(GPSInfo info)
		{
			infoText.text = info.ToString();
		}

		void printStatus(GPSServiceStatus status)
		{
			statusText.text = status.ToString();
		}

		public void StartGPSTap()
		{
			GPSUnit.GPSInterface.StartGPS(1, 1);
		}

		public void StopGPSTap()
		{
			GPSUnit.GPSInterface.StopGPS();
		}

		public void OnStartTap()
		{
			ARInterface.StartARSession();
		}

		public void OnReStartTap()
		{
			ARInterface.ReStartARSession();
		}

		#region events

		void GPSUpdate(GPSInfo info)
		{
			printInfo(info);
			UIDebug.Log("GPSInfo: " + GPSUtility.GPSToVector(new GPSInfo(GPSMap.latitude, GPSMap.longitude, 0), info));
			//UIDebug.Log("GPSUpdate\n" + info.ToString());
		}

		void GenerateEstimate(PositionUnit.Estimate estimate)
		{
			InstantiateEst(estimate);
			//UIDebug.Log("GenerateEstimate\n" + estimate.ToString());
		}

		void UpdateGPSStatus(GPSServiceStatus status)
		{
			printStatus(status);
			UIDebug.Log("UpdateGPSStatus\n" + status.ToString());
		}

		void StartGPS(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			UIDebug.Log("StartGPS\ndesiredAccuracyInMeters = " + desiredAccuracyInMeters + "\ndesiredAccuracyInMeters = " + desiredAccuracyInMeters);
		}

		void StopGPS()
		{
			UIDebug.Log("StopGPS");
		}
		#endregion
	}
}
