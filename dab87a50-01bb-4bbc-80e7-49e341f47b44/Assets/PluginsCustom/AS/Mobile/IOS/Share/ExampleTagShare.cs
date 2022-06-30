using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ExampleTagShare : MonoBehaviour
{
#if UNITY_IOS || UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _TAG_Share(string iosPath, string message, string subject, int posX, int posY);
#endif
	void Demo()
	{
#if UNITY_IOS || UNITY_IPHONE
		_TAG_Share("path", "mess", "subj", 100, 100);
#endif
	}

	void Demo1()
	{
		AS.IOS.Native.ShareDialog("path", "mess", "subj", 100, 100);
	}
}
