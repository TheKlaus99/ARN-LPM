namespace PositionUnit
{
	[System.Serializable]
	public class Estimate
	{
		//TODO: rotation
		public string name;
		public Vector3S realPos;
		public Vector3S mapPos;
		public float correctAngle = 0;
		public float angleAccuracy = 0;
		public float horizontalAccuracy = 360;

		public Estimate(string name, Vector3S realPos, Vector3S mapPos, float correctAngle, float angleAccuracy, float horizontalAccuracy)
		{
			this.name = name;
			this.realPos = realPos;
			this.mapPos = mapPos;
			this.correctAngle = correctAngle;
			this.angleAccuracy = angleAccuracy;
			this.horizontalAccuracy = horizontalAccuracy;
		}


		public override string ToString()
		{
			return string.Format(
				"name = {0}\nrealPos = {1}\nmapPos = {2}\ncorrectAngle = {3}\nangleAccuracy = {4}\nhorizontalAccuracy = {5}",
				name, realPos.ToString(), mapPos.ToString(), correctAngle, angleAccuracy, horizontalAccuracy
			);
		}
	}
}
