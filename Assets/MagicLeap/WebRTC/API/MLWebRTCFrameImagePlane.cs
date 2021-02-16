// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCFrameImagePlane.cs" company="Magic Leap, Inc">
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
        public partial class VideoSink
        {
            /// <summary>
            /// Struct representing a captured camera frame.
            /// </summary>
            public partial struct Frame
            {
                /// <summary>
                /// Struct representing an image plane that comes from some captured camera frame.
                /// </summary>
                public struct ImagePlane
                {
                    /// <summary>
                    /// Max amount of image planes that can be in a single frame.
                    /// </summary>
                    public const int MaxImagePlanes = 3;

                    /// <summary>
                    /// Gets the width of the image plane.
                    /// </summary>
                    public uint Width { get; private set; }

                    /// <summary>
                    /// Gets the height of the image plane.
                    /// </summary>
                    public uint Height { get; private set; }

                    /// <summary>
                    /// Gets the stride of the image plane, representing how many bytes one row of the image plane contains.
                    /// </summary>
                    public uint Stride { get; private set; }

                    /// <summary>
                    /// Gets the bytes per pixel of the image plane.
                    /// </summary>
                    public uint BytesPerPixel { get; private set; }

                    /// <summary>
                    /// Gets the size of the image plane, representing how many bytes in total the entire image plane contains.
                    /// </summary>
                    public uint Size { get; private set; }

                    /// <summary>
                    /// Gets the pointer to the unmanaged memory where the actual image data is found.
                    /// </summary>
                    public IntPtr Data { get; private set; }

                    /// <summary>
                    /// Creates and returns an initialized version of this struct.
                    /// </summary>
                    /// <param name="width">Width of the image plane.</param>
                    /// <param name="height">Height of the image plane.</param>
                    /// <param name="stride">Stride of the image plane.</param>
                    /// <param name="bytesPerPixel">Bytes per pixel of the image plane.</param>
                    /// <param name="size">Size of the image plane.</param>
                    /// <param name="data">Pointer to the image data for the image plane.</param>
                    /// <param name="bytes">Byte array representation of to the image data for the image plane.</param>
                    /// <returns>An initialized version of this struct.</returns>
                    public static ImagePlane Create(uint width, uint height, uint stride, uint bytesPerPixel, uint size, IntPtr data)
                    {
                        ImagePlane imagePlane = new ImagePlane()
                        {
                            Width = width,
                            Height = height,
                            Stride = stride,
                            BytesPerPixel = bytesPerPixel,
                            Size = size,
                            Data = data
                        };
                        return imagePlane;
                    }

                    /// <summary>
                    /// Override to display the contents of a image plane as a string.
                    /// </summary>
                    /// <returns>A string representation of this struct.</returns>
                    public override string ToString() => $"\nWidth: {this.Width} pixels, \nHeight: {this.Height} pixels, \nBytes Per Pixel:{this.BytesPerPixel} bytes, \nStride: {this.Stride} bytes per row, \nSize: {this.Size} total bytes";
                }
            }
        }
    }
}
