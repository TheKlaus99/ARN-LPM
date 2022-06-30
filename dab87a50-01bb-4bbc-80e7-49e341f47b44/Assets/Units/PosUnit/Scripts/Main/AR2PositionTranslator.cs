using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit
{
	public class AR2PositionTranslator : MonoBehaviour
	{
		private void Awake()
		{
			ARUnit.ARInterface.onARTransformUpdate += OnARTransformUpdate;
		}

		void OnARTransformUpdate(ARUnit.ARTransform ARTransform)
		{
			PositionUnit.PositionInterface.UpdateARRAWCameraTransform(ARTransform.position, ARTransform.rotation);
		}

		private void OnDestroy()
		{
			ARUnit.ARInterface.onARTransformUpdate -= OnARTransformUpdate;
		}
	}
}
