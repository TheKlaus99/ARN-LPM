using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow3D : MonoBehaviour
{



	public Transform target;
	public float targetScale;

	public float time;

	public Material normal, hightLight;
	public GameObject[] arrows;

	IEnumerator AnimateIE()
	{
		while (true)
		{
			for (int i = 0; i < arrows.Length; i++)
			{
				arrows[i].GetComponent<MeshRenderer>().material = normal;
				arrows[(i + 1) % arrows.Length].GetComponent<MeshRenderer>().material = hightLight;

				yield return new WaitForSeconds(time / arrows.Length);
			}
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < arrows.Length; i++)
		{
			arrows[i].GetComponent<MeshRenderer>().material = normal;
		}
		StartCoroutine(AnimateIE());
	}


	private void Update()
	{
		if (target != null)
		{
			float d = Vector3.Distance(transform.position, target.position);
			if (d < 5)
			{
				transform.localScale = Vector3.one * targetScale;
			}
			else if (d < 10)
			{
				transform.localScale = Vector3.one * targetScale + Vector3.one * targetScale * (d - 5) * .2f;
			}
			else
			{
				transform.localScale = Vector3.one * targetScale * 2;

			}
			//transform.localScale = Vector3.one + Vector3.one * targetScale * scale.Evaluate(d / 500f) * 500f;
		}
	}
}
