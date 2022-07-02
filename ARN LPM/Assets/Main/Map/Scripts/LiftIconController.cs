using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiftIconController : MonoBehaviour
{
	public TMPro.TextMeshProUGUI text;
	public GameObject disabledObj;

	public RectTransform arrow;

	RectTransform rt;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
	}

	public void SetActive(bool isEnable)
	{
		disabledObj.SetActive(isEnable);
	}

	public void Set(Vector2 pos, bool from, bool isUp, string toFloor)
	{

		rt.anchoredPosition = pos;
		if (!from)
		{
			text.text = toFloor;
			arrow.gameObject.SetActive(true);
			arrow.localRotation = Quaternion.Euler(0, 0, isUp ? 0 : 180);
		}
		else
		{
			text.text = "";
			arrow.gameObject.SetActive(false);
		}

	}
}
