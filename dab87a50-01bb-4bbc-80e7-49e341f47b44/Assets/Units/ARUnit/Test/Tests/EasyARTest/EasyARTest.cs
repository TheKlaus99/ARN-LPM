using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit.Test
{

	public class EasyARTest : MonoBehaviour
	{

		private void Start()
		{
			EasyARInterface.onImageAdd += OnImageAdd;
			EasyARInterface.onImageRemoved += OnImageRemoved;
			EasyARInterface.onImageUpdate += OnImageUpdate;
			EasyARInterface.onStartEasyAR += OnStartEasyAR;
			EasyARInterface.onStopEasyAR += OnStopEasyAR;
			EasyARInterface.onStatusChange += OnStatusChange;
		}

		private void OnStatusChange(ARStatus ARStatus)
		{
			Debug.Log(ARStatus);
		}

		private void OnStopEasyAR()
		{
			Debug.Log("OnStopEasyAR");
		}

		private void OnStartEasyAR()
		{
			Debug.Log("OnStartEasyAR");
		}

		private void OnImageUpdate(ARImage ARImage)
		{
			Debug.Log("OnImageUpdate " + ARImage.ToString());
		}

		private void OnImageRemoved(ARImage ARImage)
		{
			Debug.Log("OnImageRemoved " + ARImage.ToString());
		}

		private void OnImageAdd(ARImage ARImage)
		{
			Debug.Log("OnImageAdd " + ARImage.ToString());
		}

		public void OnTapInitialize()
		{
			EasyARTracker.Initialization();
		}

		public void OnTapStart()
		{
			EasyARInterface.StartEasyAR();
		}

		public void OnTapStop()
		{
			EasyARInterface.StopEasyAR();
		}
	}
}
