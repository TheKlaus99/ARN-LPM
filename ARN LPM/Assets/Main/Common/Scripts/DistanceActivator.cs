using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceActivator : MonoBehaviour
{

	[System.Serializable]
	public struct Group
	{
		public float distance;
		public GameObject[] objects;
	}


	public Transform target;
	public Group[] Groups;

	// Update is called once per frame
	void Update()
	{
		foreach (var item in Groups)
		{
			foreach (var obj in item.objects)
			{
				if (Vector3.Distance(target.position, obj.transform.position) > item.distance)
				{
					obj.SetActive(false);
				}
				else
				{
					obj.SetActive(true);
				}
			}
		}
	}
}
