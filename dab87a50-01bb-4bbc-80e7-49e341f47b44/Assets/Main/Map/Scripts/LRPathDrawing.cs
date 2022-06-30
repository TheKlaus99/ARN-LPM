using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class LRPathDrawing : MonoBehaviour
{
	public class HouseFloorInfo
	{
		public HouseItem house;
		public int floorID;
	}

	public class LinePath : HouseFloorInfo
	{
		public UILineRenderer line;
	}

	public class LiftFloor : HouseFloorInfo
	{
		public LiftIconController lift;
	}

	public MapHouseController mapHouseController;
	public GameObject UILinrePref, LiftPref;
	public Transform lineT, liftT;
	public RectTransform gpsPoint, arPoint;

	Transform transformH;

	int linesUsed = 0;
	List<LinePath> lines = new List<LinePath>();
	int liftUsed = 0;
	List<LiftFloor> lifts = new List<LiftFloor>();


	// Use this for initialization
	void Start()
	{
		transformH = transform;
		MapHouseController.onFloorSelectedChange += OnFloorChange;
		//Vector3[] v = { new Vector3(113, 0, 159), new Vector3(182, 0, 202), new Vector3(266, 0, 250) };
		//Set(v);
		//UpdateT();
	}

	float scale = 1;
	public void ChangeThickness(float size)
	{
		scale = size;
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].line.LineThickness = size;
		}
	}

	private void OnFloorChange(HouseItem house, int floorID, bool isSelect)
	{
		for (int i = 0; i < linesUsed; i++)
		{
			if (lines[i].house == house && lines[i].floorID == floorID)
			{
				lines[i].line.enabled = isSelect;
			}
		}

		for (int i = 0; i < liftUsed; i++)
		{
			if (lifts[i].house == house && lifts[i].floorID == floorID)
			{
				lifts[i].lift.SetActive(isSelect);
			}
		}
	}

	Vector2 GetV2fromV3(Vector3 pos)
	{
		return new Vector2(pos.x, pos.z);
	}

	Vector2 WorldPoint(Vector2 pos)
	{
		return transformH.TransformPoint(pos);
	}

	public void Set(Vector3[] positions)
	{

		linesUsed = 0;
		liftUsed = 0;
		if (positions.Length == 0)
		{
			DisableAllUnsed();
			return;
		}

		List<Vector2> points = new List<Vector2>();

		HouseItem lastHouse = mapHouseController.IsPointInHouse(GetV2fromV3(positions[0]));
		int lastFloorID = (lastHouse) ? lastHouse.GetFloorIDByHeight(positions[0].y) : -1;
		bool lastIndoor = lastHouse;
		if (PositionUnit.PositionInterface.posStatus == PositionUnit.PosStatus.normal)
			points.Add(arPoint.anchoredPosition);

		for (int i = (PositionUnit.PositionInterface.posStatus == PositionUnit.PosStatus.normal) ? 1 : 0; i < positions.Length; i++)
		{
			HouseItem house = mapHouseController.IsPointInHouse(GetV2fromV3(positions[i]));
			if (house == null)
			{
				if (lastIndoor)
				{
					//переход из дома на улицу 
					lastIndoor = false;
					Vector2 pos = transformH.InverseTransformPoint(lastHouse.GetIntersect(WorldPoint(GetV2fromV3(positions[i])), WorldPoint(GetV2fromV3(positions[i - 1]))));
					points.Add(pos);
					AddLine(points.ToArray(), lastFloorID, lastHouse);
					lastFloorID = -1;
					points.Clear();
					points.Add(pos);
					points.Add(GetV2fromV3(positions[i]));
				}
				else
				{
					//улица 
					points.Add(GetV2fromV3(positions[i]));
				}
			}
			else
			{
				lastHouse = house;
				if (!lastIndoor)
				{
					//переход с улицы в дом
					lastIndoor = true;
					Vector2 pos = transformH.InverseTransformPoint(lastHouse.GetIntersect(WorldPoint(GetV2fromV3(positions[i - 1])), WorldPoint(GetV2fromV3(positions[i]))));
					points.Add(pos);
					AddLine(points.ToArray());
					points.Clear();

					lastFloorID = house.GetFloorIDByHeight(positions[i].y);
					points.Add(pos);
					points.Add(GetV2fromV3(positions[i]));
				}
				else
				{
					//дом
					int floorID = house.GetFloorIDByHeight(positions[i].y);
					if (lastFloorID != floorID)
					{
						Vector3 d = positions[i] - positions[i - 1];

						if (GetV2fromV3(d).magnitude / d.magnitude > 0.17f)
						{
							points.Add(GetV2fromV3(positions[i]));
							AddLine(points.ToArray(), lastFloorID, lastHouse);
							points.Clear();
							points.Add(GetV2fromV3(positions[i - 1]));
							points.Add(GetV2fromV3(positions[i]));
						}
						else
						{
							//islift
							AddLift(GetV2fromV3(positions[i - 1]), false, floorID > lastFloorID, house.floors[floorID].name, lastFloorID, house);
							AddLift(GetV2fromV3(positions[i]), true, floorID < lastFloorID, house.floors[lastFloorID].name, floorID, house);

							AddLine(points.ToArray(), lastFloorID, lastHouse);
							points.Clear();
							points.Add(GetV2fromV3(positions[i]));
						}

						lastFloorID = floorID;
					}
					else
					{
						points.Add(GetV2fromV3(positions[i]));
					}
				}

			}
		}
		if (lastFloorID == -1)
		{
			AddLine(points.ToArray());
		}
		else
		{
			AddLine(points.ToArray(), lastFloorID, lastHouse);

		}

		DisableAllUnsed();
	}

	void AddLift(Vector2 pos, bool from, bool isUp, string name, int floorID, HouseItem house)
	{
		if (liftUsed >= lifts.Count)
		{
			lifts.Add(new LiftFloor { house = house, floorID = floorID, lift = Instantiate(LiftPref, liftT).GetComponent<LiftIconController>() });
			//lifts[liftUsed].lift.transform.localScale = Vector3.one / lifts[liftUsed].lift.transform.lossyScale.x;
			lifts[liftUsed].lift.transform.rotation = Quaternion.Euler(0, 0, 0);
			constantUIScale c = lifts[liftUsed].lift.GetComponent<constantUIScale>();
			c.SetScale(1);
			c.SetMinScale(.1f);
			c.Recalculate();
		}
		else
		{
			lifts[liftUsed].house = house;
			lifts[liftUsed].floorID = floorID;
		}
		lifts[liftUsed].lift.SetActive((floorID == -1) ? true : (house.selectedFloorID != floorID) ? false : (house.floors[floorID].isSelect) ? true : false);
		lifts[liftUsed].lift.Set(pos, from, isUp, name);

		liftUsed++;
	}

	void DisableAllUnsed()
	{
		for (int i = linesUsed; i < lines.Count; i++)
		{
			lines[i].line.enabled = false;
		}

		for (int i = liftUsed; i < lifts.Count; i++)
		{
			lifts[i].lift.SetActive(false);
		}
	}

	void AddLine(Vector2[] positions, int floorID = -1, HouseItem house = null)
	{
		if (positions.Length == 0)
			return;

		if (linesUsed >= lines.Count)
		{
			lines.Add(new LinePath { house = house, floorID = floorID, line = Instantiate(UILinrePref, lineT).GetComponent<UILineRenderer>() });
		}
		else
		{
			lines[linesUsed].house = house;
			lines[linesUsed].floorID = floorID;
		}

		lines[linesUsed].line.enabled = (floorID == -1) ? true : lines[linesUsed].house.floors[floorID].isSelect;
		lines[linesUsed].line.Points = positions;
		lines[linesUsed].line.LineThickness = scale;

		linesUsed++;

	}

	Vector2 lastPos;
	private void Update()
	{
		if (linesUsed != 0 && PositionUnit.PositionInterface.posStatus == PositionUnit.PosStatus.normal && (arPoint.anchoredPosition - lastPos).sqrMagnitude > 1f)
		{
			lastPos = arPoint.anchoredPosition;
			lines[0].line.m_points[0] = arPoint.anchoredPosition;
			lines[0].line.SetAllDirty();
		}
	}

}
