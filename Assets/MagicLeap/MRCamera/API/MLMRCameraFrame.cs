// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMRCameraFrame.cs" company="Magic Leap">
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
            /// Gets the id of the frame.
            /// </summary>
            public ulong Id { get; private set; }

            /// <summary>
            /// Gets the timestamp of the frame in nanoseconds (ns).
            /// </summary>
            public ulong TimeStampNs { get; private set; }

            /// <summary>
            /// Gets the array of image planes contained in this frame.
            /// </summary>
            public MLMRCamera.Frame.ImagePlane[] ImagePlanes { get; private set; }

            /// <summary>
            /// Gets the format of the image planes in this frame.
            /// </summary>
            public MLMRCamera.OutputFormat Format { get; private set; }

            /// <summary>
            /// Override to display the contents of a frame as a string.
            /// </summary>
            /// <returns>A string representation of this struct.</returns>
            public override string ToString() => $"\nId: {this.Id}, \nTimeStamp: {this.TimeStampNs}, \nNumImagePlanes: {this.ImagePlanes.Length}, \nFormat: {this.Format}";

            /// <summary>
            /// Creates and returns an initialized version of this struct.
            /// </summary>
            /// <param name="id">Id of the frame.</param>
            /// <param name="timeStampNs">Timestamp of the frame.</param>
            /// <param name="imagePlanes">Array of image planes this frame contains.</param>
            /// <param name="format">The output format of this frame.</param>
            /// <returns>An initialized version of this struct.</returns>
            public static Frame Create(ulong id, ulong timeStampNs, MLMRCamera.Frame.ImagePlane[] imagePlanes, MLMRCamera.OutputFormat format)
            {
                Frame frame = new Frame();
                frame.Id = id;
                frame.TimeStampNs = timeStampNs;
                frame.ImagePlanes = imagePlanes;
                frame.Format = format;
                return frame;
            }
        }
    }
}
