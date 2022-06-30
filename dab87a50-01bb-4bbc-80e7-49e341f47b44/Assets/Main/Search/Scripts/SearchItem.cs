using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Search
{

	public class SearchItem : MonoBehaviour
	{
		public GameObject content, infoPref;
		public Text nameT;

		bool isDiable = false;
		[HideInInspector] public SearchSer data;
		System.Action<SearchItem> onTap;

		public void Set(SearchSer data, System.Action<SearchItem> onTap)
		{
			this.data = data;
			this.onTap = onTap;
			nameT.text = data.name;

			if (isDiable)
			{
				isDiable = false;
				content.SetActive(true);
			}
		}

		public void Disable()
		{
			if (!isDiable)
			{
				isDiable = true;
				content.SetActive(false);
			}
		}

		public void OnTap()
		{
			Instantiate(infoPref).GetComponent<SearchInfo>().Open(data, OnTapNavButton);
		}

		private void OnTapNavButton()
		{
			onTap.Invoke(this);
		}
	}
}
