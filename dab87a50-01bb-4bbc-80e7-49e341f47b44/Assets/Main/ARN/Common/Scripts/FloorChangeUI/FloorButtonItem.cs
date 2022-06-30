using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorButtonItem : MonoBehaviour
{
	public Text nameT;
	int id;
	System.Action<int> onTapCollback;

	public void Set(string name, int id, System.Action<int> onTap)
	{
		this.id = id;
		onTapCollback = onTap;
		nameT.text = name;
	}

	public void Tap()
	{
		onTapCollback.Invoke(id);
	}
}
