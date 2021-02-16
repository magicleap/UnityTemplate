// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMRCameraFrameImagePlane.cs" company="Magic Leap">
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

    /// <summary>
    /// Mixed Reality Camera API, used to capture camera frames that include mixed reality content.
    /// </summary>
    public partial class MLMRCamera
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
                /// Image Data
                /// </summary>
                public byte[] Data;

                /// <summary>
                /// Gets the pointer to the unmanaged memory where the actual image data is found.
                /// Only valid when received via the OnFrameCapture_NativeCallbackThread event.
                /// </summary>
                public IntPtr DataPtr { get; private set; }

                /// <summary>
                /// Override to display the contents of a image plane as a string.
                /// </summary>
                /// <returns>A string representation of this struct.</returns>
                public override string ToString() => $"\nWidth: {this.Width} pixels, \nHeight: {this.Height} pixels, \nBytes Per Pixel:{this.BytesPerPixel} bytes, \nStride: {this.Stride} bytes per row, \nSize: {this.Size} total bytes";

                /// <summary>
                /// Creates and returns an initialized version of this struct.
                /// </summary>
                /// <param name="width">Width of the image plane.</param>
                /// <param name="height">Height of the image plane.</param>
                /// <param name="stride">Stride of the image plane.</param>
                /// <param name="bytesPerPixel">Bytes per pixel of the image plane.</param>
                /// <param name="size">Size of the image plane.</param>
                /// <param name="data">Pointer to the image data for the image plane.</param>
                /// <param name="byteArrayToUse">Optional byte array to store frame data in managed memory.</param>
                /// <returns>An initialized version of this struct.</returns>
                public static ImagePlane Create(uint width, uint height, uint stride, uint bytesPerPixel, uint size, IntPtr data, byte[] byteArrayToUse = null)
                {
                    ImagePlane imagePlane = new ImagePlane();
                    imagePlane.Width = width;
                    imagePlane.Height = height;
                    imagePlane.Stride = stride;
                    imagePlane.BytesPerPixel = bytesPerPixel;
                    imagePlane.Size = size;
                    imagePlane.DataPtr = data;
                    if (byteArrayToUse != null)
                    {
                        imagePlane.Data = byteArrayToUse;
                        Marshal.Copy(data, imagePlane.Data, 0, imagePlane.Data.Length);
                    }
                    return imagePlane;
                }
            }
        }
    }
}
