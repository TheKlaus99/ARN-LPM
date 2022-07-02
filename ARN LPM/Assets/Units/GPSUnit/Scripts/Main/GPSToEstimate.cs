using System;
using System.Collections;
using System.Collections.Generic;
using ARUnit;
using UnityEngine;

namespace GPSUnit
{
	public class GPSToEstimate : MonoBehaviour
	{
		private GPSMap map;

		private static GPSToEstimate instance;

		public delegate void OnEstimateGenerate(PositionUnit.Estimate estimate);
		public static event OnEstimateGenerate onEstimateGenerate;

		public static void GenerateEstimate(PositionUnit.Estimate estimate)
		{
			PositionUnit.PositionInterface.AddEstimate(estimate);
			if (onEstimateGenerate != null)
				onEstimateGenerate.Invoke(estimate);
		}


		void Awake()
		{
			if (instance != null)
			{
				Destroy(this);
			}
			else
			{
				instance = this;
			}
			map = ARNSettings.settings.GPSMap;
			ARUnit.ARInterface.onStartSession += StartARSession;
			ARUnit.ARInterface.onStopSession += StopARSession;
		}

		private void StartARSession()
		{
			GPSInterface.onGPSUpdate += onGPSUpdate;
		}

		private void StopARSession()
		{
			GPSInterface.onGPSUpdate -= onGPSUpdate;
		}

		Color GetColor(Vector3 pos)
		{
			if (map.filter != null)
				return map.filter.GetPixel(Mathf.Clamp((int) (map.filter.width * (pos.x + map.width / 2) / map.width), 0, map.filter.width),
					Mathf.Clamp((int) (map.filter.height * (pos.z + map.height / 2) / map.height), 0, map.filter.height));

			return Color.black;
		}

		void onGPSUpdate(GPSInfo info)
		{

			GPSInfo mapInfo = new GPSInfo(map.latitude, map.longitude, map.altitude);

			if (GPSUtility.distance(info, mapInfo) > ARNSettings.settings.maxDistanceBetweenMapAndGPS)
			{
				return;
			}

			Vector3 mapPosition = GPSUtility.GPSToVector(mapInfo, info) + map.localPos; //координата gps отсносительно карты 
			mapPosition.y = ARUnit.ARInterface.rawARTransform.position.y - ARUnit.ARInterface.floorLevel;

			float accuracyMultiply = 255 * GetColor(mapPosition).r;
			UIDebug.Log("GPS Update, ha = " + info.horizontalAccuracy + ", m = " + accuracyMultiply + ", r = " + (int) (info.horizontalAccuracy + accuracyMultiply));
			PositionUnit.Estimate estimate;
			if (ARUnit.ARInterface.rawARTransform != null)
			{
				estimate = new PositionUnit.Estimate(
					"GPS_" + Time.time.ToString(),
					new PositionUnit.Vector3S(ARUnit.ARInterface.rawARTransform.position),
					new PositionUnit.Vector3S(mapPosition),
					info.compassInfo.trueHeading - ARUnit.ARInterface.rawARTransform.rotation.eulerAngles.y,
					Mathf.Clamp(info.compassInfo.headingAccuracy, ARNSettings.settings.minCompasAccuracy, 360),
					info.horizontalAccuracy + accuracyMultiply
				);
				if (estimate.horizontalAccuracy < 30)
					GenerateEstimate(estimate);
			}
		}

	}
}
