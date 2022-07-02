using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPSUnit
{
	public class GPSTracker : MonoBehaviour
	{

		Coroutine StartTrack;
		Coroutine Tracking;
		Coroutine TrackingCompass;

		private static GPSTracker instance;

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = this;
			}
			GPSInterface.onStartGPS += StartTracking;
			GPSInterface.onStartCompass += StartCompass;
		}

		void StartTracking(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			GPSInterface.onStartGPS -= StartTracking;
			GPSInterface.onStopGPS += StopTracking;
			if (StartTrack == null)
				StartTrack = StartCoroutine(StartTrackIE(desiredAccuracyInMeters, desiredAccuracyInMeters));
		}

		void StopTracking()
		{
			GPSInterface.onStopGPS -= StopTracking;
			GPSInterface.onStartGPS += StartTracking;

			if (Tracking != null)
			{
				StopCoroutine(Tracking);
			}
			if (StartTrack != null)
			{
				StopCoroutine(StartTrack);
			}

			Tracking = null;
			StartTrack = null;
			Input.location.Stop();
			if (GPSInterface.gpsStatus == GPSServiceStatus.Running || GPSInterface.gpsStatus == GPSServiceStatus.Initializing)
				GPSInterface.UpdateGPSStatus(GPSServiceStatus.Stopped);
		}

		void ErrorTracking(GPSServiceStatus status)
		{
			GPSInterface.onStopGPS -= StopTracking;
			GPSInterface.onStartGPS += StartTracking;

			GPSInterface.UpdateGPSStatus(status);
			if (Input.compass.enabled)
			{
				StopCompass();
			}
		}

		void StartCompass()
		{
			if (Input.location.status == LocationServiceStatus.Running || Input.location.status == LocationServiceStatus.Initializing)
			{
				GPSInterface.onStopCompass += StopCompass;
				GPSInterface.onStartCompass -= StartCompass;
				Input.compass.enabled = true;
				if (TrackingCompass == null)
				{
					TrackingCompass = StartCoroutine(TrackingCompassIE());
				}
			}
			else
			{
				Debug.Log("GPS is inactive");
			}

		}

		void StopCompass()
		{
			GPSInterface.onStopCompass -= StopCompass;
			GPSInterface.onStartCompass += StartCompass;
			Input.compass.enabled = false;
			if (TrackingCompass != null)
			{
				StopCoroutine(TrackingCompass);
				TrackingCompass = null;
			}
		}

		IEnumerator StartTrackIE(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			if (Input.location.isEnabledByUser)
			{
				GPSInterface.UpdateGPSStatus(GPSServiceStatus.Initializing);
				Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
				int maxWait = 20;
				while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
				{
					yield return new WaitForSeconds(1);
					maxWait--;
				}

				if (maxWait > 1)
				{
					if (Input.location.status != LocationServiceStatus.Failed)
					{
						if (Tracking == null)
							Tracking = StartCoroutine(TrackingIE());
					}
					else
					{
						print("Unable to determine device location");
						ErrorTracking(GPSServiceStatus.Failed);
					}
				}
				else
				{
					print("Timed out");
					ErrorTracking(GPSServiceStatus.Failed);
				}
			}
			else
			{
				ErrorTracking(GPSServiceStatus.Disable);
			}

			StartTrack = null;
		}

		GPSServiceStatus LocationServiceStatus2GPSServiceStatus(LocationServiceStatus status)
		{
			switch (status)
			{
				case LocationServiceStatus.Running:
					return GPSServiceStatus.Running;
				case LocationServiceStatus.Stopped:
					return GPSServiceStatus.Stopped;
				case LocationServiceStatus.Initializing:
					return GPSServiceStatus.Initializing;
				case LocationServiceStatus.Failed:
					return GPSServiceStatus.Failed;
			}

			return GPSServiceStatus.Disable;
		}

		IEnumerator TrackingIE()
		{
			GPSInterface.UpdateGPSStatus(GPSServiceStatus.Running);
			double timestamp = Input.location.lastData.timestamp;
			while (Input.location.status == LocationServiceStatus.Running)
			{
				if (timestamp != Input.location.lastData.timestamp)
				{
					timestamp = Input.location.lastData.timestamp;
					GPSInfo info = new GPSInfo(Input.location.lastData, new GPSCompassInfo(Input.compass.trueHeading, Input.compass.magneticHeading));
					GPSInterface.GPSUpdate(info);
				}
				yield return new WaitForSeconds(0.1f);
			}
			ErrorTracking(LocationServiceStatus2GPSServiceStatus(Input.location.status));
			Tracking = null;
			StartTrack = null;
		}

		GPSCompassInfo compassInfo = new GPSCompassInfo(0, 360);
		IEnumerator TrackingCompassIE()
		{
			double timestamp = Input.compass.timestamp;
			while (true)
			{
				if (timestamp != Input.compass.timestamp && Input.location.status == LocationServiceStatus.Running)
				{
					timestamp = Input.compass.timestamp;
					compassInfo = new GPSCompassInfo(Input.compass.trueHeading, Input.compass.magneticHeading);
					GPSInterface.CompassUpdate(compassInfo);
				}
				yield return new WaitForSeconds(0.1f);
			}
		}
	}
}
