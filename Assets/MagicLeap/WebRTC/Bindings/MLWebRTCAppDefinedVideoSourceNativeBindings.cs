// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCAppDefinedVideoSourceNativeBindings.cs" company="Magic Leap, Inc">
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
        public partial class AppDefinedVideoSource
        {
            /// <summary>
            /// Native bindings for the MLWebRTC.AppDefinedVideoSource class. 
            /// </summary>
            internal class NativeBindings
            {
                /// <summary>
                /// A delegate that describes the requirements of the OnSetEnabled callback.
                /// </summary>
                /// <param name="enabled">True if the source was enabled.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnSetEnabledDelegate([MarshalAs(UnmanagedType.I1)] bool enabled, IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnDestroyed callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnDestroyedDelegate(IntPtr context);

                /// <summary>
                /// Initialized a given appDefinedVideoSource object and sets it's callbacks.
                /// </summary>
                /// <param name="appDefinedVideoSource">The AppDefinedVideoSource object to initialize.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the connection was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code InitializeAppDefinedVideoSource(MLWebRTC.AppDefinedVideoSource appDefinedVideoSource)
                {
#if PLATFORM_LUMIN
                    appDefinedVideoSource.TrackType = Type.Video;
                    appDefinedVideoSource.IsLocal = true;
                    appDefinedVideoSource.gcHandle = GCHandle.Alloc(appDefinedVideoSource);
                    IntPtr gcHandlePtr = GCHandle.ToIntPtr(appDefinedVideoSource.gcHandle);

                    ulong appDefinedVideoSourceHandle = MagicLeapNativeBindings.InvalidHandle;

                    NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks callbacks = NativeBindings.MLWebRTCAppDefinedSourceEventCallbacks.Create(gcHandlePtr);
                    MLResult.Code resultCode = NativeBindings.MLWebRTCSourceCreateAppDefinedVideoSource(in callbacks, out appDefinedVideoSourceHandle);
                    appDefinedVideoSource.Handle = appDefinedVideoSourceHandle;

                    if (!MLResult.IsOK(resultCode))
                    {
                        appDefinedVideoSource.gcHandle.Free();
                    }

                    return resultCode;
#else
                    appDefinedVideoSource = null;
                    return MLResult.Code.Ok;
#endif
                }

#if PLATFORM_LUMIN
                /// <summary>
                /// Callback that is invoked when the source has been enabled or disabled. This callback will be called on the main thread.
                /// </summary>
                /// <param name="enabled">True if the source was enabled.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(OnSetEnabledDelegate))]
                private static void OnSetEnabled(bool enabled, IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    AppDefinedVideoSource videoSource = gcHandle.Target as AppDefinedVideoSource;
                    videoSource?.OnSourceSetEnabled(enabled);
                }

                /// <summary>
                /// Callback that is invoked when the source has been destroyed. This callback will be called on the main thread.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(OnDestroyedDelegate))]
                private static void OnDestroyed(IntPtr context)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(context);
                    AppDefinedVideoSource videoSource = gcHandle.Target as AppDefinedVideoSource;
                    videoSource?.OnSourceDestroy();
                }
#endif
                /// <summary>
                /// Creates the local source that links to the user's camera and mic.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceCreateAppDefinedVideoSource(in MLWebRTCAppDefinedSourceEventCallbacks callbacks, out ulong sourceHandle);

                /// <summary>
                /// Creates the local source that links to the user's camera and mic.
                /// </summary>
                /// <param name="sourceHandle">The handle to the local source to return to the caller.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the local source was successfully created.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCSourceAppDefinedVideoSourcePushFrame(ulong sourceHandle, in MLWebRTC.VideoSink.Frame.NativeBindings.MLWebRTCFrame frameNative);

                /// <summary>
                /// The native representation of the MLWebRTC data channel callback events.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCAppDefinedSourceEventCallbacks
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public IntPtr Context;

                    /// <summary>
                    /// OnSetEnabled event.
                    /// </summary>
                    public OnSetEnabledDelegate OnSetEnabled;

                    /// <summary>
                    /// OnDestroyed event.
                    /// </summary>
                    public OnDestroyedDelegate OnDestroyed;

                    /// <summary>
                    /// Factory method used to create a new MLWebRTCAppDefinedVideoSourceEventCallbacks object.
                    /// </summary>
                    /// <param name="context">Pointer to the context object to use for the callbacks.</param>
                    /// <returns>An MLWebRTCAppDefinedVideoSourceEventCallbacks object with the given handle.</returns>
                    public static MLWebRTCAppDefinedSourceEventCallbacks Create(IntPtr context)
                    {
                        MLWebRTCAppDefinedSourceEventCallbacks callbacks = new MLWebRTCAppDefinedSourceEventCallbacks();
                        callbacks.Version = 1;
#if PLATFORM_LUMIN
                        callbacks.OnSetEnabled = NativeBindings.OnSetEnabled;
                        callbacks.OnDestroyed = NativeBindings.OnDestroyed;
#endif
                        callbacks.Context = context;
                        return callbacks;
                    }
                }
            }
        }
    }
}
