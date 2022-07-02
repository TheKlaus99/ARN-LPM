using System.Collections;
using System.Collections.Generic;
using ARUnit;
using GPSUnit;
using UnityEngine;

namespace PositionUnit.Test
{

	public class EventRecorderPOSU : MonoBehaviour
	{
		public float speed = 1;
		[Space]
		[Space]

		[Header("AR Session events")]
		public bool onStartSessionEnable = false;
		public bool onReStartSessionEnable = false;
		public bool onStopSessionEnable = false;
		public bool onSessionFaildEnable = false;

		[Header("AR Status events")]
		public bool onStatusChangeEnable = false;
		public bool onTrackingStateChangeEnable = false;
		public bool onTrackingStateReasonChangeEnable = false;
		public bool onChangePaneModeEnable = false;

		[Header("AR Image events")]
		public bool onImageAddedEnable = false;
		public bool onImageUpdateEnable = false;
		public bool onImageRemovedEnable = false;

		[Header("AR Plane ebvents")]
		public bool onPlaneAddedEnable = false;
		public bool onPlaneUpdateEnable = false;
		public bool onPlaneRemovedEnable = false;

		[Header("AR Camera events")]
		public bool onARTransformUpdateEnable = false;
		public bool OnARCameraProjectionMatrixUpdateEnable = false;


		[Header("GPS events")]
		public bool onGPSUpdateEnable = false;
		public bool onGPSCompassUpdateEnable = false;
		public bool onGPSStatusUpdateEnable = false;
		public bool onStartGPSEnable = false;
		public bool onStopGPSEnable = false;
		public bool onStartCompassEnable = false;
		public bool onStopCompassEnable = false;

		[Space]
		public EventRecord e = new EventRecord();
		public float t = 0;

		// Use this for initialization
		void Start()
		{
			SubscribeAR();
			SubscribeGPS();
		}

		public void Play(EventRecord e)
		{
			StartCoroutine(PlayIE(e));
		}

		void WriteToLog(object o)
		{
			//Debug.Log(o.ToString());
		}

		IEnumerator PlayIE(EventRecord e)
		{
			t = e.events[0].time;
			int count = 0;
			foreach (var item in e.events)
			{
				count++;
				if (count % speed == 0)
					yield return new WaitForSeconds((item.time - t) / speed);
				t = item.time;

				switch (item.name)
				{
					#region AREvents

					case "onARTransformUpdate":
						if (onARTransformUpdateEnable)
							ARInterface.UpdateARTransform(((ARTransformS) item.args).ARTransform());
						WriteToLog("onARTransformUpdate " + ((ARTransformS) item.args).ARTransform().ToString());
						break;

					case "onImageAdded":
						if (onImageAddedEnable)
							ARInterface.ARImageAdd(((ARImageS) item.args).ARImage());
						WriteToLog("onImageAdded " + ((ARImageS) item.args).ARImage().ToString());
						break;

					case "onImageUpdate":
						if (onImageUpdateEnable)
							ARInterface.ARImageUpdate(((ARImageS) item.args).ARImage());
						WriteToLog("onImageUpdate " + ((ARImageS) item.args).ARImage().ToString());
						break;

					case "onImageRemoved":
						if (onImageRemovedEnable)
							ARInterface.ARImageRemove(((ARImageS) item.args).ARImage());
						WriteToLog("onImageRemoved " + ((ARImageS) item.args).ARImage().ToString());
						break;

					case "onPlaneAdded":
						if (onPlaneAddedEnable)
							ARInterface.ARPlaneAdd(((ARPlaneS) item.args).ARPlane());
						WriteToLog("onPlaneAdded " + ((ARPlaneS) item.args).ARPlane().ToString());
						break;

					case "onPlaneUpdate":
						if (onPlaneUpdateEnable)
							ARInterface.ARPlaneUpdate(((ARPlaneS) item.args).ARPlane());
						WriteToLog("onPlaneUpdate " + ((ARPlaneS) item.args).ARPlane().ToString());
						break;

					case "onPlaneRemoved":
						if (onPlaneRemovedEnable)
							ARInterface.ARPlaneRemove(((ARPlaneS) item.args).ARPlane());
						WriteToLog("onPlaneRemoved " + ((ARPlaneS) item.args).ARPlane().ToString());
						break;

					case "OnARCameraProjectionMatrixUpdate":
						if (OnARCameraProjectionMatrixUpdateEnable)
							ARInterface.UpdateCameraProjectionMatrix(((Matrix4x4S) item.args).Matrix4x4());
						WriteToLog("OnARCameraProjectionMatrixUpdate " + ((Matrix4x4S) item.args).Matrix4x4().ToString());
						break;

						#endregion

						#region GPSEvents
					case "onGPSUpdate":
						if (onGPSUpdateEnable)
							GPSInterface.GPSUpdate((GPSInfo) item.args);
						WriteToLog("onGPSUpdate " + ((GPSInfo) item.args));
						break;

					case "onGPSCompassUpdate":
						if (onGPSCompassUpdateEnable)
							GPSInterface.CompassUpdate((GPSCompassInfo) item.args);
						WriteToLog("onGPSCompassUpdate " + ((GPSCompassInfo) item.args));
						break;

					case "onGPSStatusUpdate":
						if (onGPSStatusUpdateEnable)
							GPSInterface.UpdateGPSStatus((GPSServiceStatus) item.args);
						WriteToLog("onGPSStatusUpdate " + ((GPSServiceStatus) item.args));
						break;


					case "onStartGPS":
						if (onStartGPSEnable)
						{
							GPSStartParamS g = (GPSStartParamS) item.args;
							GPSInterface.StartGPS(g.desiredAccuracyInMeters, g.updateDistanceInMeters);
						}
						WriteToLog("onStartGPS " + ((GPSStartParamS) item.args));
						break;

					case "onStopGPS":
						if (onStopGPSEnable)
							GPSInterface.StopGPS();
						WriteToLog("onStopGPS");
						break;

					case "onStartCompass":
						if (onStartCompassEnable)
							GPSInterface.StartCompass();
						WriteToLog("onStartCompass");
						break;

					case "onStopCompass":
						if (onStopCompassEnable)
							GPSInterface.StopCompass();
						WriteToLog("onStopCompass");
						break;
						#endregion

					default:
						Debug.Log("Unsupported Event");
						break;
				}
			}
		}

