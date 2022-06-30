using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PositionUnit
{
	public class PositionController : MonoBehaviour
	{
		public float maxHorizontalAccuracy = 20; //погрешность после которой будут удаляться эстимейты
		public List<Estimate> estimates = new List<Estimate>();

		public float currentAngle = 0;
		public float currentAngleAccuracy = 360;
		public Estimate currentEstimate = new Estimate("startPos", new Vector3S(0, 0, 0), new Vector3S(0, 0, 0), 0, 360, 50);

		public Estimate pivot = new Estimate("startPos", new Vector3S(0, 0, 0), new Vector3S(0, 0, 0), 0, 360, 50);

		Estimate fullBestAngleAccuracy;

		Vector3 camPos = Vector3.zero;



		#region Dubug
		public Estimate a1, a2; //TODO: remove this debug
		#endregion

		private void Awake()
		{
			PositionInterface.PositionController = this;
			PositionInterface.onEstimateAdd += UpdateEstimate;
			PositionInterface.onARRAWCameraTramsformUpdate += UpdateARCam;
			PositionInterface.onResetPosition += ResetPosition;

			ARUnit.ARInterface.onFloorLevelUpdate += onFloorLevelUpdate;
		}


		#region Events

		public void AddEstimate(Estimate estimate)
		{
			AddEstimateCalculate(estimate);
		}

		void AddEstimateCalculate(Estimate estimate)
		{
			estimates.Add(recalculateEstimate(ref estimate));

			FindFullBestAngleAccuracy(estimate);

			//RemoveOldEstimates();
			if (estimate.horizontalAccuracy < ARNSettings.settings.maxAccToPivotByOutOfAcc)
			{
				UpdatePivotByOutOfAcc(estimate);
			}
			else
			{
				UpdatePivotByByMinAcc(estimate);
			}

			UpdateAngle();

			if (currentAngleAccuracy < ARNSettings.settings.minAngleAccuracyToStart)
			{
				if (PositionInterface.posStatus != PosStatus.normal)
					PositionInterface.ChangeStatus(PosStatus.normal);
			}
			else if (PositionInterface.posStatus == PosStatus.normal)
			{
				PositionInterface.ChangeStatus(PosStatus.lost);
			}
		}

		Estimate recalculateEstimate(ref Estimate estimate)
		{
			if (estimate.name.Contains("GPS"))
			{
				Vector3 samplePos = PositionCorrecter.GetSamplePositoin(estimate.mapPos.ToVector3(), estimate.horizontalAccuracy);
				samplePos.y = estimate.mapPos.y;
				estimate.horizontalAccuracy = estimate.horizontalAccuracy + Vector3.Distance(estimate.mapPos.ToVector3(), samplePos);
				estimate.mapPos = new Vector3S(samplePos);

				UIDebug.Log("Recalculate estimate");
			}

			return estimate;
		}

		void RemoveEstimate(Estimate estimate)
		{
			for (int i = estimates.Count - 1; i >= 0; i -= 1)
			{
				if (estimates[i].name == estimate.name)
				{
					estimates.RemoveAt(i);
					UIDebug.Log("remove estimate at " + i);
					break;
				}
			}
		}

		void UpdateEstimate(Estimate estimate)
		{
			RemoveEstimate(estimate);
			AddEstimate(estimate);
		}

		void UpdateARCam(Vector3 position, Quaternion rotation)
		{
			AddAccuracy(position);
			camPos = TranslatePosition(position);
			if (PositionInterface.area == Area.outDoor)
			{
				Vector3 samplePos = PositionCorrecter.GetSamplePositoin(camPos, ARNSettings.settings.accuracyToSample);
				camPos.x = samplePos.x;
				camPos.z = samplePos.z;
			}

			PositionInterface.UpdateARCameraTransform(camPos, TranslateRotation(rotation));
		}

		void onFloorLevelUpdate(float floorLevel)
		{
			if (currentEstimate.name.Contains("GPS"))
			{
				currentEstimate.mapPos.y = currentEstimate.realPos.y - floorLevel;
				pivot.mapPos.y = currentEstimate.mapPos.y;
				UIDebug.Log("Update floor Level " + pivot.mapPos.y);
			}
		}

		private void ResetPosition()
		{
			estimates.Clear();
			pivot = new Estimate("startPos", new Vector3S(0, 0, 0), new Vector3S(0, 0, 0), 0, 360, 50);
			currentEstimate = new Estimate("startPos", new Vector3S(0, 0, 0), new Vector3S(0, 0, 0), 0, 360, 50);
			currentAngleAccuracy = 360;
			currentAngle = 0;
			camPos = Vector3.zero;
			lastPos = Vector3.zero;
			fullBestAngleAccuracy = null;
		}

		#endregion

		void UpdateAreaIfNeed(Estimate estimate)
		{

			/*
			TODO: красиво, но ввиду привязки к пешеходным маршрутом пришлось изменять через UI и координаты домов на карте 
			if (PositionInterface.area == Area.unknown || currentEstimate == null || estimate.name.Contains("GPS") != currentEstimate.name.Contains("GPS"))
			{
				PositionInterface.ChangeArea(estimate.name.Contains("GPS") ? Area.outDoor : Area.inDoor);
			}*/

			if (PositionInterface.area == Area.unknown)
			{
				PositionInterface.ChangeArea(estimate.name.Contains("GPS") ? Area.outDoor : Area.inDoor);
			}
		}

		//если вышел за пределы новой погрешности, корректировать по ней
		void UpdatePivotByOutOfAcc(Estimate estimate)
		{
			if (estimate.horizontalAccuracy < Vector3.Distance(estimate.mapPos.ToVector3(), camPos))
			{
				UIDebug.Log("Update pivot by OutOfAcc: cur = " + ((currentEstimate != null) ? currentEstimate.horizontalAccuracy.ToString() : "-") + "; new = " + currentEstimate.horizontalAccuracy);
				UpdateAreaIfNeed(estimate);
				pivot = estimate;
				currentEstimate = estimate;
			}

		}

		//если новая погрешность * k < текущей 
		void UpdatePivotByByMinAcc(Estimate estimate)
		{
			if (currentEstimate == null || estimate.horizontalAccuracy * ARNSettings.settings.exceedKToPivotByMinAcc < currentEstimate.horizontalAccuracy)
			{
				UIDebug.Log("Update pivot by MinAcc: cur = " + ((currentEstimate != null) ? currentEstimate.horizontalAccuracy.ToString() : "-") + "; new = " + currentEstimate.horizontalAccuracy);
				UpdateAreaIfNeed(estimate);
				pivot = estimate;
				currentEstimate = estimate;
			}
		}

		void FindFullBestAngleAccuracy(Estimate estimate)
		{
			if (fullBestAngleAccuracy == null)
			{
				fullBestAngleAccuracy = estimate;
			}
			else if (estimate.angleAccuracy <= fullBestAngleAccuracy.angleAccuracy)
			{
				fullBestAngleAccuracy = estimate;
			}
		}

		void UpdateAngle()
		{
			Debug.Log(estimates.Count);

			if (estimates.Count >= 2)
			{
				float minAngleAccuracy = 360;
				Estimate best1 = estimates[0],
					best2 = estimates[0];


				#region samrtCorrect
				/*
				float acc = 0;
				float summ = 0;
				for (int i = 0; i < estimates.Count - 1; i++)
				{
					for (int j = i + 1; j < estimates.Count; j++)
					{
						acc = 2 * Mathf.Rad2Deg * Mathf.Atan(TangensAngleAccuracy(estimates[i], estimates[j]));
						if (acc < 80)
							summ += acc;
					}
				}
				float res = 0;
				for (int i = 0; i < estimates.Count - 1; i++)
				{
					for (int j = i + 1; j < estimates.Count; j++)
					{
						acc = 2 * Mathf.Rad2Deg * Mathf.Atan(TangensAngleAccuracy(estimates[i], estimates[j]));
						if (acc < 80)
							res += CalculateAngle(estimates[i], estimates[j]) * acc / summ;
					}
				}*/
				#endregion

				//поиск тангенса минимального угла и двух эстимейтов его образующего
				for (int i = 0; i < estimates.Count - 1; i++)
				{
					for (int j = i + 1; j < estimates.Count; j++)
					{
						float angleAccuracy = TangensAngleAccuracy(estimates[i], estimates[j]);
						if (angleAccuracy < minAngleAccuracy)
						{
							minAngleAccuracy = angleAccuracy;
							best1 = estimates[i];
							best2 = estimates[j];
						}
					}
				}
				minAngleAccuracy = 2 * Mathf.Rad2Deg * Mathf.Atan(minAngleAccuracy);


				if (fullBestAngleAccuracy != null && fullBestAngleAccuracy.angleAccuracy <= minAngleAccuracy + 1)
				{
					currentAngleAccuracy = fullBestAngleAccuracy.angleAccuracy;
					currentAngle = fullBestAngleAccuracy.correctAngle;
					UIDebug.Log("Correct by best estimate a = " + currentAngle + "; acc = " + currentAngleAccuracy);
					a1 = fullBestAngleAccuracy;
					a2 = fullBestAngleAccuracy;
				}
				else
				{
					if (minAngleAccuracy <= currentAngleAccuracy + 1)
					{
						currentAngleAccuracy = minAngleAccuracy;
						currentAngle = CalculateAngle(best1, best2);
						UIDebug.Log("Calculate between 2 est a = " + currentAngle + "; acc = " + currentAngleAccuracy);
						a1 = best1;
						a2 = best2;
					}
				}
			}
			else if (estimates.Count == 1)
			{
				if (estimates[0].angleAccuracy <= currentAngleAccuracy + 1)
				{
					currentAngle = estimates[0].correctAngle;
					currentAngleAccuracy = estimates[0].angleAccuracy;
					UIDebug.Log("One estimate correct a = " + currentAngle + "; acc = " + currentAngleAccuracy);
					a1 = estimates[0];
					a2 = estimates[0];
				}
			}
		}

		Vector3 lastPos = Vector3.zero;
		void AddAccuracy(Vector3 position)
		{
			foreach (var item in estimates)
			{
				item.horizontalAccuracy += (Vector3.Distance(lastPos, position) * ARNSettings.settings.accuracyByMeter);
			}
			lastPos = position;
		}

		void RemoveOldEstimates()
		{
			estimates = estimates.Where(a => a.horizontalAccuracy < maxHorizontalAccuracy).ToList();
		}

		float CalculateAngle(Estimate e1, Estimate e2)
		{
			Vector3 v1 = (e1.realPos - e2.realPos).ToVector3(); //получаю смещение между эстимейтами в жизни
			Vector3 v4 = (e1.mapPos - e2.mapPos).ToVector3(); //олучаю смещение между эстимейтами на карте
			return -AngleBetvinVectors(new Vector2(v1.x, v1.z), new Vector2(v4.x, v4.z));
		}

		float AngleBetvinVectors(Vector2 a, Vector2 b)
		{
			// angle in [0,180]
			float angle = Vector3.Angle(a, b);
			float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(a, b)));

			// angle in [-179,180]
			float signed_angle = angle * sign;

			// angle in [0,360] (not used but included here for completeness)
			//float angle360 =  (signed_angle + 180) % 360;

			return signed_angle;
		}

		float TangensAngleAccuracy(Estimate e1, Estimate e2)
		{
			return (e1.horizontalAccuracy + e2.horizontalAccuracy) / Vector3.Distance(e1.mapPos.ToVector3(), e2.mapPos.ToVector3());
		}

		public Vector3 TranslatePositionReleativeCam(Vector3 position)
		{

			Vector3 pos = position - ARUnit.ARInterface.rawARTransform.position;

			Quaternion q = Quaternion.Euler(0, currentAngle, 0);
			pos = q * pos + camPos;

			return pos;
		}

		public Vector3 TranslatePosition(Vector3 position)
		{
			Vector3 pos = (position - pivot.realPos.ToVector3());
			Quaternion q = Quaternion.Euler(0, currentAngle, 0);
			pos = q * pos;
			pos += pivot.mapPos.ToVector3();
			return pos;
		}

		public Quaternion TranslateRotation(Quaternion rotation)
		{
			return Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y + currentAngle, rotation.eulerAngles.z);
		}

	}
}
