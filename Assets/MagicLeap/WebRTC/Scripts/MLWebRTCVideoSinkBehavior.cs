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

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class MLWebRTCVideoSinkBehavior : MonoBehaviour
    {
        public MLWebRTC.VideoSink VideoSink
        {
            get
            {
                return videoSink;
            }
            set
            {
                if (videoSink != null)
                {
                    videoSink.OnNewFrame -= RenderWebRTCFrame;
                }

                videoSink = value;

                if (videoSink != null)
                {
                    videoSink.OnNewFrame += RenderWebRTCFrame;
                }
            }
        }

        private MLWebRTC.VideoSink videoSink;

        [SerializeField, Tooltip("The display to show the remote YUV video capture on.")]
        private Renderer remoteDisplayYUV = null;

        [SerializeField, Tooltip("The display to show the remote RGB video capture on.")]
        private Renderer remoteDisplayRGB = null;

        [SerializeField]
        private UnityEngine.UI.Text fpsText;

#if PLATFORM_LUMIN
        private Timer fpsTimer;
#endif
        private Texture2D[] rawVideoTexturesYUV = new Texture2D[(int)MLWebRTC.VideoSink.Frame.NativeImagePlanesLength.YUV_420_888];
        private Texture2D[] rawVideoTexturesRGB = new Texture2D[(int)MLWebRTC.VideoSink.Frame.NativeImagePlanesLength.RGBA_8888];

        private bool isFrameFit = false;

        private static readonly string[] samplerNamesYUV = new string[] { "_MainTex", "_UTex", "_VTex" };

        public void ResetFrameFit()
        {
            isFrameFit = false;
        }


        void Awake()
        {
#if PLATFORM_LUMIN
            MLResult result = MLPrivileges.RequestPrivileges(MLPrivileges.Id.Internet, MLPrivileges.Id.LocalAreaNetwork, MLPrivileges.Id.CameraCapture, MLPrivileges.Id.AudioCaptureMic);

            if (result.Result != MLResult.Code.PrivilegeGranted)
            {
                Debug.LogError("MLPrivileges failed to grant all needed privileges.");
                enabled = false;
            }

            videoSink = MLWebRTC.VideoSink.Create(out  result);
            fpsTimer = new Timer(1000);
            videoSink.OnNewFrame += RenderWebRTCFrame;
#endif
        }

        private void RenderWebRTCFrame(MLWebRTC.VideoSink.Frame frame)
        {
            if (frame.ImagePlanes == null)
            {
                return;
            }

#if PLATFORM_LUMIN
            float fps = 1.0f / fpsTimer.Elapsed();
            
            if (fpsText != null)
            {
                fpsText.text = string.Format("{0:0.00}", fps);
                if (fps >= 24f)
                {
                    fpsText.color = Color.green;
                }
                else if (fps >= 16f)
                {
                    fpsText.color = Color.yellow;
                }
                else
                {
                    fpsText.color = Color.red;
                }

            }
#endif
            if (!isFrameFit && frame.ImagePlanes.Length > 1)
            {
                float aspectRatio = frame.ImagePlanes[0].Width / (float)frame.ImagePlanes[0].Height;
                float scaleWidth = transform.lossyScale.y * aspectRatio;

                // sets the plane to the aspect ratio of the frame
                if (transform.lossyScale.x != scaleWidth)
                {
                    Transform parent = transform.parent;
                    transform.parent = null;
                    transform.localScale = new Vector3(scaleWidth, transform.localScale.y, transform.localScale.z);
                    transform.parent = parent;
                }
                isFrameFit = true;
            }

            switch (frame.Format)
            {
                case MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888:
                {
                    if (!this.remoteDisplayYUV.enabled || this.remoteDisplayRGB.enabled)
                    {
                        this.remoteDisplayRGB.enabled = false;
                        this.remoteDisplayYUV.enabled = true;
                    }
                    RenderWebRTCFrameYUV(frame);
                    break;
                }

                case MLWebRTC.VideoSink.Frame.OutputFormat.RGBA_8888:
                {
                    if (!this.remoteDisplayRGB.enabled || this.remoteDisplayYUV.enabled)
                    {
                        this.remoteDisplayYUV.enabled = false;
                        this.remoteDisplayRGB.enabled = true;
                    }
                    RenderWebRTCFrameRGB(frame);
                    break;
                }
            }

#if PLATFORM_LUMIN
            fpsTimer.Reset();
#endif
        }

        private void RenderWebRTCFrameYUV(MLWebRTC.VideoSink.Frame frame)
        {
            for (int i = 0; i < rawVideoTexturesYUV.Length; ++i)
            {
                MLWebRTC.VideoSink.Frame.ImagePlane imagePlane = frame.ImagePlanes[i];
                if(imagePlane.Data == System.IntPtr.Zero)
                {
                    return;
                }

                UpdateYUVTextureChannel(ref rawVideoTexturesYUV[i], imagePlane, remoteDisplayYUV, samplerNamesYUV[i]);
            }
        }

        private void UpdateYUVTextureChannel(ref Texture2D channelTexture, MLWebRTC.VideoSink.Frame.ImagePlane imagePlane, Renderer renderer, string samplerName)
        {
            if (channelTexture == null || channelTexture.width != imagePlane.Stride || channelTexture.height != (int)(imagePlane.Height))
            {
                channelTexture = new Texture2D((int)imagePlane.Stride, (int)(imagePlane.Height), TextureFormat.Alpha8, false);
                channelTexture.filterMode = FilterMode.Bilinear;
                renderer.material.SetTexture(samplerName, channelTexture);
            }

            channelTexture.LoadRawTextureData(imagePlane.Data, (int)(imagePlane.Stride * imagePlane.Height));
            channelTexture.Apply();
        }

        private void RenderWebRTCFrameRGB(MLWebRTC.VideoSink.Frame frame)
        {
            for (int i = 0; i < rawVideoTexturesRGB.Length; ++i)
            {
                MLWebRTC.VideoSink.Frame.ImagePlane imagePlane = frame.ImagePlanes[i];
                Texture2D rawVideoTextureRGB = rawVideoTexturesRGB[i];

                int width = (int)(imagePlane.Stride / imagePlane.BytesPerPixel);
                if (rawVideoTextureRGB == null || rawVideoTextureRGB.width != width || rawVideoTextureRGB.height != imagePlane.Height)
                {
                    rawVideoTextureRGB = new Texture2D(width, (int)imagePlane.Height, TextureFormat.RGBA32, false);
                    rawVideoTextureRGB.filterMode = FilterMode.Bilinear;
                    remoteDisplayRGB.material.mainTexture = rawVideoTextureRGB;
                    remoteDisplayRGB.material.mainTextureScale = new Vector2(1.0f, -1.0f);
                }

                rawVideoTexturesRGB[i] = rawVideoTextureRGB;
                rawVideoTextureRGB.LoadRawTextureData(imagePlane.Data, (int)imagePlane.Size);
                rawVideoTextureRGB.Apply();
            }
        }

        void OnDestroy()
        {
            if (videoSink != null)
            {
                videoSink.OnNewFrame -= RenderWebRTCFrame;
            }
        }
    }

}
