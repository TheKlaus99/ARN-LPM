namespace GPSUnit
{
	[System.Serializable]
	public class GPSInfo
	{
		public float latitude, longitude, altitude;
		public float horizontalAccuracy, verticalAccuracy;
		public double timestamp;
		public GPSCompassInfo compassInfo;

		public GPSInfo(UnityEngine.LocationInfo info, GPSCompassInfo compassInfo)
		{
			latitude = info.latitude;
			longitude = info.longitude;
			altitude = info.altitude;
			horizontalAccuracy = info.horizontalAccuracy;
			verticalAccuracy = info.verticalAccuracy;
			timestamp = info.timestamp;
			this.compassInfo = compassInfo;
		}

		public GPSInfo(float latitude, float longitude, float altitude)
		{
			this.latitude = latitude;
			this.longitude = longitude;
			this.altitude = altitude;
			horizontalAccuracy = 0;
			verticalAccuracy = 0;
			timestamp = 0;
			compassInfo = new GPSCompassInfo(0, 360);
		}

		public GPSInfo(GPSAnchor GPSAnchor) : this(GPSAnchor.latitude, GPSAnchor.longitude, GPSAnchor.altitude) { }

		public override string ToString()
		{
			return string.Format(
				"latitude = {0}\nlongitude = {1}\naltitude = {2}\nhorizontalAccuracy = {3}\nverticalAccuracy = {4}\ntrueHeading = {5},\nheadingAccuracy = {6}",
				latitude, longitude, altitude, horizontalAccuracy, verticalAccuracy, compassInfo.trueHeading, compassInfo.headingAccuracy
			);
		}
	}
}
