using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Search.Test
{
	public class SearchTest : MonoBehaviour
	{
		public List<SearchSer> data;


		void Start()
		{
			Application.targetFrameRate = 60;
			SearchUIController.Open(data, onTap, onCansel);
		}

		private void onCansel()
		{
			Debug.Log("OnCansel");
		}

		private void onTap(SearchSer obj)
		{
			Debug.Log("onTap " + obj.name);
		}
	}
}
