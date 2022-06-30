using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiftMarkerItem : MonoBehaviour
{
	public Text floorT;
	public SpriteRenderer sprite;
	public void Set(Transform cam, string floorName, bool isUp)
	{
		GetComponent<LookAtTransform>().source = cam;
		GetComponent<ConstantScaleReleativeCam>().target = cam;
		sprite.flipY = !isUp;
		floorT.text = floorName;
	}
}
