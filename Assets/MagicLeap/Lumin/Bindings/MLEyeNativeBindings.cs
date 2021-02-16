//%BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLEyes.cs" company="Magic Leap, Inc">
//     Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
//%BANNER_END%

#if PLATFORM_LUMIN
using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLEyes : MLAutoAPISingleton<MLEyes>
    {
        private MLResult.Code CreateEyeTracker() => NativeBindings.MLEyeTrackingCreate(ref Instance.Handle);
        private MLResult.Code DestroyEyeTracker() => NativeBindings.MLEyeTrackingDestroy(Instance.Handle);
        private MLResult.Code GetStateInternal() => NativeBindings.MLEyeTrackingGetStateEx(Instance.Handle, ref NativeBindings.State);

        /// <summary>
        ///     See ml_eye_tracking.h for additional comments
        /// </summary>
        private class NativeBindings : MagicLeapNativeBindings
        {
            public static MLEyeTrackingStateEx State = MLEyeTrackingStateEx.CreateEmpty();

            /// <summary>
            ///     A set of possible error codes that the Eye Tracking system can report.
            /// </summary>
            public enum MLEyeTrackingError
            {

                /// <summary>
                ///     No error, tracking is nominal.
                /// </summary>
                None,
                /// <summary>
                ///     Eye Tracker failed.
                /// </summary>
                GenericError
            }

            /// <summary>
            ///     A set of possible calibration status codes that the Eye Tracking system can report.
            /// </summary>
            public enum MLEyeTrackingCalibrationStatus
            {
                /// <summary>
                ///     Calibration not completed
                /// </summary>
                None,
                /// <summary>
                ///     Calibration completed but outside desired thresholds.
                /// </summary>
                Bad,
                /// <summary>
                ///     Calibration completed inside desired thresholds.
                /// </summary>
                Good,               
            }            

            [StructLayout(LayoutKind.Sequential)]
            public struct MLEyeTrackingStateEx
            {
                /// <summary>
                ///     Struct version
                /// </summary>
                private uint Version;

                /// <summary>
                ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of fixation.
                /// </summary>
                public float FixationConfidence;

                /// <summary>
                ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of left eye center.
                /// </summary>
                public float LeftCenterConfidence;

                /// <summary>
                ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of right
                ///     eye center.
                /// </summary>
                public float RightCenterConfidence;

                /// <summary>
                ///     <c> true </c> if left eye is inside a blink. When not wearing the device,
                ///     values can be arbitrary.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool LeftBlink;

                /// <summary>
                ///     <c> true </c> if right eye is inside a blink. When not wearing the device,
                ///     values can be arbitrary.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool RightBlink;

                /// <summary>
                ///     Represents what eye tracking error (if any) is present.
                /// </summary>
                public MLEyeTrackingError Error;

                /// <summary>
                ///     Status of eye tracking calibration
                /// </summary>
                private MLEyeTrackingCalibrationStatus CalibrationStatus;

                [Obsolete]
                [MarshalAs(UnmanagedType.I1)]
                bool fixation_depth_is_uncomfortable;

                [Obsolete]
                float remaining_time_at_uncomfortable_depth;

                [Obsolete]
                [MarshalAs(UnmanagedType.I1)]
                bool fixation_depth_violation_has_occurred;

                /// <summary>
                ///     Timestamp for all the data fields in this struct
                /// </summary>
                public ulong Timestamp;

                /// <summary>
                ///     Pupil diameter in mm, left eye
                /// </summary>
                public float LeftPupilDiameter;

                /// <summary>
                ///     pupil diameter in mm, right eye
                /// </summary>
                public float RightPupilDiameter;

                public static MLEyeTrackingStateEx CreateEmpty() =>
                    new MLEyeTrackingStateEx() 
                    { 
                        Version = 1, 
                        CalibrationStatus = MLEyeTrackingCalibrationStatus.None,  
                        Error = MLEyeTrackingError.None
                    };
            }

            /// <summary>
            ///     Returns the active status of the eye tracker.
            /// </summary>
            /// <returns> The active status of the eye tracker. </returns>
            [DllImport("UnityMagicLeap", EntryPoint = "UnityMagicLeap_InputGetEyeTrackerActive")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool GetEyeTrackerActive();

            /// <summary>
            ///     Sets the active status of the eye tracker. This will enable/disable the eye
            ///     tracker subsystem.
            /// </summary>
            /// <param name="value"> The active status of the eye tracker. </param>
            [DllImport("UnityMagicLeap", EntryPoint = "UnityMagicLeap_InputSetEyeTrackerActive")]
            public static extern void SetEyeTrackerActive([MarshalAs(UnmanagedType.I1)] bool value);

            /// <summary>
            ///     Creates an eye tracker.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingCreate(ref ulong handle);

            /// <summary>
            ///     Destroys an eye tracker.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingDestroy(ulong handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingGetStateEx(ulong handle, ref MLEyeTrackingStateEx state);
        }
    }
}
#endif
