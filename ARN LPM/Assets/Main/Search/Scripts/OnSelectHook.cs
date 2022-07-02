using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Search
{
	public class OnSelectHook : MonoBehaviour, ISelectHandler
	{
		public UnityEvent onSelect;
		//Do this when the selectable UI object is selected.
		public void OnSelect(BaseEventData eventData)
		{
			onSelect.Invoke();
		}
	}
}
