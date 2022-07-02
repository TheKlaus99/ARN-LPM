using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PositionUnit
{
	public enum Area
	{
		unknown,
		inDoor,
		outDoor
	}

	public enum PosStatus
	{
		unknown,
		normal,
		lost
	}

	public class PositionInterface
	{
		private static Area area_m = Area.unknown;
		public static Area area
		{
			get
			{
				return area_m;
			}
		}

		private static Vector3 camPosition_m = Vector3.zero;
		public static Vector3 camPosition
		{
			get
			{
				return camPosition_m;
			}
		}

		private static PosStatus posStatus_m = PosStatus.unknown;
		public static PosStatus posStatus
		{
			get
			{
				return posStatus_m;
			}
		}

		private static PositionController PositionController_m;
		public static PositionController PositionController
		{
			get
			{
				if (PositionController_m == null)
				{
					Debug.Log("Create PositionController from PositionInterface");
					GameObject go = new GameObject("PositionController", typeof(PositionController));
				}
				return PositionController_m;
			}

			set
			{
				PositionController_m = value;
			}
		}

		public delegate void OnEstimateAdd(Estimate estimate);
		public delegate void OnAreaChange(Area area);
		public delegate void OnStatusChange(PosStatus status);
		public delegate void OnARRAWCameraTramsformUpdate(Vector3 position, Quaternion rotation);
		public delegate void OnARCameraTramsformUpdate(Vector3 position, Quaternion rotation);
		public delegate void OnResetPosition();

		public static event OnEstimateAdd onEstimateAdd;
		public static event OnAreaChange onAreaChange;
		public static event OnStatusChange onStatusChange;
		public static event OnARRAWCameraTramsformUpdate onARRAWCameraTramsformUpdate;
		public static event OnARCameraTramsformUpdate onARCameraTramsformUpdate;
		public static event OnResetPosition onResetPosition;

		public static void ChangeArea(Area area)
		{
			if (area_m != area)
			{
				area_m = area;
				Debug.Log("Area changed to " + area_m);
				if (onAreaChange != null)
					onAreaChange.Invoke(area);
			}
		}

		public static void ChangeStatus(PosStatus status)
		{
			if (posStatus_m == status)
				return;

			posStatus_m = status;
			if (onStatusChange != null)
				onStatusChange.Invoke(status);
		}

		public static void AddEstimate(Estimate estimate)
		{
			if (onEstimateAdd != null)
				onEstimateAdd.Invoke(estimate);
		}

		public static void UpdateARRAWCameraTransform(Vector3 position, Quaternion rotation)
		{
			if (onARRAWCameraTramsformUpdate != null)
				onARRAWCameraTramsformUpdate.Invoke(position, rotation);
		}

		public static void UpdateARCameraTransform(Vector3 position, Quaternion rotation)
		{
			camPosition_m = position;
			if (onARCameraTramsformUpdate != null)
				onARCameraTramsformUpdate.Invoke(position, rotation);
		}

		public static void ResetPosition()
		{
			if (onResetPosition != null)
				onResetPosition.Invoke();
		}

	}
}
