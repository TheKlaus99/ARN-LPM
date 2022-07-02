using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerDebug : MonoBehaviour
{
	public GameObject point;
	List<GameObject> points = new List<GameObject>();
	// Update is called once per frame
	void Update()
	{
		for (int i = 0; i < Input.touchCount && i < points.Count; i++)
		{
			points[i].SetActive(true);
			points[i].transform.position = Input.touches[i].position;
		}

		for (int i = points.Count; i < Input.touchCount; i++)
		{
			GameObject go = Instantiate(point, transform);
			points.Add(go);
			points[i].transform.position = Input.touches[i].position;
		}

		for (int i = Input.touchCount; i < points.Count; i++)
		{
			points[i].SetActive(false);
		}
	}
}
