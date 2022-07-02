using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DrawPath.Test
{
	public class DrawPath_test : MonoBehaviour
	{

		public bool onStart, onUpdate;
		public LineRenderer lr;

		DrawPath drawPath;
		// Use this for initialization
		void Start()
		{
			drawPath = GetComponent<DrawPath>();
			if (onStart)
			{
				drawPath.Draw(new Path(lr));
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (onUpdate)
			{
				drawPath.Draw(new Path(lr));
			}
		}
	}
}
