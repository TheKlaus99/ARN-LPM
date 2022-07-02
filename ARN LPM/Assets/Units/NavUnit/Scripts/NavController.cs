using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NavUnit
{
	public class NavController : MonoBehaviour
	{
		public LineRenderer lr;
		NavMeshPath path;

		private void Awake()
		{
			path = new NavMeshPath();

			NavInterface.pathFinderTransform = transform;
			NavInterface.onUpdatePath += OnUpdatePath;
			NavInterface.onResetPath += OnResetPath;
			if (lr)
				NavInterface.onPathFound += OnPathFound;
		}

		private void OnResetPath()
		{
			NavInterface.FoundPath(new Vector3[0]);
		}

		private void OnPathFound(Vector3[] points)
		{
			lr.positionCount = points.Length;
			lr.SetPositions(points);
		}

		private void OnUpdatePath(Vector3 startPos, Vector3 endPos)
		{
			if (NavMesh.CalculatePath(NavInterface.SamplePosition(startPos), NavInterface.SamplePosition(endPos), NavMesh.AllAreas, path))
			{
				NavInterface.FoundPath(convertPath(path.corners));
			}
			else
			{
				NavInterface.FoundPath(new Vector3[0]);

			}
		}

		Vector3[] convertPath(Vector3[] path)
		{
			for (int i = 0; i < path.Length; i++)
			{
				path[i] = transform.InverseTransformPoint(path[i]);
			}
			return path;
		}

	}
}
