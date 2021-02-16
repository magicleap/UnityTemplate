// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAudioSinkNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
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
        /// Class that represents an audio sink used by the MLWebRTC API.
        /// </summary>
        public partial class AudioSink
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AudioSink class. 
            /// </summary>
            internal class NativeBindings
            {
                /// <summary>
                /// Creates an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio sink was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkCreate(out ulong sinkHandle);

                /// <summary>
                /// Sets the source of an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="sourceHandle">The handle to the source to set onto the audio sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetSource(ulong sinkHandle, ulong sourceHandle);

#if PLATFORM_LUMIN
                /// <summary>
                /// Sets the world position of an audio sink for <c>spatialized</c> audio.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="position">The position to set the audio sink to.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetPosition(ulong sinkHandle, in MagicLeapNativeBindings.MLVec3f position);
#endif

                /// <summary>
                /// Resets the world position of an audio sink for <c>spatialized</c> audio.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkResetPosition(ulong sinkHandle);

                /// <summary>
                /// Sets the number of milliseconds of audio that should be cached in the buffers before dropping the packets.
                /// Dictates the audio latency when app recovers from lifecycle state transitions like standby & reality.
                /// Default is 200ms.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <param name="millisecondsToCache">How many milliseconds worth of audio to cache.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkSetCacheSize(ulong sinkHandle, uint millisecondsToCache);

                /// <summary>
                /// Destroys an audio sink.
                /// </summary>
                /// <param name="sinkHandle">The handle to the audio sink.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCAudioSinkDestroy(ulong sinkHandle);
            }
        }
    }
}
