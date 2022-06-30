using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GPSUnit.Tool
{

	public class GPSMapTool : MonoBehaviour
	{
		public GPSMap targetScriptable;

		public Transform pivotMain, pivotScale, scaleP;

		[Space]
		public Transform source;


		public void SetMap()
		{
			targetScriptable.altitude = pivotMain.GetComponent<GPSAnchor>().altitude;
			targetScriptable.latitude = pivotMain.GetComponent<GPSAnchor>().latitude;
			targetScriptable.longitude = pivotMain.GetComponent<GPSAnchor>().longitude;
			targetScriptable.localPos = pivotMain.localPosition;

			targetScriptable.width = scaleP.localPosition.x * 2;
			targetScriptable.height = scaleP.localPosition.z * 2;

		}

		public void SetSize()
		{
			GPSInfo pMainInfo = new GPSInfo(pivotMain.GetComponent<GPSAnchor>());
			GPSInfo pScaleInfo = new GPSInfo(pivotScale.GetComponent<GPSAnchor>());
			Vector3 realDelta = GPSUtility.GPSToVector(pMainInfo, pScaleInfo);
			Vector3 currentDelta = pivotMain.localPosition - pivotScale.localPosition;

			float scaleFactor = realDelta.magnitude / currentDelta.magnitude;
			source.localScale *= scaleFactor;

			pivotMain.localPosition = pivotMain.localPosition * scaleFactor;
			pivotScale.localPosition = pivotScale.localPosition * scaleFactor;

			scaleP.localPosition = new Vector3(source.localScale.x * 10, 0, source.localScale.z * 10);

		}
	}
}
