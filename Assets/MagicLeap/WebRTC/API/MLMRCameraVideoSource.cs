// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Threading.Tasks;

namespace UnityEngine.XR.MagicLeap
{
    public class MLMRCameraVideoSource : MLWebRTC.AppDefinedVideoSource
    {
        private MLMRCamera.InputContext inputContext;
        private MLWebRTC.VideoSink.Frame.ImagePlane[] imagePlanesRGB = new MLWebRTC.VideoSink.Frame.ImagePlane[(int)MLWebRTC.VideoSink.Frame.NativeImagePlanesLength.RGBA_8888];

        public static MLMRCameraVideoSource CreateLocal(MLMRCamera.InputContext context, out MLResult result)
        {
            MLMRCameraVideoSource mlMRCameraVideoSource = new MLMRCameraVideoSource()
            {
                inputContext = context
            };
#if PLATFORM_LUMIN
            result = MLWebRTC.AppDefinedVideoSource.InitializeLocal(mlMRCameraVideoSource);
            MLMRCamera.OnFrameCapture_NativeCallbackThread += mlMRCameraVideoSource.OnMLMRCameraFrameRGB;
            MLMRCamera.OnCaptureComplete += mlMRCameraVideoSource.OnCaptureCompleted;
#endif
            mlMRCameraVideoSource.StartCapture();

            return mlMRCameraVideoSource;
        }

        protected override void OnSourceSetEnabled(bool enabled)
        {
            if (enabled)
            {
                this.StartCapture();
            }
            else
            {
                this.StopCapture();
            }
        }

        protected override void OnSourceDestroy()
        {
            this.StopCapture();

#if PLATFORM_LUMIN
            MLMRCamera.OnFrameCapture_NativeCallbackThread -= OnMLMRCameraFrameRGB;
            MLMRCamera.OnCaptureComplete -= OnCaptureCompleted;
#endif
        }

        private void StartCapture()
        {
#if PLATFORM_LUMIN
            MLPrivileges.RequestPrivileges(MLPrivileges.Id.CameraCapture);
            MLMRCamera.Connect(this.inputContext);
            MLMRCamera.StartCapture();
#endif
        }

        private void StopCapture()
        {
#if PLATFORM_LUMIN
            MLMRCamera.Disconnect();
#endif
        }

        protected override void HandleDeviceReality()
        {
            // Don't do anything for reality mode. MRCamera OnCaptureCompleted will be invoked
            // and we use that to disable the track and disconnect the camera.
            // Just set the following flag so track gets enabled again.
            GetEnabled(out wasEnabledBeforeLifecycleStateChange);
        }

        protected override void HandleDeviceStandby()
        {
            // Don't do anything for standby mode. MRCamera OnCaptureCompleted will be invoked
            // and we use that to disable the track and disconnect the camera.
            // Just set the following flag so track gets enabled again.
            GetEnabled(out wasEnabledBeforeLifecycleStateChange);
        }

#if PLATFORM_LUMIN
        private void OnMLMRCameraFrameRGB(MLMRCamera.Frame mrCameraFrame)
        {
            PushRGBFrame(mrCameraFrame);
        }

        private Task PushRGBFrame(MLMRCamera.Frame mrCameraFrame)
        {
            for (int i = 0; i < imagePlanesRGB.Length; i++)
            {
                MLMRCamera.Frame.ImagePlane imagePlane = mrCameraFrame.ImagePlanes[i];
                imagePlanesRGB[i] = MLWebRTC.VideoSink.Frame.ImagePlane.Create(imagePlane.Width, imagePlane.Height, imagePlane.Stride, imagePlane.BytesPerPixel, imagePlane.Size, imagePlane.DataPtr);
            }

            MLWebRTC.VideoSink.Frame frame = MLWebRTC.VideoSink.Frame.Create(mrCameraFrame.Id, mrCameraFrame.TimeStampNs / 1000, imagePlanesRGB, MLWebRTC.VideoSink.Frame.OutputFormat.RGBA_8888);

            _ = this.PushFrameAsync(frame);
            return Task.CompletedTask;
        }

        private void OnCaptureCompleted()
        {
            SetEnabled(false);
        }
#endif

    }
}
