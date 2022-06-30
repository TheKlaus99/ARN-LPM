using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class constantUIScale : MonoBehaviour
{

	public enum AxisType
	{
		x,
		y,
		z
	}
	public AxisType axis;

	float startScale = -1;
	public float minScale = -1;

	float getScale()
	{
		return (axis == AxisType.x) ? transform.lossyScale.x : ((axis == AxisType.y) ? transform.lossyScale.y : transform.lossyScale.z);
	}

	public void SetScale(float scale)
	{
		startScale = scale;
	}

	public void SetMinScale(float scale)
	{
		minScale = scale;
	}

	public void Recalculate()
	{
		transform.localScale = Vector3.one * startScale * Mathf.Max(transform.localScale.x / getScale(), minScale);
	}

	private void Start()
	{
		if (startScale == -1)
			startScale = getScale();
	}

	// Update is called once per frame
	void Update()
	{
		Recalculate();
	}
}
