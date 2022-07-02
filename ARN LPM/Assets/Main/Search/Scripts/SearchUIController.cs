using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Search
{

	public class SearchUIController : MonoBehaviour
	{
		const int maxCount = 25;

		public GameObject itemPref, itemEPref, content;
		public InputField inputField;
		public RectTransform contentList;

		public Image[] underline;
		public GameObject[] groups;

		List<SearchSer> data;
		List<SearchEventSer> dataE;

		public string eventsURL;

		public SearchEventArraySer jsonData;
		public string json;

		System.Action<SearchSer> onTap;
		System.Action onCansel;

		int useItemRCount = 0;
		int useItemECount = 0;
		List<SearchItem> itemsR = new List<SearchItem>();
		List<SearchEventItem> itemsE = new List<SearchEventItem>();


		private static SearchUIController obj;

		public static void Open(DataScriptable data, System.Action<SearchSer> onTap, System.Action onCansel)
		{
			Open(data.data, onTap, onCansel);
		}

		public static void Open(List<SearchSer> data, System.Action<SearchSer> onTap, System.Action onCansel)
		{
			if (obj != null)
				obj.OpenSearch(data, onTap, onCansel);
		}

		private void Awake()
		{
			json = JsonUtility.ToJson(jsonData);


			GameObject go = new GameObject("Plugins");
			go.AddComponent<Mopsicus.Plugins.Plugins>();

			obj = this;
			inputField.onValueChanged.AddListener(OnValueChange);

			StartCoroutine(LoadIE());
		}

		bool downlading = false;
		IEnumerator LoadIE()
		{
			if (string.IsNullOrWhiteSpace(eventsURL))
				dataE = JsonUtility.FromJson<SearchEventArraySer>(json).data.ToList();


			downlading = true;
			UnityWebRequest www = UnityWebRequest.Get(eventsURL);
			yield return www.SendWebRequest();

			if (www.isNetworkError)
			{
				Debug.Log(www.error);
				dataE = new List<SearchEventSer>();
			}
			else
			{
				try
				{
					dataE = JsonUtility.FromJson<SearchEventArraySer>(www.downloadHandler.text).data.ToList();
				}
				catch (System.Exception)
				{

					dataE = new List<SearchEventSer>();
				}
			}


			downlading = false;
		}

		public void OpenSearch(List<SearchSer> data, System.Action<SearchSer> onTap, System.Action onCansel)
		{
			this.onTap = onTap;
			this.onCansel = onCansel;
			this.data = data;
			Show();

		}

		public void OnTapGroup(bool isEvents)
		{
			if (underline[0].enabled != isEvents)
			{
				underline[0].enabled = isEvents;
				underline[1].enabled = !isEvents;
				groups[0].SetActive(isEvents);
				groups[1].SetActive(!isEvents);
				OnValueChange(inputField.text);
			}

		}

		void Show()
		{
			content.SetActive(true);
			if (!downlading)
				StartCoroutine(LoadIE());
			OnValueChange("");
		}

		private void OnApplicationFocus(bool focusStatus)
		{
			if (!downlading && focusStatus)
				StartCoroutine(LoadIE());
		}

		void Hide()
		{
			content.SetActive(false);
		}

		void OnValueChange(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (underline[0].enabled)
				{
					DrawE(dataE.Where(x => x.name.ToLower().Contains(text.ToLower())).OrderBy(x => x.name.ToLower().IndexOfAny(text.ToLower().ToCharArray())).ToList());
				}
				else
				{
					DrawR(data.Where(x => x.name.ToLower().Contains(text.ToLower())).OrderBy(x => x.name.ToLower().IndexOfAny(text.ToLower().ToCharArray())).ToList());
				}
			}
			else
			{
				if (underline[0].enabled)
				{
					DrawE(dataE.OrderByDescending(x => x.sortingOrder).ThenBy(x => x.name.ToLower()).ToList());
				}
				else
				{
					DrawR(data.OrderByDescending(x => x.sortingOrder).ThenBy(x => x.name.ToLower()).ToList());
				}
				/*
				useItemRCount = 0;
				contentList.sizeDelta = new Vector2(contentList.sizeDelta.x, 0);
				DeactivateR();*/
			}
		}

		void DrawR(List<SearchSer> displayData)
		{
			useItemRCount = 0;
			for (int i = 0; i < maxCount && i < displayData.Count; i++)
			{
				AddItemR(displayData[i]);
			}
			contentList.sizeDelta = new Vector2(contentList.sizeDelta.x, itemPref.GetComponent<RectTransform>().sizeDelta.y * useItemRCount);
			contentList.anchoredPosition = new Vector2(0, 0);
			DeactivateR();
		}

		void DrawE(List<SearchEventSer> displayData)
		{
			useItemECount = 0;
			for (int i = 0; i < maxCount && i < displayData.Count; i++)
			{
				AddItemE(displayData[i]);
			}
			contentList.sizeDelta = new Vector2(contentList.sizeDelta.x, itemEPref.GetComponent<RectTransform>().sizeDelta.y * useItemECount);
			contentList.anchoredPosition = new Vector2(0, 0);
			DeactivateE();
		}

		void AddItemR(SearchSer sdata)
		{
			if (useItemRCount >= itemsR.Count)
			{
				RectTransform rt = Instantiate(itemPref, groups[1].transform).GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(0, -useItemRCount * rt.sizeDelta.y - rt.sizeDelta.y / 2f);
				itemsR.Add(rt.gameObject.GetComponent<SearchItem>());
			}
			itemsR[useItemRCount].Set(sdata, OnTapItem);
			useItemRCount++;
		}

		void AddItemE(SearchEventSer sdata)
		{
			if (useItemECount >= itemsE.Count)
			{
				RectTransform rt = Instantiate(itemEPref, groups[0].transform).GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(0, -useItemECount * rt.sizeDelta.y - rt.sizeDelta.y / 2f - 2);
				itemsE.Add(rt.gameObject.GetComponent<SearchEventItem>());
			}
			itemsE[useItemECount].Set(sdata, OnTapEvent);
			useItemECount++;
		}

		void DeactivateR()
		{
			for (int i = useItemRCount; i < itemsR.Count; i++)
			{
				itemsR[i].Disable();
			}
		}

		void DeactivateE()
		{
			for (int i = useItemECount; i < itemsE.Count; i++)
			{
				itemsE[i].Disable();
			}
		}

		void OnTapItem(SearchItem item)
		{
			if (onTap != null)
				onTap.Invoke(item.data);
			Hide();
		}

		void OnTapEvent(SearchEventItem item)
		{
			if (onTap != null)
				onTap.Invoke(item.data.GetSearchSer());
			Hide();
		}

		public void OnTapBack()
		{
			if (onCansel != null)
				onCansel.Invoke();
			Hide();
		}

	}
}
