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

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLCamera class exposes static functions to query camera related
    /// functions. Most functions are currently a direct pass through functions to the
    /// native C-API functions and incur no overhead.
    /// </summary>
    [RequireXRLoader]
    public sealed partial class MLCVCamera : MLAutoAPISingleton<MLCVCamera>
    {
        /// <summary>
        /// Contains camera intrinsic parameters.
        /// </summary>
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

#if PLATFORM_LUMIN
        private ulong headTrackerHandle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Get camera intrinsic parameter.
        /// Requires ComputerVision privilege.
        /// </summary>
        /// <param name="outParameters">Output structure containing intrinsic parameters on success.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if unable to retrieve intrinsic parameter.
        /// </returns>
        public static MLResult GetIntrinsicCalibrationParameters(out MLCVCamera.IntrinsicCalibrationParameters outParameters)
        {
            return Instance.InternalGetIntrinsicCalibrationParameters(MLCVCameraNativeBindings.CameraID.ColorCamera, out outParameters);
        }

        /// <summary>
        /// Get transform between world origin and the camera. This method relies on a camera timestamp
        /// that is normally acquired from the MLCameraResultExtras structure, therefore this method is
        /// best used within a capture callback to maintain as much accuracy as possible.
        /// Requires ComputerVision privilege.
        /// </summary>tran
        /// <param name="vcamTimestamp">Time in nanoseconds to request the transform.</param>
        /// <param name="outTransform">Output transformation matrix on success.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if outTransform parameter was not valid (null).
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        public static MLResult GetFramePose(ulong vcamTimestamp, out Matrix4x4 outTransform)
        {
            return Instance.InternalGetFramePose(MLCVCameraNativeBindings.CameraID.ColorCamera, vcamTimestamp, out outTransform);
        }

        protected override MLResult.Code StartAPI()
        {
            if (!MLDevice.IsReady())
            {
                MLPluginLog.WarningFormat("MLCamera API is attempting to start before the MagicLeap XR Loader has been initialiazed, this could cause issues with MLCVCamera features. If your application needs these features please wait to start API until Monobehavior.Start and if issue persists make sure ProjectSettings/XR/Initialize On Startup is enabled.");
            }

            MLResult result = MLHeadTracking.Start();
            if (result.IsOk)
            {
                result = MLHeadTracking.GetState(out MLHeadTracking.State headTrackingState);
                if (!result.IsOk)
                {
                    MLPluginLog.ErrorFormat("MLCVCamera.StartAPI failed to get head pose state. Reason: {0}", result);
                }

                headTrackerHandle = headTrackingState.Handle;
                MLHeadTracking.Stop();
            }
            else
            {
                MLPluginLog.ErrorFormat("MLCVCamera.StartAPI failed to get head pose state. MLHeadTracking could not be successfully started.");
            }

            return MLCVCameraNativeBindings.MLCVCameraTrackingCreate(ref Handle);
        }

        protected override MLResult.Code StopAPI() => MLCVCameraNativeBindings.MLCVCameraTrackingDestroy(Handle);

        /// <summary>
        /// Get the intrinsic calibration parameters.
        /// </summary>
        /// <param name="cameraId">The id of the camera.</param>
        /// <param name="outParameters">The intrinsic calibration parameters.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if obtained result extras successfully.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to obtain result extras due to invalid input parameter.
        /// </returns>
        private MLResult InternalGetIntrinsicCalibrationParameters(MLCVCameraNativeBindings.CameraID cameraId, out MLCVCamera.IntrinsicCalibrationParameters outParameters)
        {
            outParameters = new MLCVCamera.IntrinsicCalibrationParameters();

            MLCVCameraNativeBindings.IntrinsicCalibrationParametersNative internalParameters =
                MLCVCameraNativeBindings.IntrinsicCalibrationParametersNative.Create();

            MLResult.Code resultCode = MLCVCameraNativeBindings.MLCVCameraGetIntrinsicCalibrationParameters(Handle, cameraId, ref internalParameters);

            MLResult parametersResult = MLResult.Create(resultCode);
            if (!parametersResult.IsOk)
            {
                MLPluginLog.ErrorFormat("MLCamera.InternalGetIntrinsicCalibrationParameters failed to get camera parameters. Reason: {0}", parametersResult);
            }
            else
            {
                outParameters.Width = internalParameters.Width;
                outParameters.Height = internalParameters.Height;
                outParameters.FocalLength = new Vector2(internalParameters.FocalLength.X, internalParameters.FocalLength.Y);
                outParameters.PrincipalPoint = new Vector2(internalParameters.PrincipalPoint.X, internalParameters.PrincipalPoint.Y);
                outParameters.FOV = internalParameters.FOV;
                outParameters.Distortion = new double[internalParameters.Distortion.Length];
                internalParameters.Distortion.CopyTo(outParameters.Distortion, 0);
            }

            return parametersResult;
        }

        /// <summary>
        /// Get the frame pose.
        /// </summary>
        /// <param name="cameraId">The camera id.</param>
        /// <param name="vcamTimestamp">The timestamp of the frame pose.</param>
        /// <param name="outTransform">The transform of the frame pose.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult InternalGetFramePose(MLCVCameraNativeBindings.CameraID cameraId, ulong vcamTimestamp, out Matrix4x4 outTransform)
        {
            MagicLeapNativeBindings.MLTransform outInternalTransform = new MagicLeapNativeBindings.MLTransform();

            MLResult.Code resultCode = MLCVCameraNativeBindings.MLCVCameraGetFramePose(Handle, headTrackerHandle, cameraId, vcamTimestamp, ref outInternalTransform);
            MLResult poseResult = MLResult.Create(resultCode);

            if (!poseResult.IsOk)
            {
                MLPluginLog.ErrorFormat("MLCamera.InternalGetFramePose failed to get camera frame pose. Reason: {0}", poseResult);
                outTransform = new Matrix4x4();
            }
            else
            {
                outTransform = MLConvert.ToUnity(outInternalTransform);
            }

            return poseResult;
        }
#endif
    }
}
