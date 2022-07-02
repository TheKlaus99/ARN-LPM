using System;
using System.Collections;
using System.Collections.Generic;
using PositionUnit;
using UnityEngine;

public class MapController : MonoBehaviour
{
	public float speed;

	public GPSPointMover GPSPointMover;

	public Animator targetAnim;

	public RectTransform contentRT, mapRT, mapUIRT, camRT, targetRT;
	public LRPathDrawing lRPathDrawing;
	public SVGImage mapImage;

	private bool mapView_m = true;
	public bool mapView
	{
		set
		{
			if (mapView_m == value)
				return;

			mapView_m = value;
			moveAuto = false;
			if (!value)
			{
				wait = 0;
				if (PositionUnit.PositionInterface.area == PositionUnit.Area.inDoor && scrollRectMap.currentZoom < ARNSettings.settings.inDoorZoom)
				{
					StartCoroutine(MoveMapToPointer(ARNSettings.settings.inDoorZoom));
				}
				else if (PositionUnit.PositionInterface.area == PositionUnit.Area.outDoor && scrollRectMap.currentZoom > ARNSettings.settings.outDoorZoom)
				{
					StartCoroutine(MoveMapToPointer(ARNSettings.settings.outDoorZoom));
				}
				else
				{
					StartCoroutine(MoveMapToPointer());
				}
			}
		}
	}


	float wait;
	bool moveAuto = true;
	Vector2 targetPos;
	Vector2 pixelsInMeter;
	float targetRot;
	[HideInInspector] public ScrollRectMap scrollRectMap;


	private void Awake()
	{

		scrollRectMap = GetComponentInChildren<ScrollRectMap>();

		if (ARNSettings.settings.UIMap != null)
			mapImage.sprite = ARNSettings.settings.UIMap;
		pixelsInMeter = ARNSettings.settings.pixelsInMeter;
		NavUnit.NavInterface.onPathFound += OnPathFound;
		PositionInterface.onAreaChange += OnAreaChange;
		GetComponentInChildren<ScrollRectMap>().onScale += OnScaleMap;
	}

	private void OnAreaChange(Area area)
	{
		if (moveAuto)
		{
			if (area == Area.inDoor)
			{
				if (scrollRectMap.currentZoom < ARNSettings.settings.inDoorZoom)
				{
					StartCoroutine(ZoomMapIE(ARNSettings.settings.inDoorZoom));
				}
			}
			else
			{
				if (scrollRectMap.currentZoom > ARNSettings.settings.outDoorZoom)
				{
					StartCoroutine(ZoomMapIE(ARNSettings.settings.outDoorZoom));
				}
			}
		}
	}

	private void OnScaleMap(float scale)
	{
		lRPathDrawing.ChangeThickness(ARNSettings.settings.pathLineThickness / scale);
	}

