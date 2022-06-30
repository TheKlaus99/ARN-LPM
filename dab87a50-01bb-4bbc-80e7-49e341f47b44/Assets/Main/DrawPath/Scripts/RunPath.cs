using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawPath
{
	public class RunPath : MonoBehaviour
	{

		public Path path;

		int currentPoint;

		float t = 0;
		public float distance = 0;

		public void Init(ref Path p, int point)
		{
			path = p;
			currentPoint = point;

			distance = Vector3.Distance(path.points[currentPoint], path.points[currentPoint + 1]);
			t = Vector3.Distance(transform.localPosition, path.points[currentPoint]) / distance;
		}


		void NextPoint()
		{
			currentPoint++;
			if (currentPoint < path.points.Count - 1)
			{
				distance = Vector3.Distance(path.points[currentPoint], path.points[currentPoint + 1]);
				t = 0;
			}
			else
			{
				currentPoint = 0;
				distance = Vector3.Distance(path.points[currentPoint], path.points[currentPoint + 1]);
				t = 0;
			}

		}

		// Update is called once per frame
		void Update()
		{
			if (t >= 1)
				NextPoint();
			transform.localPosition = Vector3.Lerp(path.points[currentPoint], path.points[currentPoint + 1], t);
			t += Time.deltaTime * ARNSettings.settings.arrowSpeed / distance;

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(path.points[currentPoint + 1] - path.points[currentPoint]), Time.deltaTime * 10);
		}
	}
}
