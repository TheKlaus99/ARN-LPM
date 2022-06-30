using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtTransform : MonoBehaviour
{

	public Transform source;
	public Vector3 up;

	Vector3 foward = new Vector3(0, 0, 0);
	// Update is called once per frame
	void Update()
	{
		if (source != null)
		{
			foward.x = source.position.x - transform.position.x;
			foward.z = source.position.z - transform.position.z;
			transform.rotation = Quaternion.LookRotation(foward, up);
		}
	}
}
