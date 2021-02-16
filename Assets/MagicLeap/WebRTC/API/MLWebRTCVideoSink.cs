// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCVideoSink.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Threading;
    using System.Collections.Generic;
    
    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a video sink used by the MLWebRTC API.
        /// Video sinks are fed data by media sources and produces frames to render.
        /// </summary>
        public partial class VideoSink : Sink
        {
            /// <summary>
            /// Buffer for the image planes array to use to hold the image plane data.
            /// </summary>
            private CircularBuffer<Frame.ImagePlane[]> imagePlanesBuffer = CircularBuffer<Frame.ImagePlane[]>.Create(new Frame.ImagePlane[Frame.ImagePlane.MaxImagePlanes], 3);

            /// <summary>
            /// The newest frame that the video sink knows of.
            /// </summary>
            private Frame newFrame;

            /// <summary>
            /// The newest frame handle that the video sink knows of.
            /// </summary>
            private ulong newFrameHandle;

            private AutoResetEvent updateVideoEvent = new AutoResetEvent(true);

            /// <summary>
            /// Initializes a new instance of the <see cref="VideoSink" /> class.
            /// </summary>
            internal VideoSink()
            {
                this.Type = MediaStream.Track.Type.Video;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VideoSink" /> class.
            /// </summary>
            /// <param name="Handle">The Handle of the video sink.</param>
            internal VideoSink(ulong Handle) : base(Handle)
            {
                this.Type = MediaStream.Track.Type.Video;
            }

            /// <summary>
            /// A delegate that describes the requirements of the OnNewFrame callback.
            /// </summary>
            /// <param name="frame">The new frame of the video sink.</param>
            public delegate void OnNewFrameDelegate(VideoSink.Frame frame);

            /// <summary>
            /// The event invoked when a new frame is available for the video sink.
            /// </summary>
            public event OnNewFrameDelegate OnNewFrame;

            /// <summary>
            /// Creates an initialized VideoSink object.
            /// </summary>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized VideoSink object.</returns>
            public static VideoSink Create(out MLResult result)
            {
                VideoSink videoSink = null;
#if PLATFORM_LUMIN
                List<MLWebRTC.Sink> sinks = MLWebRTC.Instance.sinks;
                ulong Handle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkCreate(out Handle);
                if (!DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkCreate()"))
                {
                    result = MLResult.Create(resultCode);
                    return videoSink;
                }

                videoSink = new VideoSink(Handle);
                if (MagicLeapNativeBindings.MLHandleIsValid(videoSink.Handle))
                {
                    sinks.Add(videoSink);
                }

                result = MLResult.Create(resultCode);
#else
                result = new MLResult();
#endif
                return videoSink;
            }

            /// <summary>
            /// Invokes the OnNewFrame event and releases the unmanaged frame. 
            /// </summary>
            internal void InvokeOnNewFrame()
            {
#if PLATFORM_LUMIN
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return;
                }

                if (MagicLeapNativeBindings.MLHandleIsValid(newFrameHandle))
                {
                    this.OnNewFrame?.Invoke(newFrame);
                    DidNativeCallSucceed(NativeBindings.MLWebRTCVideoSinkReleaseFrame(Handle, newFrameHandle), "MLWebRTCVideoSinkReleaseFrame()");
                    newFrameHandle = MagicLeapNativeBindings.InvalidHandle;
                }
#endif
            }

            /// <summary>
            /// Queries the video sink for a new frame to broadcast, then builds that frame.
            /// </summary>
            public void UpdateVideoSink()
            {
#if PLATFORM_LUMIN
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return;
                }

                updateVideoEvent.Reset();
                bool newFrameAvailable = false;
                // Checks for a new frame.
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkIsNewFrameAvailable(this.Handle, out newFrameAvailable);
                DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkIsNewFrameAvailable()");
                // If one exists, acquire the frame and it's characteristics to send to the YUV converter for rendering.
                if (!newFrameAvailable)
                {
                    return;
                }

                ulong frameHandle = MagicLeapNativeBindings.InvalidHandle;
                resultCode = NativeBindings.MLWebRTCVideoSinkAcquireNextAvailableFrame(this.Handle, out frameHandle);
                if (!DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkAcquireNextAvailableFrame()"))
                {
                    return;
                }

                Frame.NativeBindings.MLWebRTCFrame nativeFrame = Frame.NativeBindings.MLWebRTCFrame.Create();
                resultCode = Frame.NativeBindings.MLWebRTCFrameGetData(frameHandle, ref nativeFrame);
                DidNativeCallSucceed(resultCode, "MLWebRTCFrameGetData()");
                newFrame = Frame.Create(frameHandle, nativeFrame, imagePlanesBuffer.Get());
                newFrameHandle = frameHandle;

                updateVideoEvent.Set();
#endif
            }

            /// <summary>
            /// Sets the track of the video sink.
            /// </summary>
            /// <param name="track">The track to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            protected override MLResult SetTrack(MediaStream.Track track)
            {
#if PLATFORM_LUMIN
                ulong sourceHandle = track != null ? track.Handle : MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkSetSource(this.Handle, sourceHandle);
                DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkSetSource()");
                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Sets the stream of the video sink sink.
            /// </summary>
            /// <param name="stream">The stream to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult SetStream(MediaStream stream)
            {
                if (this.Stream == stream)
                {
#if PLATFORM_LUMIN
                    return MLResult.Create(MLResult.Code.InvalidParam);
#endif
                }
                    
                this.Stream = stream;
                if (this.Stream == null)
                {
                    return this.SetTrack(null);
                }

                return this.SetTrack(this.Stream.ActiveVideoTrack);
            }

            /// <summary>
            /// Destroys the video sink.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public override MLResult Destroy()
            {
#if PLATFORM_LUMIN
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid.");
                }

                updateVideoEvent.WaitOne(250);
                this.SetStream(null);

                MLResult.Code resultCode = NativeBindings.MLWebRTCVideoSinkDestroy(this.Handle);
                DidNativeCallSucceed(resultCode, "MLWebRTCVideoSinkDestroy()");
                this.InvalidateHandle();
                MLWebRTC.Instance.sinks.Remove(this);

                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }
        }
    }
}
