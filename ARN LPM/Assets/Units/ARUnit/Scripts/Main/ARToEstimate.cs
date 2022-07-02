using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ARUnit
{
	public class ARToEstimate : MonoBehaviour
	{

		private ARMap map;
		private static ARToEstimate instance;

		public delegate void OnEstimateGenerate(PositionUnit.Estimate estimate);
		public static event OnEstimateGenerate onEstimateGenerate;

		public delegate void OnImageTrackUpdate(ARImage ARImage, float p);
		public static event OnImageTrackUpdate onImageTrackUpdate;

		public delegate void OnImageTrackStop();
		public static event OnImageTrackStop onImageTrackStop;

		public static void GenerateEstimate(PositionUnit.Estimate estimate)
		{
			PositionUnit.PositionInterface.AddEstimate(estimate);
			if (onEstimateGenerate != null)
				onEstimateGenerate.Invoke(estimate);
		}

		void Awake()
		{
			if (instance != null)
			{
				Destroy(this);
			}
			else
			{
				instance = this;
			}
			map = ARNSettings.settings.ARMap;
			ARInterface.onImageUpdate += onImageUpdate;
			ARInterface.onImageAdd += onImageAdd;
			ARInterface.onImageRemoved += onImageRemove;
		}

		bool needToUpdate(ARImage newImage)
		{
			if (Vector3.Distance(pos, newImage.position) < ARNSettings.settings.minDistanceToUpdate &&
				Quaternion.Angle(rot, newImage.rotation) < ARNSettings.settings.minAngleToUpdate)
				return false;

			return true;
		}

		ARImage lastTracked;
		float timeStartTracked;
		float timeLastTracked;

		Quaternion rot;
		Vector3 pos;

		void GenerateEstimate(ARImage ARImage)
		{
			if (ARInterface.ARStatus != ARStatus.Running)
				return;

			ARMap.ARImageTransform imageTransform;
			if (TryGetARImageValue(map.imageAnchors, ARImage.name, out imageTransform))
			{
				PositionUnit.Estimate estimate = new PositionUnit.Estimate(
					ARImage.name,
					new PositionUnit.Vector3S(ARImage.position),
					new PositionUnit.Vector3S(imageTransform.position),
					deltaAngle(imageTransform.rotation, ARImage.rotation),
					ARNSettings.settings.angleAccuracy,
					ARNSettings.settings.horizontalAccuracy
				);
				GenerateEstimate(estimate);
			}
		}

		float deltaAngle(Quaternion rot1, Quaternion rot2)
		{
			Vector2 d = new Vector2((rot1 * Vector3.right).x, (rot1 * Vector3.right).z).normalized;
			Vector2 d2 = new Vector2((rot2 * Vector3.right).x, (rot2 * Vector3.right).z).normalized;

			return angleBetweenVectors(d, d2);
		}

		float angleBetweenVectors(Vector2 a, Vector2 b)
		{
			float angle = Vector3.Angle(a, b);
			float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(a, b)));
			float signed_angle = angle * sign;
			return signed_angle;
		}



		Vector3 lastNoisPos;
		void onImageAdd(ARImage ARImage)
		{
			lastTracked = ARImage;
			timeStartTracked = Time.time;
			timeLastTracked = Time.time;

			rot = ARImage.rotation;
			pos = ARImage.position;
			if (addImage != null)
			{
				StopCoroutine(addImage);
			}
			addImage = StartCoroutine(AddImageIE());
		}

		void onImageRemove(ARImage ARImage)
		{
			lastTracked = null;
		}

		void onImageUpdate(ARImage ARImage)
		{
			if (lastTracked == null)
				return;

			if (lastTracked.name != ARImage.name || needToUpdate(ARImage))
			{
				onImageAdd(ARImage);
			}
			lastTracked = ARImage;
			timeLastTracked = Time.time;
		}

		bool TryGetARImageValue(ARMap.ARImageTransform[] array, string name, out ARMap.ARImageTransform element)
		{
			foreach (var item in array)
			{
				if (item.name == name)
				{
					element = item;
					return true;
				}
			}
			element = null;
			return false;
		}


		Coroutine addImage;
		IEnumerator AddImageIE()
		{
			float timeLastUpdate = timeStartTracked;
			float lastSend = timeStartTracked;
			while (Time.time - timeLastTracked < (ARNSettings.settings.timeToGet / 3f))
			{
				yield return new WaitForEndOfFrame();
				if (Time.time != timeLastUpdate)
				{
					rot = Quaternion.Slerp(rot, lastTracked.rotation, (Time.time - timeLastUpdate) / (timeLastUpdate - timeStartTracked));
					pos = Vector3.Lerp(pos, lastTracked.position, (Time.time - timeLastUpdate) / (timeLastUpdate - timeStartTracked));
				}
				timeLastUpdate = Time.time;

				if (onImageTrackUpdate != null)
				{
					ARImage image = new ARImage(lastTracked.name, pos, rot);
					onImageTrackUpdate.Invoke(image, (Time.time - timeStartTracked) / ARNSettings.settings.timeToGet);
				}

				if (Time.time - timeStartTracked > ARNSettings.settings.timeToGet)
				{
					if (Time.time - lastSend > ARNSettings.settings.timeToGet / 3f)
					{
						ARImage image = new ARImage(lastTracked.name, pos, rot);
						GenerateEstimate(image);
						lastSend = Time.time;
					}
				}
			}

			if (onImageTrackStop != null)
			{
				onImageTrackStop.Invoke();
			}
			addImage = null;
		}

	}
}
