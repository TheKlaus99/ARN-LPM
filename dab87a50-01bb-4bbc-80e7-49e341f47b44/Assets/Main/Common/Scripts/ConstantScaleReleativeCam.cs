using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantScaleReleativeCam : MonoBehaviour
{

	public Transform target;
	public float minScale = 1f;
	public float addScale = 1f;
	Vector3 startScale = Vector3.one;

	// Use this for initialization
	void Start()
	{
		startScale = transform.localScale;
	}

	// Update is called once per frame
	void Update()
	{
		float s = Vector3.Distance(target.position, transform.position) + addScale;
		transform.localScale = Vector3.one * ((s < minScale) ? minScale : s);
	}
}
