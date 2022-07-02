using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
[RequireComponent (typeof (RectTransform))]
public class ScaledRect : MonoBehaviour
{


	Canvas canvas;
	public RectTransform targetRect;
	public Vector2 scaleFactor;

	public List<RectTransform> constantScaleChilds;

	public RectTransform rt;

	public Vector2 scale;

	// Use this for initialization

	private void Start ()
	{
		canvas = targetRect.gameObject.GetComponentInParent<Canvas> ();
		//rt = transform.GetComponent<RectTransform> ();
		//Set();
	}


	public void Set ()
	{
		if (targetRect != null)
		{
			canvas = targetRect.gameObject.GetComponentInParent<Canvas> ();
			Rect c = RectTransformUtility.PixelAdjustRect (targetRect, canvas);
			scale.x = c.width;
			scale.y = c.height;
			rt = transform.GetComponent<RectTransform> ();

		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (canvas == null || targetRect == null || rt == null)
			return;
		Rect c = RectTransformUtility.PixelAdjustRect (targetRect, canvas);
		scaleFactor = new Vector2 (c.width / scale.x, c.height / scale.y);
		if (scaleFactor.x < float.Epsilon) scaleFactor.x = 1;
		if (scaleFactor.y < float.Epsilon) scaleFactor.y = 1;

		rt.localScale = new Vector3 (scaleFactor.x, scaleFactor.y, 1);



		foreach (var item in constantScaleChilds)
		{
			if (item != null)
				item.localScale = new Vector3 (1 / scaleFactor.x, 1 / scaleFactor.y, 1);
		}
	}
}
