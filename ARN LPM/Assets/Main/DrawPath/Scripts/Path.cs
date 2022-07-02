using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawPath
{
	[System.Serializable]
	public class Path
	{
		public List<Vector3> points = new List<Vector3>();
		public float distance = 0;

		public Path(LineRenderer lr)
		{
			for (int i = 0; i < lr.positionCount; i++)
			{
				points.Add(lr.GetPosition(i));
			}
			calculateDistance();
		}

		public Path(Vector3[] points)
		{
			this.points = new List<Vector3>(points);
			calculateDistance();
		}

		void calculateDistance()
		{
			distance = 0;
			for (int i = 0; i < points.Count - 1; i++)
			{
				distance += Vector3.Distance(points[i], points[i + 1]);
			}

			if (points.Count > 1)
			{
				float d = (distance % ARNSettings.settings.arrowDistance) / Vector3.Distance(points[points.Count - 2], points[points.Count - 1]);

				points[points.Count - 1] = Vector3.Lerp(points[points.Count - 2], points[points.Count - 1], 1 - d);
			}
		}
	}
}
