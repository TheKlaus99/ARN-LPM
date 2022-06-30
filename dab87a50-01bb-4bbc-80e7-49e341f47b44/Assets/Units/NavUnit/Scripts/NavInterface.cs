using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace NavUnit
{
	public class NavInterface
	{
		public static Transform pathFinderTransform;
		static System.Func<Vector3, Vector3> sampleToFloor_p;
		/// <summary>
		///First vector is world pos, return is world pos when Y on floor
		/// </summary>
		/// <value></value>
		public static System.Func<Vector3, Vector3> sampleToFloorFunc
		{
			get
			{
				return sampleToFloor_p;
			}

			set
			{
				if (sampleToFloor_p != null)
				{
					Debug.LogError("double SamplePosition func defenition");
				}
				sampleToFloor_p = value;
			}
		}

		public delegate void OnUpdatePath(Vector3 startPos, Vector3 endPos);
		public delegate void OnUpdatePathBetweenTransform();
		public delegate void OnResetPath();
		public delegate void OnPathFound(Vector3[] points);


		public static event OnUpdatePath onUpdatePath;
		public static event OnUpdatePathBetweenTransform onUpdatePathBetweenTransform;
		public static event OnResetPath onResetPath;
		public static event OnPathFound onPathFound;


		/// <summary>
		/// World position on NavMesh map
		/// </summary>
		/// <param name="pos">local position</param>
		/// <returns></returns>
		public static Vector3 SamplePosition(Vector3 pos)
		{
			Vector3 p = pathFinderTransform.TransformVector(pos) + pathFinderTransform.position;
			Vector3 p1 = pathFinderTransform.TransformVector(pos) + pathFinderTransform.position;
			RaycastHit hit;
			NavMeshHit navHit;

			if (pathFinderTransform != null)
			{
				if (Physics.Raycast(p, Vector3.down, out hit, 1, 1 << 12))
				{
					if (NavMesh.SamplePosition(hit.point, out navHit, 10, NavMesh.AllAreas))
					{
						Debug.DrawLine(p, hit.point, Color.green, 1);
						return navHit.position;
					}

				}
				else
				{
					Debug.DrawRay(p, Vector3.down, Color.red, 1);
				}

				if (sampleToFloor_p != null)
				{
					float y = (int) p.y;
					p = pathFinderTransform.TransformVector(sampleToFloor_p.Invoke(pos)) + pathFinderTransform.position;

					if (p.y != y)
					{

						if (NavMesh.SamplePosition(p, out navHit, 10, NavMesh.AllAreas))
						{
							return navHit.position;
						}
					}
				}
			}


			if (NavMesh.SamplePosition(p, out navHit, 10, NavMesh.AllAreas))
			{
				return navHit.position;
			}


			return p;
		}

		public static void UpdatePath(Vector3 startPos, Vector3 endPos)
		{
			if (onUpdatePath != null)
			{
				onUpdatePath.Invoke(startPos, endPos);
			}
		}

		public static void UpdatePathBetweenTransform()
		{
			if (onUpdatePathBetweenTransform != null)
			{
				onUpdatePathBetweenTransform.Invoke();
			}
		}

		public static void FoundPath(Vector3[] points)
		{
			if (onPathFound != null)
			{
				onPathFound.Invoke(points);
			}
		}

		public static void ResetPath()
		{
			if (onResetPath != null)
			{
				onResetPath.Invoke();
			}
		}

	}
}
