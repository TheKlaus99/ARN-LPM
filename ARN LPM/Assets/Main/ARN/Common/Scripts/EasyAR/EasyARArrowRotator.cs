using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyARArrowRotator : MonoBehaviour
{

	public Transform arrow;
	ARUnit.ARMap.ARImageTransform image;

	public void Set(ARUnit.ARMap.ARImageTransform arImage)
	{
		image = arImage;
		if (image == null)
		{
			arrow.gameObject.SetActive(false);
		}
		else
		{
			arrow.gameObject.SetActive(true);

		}
	}

	private void OnEnable()
	{
		NavUnit.NavInterface.onPathFound += OnPathFound;
	}

	private void OnDisable()
	{
		NavUnit.NavInterface.onPathFound -= OnPathFound;
	}

	private void OnPathFound(Vector3[] points)
	{
		if (image == null)
			return;
		if (points.Length < 2)
			return;
		Vector3 point = (points[1] - points[0]);


		int N = 2;
		float dist = 0;
		int lastIndex = 0;

		while (dist < N && lastIndex < points.Length - 1)
		{
			dist += Vector3.Distance(points[lastIndex], points[lastIndex + 1]);
			lastIndex++;
		}

		if (dist <= N)
		{
			point = points[points.Length - 1] - points[0];
		}
		else
		{
			lastIndex -= 1;
			float d = Vector3.Distance(points[lastIndex], points[lastIndex + 1]);
			point = Vector3.Lerp(points[lastIndex], points[lastIndex + 1], 1 - (dist - N) / d) - points[0];
		}

		arrow.localRotation = Quaternion.LookRotation(Quaternion.Euler(-90, 0, 0) * InverseTransformPointUnscaledf(image.rotation, point));
	}

	public Vector3 InverseTransformPointUnscaledf(Quaternion rotation, Vector3 position)
	{
		var worldToLocalMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one).inverse;
		return worldToLocalMatrix.MultiplyPoint3x4(position);
	}
}
