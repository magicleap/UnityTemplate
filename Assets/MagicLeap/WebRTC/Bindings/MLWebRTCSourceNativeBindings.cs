// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCSourceNativeBindings.cs" company="Magic Leap, Inc">
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
        /// Class that represents a source used by the MLWebRTC API.
        /// </summary>
        public partial class Source
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.Source struct. 
            /// </summary>
            internal class NativeBindings
            {
#if PLATFORM_LUMIN
                /// <summary>
                /// Creates the local source that links to the user's MRCamera.
                /// </summary>
                /// <param name="inputContext">The MLMRCamera input context native object to use when initializing the MRCamera.</param>
                /// <param name="sourceHandle">The handle of the created source.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateLocalSourceForMRCamera(in MLMRCamera.NativeBindings.InputContextNative inputContext, out ulong sourceHandle);
#endif
                /// <summary>
                /// Creates the local source that links to the user's MLCamera.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateLocalSourceForCamera(out ulong sourceHandle);

                /// <summary>
                /// Creates the local source that links to the user's microphone.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateLocalSourceForMicrophone(out ulong sourceHandle);

                /// <summary>
                /// Checks if an audio source is currently enabled.
                /// </summary>
                /// <param name="sourceHandle">The handle of the source.</param>
                /// <param name="enabled">True if source is enabled.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio source status was queried successfully.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceIsEnabled(ulong sourceHandle, [MarshalAs(UnmanagedType.I1)] out bool enabled);

                /// <summary>
                /// Enables or disables a audio source.
                /// </summary>
                /// <param name="sourceHandle">The handle of the audio source.</param>
                /// <param name="enabled">Sets the audio source to be enabled or disabled.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the audio source was enabled/disabled successfully.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceSetEnabled(ulong sourceHandle, [MarshalAs(UnmanagedType.I1)] bool enabled);

                /// <summary>
                /// Checks if an video source is currently enabled.
                /// </summary>
                /// <param name="sourceHandle">The handle of the video source.</param>
                /// <param name="sourceType">Type of the source.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the video source status was queried successfully.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceGetType(ulong sourceHandle, out MLWebRTC.MediaStream.Track.Type sourceType);

                /// <summary>
                /// Destroys the local source.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to destroy.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully destroyed.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceDestroy(ulong sourceHandle);

                /// <summary>
                /// The native representation of an MLWebRTC source.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCSource
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Type of the struct.
                    /// </summary>
                    public MLWebRTC.MediaStream.Track.Type Type;

                    /// <summary>
                    /// Handle of the struct.
                    /// </summary>
                    public ulong Handle;

                    /// <summary>
                    /// Gets an MLWebRTC.Source object from the data of this object.
                    /// </summary>
                    public MLWebRTC.MediaStream.Track Data
                    {
                        get
                        {
                            MLWebRTC.MediaStream.Track track = new MLWebRTC.MediaStream.Track()
                            {
                                Handle = this.Handle,
                                TrackType = this.Type
                            };
                            return track;
                        }
                    }

                    /// <summary>
                    /// Creates and returns an initialized version of this struct from a native MLWebRTCSource object.
                    /// </summary>
                    /// <returns>An initialized version of this struct.</returns>
                    public static MLWebRTCSource Create()
                    {
                        MLWebRTCSource source = new MLWebRTCSource();
                        source.Version = 1;
#if PLATFORM_LUMIN
                        source.Handle = MagicLeapNativeBindings.InvalidHandle;
#endif
                        return source;
                    }
                }
            }
        }
    }
}
