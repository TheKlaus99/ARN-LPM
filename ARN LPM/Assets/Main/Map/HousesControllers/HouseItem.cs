using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseItem : MonoBehaviour
{
	public Floor[] floors;
	public Floor currentFloor;

	public int selectedFloorID = 0;

	Animator animator;
	Collider2D collider;
	RectTransform rectTransform;

	bool isSelect = false;

	public bool IsRectPointInCollider(Vector2 wotldPos)
	{
		return collider.OverlapPoint(wotldPos);

		//Ray ray = new Ray();
		//collider.bounds.IntersectRay();
	}

	public Vector2 GetIntersect(Vector2 left, Vector2 right)
	{
		Vector2 p = Vector2.Lerp(left, right, 0.5f);

		if (Vector2.Distance(left, right) < 0.1f)
			return p;

		if (collider.OverlapPoint(p))
		{
			return GetIntersect(left, p);
		}
		else
		{
			return GetIntersect(p, right);
		}
	}

	public bool IsRaycastLocationValid(Vector2 screenPos)
	{

		Vector3 worldPoint = Vector3.zero;
		bool isInside = RectTransformUtility.ScreenPointToWorldPointInRectangle(
			rectTransform,
			screenPos,
			null,
			out worldPoint
		);
		if (isInside)
			isInside = collider.OverlapPoint(worldPoint);

		return isInside;
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
		collider = GetComponentInChildren<Collider2D>();
		rectTransform = GetComponentInChildren<RectTransform>();
		for (int i = 0; i < floors.Length; i++)
		{
			floors[i].house = this;
			floors[i].id = i;
		}
		selectedFloorID = 0;
		currentFloor = floors[0];
	}

	public void Select()
	{
		if (!isSelect)
			animator.SetTrigger("Show");
		isSelect = true;
	}

	public void DeSelect()
	{
		if (isSelect)
			animator.SetTrigger("Hide");
		isSelect = false;
	}

	public int GetFloorIDByHeight(float h)
	{
		for (int i = 1; i < floors.Length; i++)
		{
			if (floors[i].height > h)
				return i - 1;
		}
		return floors.Length - 1;
	}

	public void ChangeFloor(int floorID)
	{
		if (selectedFloorID != floorID && floorID < floors.Length)
		{
			selectedFloorID = floorID;
			currentFloor.Hide();
			currentFloor = floors[floorID];
			currentFloor.Show();
		}
	}

	[System.Serializable]
	public class Floor
	{
		public Animator anim;
		public string name;
		public float height;

		[HideInInspector] public HouseItem house;
		[HideInInspector] public int id;
		[HideInInspector] public bool isSelect;

		void Invoke()
		{
			if (MapHouseController.onFloorSelectedChange != null)
				MapHouseController.onFloorSelectedChange.Invoke(house, id, isSelect);
		}

		public void Show()
		{
			if (!isSelect)
			{
				isSelect = true;
				Invoke();
				if (anim)
					anim.SetTrigger("Show");
			}
		}

		public void Hide()
		{
			if (isSelect)
			{
				isSelect = false;
				Invoke();
				if (anim)
					anim.SetTrigger("Hide");
			}
		}
	}
}
