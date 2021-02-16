// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAudioSink.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
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
        /// Class that represents an audio sink used by the MLWebRTC API.
        /// </summary>
        public partial class AudioSink : Sink
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AudioSink" /> class.
            /// </summary>
            internal AudioSink()
            {
                this.Type = MediaStream.Track.Type.Audio;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AudioSink" /> class.
            /// </summary>
            internal AudioSink(ulong handle) : base(handle)
            {
                this.Type = MediaStream.Track.Type.Audio;
            }

            /// <summary>
            /// Creates an initialized AudioSink object.
            /// </summary>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized AudioSink object.</returns>
            public static AudioSink Create(out MLResult result)
            {
                AudioSink audioSink = null;

#if PLATFORM_LUMIN
                List<MLWebRTC.Sink> sinks = MLWebRTC.Instance.sinks;
                ulong handle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkCreate(out handle);
                if (!DidNativeCallSucceed(resultCode, "MLWebRTCAudioSinkCreate()"))
                {
                    result = MLResult.Create(resultCode);
                    return audioSink;
                }

                audioSink = new AudioSink(handle);
                if (MagicLeapNativeBindings.MLHandleIsValid(audioSink.Handle))
                {
                    sinks.Add(audioSink);
                }

                result = MLResult.Create(resultCode);
#else
                result = new MLResult();
#endif
                return audioSink;
            }

            /// <summary>
            /// Sets the stream of the audio sink.
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

                return this.SetTrack(this.Stream.ActiveAudioTrack);
            }

            /// <summary>
            /// Sets the world position of the audio sink for <c>spatialized</c> audio.
            /// </summary>
            /// <param name="position">The position to set the audio sink to.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult SetPosition(Vector3 position)
            {
#if PLATFORM_LUMIN
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetPosition(this.Handle, MLConvert.FromUnity(position));
                DidNativeCallSucceed(resultCode, "MLWebRTCAudioSinkSetPosition()");
                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }

#if PLATFORM_LUMIN
            /// <summary>
            /// Resets the world position of the audio sink for <c>spatialized</c> audio.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult ResetPosition()
            {
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkResetPosition(this.Handle);
                DidNativeCallSucceed(resultCode, "MLWebRTCAudioSinkResetPosition()");
                return MLResult.Create(resultCode);
            }
#endif

            /// <summary>
            /// Sets the number of milliseconds of audio that should be cached in the buffers before dropping the packets.
            /// Dictates the audio latency when app recovers from lifecycle state transitions like standby & reality.
            /// Default is 200ms.
            /// </summary>
            /// <param name="sinkHandle">The handle to the audio sink.</param>
            /// <param name="millisecondsToCache">How many milliseconds worth of audio to cache.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult SetCacheSize(uint millisecondsToCache)
            {
#if PLATFORM_LUMIN
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetCacheSize(this.Handle, millisecondsToCache);
                DidNativeCallSucceed(resultCode, $"MLWebRTCAudioSinkSetCacheSize({millisecondsToCache})");
                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Destroys this audio sink object.
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

                this.SetStream(null);
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkDestroy(this.Handle);
                DidNativeCallSucceed(resultCode, "MLWebRTCAudioSinkDestroy()");
                this.InvalidateHandle();

                MLWebRTC.Instance.sinks.Remove(this);

                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Sets the track of the audio sink.
            /// </summary>
            /// <param name="track">The track to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            protected override MLResult SetTrack(MLWebRTC.MediaStream.Track track)
            {
#if PLATFORM_LUMIN
                ulong sourceHandle = track != null ? track.Handle : MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCAudioSinkSetSource(this.Handle, sourceHandle);
                DidNativeCallSucceed(resultCode, "MLWebRTCAudioSinkSetSource()");
                return MLResult.Create(resultCode);
#else
                return new MLResult();
#endif
            }
        }
    }
}
