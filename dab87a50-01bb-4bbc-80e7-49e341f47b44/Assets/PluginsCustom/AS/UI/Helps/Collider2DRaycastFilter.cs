using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Collider2D))]
public class Collider2DRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
{
	Collider2D myCollider;
	RectTransform rectTransform;

	void Awake()
	{
		myCollider = GetComponent<Collider2D>();
		rectTransform = GetComponent<RectTransform>();
	}

	public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
	{
		Vector3 worldPoint = Vector3.zero;
		bool isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
			rectTransform,
			screenPos,
			eventCamera,
			out worldPoint
		);
		if (isInside)
			isInside = myCollider.OverlapPoint(worldPoint);
		return isInside;
	}
}
