using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Search
{
    public class SearchEventItem : MonoBehaviour
    {
        public GameObject content, infoPref;
        public Text nameT, dateT, timeT;

        bool isDiable = false;
        [HideInInspector] public SearchEventSer data;
        System.Action<SearchEventItem> onTap;

        public void Set(SearchEventSer data, System.Action<SearchEventItem> onTap)
        {
            this.data = data;
            this.onTap = onTap;
            nameT.text = data.name;
            dateT.text = data.date;
            timeT.text = data.time;

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
            Instantiate(infoPref).GetComponent<SearchInfo>().Open(data.GetSearchSer(), OnTapNavButton);
        }

        private void OnTapNavButton()
        {
            onTap.Invoke(this);
        }
    }
}
