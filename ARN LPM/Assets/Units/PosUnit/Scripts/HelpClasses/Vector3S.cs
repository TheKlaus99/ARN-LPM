namespace PositionUnit
{
	[System.Serializable]
	public class Vector3S
	{
		public float x, y, z;



		public Vector3S (float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3S (UnityEngine.Vector3 vector3)
		{
			this.x = vector3.x;
			this.y = vector3.y;
			this.z = vector3.z;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", x, y, x);
		}

		public UnityEngine.Vector3 ToVector3 ()
		{
			return new UnityEngine.Vector3 (x, y, z);
		}

		public static Vector3S operator + (Vector3S c1, Vector3S c2)
		{
			return new Vector3S (c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
		}

		public static Vector3S operator - (Vector3S c1, Vector3S c2)
		{
			return c1 + (-c2);
		}

		public static Vector3S operator - (Vector3S c1)
		{
			return new Vector3S (-c1.x, -c1.y, -c1.z);
		}
	}
}
