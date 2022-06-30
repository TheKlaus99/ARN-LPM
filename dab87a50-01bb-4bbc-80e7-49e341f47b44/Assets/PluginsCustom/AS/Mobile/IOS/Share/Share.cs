using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AS.IOS
{
	public static class Native
	{
#if UNITY_IOS || UNITY_IPHONE
		[DllImport("__Internal")]
		private static extern void _TAG_Share(string iosPath, string message, string subject, int posX, int posY);
#endif
		public static void ShareDialog(string path, string subject, string message, int x, int y)
		{
#if UNITY_IOS || UNITY_IPHONE
			_TAG_Share(path, subject, message, x, y);
#endif
		}
	}
}
