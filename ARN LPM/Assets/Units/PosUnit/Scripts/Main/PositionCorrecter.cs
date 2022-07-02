using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PositionUnit
{
	public class PositionCorrecter : MonoBehaviour
	{
		private static PositionCorrecter PositionCorrecter_m = null;

		private void Awake()
		{
			PositionCorrecter_m = this;
		}

		Vector3 GetSamplePositoinNS(Vector3 referencePos, float acc)
		{
			if (PositionCorrecter_m == null)
			{
				return referencePos;
			}
			else
			{
				NavMeshHit hit;
				if (NavMesh.SamplePosition(transform.TransformVector(referencePos) + transform.position, out hit, acc * transform.lossyScale.x, NavMesh.AllAreas))
				{
					return transform.InverseTransformPoint(hit.position);
				}
				return referencePos;
			}
		}

		public static Vector3 GetSamplePositoin(Vector3 referencePos, float acc)
		{
			if (PositionCorrecter_m != null)
				return PositionCorrecter_m.GetSamplePositoinNS(referencePos, acc);
			else
				return referencePos;
		}
	}
}
