using System;
using System.Collections;
using System.Collections.Generic;
using NavUnit;
using PositionUnit;
using UnityEngine;

public class PathUpdator : MonoBehaviour
{
	public Transform startT, endT;

	Vector3 lastStart = Vector3.zero, lastEnd = Vector3.zero;

	// Use this for initialization
	void Start()
	{
		PositionInterface.onStatusChange += UpdateStatus;
		NavInterface.onUpdatePathBetweenTransform += OnUpdatePathBetweenTransform;
	}

	private void OnUpdatePathBetweenTransform()
	{
		NavInterface.UpdatePath(startT.localPosition, endT.localPosition);
	}

	private void UpdateStatus(PosStatus status)
	{
		if (status == PosStatus.normal)
		{
			UpdatePath();
			if (distanceUpdator == null)
			{
				distanceUpdator = StartCoroutine(DistanceUpdatorIE());
			}
		}
		else
		{
			if (distanceUpdator != null)
			{
				StopCoroutine(distanceUpdator);
				distanceUpdator = null;
			}
		}
	}

	void UpdatePath()
	{
		NavInterface.UpdatePath(startT.localPosition, endT.localPosition);
		lastStart = startT.position;
		lastEnd = endT.position;
	}

	Coroutine distanceUpdator = null;
	IEnumerator DistanceUpdatorIE()
	{
		while (true)
		{
			if (Vector3.Distance(startT.position, lastStart) > ARNSettings.settings.distanceToRecalculate ||
				Vector3.Distance(endT.position, lastEnd) > ARNSettings.settings.distanceToRecalculate)
			{
				UpdatePath();
			}
			yield return new WaitForSeconds(.1f);
		}
	}
}