		void SubscribeGPS()
		{
			GPSInterface.onGPSCompassUpdate += onGPSCompassUpdate;
			GPSInterface.onGPSStatusUpdate += onGPSStatusUpdate;
			GPSInterface.onGPSUpdate += onGPSUpdate;
			GPSInterface.onStartCompass += onStartCompass;
			GPSInterface.onStopCompass += onStopCompass;
			GPSInterface.onStartGPS += onStartGPS;
			GPSInterface.onStopGPS += onStopGPS;

		}

		private void Update()
		{
			//onARTransformUpdate(new ARTransform(new Vector3(0, 1, 4), Quaternion.identity));
		}

		void SubscribeAR()
		{
			ARInterface.onARTransformUpdate += onARTransformUpdate;
			ARInterface.onImageUpdate += onImageUpdate;
			ARInterface.onImageAdd += onImageAdded;
			ARInterface.onImageRemoved += onImageRemoved;

			ARInterface.onPlaneUpdate += onPlaneUpdate;
			ARInterface.onPlaneAdd += onPlaneAdded;
			ARInterface.onPlaneRemoved += onPlaneRemoved;

			ARInterface.onStatusChange += onStatusChange;
			ARInterface.onTrackingStateChange += onTrackingStateChange;
			ARInterface.onTrackingStateReasonChange += onTrackingStateReasonChange;
			ARInterface.onStartSession += onStartSession;
			ARInterface.onReStartSession += onReStartSession;
			ARInterface.onChangePaneMode += onChangePaneMode;
			ARInterface.onStopSession += onStopSession;
			ARInterface.onSessionFaild += onSessionFaild;
			ARInterface.onARCameraProjectionMatrixUpdate += OnARCameraProjectionMatrixUpdate;
		}

		#region AREvents
		void onARTransformUpdate(ARTransform ARTransform)
		{
			e.events.Add(new Event("onARTransformUpdate", new ARTransformS(ARTransform)));
		}

		void onImageAdded(ARImage ARImage)
		{
			e.events.Add(new Event("onImageAdded", new ARImageS(ARImage)));
		}

		void onImageUpdate(ARImage ARImage)
		{
			e.events.Add(new Event("onImageUpdate", new ARImageS(ARImage)));
		}

		void onImageRemoved(ARImage ARImage)
		{
			e.events.Add(new Event("onImageRemoved", new ARImageS(ARImage)));
		}

		void onPlaneAdded(ARPlane ARPlane)
		{
			e.events.Add(new Event("onPlaneAdded", new ARPlaneS(ARPlane)));
		}

		void onPlaneUpdate(ARPlane ARPlane)
		{
			e.events.Add(new Event("onPlaneUpdate", new ARPlaneS(ARPlane)));
		}

		void onPlaneRemoved(ARPlane ARPlane)
		{
			e.events.Add(new Event("onPlaneRemoved", new ARPlaneS(ARPlane)));
		}

		void onStatusChange(ARStatus ARStatus) { }

		void onTrackingStateChange(ARUnit.ARTrackingState ARTrackingState) { }

		void onTrackingStateReasonChange(ARUnit.ARTrackingStateReason ARTrackingStateReason) { }

		void onStartSession() { }

		void onReStartSession() { }

		void onChangePaneMode(bool isActive) { }

		void onStopSession() { }

		void onSessionFaild(string error) { }

		void OnARCameraProjectionMatrixUpdate(Matrix4x4 ProjectionMatrix)
		{
			e.events.Add(new Event("OnARCameraProjectionMatrixUpdate", new Matrix4x4S(ProjectionMatrix)));
		}
		#endregion

		#region GPSEvents

