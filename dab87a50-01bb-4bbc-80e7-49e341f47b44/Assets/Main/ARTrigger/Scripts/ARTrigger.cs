using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTrigger : MonoBehaviour
{
	[System.Serializable]
	public class OnTriggerEvent : UnityEngine.Events.UnityEvent<string> { }

	public OnTriggerEvent onEnter = new OnTriggerEvent();
	public OnTriggerEvent onExit = new OnTriggerEvent();

	public string paramer;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "MainCamera")
		{
			Debug.Log("Enter");
			onEnter.Invoke(paramer);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "MainCamera")
		{
			Debug.Log("Exit");
			onExit.Invoke(paramer);
		}
	}
}
