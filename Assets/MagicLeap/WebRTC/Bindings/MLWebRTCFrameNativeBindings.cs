// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCFrameNativeBindings.cs" company="Magic Leap, Inc">
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
    using System.Collections.Generic;
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
        public partial class VideoSink
        {
            /// <summary>
            /// Struct representing a captured camera frame.
            /// </summary>
            public partial struct Frame
            {
                /// <summary>
                /// Native bindings for the MLWebRTC.Frame struct. 
                /// </summary>
                internal class NativeBindings
                {
                    /// <summary>
                    /// Gets frame data.
                    /// </summary>
                    /// <param name="frameHandle">The handle to the frame to query.</param>
                    /// <param name="frame">Pointer to the frame data.</param>
                    /// <returns>
                    /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the dimensions were successfully obtained.
                    /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                    /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                    /// </returns>
                    [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLWebRTCFrameGetData(ulong frameHandle, ref MLWebRTCFrame frame);

                    /// <summary>
                    /// Frees any allocated memory made from building a WebRTC frame.
                    /// Only used when building your own frame objects, such as in an app defined video source.
                    /// </summary>
                    /// <param name="frame">Frame object with the allocated memory.</param>
                    /// <param name="frameNative">Timestamp of the frame.</param>
                    internal static void FreeAllocatedImageMemory(MLWebRTC.VideoSink.Frame frame, MLWebRTC.VideoSink.Frame.NativeBindings.MLWebRTCFrame frameNative)
                    {
                        for (int i = 0; i < frame.ImagePlanes.Length; ++i)
                        {
                            MLWebRTC.VideoSink.Frame.ImagePlane imagePlane = frame.ImagePlanes[i];
                            MLWebRTC.VideoSink.Frame.NativeBindings.ImagePlaneInfoNative imagePlaneNative = frameNative.ImagePlanes[i];
                        }
                    }

                    /// <summary>
                    /// Representation of the native frame structure.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLWebRTCFrame
                    {
                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public uint Version;

                        /// <summary>
                        /// Number of valid image planes in the ImagePlanes array.
                        /// </summary>
                        public ushort PlaneCount;

                        /// <summary>
                        /// ImagePlanes array containing the image data.
                        /// </summary>
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MLWebRTC.VideoSink.Frame.ImagePlane.MaxImagePlanes)]
                        public ImagePlaneInfoNative[] ImagePlanes;

                        /// <summary>
                        /// Timestamp of the frame.
                        /// </summary>
                        public ulong TimeStamp;

                        /// <summary>
                        /// Output format that the image planes will be in.
                        /// </summary>
                        public MLWebRTC.VideoSink.Frame.OutputFormat Format;

                        public static MLWebRTCFrame Create()
                        {
                            MLWebRTCFrame frameNative = new MLWebRTCFrame();
                            frameNative.Version = 1;
                            frameNative.PlaneCount = ImagePlane.MaxImagePlanes;
                            frameNative.Format = OutputFormat.YUV_420_888;
                            return frameNative;
                        }

                        /// <summary>
                        /// Creates and returns an initialized version of this struct from a MLWebRTC.VideoSink.Frame object.
                        /// </summary>
                        /// <param name="frame">The frame object to use for initializing.</param>
                        /// <returns>An initialized version of this struct.</returns>
                        public static MLWebRTCFrame Create(MLWebRTC.VideoSink.Frame frame)
                        {
                            MLWebRTCFrame frameNative = new MLWebRTCFrame();
                            frameNative.Version = 1;
                            frameNative.PlaneCount = (ushort)frame.ImagePlanes.Length;
                            frameNative.ImagePlanes = nativeImagePlanesBuffer.Get();

                            for (int i = 0; i < frame.ImagePlanes.Length; ++i)
                            {
                                frameNative.ImagePlanes[i].Data = frame.ImagePlanes[i];
                            }

                            frameNative.TimeStamp = frame.TimeStampUs;
                            frameNative.Format = frame.Format;
                            return frameNative;
                        }
                    }

                    /// <summary>
                    /// Buffer for native image plane arrays.
                    /// </summary>
                    static CircularBuffer<ImagePlaneInfoNative[]> nativeImagePlanesBuffer = CircularBuffer<ImagePlaneInfoNative[]>.Create(new ImagePlaneInfoNative[MLWebRTC.VideoSink.Frame.ImagePlane.MaxImagePlanes], 3);

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
                        public IntPtr ImageData;

                        /// <summary>
                        /// Size of the image plane.
                        /// </summary>
                        public uint Size;

                        /// <summary>
                        /// Sets data from an MLWebRTC.VideoSink.Frame.ImagePlane object.
                        /// </summary>
                        public MLWebRTC.VideoSink.Frame.ImagePlane Data
                        {
                            set
                            {
                                this.Width = value.Width;
                                this.Height = value.Height;
                                this.Stride = value.Stride;
                                this.BytesPerPixel = value.BytesPerPixel;
                                this.ImageData = value.Data;
                                this.Size = value.Size;
                            }
                        }
                    }
                }
            }
        }
    }
}
