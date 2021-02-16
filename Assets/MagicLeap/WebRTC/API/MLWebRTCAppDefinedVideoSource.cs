// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAppDefinedVideoSource.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices;
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
        /// Class that represents an app defined video source that can be used by the MLWebRTC API.
        /// </summary>
        public abstract partial class AppDefinedVideoSource : MediaStream.Track
        {
            /// <summary>
            /// Gets the GCHandle of this managed object.
            /// </summary>
            private GCHandle gcHandle;

            /// <summary>
            /// Instance method that is called when the source is destroyed via <c>MLWebRTC.MediaStream.Track.DestroyLocal()</c>.
            /// </summary>
            protected abstract void OnSourceDestroy();

            /// <summary>
            /// Instance method that is called when the source is enabled or disabled via <c>MLWebRTC.MediaStream.Track.SetEnabled()</c>
            /// </summary>
            /// <param name="enabled">True if enabled.</param>
            protected abstract void OnSourceSetEnabled(bool enabled);

            /// <summary>
            /// Initializes a new instance of the <see cref="AppDefinedVideoSource" /> class.
            /// </summary>
            protected internal AppDefinedVideoSource()
            {
                this.TrackType = Type.Video;
            }

#if PLATFORM_LUMIN
            /// <summary>
            /// Initializes the given AppDefinedVideoSource object.
            /// </summary>
            /// <param name="appDefinedVideoSource">The AppDefinedVideoSource object to initialize.</param>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns>An AppDefinedVideoSource object with the given handle.</returns>
            public static MLResult InitializeLocal(AppDefinedVideoSource appDefinedVideoSource)
            {
                MLWebRTC.Instance.localTracks.Add(appDefinedVideoSource);
                MLResult.Code resultCode = NativeBindings.InitializeAppDefinedVideoSource(appDefinedVideoSource);
                DidNativeCallSucceed(resultCode, "InitializeAppDefinedVideoSource()");
                return MLResult.Create(resultCode);
            }

            private AutoResetEvent pushFrameEvent = new AutoResetEvent(true);

            /// <summary>
            /// Pushes a frame into the defined video source.
            /// </summary>
            /// <param name="frame">The frame to push to the video source.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInvalidFrameFormat</c> if an invalid frame format was specified.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInvalidFramePlaneCount</c> if an invalid plane count was specified for the given frame format.
            /// </returns>
            protected async Task<MLResult> PushFrameAsync(MLWebRTC.VideoSink.Frame frame)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return await Task.FromResult(MLResult.Create(MLResult.Code.InvalidParam, "Handle is invalid."));
                }
                pushFrameEvent.Reset();

                MLWebRTC.VideoSink.Frame.NativeBindings.MLWebRTCFrame frameNative = MLWebRTC.VideoSink.Frame.NativeBindings.MLWebRTCFrame.Create(frame);
                MLResult.Code resultCode = NativeBindings.MLWebRTCSourceAppDefinedVideoSourcePushFrame(this.Handle, in frameNative);
                DidNativeCallSucceed(resultCode, "MLWebRTCSourceAppDefinedVideoSourcePushFrame()");
                pushFrameEvent.Set();
                return await Task.FromResult(MLResult.Create(resultCode));
            }

            public override MLResult DestroyLocal()
            {
                pushFrameEvent.WaitOne(250);
                MLResult result = base.DestroyLocal();
                return result;
            }
#endif
        }
    }
}
