using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObject : MonoBehaviour
{
	public static void Disable(GameObject go, float t)
	{
		if (!go.GetComponent<DisableObject>())
			go.AddComponent<DisableObject>().Hide(t);
	}

	public void Hide(float t)
	{
		StartCoroutine(Stop(t));
	}

	IEnumerator Stop(float t)
	{
		yield return new WaitForSeconds(t);
		gameObject.SetActive(false);
		Destroy(this);
	}
}
