using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{

	public class EasyARInterface
	{

		private static ARStatus ARStatus_p;
		public static ARStatus ARStatus
		{
			get
			{
				return ARStatus_p;
			}
		}

		public delegate void OnStatusChange(ARStatus ARStatus);
		public delegate void OnStartEasyAR();
		public delegate void OnStopEasyAR();

		public delegate void OnImageAdd(ARImage ARImage);
		public delegate void OnImageUpdate(ARImage ARImage);
		public delegate void OnImageRemoved(ARImage ARImage);


		public static event OnStatusChange onStatusChange;
		public static event OnStartEasyAR onStartEasyAR;
		public static event OnStopEasyAR onStopEasyAR;

		public static event OnImageUpdate onImageUpdate;
		public static event OnImageAdd onImageAdd;
		public static event OnImageRemoved onImageRemoved;


		public static void StartEasyAR()
		{
			if (onStartEasyAR != null && ARStatus_p != ARStatus.Running)
			{
				onStartEasyAR.Invoke();
			}
		}

		public static void StopEasyAR()
		{
			if (onStopEasyAR != null && ARStatus_p == ARStatus.Running)
			{
				onStopEasyAR.Invoke();
			}
		}

		public static void ARImageUpdate(ARImage ARImage)
		{
			if (onImageUpdate != null && ARStatus_p == ARStatus.Running)
				onImageUpdate.Invoke(ARImage);
		}

		public static void ARImageAdd(ARImage ARImage)
		{
			if (onImageAdd != null && ARStatus_p == ARStatus.Running)
				onImageAdd.Invoke(ARImage);
		}

		public static void ARImageRemove(ARImage ARImage)
		{
			if (onImageRemoved != null && ARStatus_p == ARStatus.Running)
				onImageRemoved.Invoke(ARImage);
		}

		public static void ChangeStatus(ARStatus ARStatus)
		{
			if (ARStatus_p != ARStatus)
			{
				ARStatus_p = ARStatus;
				if (onStatusChange != null)
					onStatusChange.Invoke(ARStatus);
			}
		}

	}
}
