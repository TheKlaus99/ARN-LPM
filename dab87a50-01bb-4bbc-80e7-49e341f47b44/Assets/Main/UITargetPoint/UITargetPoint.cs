using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetPoint : MonoBehaviour
{
	public static UITargetPoint UITargetPointInst;
	public Camera cam;
	public Transform target;

	public RectTransform point, wts;
	public Text distanceT;

	Canvas canvas;


	Vector2 center;
	float aspect = 0;

	private void Start()
	{
		canvas = GetComponent<Canvas>();
		UITargetPointInst = this;
		DeviceChange.OnResolutionChange += OnResolutionChange;
		OnResolutionChange(new Vector2(Screen.width, Screen.height));

	}

	private void OnResolutionChange(Vector2 res)
	{
		center = new Vector2(res.x / 2, res.y / 2);
		aspect = center.x / center.y;
	}

	int orderToSet = 4;
	public void ChangeSortingOrder(int order, float afterTime)
	{
		orderToSet = order;
		if (changeSortingOrder != null)
			StopCoroutine(changeSortingOrder);
		changeSortingOrder = StartCoroutine(ChangeSortingOrderIE(afterTime));
	}

	Coroutine changeSortingOrder;
	IEnumerator ChangeSortingOrderIE(float afterTime)
	{
		yield return new WaitForSeconds(afterTime);
		canvas.sortingOrder = orderToSet;
		changeSortingOrder = null;

	}

	// Update is called once per frame
	void Update()
	{
		Vector3 pos = cam.WorldToScreenPoint(target.position);
		wts.position = pos;
		if (pos.z < 0)
		{
			if (OutScreen(pos))
			{
				pos.x = Screen.width - pos.x;
				pos.y = Screen.height - pos.y;
				DrawPoint(intersection(pos));
			}
			else
			{
				pos.x = Screen.width - pos.x;
				pos.y = 0;
				DrawPoint(intersection(pos));
			}
			point.gameObject.SetActive(true);
		}
		else if (OutScreen(pos))
		{
			DrawPoint(intersection(pos));
			point.gameObject.SetActive(true);
		}
		else
		{
			point.gameObject.SetActive(false);
			distanceT.gameObject.SetActive(false);
		}
	}

	void DrawPoint(Vector3 pos)
	{
		point.gameObject.SetActive(true);
		distanceT.gameObject.SetActive(true);
		point.position = pos;
		distanceT.rectTransform.pivot = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
		distanceT.rectTransform.position = pos - ((pos - new Vector3(center.x, center.y)).normalized * 55f);
		distanceT.text = string.Format("{0}m", (int) Vector3.Distance(cam.transform.position, target.position));
	}

	bool OutScreen(Vector3 pos)
	{
		return (pos.x > Screen.width || pos.x < 0) || (pos.y > Screen.height || pos.y < 0);
	}


	Vector2 intersection(Vector2 pos)
	{
		if (Mathf.Abs((pos.x - center.x) / (pos.y - center.y)) > aspect)
		{
			return (pos.x > center.x) ? intersectionRight(pos) : intersectionLeft(pos);
		}
		else if (pos.y > center.y)
		{
			return intersectionUp(pos);
		}
		else
		{
			return intersectionDown(pos);
		}
	}


	Vector2 intersectionDown(Vector2 pos)
	{
		return new Vector2(
			(-(center.x * pos.y - center.y * pos.x) / (center.y - pos.y)),
			0
		);
	}

	Vector2 intersectionUp(Vector2 pos)
	{
		return new Vector2(
			(Screen.width + (center.x * pos.y - center.y * pos.x) / (center.y - pos.y)),
			Screen.height
		);
	}

	Vector2 intersectionLeft(Vector2 pos)
	{
		return new Vector2(
			0,
			(center.x * pos.y - center.y * pos.x) / (center.x - pos.x)
		);
	}

	Vector2 intersectionRight(Vector2 pos)
	{
		return new Vector2(
			Screen.width,
			Screen.height - (center.x * pos.y - center.y * pos.x) / (center.x - pos.x));
	}
}
