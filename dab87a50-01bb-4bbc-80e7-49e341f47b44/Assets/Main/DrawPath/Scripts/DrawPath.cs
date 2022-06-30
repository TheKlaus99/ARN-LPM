using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrawPath
{
	public class DrawPath : MonoBehaviour
	{
		//[HideInInspector]
		public Path path;

		MapHouseController mapHouseController;

		public GameObject liftPref;
		public Transform arCam, liftsParet;

		public GameObject arrowPref;
		float distance;
		float lastTime = -1;

		int use = 0;
		List<GameObject> arrows = new List<GameObject>();
		int liftCount = 0;
		List<GameObject> lifts = new List<GameObject>();

		private void Start()
		{
			mapHouseController = FindObjectOfType<MapHouseController>();
		}

		public void Draw(Path path)
		{
			this.path = path;
			Draw();
		}

		public void Draw(Vector3[] points)
		{
			this.path = new Path(points);

			Draw();
		}

		float getOffest()
		{
			return (Time.time * ARNSettings.settings.arrowSpeed) % ARNSettings.settings.arrowDistance;
		}

		public void TrimPath()
		{
			if (path.points.Count < 2)
				return;
			liftCount = 0;
			List<Vector3> points = new List<Vector3>();
			points.Add(path.points[0]);
			float dist = 0;
			Vector3 p = Vector3.zero;
			for (int i = 0; i < path.points.Count - 1; i++)
			{
				p = path.points[i + 1] - path.points[i];
				bool isLift = new Vector2(p.x, p.z).magnitude / p.magnitude < 0.15f;


				float d = Vector3.Distance(path.points[i], path.points[i + 1]);
				if (dist + d > ARNSettings.settings.maxPathDrawDistance)
				{
					if (!isLift)
					{
						points.Add(Vector3.Lerp(path.points[i], path.points[i + 1], 1 - (dist + d - ARNSettings.settings.maxPathDrawDistance) / d));
						dist = ARNSettings.settings.maxPathDrawDistance;
						break;
					}
				}
				else
				{
					if (!isLift)
						points.Add(path.points[i + 1]);
				}
				dist += d;

				if (isLift)
				{
					//lift
					if (liftCount >= lifts.Count)
					{
						GameObject go = Instantiate(liftPref, liftsParet);
						lifts.Add(go);
					}
					lifts[liftCount].transform.localPosition = path.points[i] + new Vector3(0, 1, 0);
					var t = mapHouseController.IsPointInHouse(new Vector2(path.points[i + 1].x, path.points[i + 1].z) * ARNSettings.settings.pixelsInMeter);;
					lifts[liftCount].GetComponent<LiftMarkerItem>().Set(arCam, t.floors[t.GetFloorIDByHeight(path.points[i + 1].y)].name, path.points[i + 1].y > path.points[i].y);
					liftCount++;
					break;
				}

			}

			if (dist < ARNSettings.settings.maxPathDrawDistance && liftCount == 0)
			{
				//points.Add(path.points[path.points.Count - 1]);
			}

			path.points = points;

			for (int i = liftCount; i < lifts.Count; i++)
			{
				Destroy(lifts[i]);
			}

			while (liftCount < lifts.Count)
			{
				lifts.RemoveAt(liftCount);
			}
		}

		public void Draw()
		{
			TrimPath();
			distance = ARNSettings.settings.arrowDistance;
			if (path == null || path.distance < distance)
				return;
			use = 0;

			float offset = distance - (Time.time * ARNSettings.settings.arrowSpeed) % distance;

			for (int i = 0; i < path.points.Count - 1; i++)
			{
				float dist = Vector3.Distance(path.points[i], path.points[i + 1]);
				int n = (int) ((dist + offset) / distance);

				for (int j = 1; j <= n; j++)
				{
					CreatePoint(Vector3.Lerp(path.points[i], path.points[i + 1], (j * distance) / dist - offset / dist), path.points[i + 1], i);
				}
				offset += dist - n * distance;
			}

			DeActivate();
		}

		RunPath CreatePoint(Vector3 position, Vector3 directon, int point)
		{
			RunPath rp;
			if (use > arrows.Count - 1)
			{
				GameObject go = Instantiate(arrowPref, transform);
				go.transform.localPosition = position;
				go.transform.rotation = Quaternion.LookRotation((directon - position), Vector3.up);
				rp = go.GetComponent<RunPath>();
				if (rp != null)
				{
					rp.Init(ref path, point);
				}
				arrows.Add(go);
			}
			else
			{
				arrows[use].transform.localPosition = position;
				arrows[use].transform.rotation = Quaternion.LookRotation((directon - position), Vector3.up);
				arrows[use].SetActive(true);
				rp = arrows[use].GetComponent<RunPath>();
				if (rp != null)
				{
					rp.Init(ref path, point);
				}
			}
			use++;
			return rp;
		}

		void DeActivate()
		{
			for (int i = use; i < arrows.Count; i++)
			{
				arrows[i].SetActive(false);
			}
		}

	}
}
