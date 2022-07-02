using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceText : MonoBehaviour
{
	public Transform start, end;
	public string format = "{0}m";

	Transform t;
	UnityEngine.UI.Text text;


	private void Start()
	{
		if (start == null)
			start = GetComponent<Transform>();
		text = GetComponent<UnityEngine.UI.Text>();
	}

	// Update is called once per frame
	void Update()
	{
		text.text = string.Format(format, (int) Vector3.Distance(start.position, end.position));
	}
}
