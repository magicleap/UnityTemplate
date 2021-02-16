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
    public class MLCameraVideoSource : MLWebRTC.AppDefinedVideoSource
    {
#if PLATFORM_LUMIN
        private CircularBuffer<MLWebRTC.VideoSink.Frame.ImagePlane[]> imagePlanesBuffer = CircularBuffer<MLWebRTC.VideoSink.Frame.ImagePlane[]>.Create(new MLWebRTC.VideoSink.Frame.ImagePlane[(int)MLWebRTC.VideoSink.Frame.NativeImagePlanesLength.YUV_420_888], 3);

        private MLCamera.CaptureSettings captureSettings;

        private bool isCapturing = false;

        public static MLCameraVideoSource CreateLocal(MLCamera.CaptureSettings settings, out MLResult result)
        {
            MLCameraVideoSource mlCameraVideoSource = new MLCameraVideoSource()
            {
                captureSettings = settings
            };

            result = MLWebRTC.AppDefinedVideoSource.InitializeLocal(mlCameraVideoSource);
            mlCameraVideoSource.StartCapture();
            return mlCameraVideoSource;
        }
#endif

        protected override void OnSourceSetEnabled(bool enabled)
        {
            if(enabled)
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
        }

        private void StartCapture()
        {
#if PLATFORM_LUMIN
            if (!isCapturing)
            {
                MLPrivileges.RequestPrivileges(MLPrivileges.Id.CameraCapture);
                MLCamera.Connect();
                MLCamera.PrepareCapture(MLCamera.CaptureType.VideoRaw, ref this.captureSettings);
                MLCamera.OnRawVideoFrameAvailableYUV_NativeCallbackThread += OnMLCameraFrameYUV;
                MLCamera.StartRawVideoCapture();
                isCapturing = true;
            }
#endif
        }

        private void StopCapture()
        {
#if PLATFORM_LUMIN
            if (isCapturing)
            {
                if (MLCamera.IsStarted)
                {
                    MLCamera.StopVideoCapture();
                    MLCamera.Disconnect();
                }
                MLCamera.OnRawVideoFrameAvailableYUV_NativeCallbackThread -= OnMLCameraFrameYUV;
                isCapturing = false;
            }
#endif
        }

#if PLATFORM_LUMIN
        private void OnMLCameraFrameYUV(MLCamera.ResultExtras results, MLCamera.YUVFrameInfo frameInfo, MLCamera.FrameMetadata metadata)
        {
            PushYUVFrame(results, frameInfo, metadata);
        }

        private void PushYUVFrame(MLCamera.ResultExtras results, MLCamera.YUVFrameInfo frameInfo, MLCamera.FrameMetadata metadata)
        {
            MLCamera.YUVBuffer buffer;
            MLWebRTC.VideoSink.Frame.ImagePlane[] imagePlaneArray = imagePlanesBuffer.Get();
            for (int i = 0; i < imagePlaneArray.Length; ++i)
            {
                switch (i)
                {
                    case 0:
                    {
                        buffer = frameInfo.Y;
                        break;
                    }
                    case 1:
                    {
                        buffer = frameInfo.U;
                        break;
                    }
                    case 2:
                    {
                        buffer = frameInfo.V;
                        break;
                    }
                    default:
                    {
                        buffer = new MLCamera.YUVBuffer();
                        break;
                    }
                }

                imagePlaneArray[i] = MLWebRTC.VideoSink.Frame.ImagePlane.Create(buffer.Width, buffer.Height, buffer.Stride, buffer.BytesPerPixel, buffer.Size, buffer.DataPtr);
            }

            MLWebRTC.VideoSink.Frame frame = MLWebRTC.VideoSink.Frame.Create((ulong)results.RequestId, results.VcamTimestampUs, imagePlaneArray, MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888);

            _ = this.PushFrameAsync(frame);
        }
#endif
    }
}
