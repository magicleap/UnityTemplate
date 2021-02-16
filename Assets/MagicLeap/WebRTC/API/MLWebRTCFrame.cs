// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCFrame.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
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
                /// Defines the supported output formats of the image planes.
                /// </summary>
                public enum OutputFormat
                {
                    /// <summary>
                    /// YUV_420_888 format.
                    /// </summary>
                    YUV_420_888,

                    /// <summary>
                    /// RGBA_8888 format.
                    /// </summary>
                    RGBA_8888
                }

                /// <summary>
                /// Defines amount of valid image planes to be found within the frame's image planes array output formats of the image planes.
                /// </summary>
                public enum NativeImagePlanesLength
                {
                    /// <summary>
                    /// Length of valid image planes for the YUV_420_888 format.
                    /// </summary>
                    YUV_420_888 = 3,

                    /// <summary>
                    /// Length of valid image planes for the YUV_420_888 format.
                    /// </summary>
                    RGBA_8888 = 1
                }

                /// <summary>
                /// Gets the id of the frame.
                /// </summary>
                public ulong Id { get; private set; }

                /// <summary>
                /// Gets the timestamp of the frame in microseconds.
                /// </summary>
                public ulong TimeStampUs { get; private set; }

                /// <summary>
                /// Gets the array of image planes contained in this frame.
                /// </summary>
                public MLWebRTC.VideoSink.Frame.ImagePlane[] ImagePlanes { get; private set; }

                /// <summary>
                /// Gets the format of the image planes in this frame.
                /// </summary>
                public MLWebRTC.VideoSink.Frame.OutputFormat Format { get; private set; }

                /// <summary>
                /// Creates and returns an initialized version of this struct.
                /// </summary>
                /// <param name="id">Id of the frame.</param>
                /// <param name="timeStampUs">Timestamp of the frame in microseconds.</param>
                /// <param name="imagePlanes">Array of image planes this frame contains.</param>
                /// <param name="format">The output format of this frame.</param>
                /// <returns>An initialized version of this struct.</returns>
                public static Frame Create(ulong id, ulong timeStampUs, MLWebRTC.VideoSink.Frame.ImagePlane[] imagePlanes, MLWebRTC.VideoSink.Frame.OutputFormat format)
                {
                    Frame frame = new Frame()
                    {
                        Id = id,
                        TimeStampUs = timeStampUs,
                        ImagePlanes = imagePlanes,
                        Format = format
                    };

                    return frame;
                }

                internal static Frame Create(ulong id, NativeBindings.MLWebRTCFrame nativeFrame, ImagePlane[] imagePlanes = null)
                {
                    Frame frame = new Frame()
                    {
                        Id = id,
                        TimeStampUs = nativeFrame.TimeStamp,
                        ImagePlanes = (imagePlanes == null) ? new ImagePlane[nativeFrame.PlaneCount] : imagePlanes,
                        Format = nativeFrame.Format
                    };

                    for (ushort i = 0; i < nativeFrame.PlaneCount; ++i)
                    {
                        frame.ImagePlanes[i] = VideoSink.Frame.ImagePlane.Create(nativeFrame.ImagePlanes[i].Width, nativeFrame.ImagePlanes[i].Height, nativeFrame.ImagePlanes[i].Stride, nativeFrame.ImagePlanes[i].BytesPerPixel, nativeFrame.ImagePlanes[i].Size, nativeFrame.ImagePlanes[i].ImageData);
                    }
                    return frame;
                }

                /// <summary>
                /// Override to display the contents of a frame as a string.
                /// </summary>
                /// <returns>A string representation of this struct.</returns>
                public override string ToString() => $"\nId: {this.Id}, \nTimeStamp (us): {this.TimeStampUs}, \nNumImagePlanes: {this.ImagePlanes.Length}, \nFormat: {this.Format}";
            }
        }
    }
}
