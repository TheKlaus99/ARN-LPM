using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit.Test
{
	public class ImageRotaion : MonoBehaviour
	{
		public Transform t1, correct, t2;

		float angleBetvinVectors(Vector2 a, Vector2 b)
		{
			// angle in [0,180]
			float angle = Vector3.Angle(a, b);
			float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(a, b)));

			// angle in [-179,180]
			float signed_angle = angle * sign;

			// angle in [0,360] (not used but included here for completeness)
			//float angle360 = (signed_angle + 180) % 360;

			return signed_angle;
		}

		// Update is called once per frame
		void Update()
		{
			Quaternion rot1 = t1.localRotation;
			Quaternion rot2 = t2.rotation;



			Vector2 d = new Vector2((rot1 * Vector3.right).x, (rot1 * Vector3.right).z).normalized;
			Vector2 d2 = new Vector2((rot2 * Vector3.right).x, (rot2 * Vector3.right).z).normalized;


			Debug.DrawRay(t1.transform.position, new Vector3((t1.rotation * Vector3.right).x, 0, (t1.rotation * Vector3.right).z).normalized);
			Debug.DrawRay(t2.transform.position, new Vector3(d2.x, 0, d2.y));


			float rotar = angleBetvinVectors(d, d2);
			correct.rotation = Quaternion.Euler(0, -rotar, 0);
		}
	}
}
