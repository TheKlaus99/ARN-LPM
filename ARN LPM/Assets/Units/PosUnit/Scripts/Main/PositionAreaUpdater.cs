using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit
{
	[RequireComponent(typeof(MapHouseController))]
	public class PositionAreaUpdater : MonoBehaviour
	{

		MapHouseController mapHouseController;
		// Use this for initialization
		void Start()
		{
			mapHouseController = GetComponent<MapHouseController>();
			StartCoroutine(UpdateIE());
		}

		IEnumerator UpdateIE()
		{
			while (true)
			{
				yield return new WaitForSeconds(1);
				if (PositionInterface.posStatus == PosStatus.normal)
				{
					Vector3 camPos = PositionInterface.PositionController.TranslatePosition(ARUnit.ARInterface.rawARTransform.position);
					if (mapHouseController.IsPointInHouse(new Vector2(camPos.x * ARNSettings.settings.pixelsInMeter.x, camPos.z * ARNSettings.settings.pixelsInMeter.y)))
					{
						PositionInterface.ChangeArea(Area.inDoor);
					}
					else
					{
						PositionInterface.ChangeArea(Area.outDoor);
					}
				}
			}
		}
	}
}
