using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{

	private static UIDebug instance;
	public static UIDebug getInstance()
	{
		if (instance == null)
		{
			Debug.Log("Create Obj: UIDebug");  
			instance = new GameObject("UIDebug", typeof(UIDebug), typeof(CanvasScaler)).GetComponent<UIDebug>();
			instance.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
			instance.GetComponent<CanvasScaler>().uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
		}
		return instance;
	}


	Text t;
	List<string> lines = new List<string>();
	private void Awake()
	{
		instance = this;
		t = new GameObject("Text", typeof(Text)).GetComponent<Text>();
		t.transform.localScale = Vector3.one;
		t.transform.SetParent(transform);
		RectTransform rt = t.GetComponent<RectTransform>();
		rt.anchorMax = new Vector2(0, 1);
		rt.anchorMin = new Vector2(0, 1);
		rt.sizeDelta = new Vector2(320, 170);
		rt.anchoredPosition = new Vector2(160, -85);
		t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		t.fontStyle = FontStyle.Bold;
		t.color = Color.white;
		t.resizeTextForBestFit = true;
		t.resizeTextMinSize = 5;
		t.resizeTextMaxSize = 12;
	}

	public void printText(string line)
	{
		if (lines.Count > 9)
		{
			lines.RemoveAt(0);
		}
		lines.Add(line);
		string s = "";
		foreach (var item in lines)
		{
			s += item + "\n";
		}
		t.text = s;
	}


	public static void Log(object line)
	{
		if (false)
			getInstance().printText("[" + Time.time.ToString("0.0") + "] " + line.ToString());
		else
			Debug.Log("UIDebug is disable. " + "[" + Time.time.ToString("0.0") + "] " + line.ToString());
	}

}
