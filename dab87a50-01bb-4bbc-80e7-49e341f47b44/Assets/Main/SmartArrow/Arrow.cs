using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

	public Transform arrow, cam;
	public float scale;
	public float direction;

	private void Update()
	{
		transform.localScale = Vector3.one * scale * Vector3.Distance(transform.position, cam.position);
		transform.rotation = Quaternion.LookRotation(cam.position - transform.position, Vector3.up);
		arrow.localRotation = Quaternion.Euler(0, 0, direction - transform.rotation.eulerAngles.y - 90);
	}
}
