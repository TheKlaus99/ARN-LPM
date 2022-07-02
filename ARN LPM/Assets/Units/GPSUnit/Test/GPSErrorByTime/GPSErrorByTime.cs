using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GPSUnit.Test
{
	public class GPSErrorByTime : MonoBehaviour
	{

		const float xLenght = 715f;
		const float yLenght = 370f;

		public float iterationTime;
		public GameObject pointPref;
		public Transform graphParent;
		public RectTransform currentTime;
		public Button resetButton, shareButton;
		public Toggle automatic;
		public Text gpsStaus;

		List<string> lines = new List<string>();

		float time = -1;
		// Use this for initialization
		void Start()
		{
			resetButton.onClick.AddListener(StartIter);
			shareButton.onClick.AddListener(Share);
			automatic.onValueChanged.AddListener(ChangeToggle);
			GPSInterface.onGPSStatusUpdate += ChangeGPSStatus;
			GPSInterface.onGPSUpdate += UpdateGPS;
		}

		public void StartIter()
		{
			GPSInterface.StartGPS(1, 1);
			StartCoroutine(IterationIE());
			resetButton.interactable = false;
		}

		public void ChangeToggle(bool isOn)
		{
			resetButton.interactable = (!isOn && (time > iterationTime || time < 0));
		}

		void ChangeGPSStatus(GPSServiceStatus status)
		{
			gpsStaus.text = status.ToString();
		}

		void UpdateGPS(GPSInfo info)
		{
			GameObject go = Instantiate(pointPref, graphParent);
			go.GetComponent<RectTransform>().anchoredPosition =
				new Vector2(xLenght * time / iterationTime, yLenght * info.horizontalAccuracy / 100f);
			lines.Add(string.Format("{0}\t{1}", time, info.horizontalAccuracy));
		}

		public void Share()
		{
			System.IO.File.WriteAllLines(Application.persistentDataPath + "/gpsData.txt", lines.ToArray());
			AS.IOS.Native.ShareDialog(Application.persistentDataPath + "/gpsData.txt", "gpsData", "gpsData",
				(int) shareButton.GetComponent<RectTransform>().position.x, (int) shareButton.GetComponent<RectTransform>().position.y);
		}

		IEnumerator IterationIE()
		{
			time = 0;
			while (time < iterationTime)
			{
				time += Time.deltaTime;
				currentTime.anchoredPosition = new Vector2(xLenght * time / iterationTime, 0);
				yield return new WaitForEndOfFrame();
			}

			GPSInterface.StopGPS();
			if (automatic.isOn)
			{
				yield return new WaitForSeconds(3);
				StartIter();
			}
			else
			{
				resetButton.interactable = true;
			}
		}
	}
}
