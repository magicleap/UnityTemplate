// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLCamera.cs" company="Magic Leap, Inc">
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

    public sealed partial class MLCamera
    {
        [Obsolete("Use MLCVCamera.IntrinsicCalibrationParameters instead")]
        public struct IntrinsicCalibrationParameters
        {
            /// <summary>
            /// Camera width.
            /// </summary>
            public uint Width;

            /// <summary>
            /// Camera height.
            /// </summary>
            public uint Height;

            /// <summary>
            /// Camera focal length.
            /// </summary>
            public Vector2 FocalLength;

            /// <summary>
            /// Camera principle point.
            /// </summary>
            public Vector2 PrincipalPoint;

            /// <summary>
            /// Field of view.
            /// </summary>
            public float FOV;

            /// <summary>
            /// Distortion Coefficients.
            /// The distortion coefficients are in the following order:
            /// [k1, k2, p1, p2, k3]
            /// </summary>
            public double[] Distortion;
        }

        [Obsolete("No need to call MLCamera.Start() anymore. API will be started automatically when MLCamera.Connect() or MLCamera.ConnectAsync() is called.")]
        public static MLResult Start()
        {
            // return Ok to allow existing code to still work
            return MLResult.Create(MLResult.Code.Ok);
        }

        [Obsolete("No need to call MLCamera.Stop() anymore. API will be stopped automatically.")]
        public static void Stop() { }

        [Obsolete("Use MLCVCamera.GetIntrinsicCalibrationParameters instead")]
        public static MLResult GetIntrinsicCalibrationParameters(out MLCamera.IntrinsicCalibrationParameters outParameters)
        {
            outParameters = new MLCamera.IntrinsicCalibrationParameters();
            return MLResult.Create(MLResult.Code.NotImplemented);
        }

        [Obsolete("Use MLCVCamera.GetFramePose instead")]
        public static MLResult GetFramePose(ulong vcamTimestamp, out Matrix4x4 outTransform)
        {
            outTransform = new Matrix4x4();
            return MLResult.Create(MLResult.Code.NotImplemented);
        }
    }
}

#endif
