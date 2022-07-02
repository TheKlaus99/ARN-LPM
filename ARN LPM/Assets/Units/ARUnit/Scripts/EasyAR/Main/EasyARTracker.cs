using System;
using System.Collections;
using System.Collections.Generic;
using easyar;
using UnityEngine;

namespace ARUnit
{
	public class EasyARTracker : MonoBehaviour
	{
		#region Static
		public static EasyARTracker instance;
		public static bool EasyARSDKInitialized;
		public static DelayedCallbackScheduler Scheduler { get; private set; }

		public static void Initialization()
		{
			if (!EasyARSDKInitialized)
			{
				instance.InitEasyAR();
			}
		}

		public static void OpenCam()
		{
			instance.Open();
		}

		public static void CloseCam()
		{
			instance.Close();
		}
		#endregion

		public GameObject session;
		public ARReferenceImagesSet imageSet;
		bool imagesCreated = false;
		ImageTrackerBehaviour imageTracker;

		void Open()
		{
			session.SetActive(true);
			if (!imagesCreated)
			{
				CreateTargets();
			}
		}

		void Close()
		{
			session.SetActive(false);
		}

		void CreateTargets()
		{
			imageTracker = GetComponentInChildren<ImageTrackerBehaviour>();
			Debug.Log(imageTracker);
			foreach (var item in imageSet.referenceImages)
			{
				LoadImageTarget(item.imageName, item.physicalSize, item.imageTexture);
			}
			imagesCreated = true;
		}

		private void LoadImageTarget(string name, float size, Texture2D sprite)
		{

			var data = sprite.EncodeToJPG(80);

			easyar.Buffer buffer = easyar.Buffer.create(data.Length);
			System.Runtime.InteropServices.Marshal.Copy(data, 0, buffer.data(), data.Length);


			Optional<easyar.ImageTarget> op_target;
			var image = ImageHelper.decode(buffer);

			var p = new ImageTargetParameters();
			p.setImage(image.Value);
			p.setName(name);
			p.setScale(size);
			p.setUid("");
			p.setMeta("");

			op_target = ImageTarget.createFromParameters(p);

			if (!op_target.OnSome)
			{
				throw new System.Exception("create image target failed from image target parameters");
			}

			image.Value.Dispose();
			buffer.Dispose();
			p.Dispose();

			//target = op_target.Value;
			imageTracker.LoadImageTarget(op_target.Value);
		}

		void InitEasyAR()
		{
			EasyARInterface.ChangeStatus(ARStatus.Initializing);
			EasyARSDKInitialized = false;
			Scheduler = new DelayedCallbackScheduler();

			var key = ARNSettings.settings.easyARKey;

#if UNITY_ANDROID && !UNITY_EDITOR
			using(var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using(var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
			using(var easyarEngineClass = new AndroidJavaClass("cn.easyar.Engine"))
			{
				var activityclassloader = currentActivity.Call<AndroidJavaObject>("getClass").Call<AndroidJavaObject>("getClassLoader");
				if (activityclassloader == null)
				{
					Debug.Log("ActivityClassLoader is null");
				}
				if (!easyarEngineClass.CallStatic<bool>("initialize", currentActivity, key))
				{
					EasyARInterface.ChangeStatus(ARStatus.Failed);
					Debug.Log("EasyAR initialization failed");
					EasyARSDKInitialized = false;
					return;
				}
				else
				{
					EasyARSDKInitialized = true;
				}
			}
#else
			if (!Engine.initialize(key))
			{
				EasyARInterface.ChangeStatus(ARStatus.Failed);
				Debug.LogError("[EasyAR] EasyAR initialization failed");
				EasyARSDKInitialized = false;
				return;
			}
			else
			{
				Debug.Log("[EasyAR] EasyAR initialization successful");
				Debug.Log("[EasyAR] EasyAR Version : " + Engine.versionString());
				EasyARSDKInitialized = true;
			}
#endif
#if UNITY_EDITOR
			Log.setLogFunc((LogLevel, msg) =>
			{
				switch (LogLevel)
				{
					case LogLevel.Error:
						Debug.LogError("[EasyAR]" + msg);
						break;
					case LogLevel.Warning:
						Debug.LogWarning("[EasyAR]" + msg);
						break;
					case LogLevel.Info:
						Debug.Log("[EasyAR]" + msg);
						break;
					default:
						break;
				}
			});
#endif
			System.AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Debug.Log("UnhandledException: " + e.ExceptionObject.ToString());
			};
			System.AppDomain.CurrentDomain.DomainUnload += (sender, e) =>
			{
				if (Scheduler != null)
				{
					Scheduler.Dispose();
				}
				Log.resetLogFunc();
			};


		}

		public void Update()
		{
			if (Scheduler != null)
			{
				while (Scheduler.runOne())
				{
					//Debug.Log("While");
				}
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				EasyARInterface.onStopEasyAR += OnStopEasyAR;
				EasyARInterface.onStartEasyAR += OnStartEasyAR;
			}
			else
			{
				Destroy(this);
			}

		}

		private void OnStartEasyAR()
		{
			Initialization();
			OpenCam();
		}

		private void OnStopEasyAR()
		{
			CloseCam();
		}

		public void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				Engine.onPause();
			}
			else
			{
				Engine.onResume();
			}
		}
	}
}
