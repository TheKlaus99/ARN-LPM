//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using ARUnit;
using UnityEngine;


namespace easyar
{
    public class ImageTrackerBehaviour : MonoBehaviour
    {
        public ImageTrackerMode Mode;
        private ImageTracker tracker = null;


        void Awake()
        {
            if (!ImageTracker.isAvailable())
            {
                throw new Exception("image tracker not support");
            }
            tracker = ImageTracker.createWithMode(Mode);
            tracker.setSimultaneousNum(1);
        }

        public FeedbackFrameSink Input()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            return tracker.feedbackFrameSink();
        }

        public OutputFrameSource Output()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            return tracker.outputFrameSource();
        }

        public void StartTracker()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.start();
        }

        public void StopTracker()
        {
            EasyARInterface.ChangeStatus(ARStatus.Stopped);

            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.stop();
        }

        public void CloseTracker()
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.close();
            tracker.Dispose();
            tracker = null;
        }

        TargetStatus lastStats = TargetStatus.Unknown;
        ARImage ARImage;
        Coroutine waitToClose;
        float imageUpdateTime = 0;
        void OnTargetFound(TargetInstance target, Matrix4x4 camRot, bool flip)
        {
            if (target.status() == TargetStatus.Tracked)
            {
                if (target.target().OnSome)
                {
                    Matrix4x4 pose = camRot * Utility.Matrix44FToMatrix4x4(target.pose());
#if UNITY_IOS && !UNITY_EDITOR
                    Quaternion quat = Quaternion.Euler(0, 0, flip?180 : 0);
#else
                    Quaternion quat = Quaternion.Euler(0, 0, 0);
#endif
                    Vector3 pos = quat * UnityEngine.XR.iOS.UnityARMatrixOps.GetPosition(pose);
                    Quaternion rot = quat * UnityEngine.XR.iOS.UnityARMatrixOps.GetRotation(pose);
                    ARImage = new ARImage(target.target().Value.name(), pos, rot);
                    if (lastStats != target.status())
                    {
                        EasyARInterface.ARImageAdd(ARImage);
                        imageUpdateTime = Time.time;
                        waitToClose = StartCoroutine(WaitToCloseIE());
                    }
                    else
                    {
                        imageUpdateTime = Time.time;
                        EasyARInterface.ARImageUpdate(ARImage);
                    }
                }
            }
            else
            {
                if (lastStats != target.status())
                {
                    if (ARImage != null)
                    {
                        EasyARInterface.ARImageRemove(ARImage);
                        ARImage = null;
                    }
                }
            }
            lastStats = target.status();

        }

        IEnumerator WaitToCloseIE()
        {
            while (ARImage != null)
            {
                yield return new WaitForSeconds(0.1f);
                if (Time.time - imageUpdateTime >.2f)
                {
                    lastStats = TargetStatus.Unknown;
                    EasyARInterface.ARImageRemove(ARImage);
                    ARImage = null;
                }
            }
            waitToClose = null;
        }

        public void UpdateFrame(ARSessionUpdateEventArgs args)
        {
            var results = args.OFrame.results();

            foreach (var _result in results)
            {
                if (_result.OnSome)
                {
                    ImageTrackerResult res = _result.Value as ImageTrackerResult;
                    if (res.targetInstances().Count > 0)
                    {
                        OnTargetFound(res.targetInstances() [0], args.ImageRotationMatrixGlobal, args.screenRotation == 0 || args.screenRotation == 180);
                    }

                }
            }
        }

        public void LoadImageTarget(Target target)
        {
            if (tracker == null)
            {
                throw new Exception("image tracker is null");
            }
            tracker.loadTarget(target, EasyARTracker.Scheduler, (t, b) => { });
        }

        private void Start()
        {
            StartTracker();
        }

        private void OnDisable()
        {
            if (ARImage != null)
            {
                lastStats = TargetStatus.Unknown;
                EasyARInterface.ARImageRemove(ARImage);
                ARImage = null;
            }

            if (waitToClose != null)
                StopCoroutine(waitToClose);

            StopTracker();
        }

        private void OnEnable()
        {
            StartTracker();
        }

        private void OnDestroy()
        {
            CloseTracker();
        }
    }
}
