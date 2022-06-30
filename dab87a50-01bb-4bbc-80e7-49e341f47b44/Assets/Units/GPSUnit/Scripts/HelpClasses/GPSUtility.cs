using System;
using UnityEngine;


namespace GPSUnit
{
	public class GPSUtility
	{
		public const float metersInLatDegree = 111194.92664f;
		public static Vector3 GPSToVector(GPSInfo pivot, GPSInfo position)
		{
			float x = (float) ((position.longitude - pivot.longitude) * metersInLatDegree * System.Math.Cos(Mathf.Deg2Rad * (pivot.latitude + position.latitude) / 2f));
			float z = (position.latitude - pivot.latitude) * metersInLatDegree;
			return new Vector3(x, position.altitude - pivot.altitude, z);
		}

		public static GPSInfo VectorToGPS(GPSInfo pivot, Vector3 position)
		{
			float latitude = (position.z / metersInLatDegree + pivot.latitude);
			float longitude = (float) (position.x / (metersInLatDegree * System.Math.Cos(Mathf.Deg2Rad * (pivot.latitude + latitude) / 2f)) + pivot.longitude);

			return new GPSInfo(latitude, longitude, 0);

		}

		public static double distance(GPSInfo point1, GPSInfo point2)
		{
			if ((point1.latitude == point2.latitude) && (point1.longitude == point2.longitude))
			{
				return 0;
			}
			else
			{
				double theta = point1.longitude - point2.longitude;
				double dist = Math.Sin(Mathf.Deg2Rad * point1.latitude) * Math.Sin(Mathf.Deg2Rad * (point2.latitude)) +
					Math.Cos(Mathf.Deg2Rad * (point1.latitude)) * Math.Cos(Mathf.Deg2Rad * (point2.latitude)) * Math.Cos(Mathf.Deg2Rad * (theta));
				dist = Math.Acos(dist);
				dist = Mathf.Rad2Deg * dist;
				dist = dist * 60 * 1.1515;
				dist = dist * 1.609344;
				return (dist);
			}
		}

	}
}