		public void onGPSUpdate(GPSInfo info)
		{
			e.events.Add(new Event("onGPSUpdate", info));
		}

		public void onGPSCompassUpdate(GPSCompassInfo info)
		{
			e.events.Add(new Event("onGPSCompassUpdate", info));
		}

		public void onGPSStatusUpdate(GPSServiceStatus status)
		{
			e.events.Add(new Event("onGPSStatusUpdate", status));
		}

		public void onStartGPS(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			e.events.Add(new Event("onStartGPS", new GPSStartParamS(desiredAccuracyInMeters, updateDistanceInMeters)));
		}

		public void onStopGPS()
		{
			e.events.Add(new Event("onStopGPS"));
		}

		public void onStartCompass()
		{
			e.events.Add(new Event("onStartCompass"));
		}

		public void onStopCompass()
		{
			e.events.Add(new Event("onStopCompass"));
		}
		#endregion
	}

	[System.Serializable]
	public class EventRecord
	{
		public List<Event> events = new List<Event>();
	}

	[System.Serializable]
	public class Event
	{
		public string name;
		public object args;
		public float time;
		public Event(string name, object args = null)
		{
			this.name = name;
			this.args = args;
			time = Time.time;
		}
	}

	#region SerializableClasses

	[System.Serializable]
	public class ARTransformS
	{
		public float pX, pY, pZ, qX, qY, qZ, qW;
		public ARTransformS(ARTransform ARTransform)
		{
			pX = ARTransform.position.x;
			pY = ARTransform.position.y;
			pZ = ARTransform.position.z;
			qX = ARTransform.rotation.x;
			qY = ARTransform.rotation.y;
			qZ = ARTransform.rotation.z;
			qW = ARTransform.rotation.w;
		}

		public ARTransform ARTransform()
		{
			return new ARTransform(new Vector3(pX, pY, pZ), new Quaternion(qX, qY, qZ, qW));
		}
	}

	[System.Serializable]
	public class ARImageS
	{
		public string s;
		public float pX, pY, pZ, qX, qY, qZ, qW;
		public ARImageS(ARImage ARImage)
		{
			s = ARImage.name;
			pX = ARImage.position.x;
			pY = ARImage.position.y;
			pZ = ARImage.position.z;
			qX = ARImage.rotation.x;
			qY = ARImage.rotation.y;
			qZ = ARImage.rotation.z;
			qW = ARImage.rotation.w;
		}

		public ARImage ARImage()
		{
			return new ARImage(s, new Vector3(pX, pY, pZ), new Quaternion(qX, qY, qZ, qW));
		}
	}

	[System.Serializable]
	public class ARPlaneS
	{
		public string id;
		public Vector3S pos, rot, ex;
		public ARPlaneS(ARPlane ARPlane)
		{
			id = ARPlane.identifier;
			pos = new Vector3S(ARPlane.position);
			rot = new Vector3S(ARPlane.rotation.eulerAngles);
			ex = new Vector3S(ARPlane.extent);
		}

		public ARPlane ARPlane()
		{
			return new ARPlane(id, pos.ToVector3(), Quaternion.Euler(rot.ToVector3()), ex.ToVector3());
		}
	}

	[System.Serializable]
	public class Matrix4x4S
	{
		public float m00;
		public float m33;
		public float m23;
		public float m13;
		public float m03;
		public float m32;
		public float m22;
		public float m02;
		public float m12;
		public float m21;
		public float m11;
		public float m01;
		public float m30;
		public float m20;
		public float m10;
		public float m31;

		public Matrix4x4S(Matrix4x4 m)
		{
			m00 = m.m00;
			m33 = m.m33;
			m23 = m.m23;
			m13 = m.m13;
			m03 = m.m03;
			m32 = m.m32;
			m22 = m.m22;
			m02 = m.m02;
			m12 = m.m12;
			m21 = m.m21;
			m11 = m.m11;
			m01 = m.m01;
			m30 = m.m30;
			m20 = m.m20;
			m10 = m.m10;
			m31 = m.m31;
		}

		public Matrix4x4 Matrix4x4()
		{
			Matrix4x4 m = new Matrix4x4();
			m.m00 = m00;
			m.m33 = m33;
			m.m23 = m23;
			m.m13 = m13;
			m.m03 = m03;
			m.m32 = m32;
			m.m02 = m02;
			m.m22 = m22;
			m.m12 = m12;
			m.m21 = m21;
			m.m11 = m11;
			m.m01 = m01;
			m.m30 = m30;
			m.m20 = m20;
			m.m10 = m10;
			m.m31 = m31;
			return m;
		}
	}

	[System.Serializable]
	public class GPSStartParamS
	{
		public float desiredAccuracyInMeters, updateDistanceInMeters;
		public GPSStartParamS(float desiredAccuracyInMeters, float updateDistanceInMeters)
		{
			this.desiredAccuracyInMeters = desiredAccuracyInMeters;
			this.updateDistanceInMeters = updateDistanceInMeters;
		}
	}

	#endregion

}
