using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawPath.DrawPath))]
public class DrawPathSubscriber : MonoBehaviour
{
	DrawPath.DrawPath drawPath;
	// Use this for initialization
	void Start()
	{
		drawPath = GetComponent<DrawPath.DrawPath>();
		NavUnit.NavInterface.onPathFound += OnPathFound;

	}

	private void OnPathFound(Vector3[] points)
	{
		drawPath.Draw(new DrawPath.Path(points));
	}
}