	private void OnPathFound(Vector3[] points)
	{
		List<Vector3> p = new List<Vector3>();
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 t = points[i];
			t.x *= pixelsInMeter.x;
			t.z *= pixelsInMeter.y;

			p.Add(t);
		}
		lRPathDrawing.Set(p.ToArray());
		//pathLR.Points = p.ToArray();
	}

	public void UpdateTransform(Vector3 position, Quaternion rotation)
	{
		Vector3 forward = rotation * Vector3.forward;
		Vector3 up = rotation * Vector3.up;

		Vector2 proj1 = new Vector2(forward.x, forward.z);
		Vector2 proj2 = new Vector2(up.x, up.z);

		if (proj1.magnitude >.7f)
		{
			targetRot = -AngleBetvinVectors(new Vector2(0, 1), proj1);

		}
		else if (proj1.magnitude >.5f)
		{
			float a1 = -AngleBetvinVectors(new Vector2(0, 1), proj1);
			float a2 = -AngleBetvinVectors(new Vector2(0, 1), proj2);
			float t = (.7f - proj1.magnitude) * 5f;

			targetRot = Mathf.LerpAngle(a1, a2, t);
		}
		else
		{
			targetRot = -AngleBetvinVectors(new Vector2(0, 1), proj2);
		}

		//targetRot = rotation.eulerAngles.y;
		targetPos = pixelsInMeter * new Vector2(position.x, position.z);
	}

	float AngleBetvinVectors(Vector2 a, Vector2 b)
	{
		float angle = Vector3.Angle(a, b);
		float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(a, b)));
		float signed_angle = angle * sign;
		float angle360 = (signed_angle) % 360;
		return angle360;
	}

	public void UpdateGPS(Vector3 position, float acc, bool force = false)
	{
		if (force)
		{
			GPSPointMover.SetTargetsForce(pixelsInMeter * new Vector2(position.x, position.z), acc * 2);
		}
		else
		{
			GPSPointMover.SetTargets(pixelsInMeter * new Vector2(position.x, position.z), acc * 2);
		}
	}

	private void Update()
	{
		camRT.anchoredPosition = targetPos;
		camRT.localRotation = Quaternion.Euler(0, 0, -targetRot);
		Vector2 targetAP = -mapRT.anchoredPosition * contentRT.localScale + (contentRT.pivot - Vector2.one / 2) * contentRT.rect.size * contentRT.localScale;
		//targetPos += Vector2.up * Time.deltaTime * 8;
		//targetRot += Time.deltaTime * 15;

		if (wait > 0)
		{
			wait -= Time.deltaTime;
			if (wait < 0 && !mapView_m)
			{
				StartCoroutine(MoveMapToPointer());
			}
		}

		if (moveAuto && !mapView_m)
		{
			contentRT.anchoredPosition = targetAP;

			SetPivot(mapRT, (targetPos + mapRT.rect.size / 2) / mapRT.rect.size);
			mapRT.localRotation = Quaternion.Euler(0, 0, targetRot);

		}

	}

	public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
	{
		if (rectTransform == null) return;

		Vector2 size = rectTransform.rect.size;

		Vector2 deltaPivot = rectTransform.pivot - pivot;
		Vector3 deltaPosition = Quaternion.Euler(0, 0, rectTransform.localRotation.eulerAngles.z) * new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y) * rectTransform.localScale.x;
		rectTransform.pivot = pivot;
		rectTransform.localPosition -= deltaPosition;
	}

	public void OnMove()
	{
		wait = 2;
		moveAuto = false;
	}

	public void OnEndMove()
	{
		Vector2 targetAP = -mapRT.anchoredPosition * contentRT.localScale + (contentRT.pivot - Vector2.one / 2) * contentRT.rect.size * contentRT.localScale;

		if (Vector2.Distance(contentRT.anchoredPosition, targetAP) < 100)
		{
			wait = Vector2.Distance(contentRT.anchoredPosition, targetAP) / 100f + 0.1f;
		}
	}

	public void SetTarget(Vector3 pos)
	{
		targetRT.anchoredPosition = pixelsInMeter * new Vector2(pos.x, pos.z);
		targetAnim.SetTrigger("Show");
	}

	IEnumerator ZoomMapIE(float mapScale)
	{
		float scartScale = scrollRectMap.currentZoom;
		float t = 0;

		while (t < 1)
		{
			yield return new WaitForEndOfFrame();
			scrollRectMap.currentZoom = scartScale + (mapScale - scartScale) * t;
			//OnScaleMap(scrollRectMap.currentZoom);
			t += Time.deltaTime * speed;
		}
	}

	IEnumerator MoveMapToPointer(float mapScale = -1)
	{
		if (mapScale == -1)
			mapScale = scrollRectMap.currentZoom;

		SetPivot(mapRT, (targetPos + mapRT.rect.size / 2) / mapRT.rect.size);


		float scartScale = scrollRectMap.currentZoom;
		Vector2 targetAP = -mapRT.anchoredPosition * contentRT.localScale + (contentRT.pivot - Vector2.one / 2) * contentRT.rect.size * contentRT.localScale;
		Vector2 startPos = contentRT.anchoredPosition;
		Quaternion targetRotation = Quaternion.Euler(0, 0, targetRot);
		Quaternion startRotation = mapRT.localRotation;
		float t = 0;

		while (wait <= 0 && t < 1)
		{
			yield return new WaitForEndOfFrame();
			targetAP = -mapRT.anchoredPosition * contentRT.localScale + (contentRT.pivot - Vector2.one / 2) * contentRT.rect.size * contentRT.localScale;
			targetRotation = Quaternion.Euler(0, 0, targetRot);
			contentRT.anchoredPosition = Vector2.Lerp(startPos, targetAP, t);
			SetPivot(mapRT, (targetPos + mapRT.rect.size / 2) / mapRT.rect.size);

			mapRT.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
			scrollRectMap.currentZoom = scartScale + (mapScale - scartScale) * t;
			OnScaleMap(scrollRectMap.currentZoom);
			t += Time.deltaTime * speed;
		}
		if (wait <= 0)
		{
			contentRT.anchoredPosition = -targetPos * contentRT.localScale - (contentRT.pivot - Vector2.one / 2) * contentRT.rect.size * contentRT.localScale;
			mapRT.localRotation = Quaternion.Euler(0, 0, targetRot);
			moveAuto = true;
		}
	}
}
