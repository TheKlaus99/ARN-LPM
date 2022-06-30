using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPSUnit
{
	public class GPSInterface
	{
		private static GPSServiceStatus gpsStatus_p;
		public static GPSServiceStatus gpsStatus
		{
			get
			{
				return gpsStatus_p;
			}
		}

		public delegate void OnGPSUpdate(GPSInfo info);
		public delegate void OnGPSCompassUpdate(GPSCompassInfo info);
		public delegate void OnGPSStatusUpdate(GPSServiceStatus status);
		public delegate void OnStartGPS(float desiredAccuracyInMeters, float updateDistanceInMeters);
		public delegate void OnStopGPS();
		public delegate void OnStartCompass();
		public delegate void OnStopCompass();

		public static event OnGPSUpdate onGPSUpdate;
		public static event OnGPSCompassUpdate onGPSCompassUpdate;
		public static event OnGPSStatusUpdate onGPSStatusUpdate;
		public static event OnStartGPS onStartGPS;
		public static event OnStopGPS onStopGPS;
		public static event OnStartCompass onStartCompass;
		public static event OnStopCompass onStopCompass;

		public static void GPSUpdate(GPSInfo info)
		{
			if (onGPSUpdate != null)
				onGPSUpdate.Invoke(info);
		}

		public static void CompassUpdate(GPSCompassInfo info)
		{
			if (onGPSCompassUpdate != null)
				onGPSCompassUpdate.Invoke(info);
		}

		public static void UpdateGPSStatus(GPSServiceStatus status)
		{
			if (gpsStatus_p != status)
			{
				gpsStatus_p = status;
				if (onGPSStatusUpdate != null)
					onGPSStatusUpdate.Invoke(status);
			}
		}

		public static void StartGPS(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			if (onStartGPS != null)
				onStartGPS.Invoke(desiredAccuracyInMeters, updateDistanceInMeters);
		}

		public static void StopGPS()
		{
			if (onStopGPS != null)
				onStopGPS.Invoke();
		}

		public static void StartCompass()
		{
			if (onStartCompass != null)
				onStartCompass.Invoke();
		}

		public static void StopCompass()
		{
			if (onStopCompass != null)
				onStopCompass.Invoke();
		}
	}

}
