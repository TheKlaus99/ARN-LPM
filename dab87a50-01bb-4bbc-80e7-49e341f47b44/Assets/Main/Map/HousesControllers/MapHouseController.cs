using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHouseController : MonoBehaviour
{

	HouseItem[] items;

	[HideInInspector]
	public HouseItem currentHouse = null;
	System.Action<HouseItem> onTapCollback;


	[HideInInspector]
	public bool clickSelect = false;

	public delegate void OnFloorSelectedChange(HouseItem house, int floorID, bool isSelect);
	public static OnFloorSelectedChange onFloorSelectedChange;


	Transform transformH;

	// Use this for initialization
	void Awake()
	{
		transformH = gameObject.transform;
		NavUnit.NavInterface.sampleToFloorFunc = SampleMapPos;
		items = GetComponentsInChildren<HouseItem>();
	}

	float lastZoom = 0;
	public void OnZoom(float zoom)
	{
		if (zoom > ARNSettings.settings.zoomToShowFloor && lastZoom < ARNSettings.settings.zoomToShowFloor)
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i].currentFloor.Show();
			}
		}
		else if (zoom < ARNSettings.settings.zoomToShowFloor && lastZoom > ARNSettings.settings.zoomToShowFloor)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] != currentHouse)
				{
					items[i].currentFloor.Hide();
				}
			}
		}

		lastZoom = zoom;
	}

	public void SetHeight(float height)
	{
		if (PositionUnit.PositionInterface.area == PositionUnit.Area.inDoor)
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i].ChangeFloor(items[i].GetFloorIDByHeight(height));
			}
		}
		else
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i].currentFloor.Hide();
				items[i].currentFloor = items[i].floors[items[i].GetFloorIDByHeight(height)];
				items[i].selectedFloorID = items[i].GetFloorIDByHeight(height);
			}
		}
	}

	public void DeSelect(bool hide = false)
	{
		if (clickSelect)
		{
			clickSelect = false;
			if (currentHouse != null)
			{
				currentHouse.DeSelect();
				if (hide)
					currentHouse.currentFloor.Hide();
			}
			currentHouse = null;
		}
	}

	public void Set(System.Action<HouseItem> onTap)
	{
		onTapCollback = onTap;
	}

	public HouseItem IsPointInHouse(Vector2 rectPos)
	{
		Vector2 worldPos = transformH.TransformPoint(rectPos);
		return HitInHouse(worldPos);
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].IsRectPointInCollider(worldPos))
			{
				return items[i];
			}
		}
		return null;
	}

	public HouseItem HitInHouse(Vector2 screenPos)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].IsRaycastLocationValid(screenPos))
			{
				return items[i];
			}
		}
		return null;
	}

	public void HideFloor()
	{
		if (clickSelect)
		{
			clickSelect = false;
			if (currentHouse != null)
			{
				currentHouse.DeSelect();
				if (lastZoom < 2)
					currentHouse.currentFloor.Hide();
			}

			currentHouse = null;
		}
	}

	public void OnTapHouse(HouseItem item)
	{
		clickSelect = true;
		if (item == currentHouse)
			return;

		if (currentHouse != null)
		{
			currentHouse.DeSelect();
			if (lastZoom < 2)
				currentHouse.currentFloor.Hide();
		}

		currentHouse = item;
		currentHouse.currentFloor.Show();
		currentHouse.Select();
		onTapCollback.Invoke(currentHouse);
	}

	public Vector3 SampleMapPos(Vector3 v)
	{
		HouseItem house = IsPointInHouse(new Vector2(v.x, v.z) * ARNSettings.settings.pixelsInMeter);
		if (house)
		{
			float f = house.floors[house.GetFloorIDByHeight(v.y)].height + .1f;
			return new Vector3(v.x, f, v.z);
		}

		return v;
	}

}
