using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARUnit
{
	public class ARImageMover : MonoBehaviour
	{
		public string imageName;

		public GameObject child;
		// Use this for initialization
		void Start()
		{
			ARInterface.onImageUpdate += ARImageUpdate;
			ARInterface.onImageAdd += ARImageAdd;
			ARInterface.onImageRemoved += ARImageRemove;
		}

		public ARImage ARImage = new ARImage();

		void ARImageAdd(ARImage ARImage)
		{
			if (string.IsNullOrEmpty(imageName) || imageName == ARImage.name)
			{
				this.ARImage = ARImage;
				child.SetActive(true);
			}
		}

		void ARImageUpdate(ARImage ARImage)
		{
			if (string.IsNullOrEmpty(imageName) || imageName == ARImage.name)
			{
				this.ARImage = ARImage;
			}
		}

		void ARImageRemove(ARImage ARImage)
		{
			if (string.IsNullOrEmpty(imageName) || imageName == ARImage.name)
			{
				this.ARImage = ARImage;
				child.SetActive(false);
			}
		}

		// Update is called once per frame
		void Update()
		{
			transform.localPosition = ARImage.position;
			transform.localRotation = ARImage.rotation;
		}
	}
}
