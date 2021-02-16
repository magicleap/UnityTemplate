// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMRCameraNativeBindings.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// Mixed Reality Camera API, used to capture camera frames that include mixed reality content.
    /// </summary>
    public sealed partial class MLMRCamera
    {
        /// <summary>
        /// Native implementations for the MLMRCamera API.
        /// </summary>
        internal class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Struct version to initialize a CallbacksNative object with.
            /// </summary>
            private const int CallbacksStructVersion = 1;

            /// <summary>
            /// Struct version to initialize a InputContextNative object with.
            /// </summary>
            private const int InputContextStructVersion = 1;

            /// <summary>
            /// The max amount of image planes a single captured frame can contain.
            /// </summary>
            private const int MaxImagePlanes = 3;

            /// <summary>
            /// Name of the library used for this API.
            /// </summary>
            private const string MLMRCameraDLL = "ml_mr_camera";

            private static List<CircularBuffer<byte[]>> byteArraysBuffers = new List<CircularBuffer<byte[]>>();

            /// <summary>
            /// Delegate used for the native OnFrameCapture callback.
            /// </summary>
            /// <param name="info">Object that holds the information of the captured frame.</param>
            public delegate void OnFrameCaptureDelegate(ref FrameCaptureInfoNative info);

            /// <summary>
            /// Delegate used for the native OnCaptureComplete callback.
            /// </summary>
            /// <param name="data">User data as passed to MLMRCameraSetCallbacks().</param>
            public delegate void OnCaptureCompleteDelegate(IntPtr data);

            /// <summary>
            /// Delegate used for the native OnError callback.
            /// </summary>
            /// <param name="resultCode">MLResult.Code of the internal error.</param>
            /// <param name="data">User data as passed to MLMRCameraSetCallbacks().</param>
            public delegate void OnErrorDelegate(MLResult.Code resultCode, IntPtr data);

            /// <summary>
            /// Creates and initializes a native callbacks object.
            /// </summary>
            /// <returns>An initialized CallbacksNative object.</returns>
            public static CallbacksNative CreateCallbacks()
            {
                CallbacksNative callbacks = new CallbacksNative();
                callbacks.Version = CallbacksStructVersion;
                callbacks.OnFrameCapture = OnFrameCapture;
                callbacks.OnCaptureComplete = OnCaptureComplete;
                callbacks.OnError = OnError;
                return callbacks;
            }

            /// <summary>
            /// Initialize the Mixed Reality Camera.
            /// </summary>
            /// <param name="context">Input context the mixed reality camera should use when capturing.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully initialized the MR camera.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid input parameter.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericAlreadyExists</c> if the MR camera was already initialized.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraConnect(ref InputContextNative context);

            /// <summary>
            /// De-initialize the Mixed Reality Camera.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the MR camera was successfully de-initialized.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraDisconnect();

            /// <summary>
            /// Set callbacks to receive Mixed Reality Camera notifications
            /// </summary>
            /// <param name="callbacks">Callbacks object to hook into the API with.</param>
            /// <param name="data">Custom data to be returned when any callback is fired.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the callbacks changed successfully.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraSetCallbacks(ref CallbacksNative callbacks, [In, Out] IntPtr data);

            /// <summary>
            ///  Get capture output size based on quality.
            /// </summary>
            /// <param name="width">Width of captured Mixed Reality Camera frame.</param>
            /// <param name="height">Height of captured Mixed Reality Camera frame.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully returned the current resolution.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if width or height was invalid.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraGetCaptureSize(out uint width, out uint height);

            /// <summary>
            ///  Prepare to capture Mixed Reality Camera frame.
            ///  The frame buffers will be passed to the application via OnFrameCaptured callback.
            /// </summary>
            /// <param name="surfaceHandle">Unused, must be MagicLeapNativeBindings.InvalidHandle.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if prepared successfully.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraPrepareCapture(ulong surfaceHandle);

            /// <summary>
            ///  Start capture of Mixed Reality Camera frames.
            ///  Caller needs to call MLMRCameraStopCapture to stop capturing.
            /// </summary>
            /// <param name="frames">Number of frames to capture. 0 will capture indefinitely.</param>
            /// <param name="waitForLock">Wait for AE and AWB lock. true = wait, false = no wait.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if started successfully.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraStartCapture(uint frames, bool waitForLock);

            /// <summary>
            ///  Stop capturing Mixed Reality Camera frames if still capturing.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if stopped successfully.
            /// MLResult.Result will be <c>MLResult.Code.MediaGenericInvalidOperation</c> if the MR camera was not properly stopped.
            /// </returns>
            [DllImport(MLMRCameraDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMRCameraStopCapture();

            /// <summary>
            /// Callback into our managed code for the OnFrameCapture event.
            /// </summary>
            /// <param name="info">Object that holds the information of the captured frame.</param>
            [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnFrameCaptureDelegate))]
            private static void OnFrameCapture(ref FrameCaptureInfoNative info)
            {
                OutputNative output = Marshal.PtrToStructure<OutputNative>(info.OutputPtr);
                ulong id = info.Id;
                ulong timeStamp = info.TimeStamp;

                bool marshalFrameData = MLMRCamera.OnFrameCapture != null;

                MLMRCamera.Frame.ImagePlane[] imagePlanes = new MLMRCamera.Frame.ImagePlane[(uint)output.ImagePlanesCount];
                for (int i = 0; i < output.ImagePlanesCount; ++i)
                {
                    ImagePlaneInfoNative planeNative = output.ImagePlanes[i];
                    if (byteArraysBuffers.Count <= i && marshalFrameData)
                    {
                        byteArraysBuffers.Add(CircularBuffer<byte[]>.Create(new byte[planeNative.Size], 3));
                    }

                    imagePlanes[i] = MLMRCamera.Frame.ImagePlane.Create(planeNative.Width, planeNative.Height, planeNative.Stride, planeNative.BytesPerPixel, planeNative.Size, planeNative.Data, (marshalFrameData) ? byteArraysBuffers[i].Get() : null);
                }

                MLMRCamera.Frame frame = MLMRCamera.Frame.Create(id, timeStamp, imagePlanes, output.Format);
                OnFrameCapture_NativeCallbackThread?.Invoke(frame);

                MLThreadDispatch.ScheduleMain(() =>
                {
                    MLMRCamera.OnFrameCapture?.Invoke(frame);
                });
            }

            /// <summary>
            /// Callback into our managed code for the native OnCaptureComplete callback.
            /// </summary>
            /// <param name="data">User data as passed to MLMRCameraSetCallbacks().</param>
            [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnCaptureCompleteDelegate))]
            private static void OnCaptureComplete(IntPtr data)
            {
                if (MLMRCamera.IsStarted)
                {
                    // Atomic operation, so setting outside main thread should be fine.
                    Instance.isCapturing = false;
                }

                MLThreadDispatch.ScheduleMain(() =>
                {
                    if (MLMRCamera.IsStarted)
                    {
                        MLMRCamera.OnCaptureComplete?.Invoke();
                    }
                });
            }

            /// <summary>
            /// Callback into our managed code for the native OnError callback.
            /// </summary>
            /// <param name="resultCode">MLResult.Code of the internal error.</param>
            /// <param name="data">User data as passed to MLMRCameraSetCallbacks().</param>
            [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnErrorDelegate))]
            private static void OnError(MLResult.Code resultCode, IntPtr data)
            {
                MLThreadDispatch.ScheduleMain(() =>
                {
                    if (MLMRCamera.IsStarted)
                    {
                        MLMRCamera.OnError?.Invoke(MLResult.Create(resultCode));
                    }
                });
            }

            /// <summary>
            /// The native representation of the MLMRCamera callback events.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct CallbacksNative
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// OnFrameCapture delegate to hook into the MLMRCamera API with.
                /// </summary>
                public OnFrameCaptureDelegate OnFrameCapture;

                /// <summary>
                /// OnCaptureComplete delegate to hook into the MLMRCamera API with.
                /// </summary>
                public OnCaptureCompleteDelegate OnCaptureComplete;

                /// <summary>
                /// OnError delegate to hook into the MLMRCamera API with.
                /// </summary>
                public OnErrorDelegate OnError;
            }

            /// <summary>
            /// Representation of the native image plane structure.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct ImagePlaneInfoNative
            {
                /// <summary>
                /// Width of the image plane.
                /// </summary>
                public uint Width;

                /// <summary>
                /// Height of the image plane.
                /// </summary>
                public uint Height;

                /// <summary>
                /// The stride of the image plane, representing how many bytes one row of the image plane contains.
                /// </summary>
                public uint Stride;

                /// <summary>
                /// The bytes per pixel of the image plane.
                /// </summary>
                public uint BytesPerPixel;

                /// <summary>
                /// Data of the image plane.
                /// </summary>
                public IntPtr Data;

                /// <summary>
                /// Size of the image plane.
                /// </summary>
                public uint Size;
            }

            /// <summary>
            /// Representation of the native input context structure.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct InputContextNative
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Quality that the capture should have.
                /// </summary>
                public RenderQuality Quality;

                /// <summary>
                /// Blend type that the capture should have.
                /// </summary>
                public BlendType BlendType;

                /// <summary>
                /// Stabilization that the capture should have.
                /// </summary>
                public bool Stabilizaiton;

                /// <summary>
                /// Sets data from the managed InputContext object into this one.
                /// </summary>
                public MLMRCamera.InputContext Data
                {
                    set
                    {
                        this.Version = InputContextStructVersion;
                        this.Quality = value.Quality;
                        this.BlendType = value.BlendType;
                        this.Stabilizaiton = value.Stabilization;
                    }
                }
            }

            /// <summary>
            /// Mixed Reality Camera Frame capture info.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct FrameCaptureInfoNative
            {
                /// <summary>
                /// Mixed Reality Camera frame id number. Id might not start from 0. 
                /// </summary>
                public ulong Id;

                /// <summary>
                /// Mixed Reality Camera frame time stamp in nanoseconds (ns).
                /// </summary>
                public ulong TimeStamp;

                /// <summary>
                /// Provides image properties and buffer pointer to image data.
                /// </summary>
                public IntPtr OutputPtr;

                /// <summary>
                /// User data as passed to MLMRCameraSetCallbacks().
                /// </summary>
                public IntPtr Data;
            }

            /// <summary>
            /// Representation of the native output structure.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct OutputNative
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Number of image planes given in the imagePlanes array. 1 for RGB8888.
                /// </summary>
                public byte ImagePlanesCount;

                /// <summary>
                /// Array of image planes contained in some captured frame.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxImagePlanes)]
                public ImagePlaneInfoNative[] ImagePlanes;

                /// <summary>
                /// The format contained in some captured frame.
                /// </summary>
                public OutputFormat Format;
            }
        }
    }
}
#endif
