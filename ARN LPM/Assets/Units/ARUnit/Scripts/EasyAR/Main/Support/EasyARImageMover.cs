using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
	public class EasyARImageMover : MonoBehaviour
	{
		public GameObject content;

		private void Start()
		{
			EasyARInterface.onImageAdd += OnImageAdd;
			EasyARInterface.onImageRemoved += OnImageRemoved;
			EasyARInterface.onImageUpdate += OnImageUpdate;
		}

		private void OnImageUpdate(ARImage ARImage)
		{
			SetPos(ARImage);
		}

		private void OnImageRemoved(ARImage ARImage)
		{
			content.SetActive(false);
		}

		private void OnImageAdd(ARImage ARImage)
		{
			SetPos(ARImage);
			content.SetActive(true);
		}

		void SetPos(ARImage ARImage)
		{
			content.transform.localPosition = ARImage.position;
			content.transform.localRotation = ARImage.rotation;
		}
	}
}
