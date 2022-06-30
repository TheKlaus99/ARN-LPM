using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerController : MonoBehaviour
{
	[System.Serializable]
	public class HouseFloorInfo
	{
		public HouseItem house = null;
		public int floorID = -1;
	}


	public MainUIController mainUIController;
	public MapHouseController mapHouseController;
	public Animator mainArrow, targetAnim, easyARPosAnim;
	public UnityEngine.UI.Text targetNameT, targetTimeT;
	RectTransform mainArrowT, targetT, easyARPosT;
	[HideInInspector] public HouseFloorInfo mainArrowHouseInfo = new HouseFloorInfo();
	[HideInInspector] public HouseFloorInfo targetHouseInfo = new HouseFloorInfo();
	[HideInInspector] public HouseFloorInfo easyARHouseInfo = new HouseFloorInfo();

	bool hideArrow = false;
	bool mainArrowIsShow = false;
	bool mainArrowActive_p = false;
	public bool mainArrowActive
	{
		get
		{
			return mainArrowActive_p;
		}
		set
		{

			if (mainArrowActive_p != value)
			{
				if (mainArrowIsShow)
				{
					if (!value)
					{
						mainArrowIsShow = false;
						mainArrow.SetTrigger("Hide");
					}
				}
				else
				{
					if (value && !hideArrow)
					{
						mainArrowIsShow = true;
						mainArrow.SetTrigger("Show");
					}
				}
				mainArrowActive_p = value;
			}
		}
	}


	public void UpdateTargetPos(Vector2 pos, string name = "")
	{
		targetNameT.text = name;
		targetAnim.SetBool("isHide", false);
		UpdateAreaTarget(pos);
	}

	public void UpdateEasyARPos(Vector3 pos)
	{
		Vector2 aPos = new Vector2(pos.x, pos.z) * ARNSettings.settings.pixelsInMeter;
		easyARPosAnim.SetBool("isHide", false);
		easyARPosT.anchoredPosition = aPos;
		easyARHouseInfo.house = mapHouseController.IsPointInHouse(aPos);
		if (easyARHouseInfo.house)
		{
			easyARHouseInfo.floorID = easyARHouseInfo.house.GetFloorIDByHeight(pos.y);
		}
	}

	// Use this for initialization
	void Start()
	{
		MapHouseController.onFloorSelectedChange += OnFloorSelectedChange;
		NavUnit.NavInterface.onPathFound += OnPathFound;
		mainArrowT = mainArrow.gameObject.GetComponent<RectTransform>();
		targetT = targetAnim.gameObject.GetComponent<RectTransform>();
		easyARPosT = easyARPosAnim.gameObject.GetComponent<RectTransform>();
		StartCoroutine(UpdateMainArrowArea());

		targetTimeT.text = "";
	}

	private void OnPathFound(Vector3[] points)
	{
		float d = 0;
		for (int i = 0; i < points.Length - 1; i++)
		{
			d += Vector3.Distance(points[i], points[i + 1]);
		}
		if (d > 5)
		{
			targetTimeT.text = string.Format("±{0}мин", Mathf.Ceil(d * 60 / 5000 * 1.2f));
		}
		else
		{
			targetTimeT.text = "";
		}
	}

	private void OnFloorSelectedChange(HouseItem house, int floorID, bool isSelect)
	{
		if (house == mainArrowHouseInfo.house && floorID == mainArrowHouseInfo.floorID)
		{
			if (isSelect)
			{
				hideArrow = false;
				if (!mainArrowIsShow)
				{
					mainArrowIsShow = true;
					mainArrow.SetTrigger("Show");
				}
			}
			else
			{
				hideArrow = true;
				if (mainArrowIsShow)
				{
					mainArrowIsShow = false;
					mainArrow.SetTrigger("Hide");
				}
			}
		}

		if (house == targetHouseInfo.house && floorID == targetHouseInfo.floorID)
		{
			targetAnim.SetBool("isHide", !isSelect);
		}

		if (house == easyARHouseInfo.house && floorID == easyARHouseInfo.floorID)
		{
			easyARPosAnim.SetBool("isHide", !isSelect);
		}

	}

	void UpdateAreaMainArrow()
	{
		HouseItem h = mapHouseController.IsPointInHouse(mainArrowT.anchoredPosition);
		if (h != null)
		{
			int floorID = h.GetFloorIDByHeight(PositionUnit.PositionInterface.camPosition.y);

			if (h != mainArrowHouseInfo.house || floorID != mainArrowHouseInfo.floorID)
			{
				if (h.floors[floorID].isSelect)
				{
					hideArrow = false;
					if (!mainArrowIsShow)
					{
						mainArrowIsShow = true;
						Debug.Log(1);
						mainArrow.SetTrigger("Show");
					}
				}
				else
				{
					hideArrow = true;
					if (mainArrowIsShow && mainUIController.isMap)
					{
						mainArrowIsShow = false;
						mainArrow.SetTrigger("Hide");
					}
				}
			}
			mainArrowHouseInfo.floorID = floorID;
		}
		else
		{
			hideArrow = false;
			if (!mainArrowIsShow && mainArrowActive_p)
			{
				mainArrowIsShow = true;
				mainArrow.SetTrigger("Show");
			}
		}
		mainArrowHouseInfo.house = h;

	}

	void UpdateAreaTarget(Vector2 pos)
	{
		targetHouseInfo.house = mapHouseController.HitInHouse(pos);
		if (targetHouseInfo.house)
		{
			targetHouseInfo.floorID = targetHouseInfo.house.currentFloor.id;
		}
	}

	IEnumerator UpdateMainArrowArea()
	{
		while (true)
		{
			UpdateAreaMainArrow();
			yield return new WaitForSeconds(1);
		}
	}
}
