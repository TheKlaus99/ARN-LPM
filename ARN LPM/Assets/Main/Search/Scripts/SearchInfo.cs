using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Search
{
	public class SearchInfo : MonoBehaviour
	{
		public Text nameT, infoT;

		System.Action onTap;
		public void Open(SearchSer data, System.Action onTap)
		{
			this.onTap = onTap;
			nameT.text = data.name;
			infoT.text = data.info;
		}


		public void OnTapBack()
		{
			Destroy(gameObject, 0.1f);
		}

		public void onTapNav()
		{
			onTap.Invoke();
			OnTapBack();
		}
	}
}
