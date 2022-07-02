using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARUnit.Tool
{
	public class ARMapTool : MonoBehaviour
	{
		public ARMap targetScriptable;

		public void SetAllTargets()
		{
			ARMapToolImageAnchor[] images = GetComponentsInChildren<ARMapToolImageAnchor>();

			List<ARMap.ARImageTransform> result = new List<ARMap.ARImageTransform>();
			foreach (var item in images)
			{
				if (item.use)
					result.Add(
						new ARMap.ARImageTransform
						{
							name = item.name,
								position = transform.InverseTransformPoint(item.gameObject.transform.position),
								rotation = Quaternion.Inverse(transform.rotation) * item.transform.rotation
						}
					);
			}

			targetScriptable.imageAnchors = result.ToArray();
		}
	}

}
