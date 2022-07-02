using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSelecter : MonoBehaviour
{
	public GameObject floorButton;
	public RectTransform content, select;
	System.Action<int> onFloorChangeCollback;

	List<FloorButtonItem> buttons = new List<FloorButtonItem>();
	FloorButtonItem current;
	Vector2 currentY = Vector2.zero;

	public void Set(HouseItem house, System.Action<int> onFloorChange)
	{
		foreach (var item in buttons)
		{
			Destroy(item.gameObject);
		}
		buttons.Clear();

		onFloorChangeCollback = onFloorChange;
		content.sizeDelta = new Vector2(content.sizeDelta.x, 10 * house.floors.Length);
		for (int i = 0; i < house.floors.Length; i++)
		{
			GameObject go = Instantiate(floorButton, content.transform);
			go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, i * 10f);
			go.GetComponent<FloorButtonItem>().Set(house.floors[i].name, i, OnTap);
			buttons.Add(go.GetComponent<FloorButtonItem>());
		}

		current = buttons[house.selectedFloorID];
		currentY.y = current.GetComponent<RectTransform>().anchoredPosition.y;
		select.anchoredPosition = currentY;

		//buttons[0].Tap();
	}

	void OnTap(int floor)
	{
		if (current == buttons[floor])
			return;
		current = buttons[floor];
		currentY.y = current.GetComponent<RectTransform>().anchoredPosition.y;
		if (changeFloor == null)
		{
			changeFloor = StartCoroutine(ChangeFloorIE());
		}

		onFloorChangeCollback.Invoke(floor);
	}

	Coroutine changeFloor = null;
	IEnumerator ChangeFloorIE()
	{
		while (Mathf.Abs(select.anchoredPosition.y - currentY.y) > 1)
		{
			select.anchoredPosition = Vector2.Lerp(select.anchoredPosition, currentY, Time.deltaTime * 20f);
			yield return new WaitForEndOfFrame();
		}
		select.anchoredPosition = currentY;
		changeFloor = null;
	}
}
