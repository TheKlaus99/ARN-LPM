namespace GPSUnit
{
	[System.Serializable]
	public class GPSCompassInfo
	{
		public float trueHeading, headingAccuracy;

		public GPSCompassInfo(float trueHeading, float headingAccuracy)
		{
			this.trueHeading = trueHeading;
			this.headingAccuracy = headingAccuracy;
		}
	}
}
