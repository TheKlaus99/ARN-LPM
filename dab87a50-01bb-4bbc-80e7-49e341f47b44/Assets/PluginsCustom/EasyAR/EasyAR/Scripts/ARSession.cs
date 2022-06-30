//================================================================================================================================
//
//  Copyright (c) 2015-2019 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================
using System;
using ARUnit;
using UnityEngine;

namespace easyar
{
    public class ARSessionUpdateEventArgs : EventArgs
    {
        public InputFrame IFrame = null;
        public OutputFrame OFrame = null;
        public Matrix4x4 ImageRotationMatrixGlobal = Matrix4x4.identity;
        public CameraParameters CameraParam = null;
        public float screenRotation;
    }

    public class ARSession : MonoBehaviour
    {

        CameraDevice easyarCamera;

        InputFrameThrottler iFrameThrottler;
        OutputFrameJoin oFrameJoin;
        OutputFrameBuffer oFrameBuffer;

        InputFrameToOutputFrameAdapter i2OAdapter;
        InputFrameFork inputFrameFork;
        OutputFrameFork outputFrameFork;
        InputFrameToFeedbackFrameAdapter i2fAdapter;
        FeedbackFrameFork feedbackFork;

        ARSessionUpdateEventArgs args;

        int frameIndex = -1;

        bool initialized = false;

        public CameraDevicePreference EasyarCameraPrefer = CameraDevicePreference.PreferObjectSensing;

        public CameraDeviceFocusMode CameraFocusMode = CameraDeviceFocusMode.Continousauto;


        public int ForkOutputNum = 0;

        public int JoinNum = 0;

        public ImageTrackerBehaviour ImgTracker;

        public CameraImageRenderer CameraBackgroundRenderer;

        private void Awake()
        {
            ARInterface.onStatusChange += OnARMainStatusChange;
            EasyARInterface.onStartEasyAR += OnStartEvent;
            var Scheduler = EasyARTracker.Scheduler;
            easyar.CameraDevice.requestPermissions(Scheduler, (System.Action<PermissionStatus, string>) ((status, msg) =>
            {

                if (status == PermissionStatus.Granted)
                {
                    Init();
                }
                Debug.Log("[EasyAR] RequestPermissions status " + status + " msg " + msg);
            }));
        }

        bool needRestart = false;
        private void OnARMainStatusChange(ARStatus ARStatus)
        {
            if (ARStatus == ARStatus.Running)
            {
                needRestart = true;
            }
        }

        private void OnStartEvent()
        {
            if (easyarCamera != null && needRestart)
            {
                needRestart = false;
                OpenCamera();
            }
        }

        void Init()
        {
            //Debug.Log("Init");
            initialized = true;

            iFrameThrottler = InputFrameThrottler.create();
            oFrameBuffer = OutputFrameBuffer.create();
            i2fAdapter = InputFrameToFeedbackFrameAdapter.create();
            inputFrameFork = InputFrameFork.create(ForkOutputNum);
            i2OAdapter = InputFrameToOutputFrameAdapter.create();
            oFrameJoin = OutputFrameJoin.create(JoinNum);

            if (CameraDevice.isAvailable())
            {
                //Debug.Log("[EasyAR] use ezar camera device");
                easyarCamera = CameraDeviceSelector.createCameraDevice(EasyarCameraPrefer);
                easyarCamera.setFocusMode(CameraFocusMode);
                var openResult = easyarCamera.openWithType(CameraDeviceType.Default);
                if (!openResult)
                {
                    // Debug.Log("[EasyAR] open camera failed");
                    initialized = false;
                    return;
                }
                easyarCamera.setSize(easyarCamera.supportedSize(0));

                easyarCamera.inputFrameSource().connect(iFrameThrottler.input());
                easyarCamera.start();
            }
            else
            {
                initialized = false;
                //Debug.Log("[EasyAR] ezar camera device can not work");
                return;
            }

            iFrameThrottler.output().connect(inputFrameFork.input());
            inputFrameFork.output(0).connect(i2OAdapter.input());
            i2OAdapter.output().connect(oFrameJoin.input(0));
            outputFrameFork = OutputFrameFork.create(1);

            int index = 0;
            if (ImgTracker != null)
            {
                index++;
                outputFrameFork = OutputFrameFork.create(2);
                inputFrameFork.output(index).connect(i2fAdapter.input());
                i2fAdapter.output().connect(ImgTracker.Input());
                ImgTracker.Output().connect(oFrameJoin.input(index));
                outputFrameFork.output(1).connect(i2fAdapter.sideInput());
            }

            oFrameJoin.output().connect(outputFrameFork.input());
            outputFrameFork.output(0).connect(oFrameBuffer.input());
            oFrameBuffer.signalOutput().connect(iFrameThrottler.signalInput());

            args = new ARSessionUpdateEventArgs();
        }

        private void Update()
        {
            if (!initialized)
            {
                return;
            }

            var oFrame = oFrameBuffer.peek();
            if (!oFrame.OnSome)
            {
                //Debug.Log("[EasyAR] oframe is null");
                return;
            }
            var iFrame = oFrame.Value.inputFrame();
            if (iFrame == null)
            {
                oFrame.Value.Dispose();
                //Debug.Log("[EasyAR] iFrame is null");
                return;
            }

            EasyARInterface.ChangeStatus(ARStatus.Running);

            var index = iFrame.index();
            if (frameIndex == index)
            {
                oFrame.Value.Dispose();
                iFrame.Dispose();
                return;
            }
            frameIndex = index;

            var camParams = iFrame.cameraParameters();
            if (camParams == null)
            {
                //Debug.Log("[EasyAR] camParams is null");
                return;
            }

            args.screenRotation = Utility.GetScreenRotation();
            var imageRotationDegree = camParams.imageOrientation((int) args.screenRotation);
            var imageRotation = (float) (imageRotationDegree) / 180.0f * Mathf.PI;
            Matrix4x4 rotationMatrixGlobal = Matrix4x4.identity;
            rotationMatrixGlobal.m00 = Mathf.Cos(-imageRotation);
            rotationMatrixGlobal.m01 = -Mathf.Sin(-imageRotation);
            rotationMatrixGlobal.m10 = Mathf.Sin(-imageRotation);
            rotationMatrixGlobal.m11 = Mathf.Cos(-imageRotation);
            args.ImageRotationMatrixGlobal = rotationMatrixGlobal;
            args.IFrame = iFrame;
            args.OFrame = oFrame.Value;
            args.CameraParam = camParams;

            if (ImgTracker != null)
                ImgTracker.UpdateFrame(args);

            if (CameraBackgroundRenderer != null)
                CameraBackgroundRenderer.UpdateFrame(args);

            oFrame.Value.Dispose();
            iFrame.Dispose();
            camParams.Dispose();
        }




        public void OpenCamera()
        {
            if (easyarCamera == null)
            {
                return;
            }
            easyarCamera.close();
            easyarCamera.Dispose();
            easyarCamera = CameraDeviceSelector.createCameraDevice(CameraDevicePreference.PreferObjectSensing);
            easyarCamera.setFocusMode(CameraFocusMode);
            bool openResult = false;

            openResult = easyarCamera.openWithType(CameraDeviceType.Back);
            Debug.Log("[EasyAR] open camera back result " + openResult);
            if (openResult)
                GL.invertCulling = false;


            easyarCamera.setSize(easyarCamera.supportedSize(0));
            easyarCamera.start();
            easyarCamera.inputFrameSource().connect(iFrameThrottler.input());
        }


    }
}
