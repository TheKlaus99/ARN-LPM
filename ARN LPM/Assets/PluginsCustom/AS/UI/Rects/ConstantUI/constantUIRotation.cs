using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constantUIRotation : MonoBehaviour
{
	[SerializeField] bool forceDownRot = false;
	Quaternion startRot = Quaternion.identity;
	RectTransform rt;

	private void Start()
	{
		rt = GetComponent<RectTransform>();
		if (forceDownRot)
		{
			startRot = Quaternion.Euler(0, 0, 0);
		}
		else
		{
			startRot = rt.rotation;
		}
	}

	// Update is called once per frame
	void Update()
	{
		rt.localRotation = rt.localRotation * Quaternion.Inverse(rt.rotation) * startRot;
	}
}
