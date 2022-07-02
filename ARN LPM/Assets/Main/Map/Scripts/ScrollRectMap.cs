using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class OnTapEvent : UnityEngine.Events.UnityEvent<Vector2> { }



public class ScrollRectMap : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public RectTransform content, contentRotate;
	public float inerta;

	public UnityEngine.Events.UnityEvent onMove, onEndMove;
	public OnTapEvent onClick = new OnTapEvent(), onLongClick = new OnTapEvent();

	public delegate void OnScale(float scale);
	public event OnScale onScale;

	Vector2 speed;
	Vector2 currentA;

	Vector2 cmin, cmax;



	[SerializeField] float _minZoom = 2;
	[SerializeField] float _maxZoom = 3;
	[SerializeField] float _zoomLerpSpeed = 10f;
	float _currentZoom = 1;
	public float currentZoom
	{
		get
		{
			return _currentZoom;
		}
		set
		{
			_currentZoom = value;
			if (onScale != null) onScale.Invoke(_currentZoom);
		}
	}
	bool _isPinching = false;
	float _startPinchDist;
	float _startPinchZoom;
	Vector2 _startPinchCenterPosition;
	Vector2 _startPinchScreenPosition;
	Vector2 startVector;
	float startAngle;

	struct Click
	{
		public PointerEventData pointerEventData;
		public float time;
		public bool isLongClick;
	}
	List<Click> clicks = new List<Click>();



	private void Start()
	{
		_minZoom = ARNSettings.settings.minZoomFactor;
		_maxZoom = ARNSettings.settings.maxZoomFactor;
		currentZoom = ARNSettings.settings.startZoomFactor;

		Vector3[] cv = new Vector3[4];
		GetComponent<RectTransform>().GetWorldCorners(cv);
		cmin = new Vector2(cv.Min(i => i.x), cv.Min(i => i.y));
		cmax = new Vector2(cv.Max(i => i.x), cv.Max(i => i.y));

		if (onScale != null)
			onScale.Invoke(currentZoom);
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		clicks.Add(new Click { pointerEventData = eventData, time = Time.time, isLongClick = false });
		speed = Vector2.zero;
		onMove.Invoke();
		if (clicks.Count == 1)
			StartCoroutine(AfterTapIE());
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Click c = clicks.Find(x => x.pointerEventData == eventData);
		clicks.Remove(c);

		if (clicks.Count == 0)
		{
			onEndMove.Invoke();
		}
		if (Vector2.Distance(eventData.pressPosition, eventData.position) / Screen.dpi < 0.3f)
		{
			if (Time.time - c.time < 0.2f)
			{
				onClick.Invoke(eventData.position);
			}
		}
	}

	IEnumerator AfterTapIE()
	{
		while (clicks.Count != 0)
		{
			for (int i = 0; i < clicks.Count; i++)
			{
				if (!clicks[i].isLongClick &&
					Time.time - clicks[i].time > 0.3f &&
					Vector2.Distance(clicks[i].pointerEventData.pressPosition, clicks[i].pointerEventData.position) / Screen.dpi < 0.1f)
				{
					Click t = clicks[i];
					t.isLongClick = true;
					clicks[i] = t;
					onLongClick.Invoke(t.pointerEventData.position);
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		onMove.Invoke();
		MoveTo(content.position + new Vector3(eventData.delta.x, eventData.delta.y, 0));
	}

	float Distance(Vector2 pos1, Vector2 pos2)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos1, null, out pos1);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos2, null, out pos2);
		return Vector2.Distance(pos1, pos2);
	}

	void OnPinchStart()
	{
		Vector2 pos1 = clicks[0].pointerEventData.position;
		Vector2 pos2 = clicks[1].pointerEventData.position;
		stoprAngle = 0;

		startVector = pos1 - pos2;
		startAngle = contentRotate.localRotation.eulerAngles.z;

		_startPinchDist = Distance(pos1, pos2) * content.localScale.x;
		_startPinchZoom = currentZoom;
		_startPinchScreenPosition = (pos1 + pos2) / 2;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, null, out _startPinchCenterPosition);

		Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
		Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;

		SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));

		Vector2 center;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRotate, _startPinchScreenPosition, null, out center);
		SetPivot(contentRotate, (center + contentRotate.rect.size * contentRotate.pivot) / contentRotate.rect.size);
	}

	public void SetPivot(Vector2 center)
	{
		Vector2 pivot;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRotate, center, null, out pivot);
		SetPivot(contentRotate, (pivot + contentRotate.rect.size * contentRotate.pivot) / contentRotate.rect.size);

		RectTransformUtility.ScreenPointToLocalPointInRectangle(content, center, null, out pivot);
		SetPivot(content, (pivot + content.rect.size * content.pivot) / content.rect.size);
	}

	static void SetPivot(RectTransform rectTransform, Vector2 pivot)
	{
		if (rectTransform == null) return;

		Vector2 size = rectTransform.rect.size;

		Vector2 deltaPivot = rectTransform.pivot - pivot;
		Vector3 deltaPosition = Quaternion.Euler(0, 0, rectTransform.localRotation.eulerAngles.z) * new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y) * rectTransform.localScale.x;
		rectTransform.pivot = pivot;
		rectTransform.localPosition -= deltaPosition;
	}

	float stoprAngle = 0;
	void OnPinch()
	{
		float currentPinchDist = Distance(clicks[0].pointerEventData.position, clicks[1].pointerEventData.position) * content.localScale.x;
		float angle = AngleBetvinVectors(startVector, clicks[0].pointerEventData.position - clicks[1].pointerEventData.position);
		if (Mathf.Abs(stoprAngle) < 10)
		{
			stoprAngle = angle;
		}
		else
		{
			contentRotate.localRotation = Quaternion.Euler(0, 0, startAngle + angle - Mathf.Sign(stoprAngle) * 10f);
		}
		currentZoom = (currentPinchDist / _startPinchDist) * _startPinchZoom;
		currentZoom = Mathf.Clamp(currentZoom, _minZoom, _maxZoom);

		if (onScale != null)
			onScale.Invoke(currentZoom);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		speed = eventData.delta;
		currentA = Vector2.one;
	}

	float AngleBetvinVectors(Vector2 a, Vector2 b)
	{
		// angle in [0,180]
		float angle = Vector3.Angle(a, b);
		float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(a, b)));

		// angle in [-179,180]
		float signed_angle = angle * sign;

		// angle in [0,360] (not used but included here for completeness)
		//float angle360 =  (signed_angle + 180) % 360;

		return signed_angle;
	}

	void Chack()
	{
		Vector3 pos = content.position;
		Vector3[] v = new Vector3[4];
		contentRotate.GetWorldCorners(v);
		Vector2 min = new Vector2(v.Min(i => i.x), v.Min(i => i.y));
		Vector2 max = new Vector2(v.Max(i => i.x), v.Max(i => i.y));
		if (max.x < cmax.x)
		{
			pos.x = content.position.x + (cmax.x - max.x);
			currentA.x = 0;
		}
		else if (min.x > cmin.x)
		{
			pos.x = content.position.x - (min.x - cmin.x);
			currentA.x = 0;
		}

		if (max.y < cmax.y)
		{
			pos.y = content.position.y + (cmax.y - max.y);
			currentA.y = 0;
		}
		else if (min.y > cmin.y)
		{
			pos.y = content.position.y - (min.y - cmin.y);
			currentA.y = 0;
		}

		content.position = pos;
	}

	public void MoveTo(Vector3 pos)
	{
		content.position = pos;
		//Chack();
	}

	private void LateUpdate()
	{
		//Chack();
	}

	// Update is called once per frame
	void Update()
	{
		if (clicks.Count == 2)
		{
			if (!_isPinching)
			{
				_isPinching = true;
				OnPinchStart();
			}
			OnPinch();
		}
		else
		{
			if (_isPinching)
			{
				_isPinching = false;
			}
		}

		if (Mathf.Abs(content.localScale.x - currentZoom) > 0.001f)
			content.localScale = Vector3.Lerp(content.localScale, Vector3.one * currentZoom, _zoomLerpSpeed * Time.deltaTime);

		if (currentA.x > 0 || currentA.y > 0)
		{
			MoveTo(content.position + new Vector3(speed.x * currentA.x, speed.y * currentA.y, 0));
			currentA -= currentA * inerta * Time.deltaTime;
			if (currentA.x < 0 && currentA.y < 0)
			{
				onEndMove.Invoke();
			}
		}
	}


}
