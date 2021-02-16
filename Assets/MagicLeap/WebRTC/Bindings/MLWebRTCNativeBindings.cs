// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLWebRTCNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// --------------------------------------------------------------------
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
#if PLATFORM_LUMIN
        /// <summary>
        /// Native bindings for the MLWebRTC class. 
        /// </summary>
        internal class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Delegate describing the callback necessary to monitor when an debug message is received.
            /// </summary>
            /// <param name="logLevel">The log level of the debug utils.</param>
            /// <param name="message">The debug message.</param>
            /// <param name="context">Pointer to a context object.</param>
            public delegate void OnDebugMessageDelegate(MagicLeapLogger.LogLevel logLevel, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context);

            /// <summary>
            /// Creates the MLWebRTC instance.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully created.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebRTCInstanceCreate();

            /// <summary>
            /// Creates the MLWebRTC instance with more debug logs.
            /// </summary>
            /// <param name="debugUtils">The MLWebRTCDebugUtils object to initialize with.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully created.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebRTCInstanceCreateWithDebugUtils(in MLWebRTCDebugUtils debugUtils);

            /// <summary>
            /// Destroys the MLWebRTC instance.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully destroyed.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebRTCInstanceDestroy();

            /// <summary>
            /// Gets the string value of MLWebRTC specific result codes.
            /// </summary>
            /// <param name="result">The MLWebRTC specific result code.</param>
            /// <returns> 
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the instance was successfully destroyed.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
            /// </returns>
            [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLWebRTCGetResultString(MLResult.Code result);

            /// <summary>
            /// Native callback that is invoked when a data channel closes.
            /// </summary>
            /// <param name="logLevel">The log level of the debug utils.</param>
            /// <param name="message">The debug message.</param>
            /// <param name="context">Pointer to a context object.</param>            
            [AOT.MonoPInvokeCallback(typeof(OnDebugMessageDelegate))]
            private static void OnDebugMessage(MagicLeapLogger.LogLevel logLevel, [MarshalAs(UnmanagedType.LPStr)] string message, IntPtr context)
            {
                Debug.LogError("MLWebRTC  Debug: " + message);
            }

            /// <summary>
            /// The native representation of the MLWebRTC debug utilities object.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLWebRTCDebugUtils
            {
                /// <summary>
                /// Version of the struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Pointer to a context object.
                /// </summary>
                public IntPtr Context;

                /// <summary>
                /// The log level to respect for debugging.
                /// </summary>
                public MagicLeapLogger.LogLevel LogLevel;

                /// <summary>
                /// The callback for when a debug message is received.
                /// </summary>
                public OnDebugMessageDelegate OnDebugMessage;

                /// <summary>
                /// Creates an initialized MLWebRTCDebugUtils object.
                /// </summary>
                /// <returns>An initialized MLWebRTCDebugUtils object.</returns>
                public static MLWebRTCDebugUtils Create()
                {
                    MLWebRTCDebugUtils debugUtils = new MLWebRTCDebugUtils
                    {
                        Version = 1,
                        LogLevel = MagicLeapLogger.LogLevel.Error,
                        OnDebugMessage = NativeBindings.OnDebugMessage
                    };

                    return debugUtils;
                }
            }
        }
#endif
    }
}
