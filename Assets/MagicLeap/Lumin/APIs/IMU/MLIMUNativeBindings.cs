//%BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLIMUNativeBindings.cs" company="Magic Leap, Inc">
//     Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
//%BANNER_END%

using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLIMU
    {
#if PLATFORM_LUMIN
        // == C# API intermediates ==

        protected override MLResult.Code StartAPI() => NativeBindings.MLImuTrackingCreate(ref Instance.Handle);

        protected override MLResult.Code StopAPI() => NativeBindings.MLImuTrackingDestroy(Instance.Handle);

        private static Sample CreateSample(NativeBindings.MLImuSample sampleNative) =>
            new Sample
            {
                IMULocation = (Location)sampleNative.sample_type,
                LinearAccelerationTimestamp = sampleNative.linear_acceleration_timestamp_in_microseconds,
                RotationalVelocityTimestamp = sampleNative.rotational_velocity_timestamp_in_microseconds,
                LinearAcceleration = sampleNative.linear_acceleration_in_meters_per_second_per_second.ToVector3(),
                RotationalVelocity = sampleNative.rotational_velocity_in_radians_per_second.ToVector3(),
                Temperature = sampleNative.temperature_in_celsius,
            };

        public partial class IMU
        {
            private NativeBindings.MLImuSample[] nativeSamplesBuffer = new NativeBindings.MLImuSample[NativeBindings.MLImuBufferSize];

            protected Data GetCurrentDataInternal()
            {
                NativeBindings.MLImuQueryFilter queryFilter = NativeBindings.MLImuQueryFilter.Create((NativeBindings.MLImuSampleType)Location, NativeBindings.MLImuBufferSize);

                MLResult.Code resultCode = NativeBindings.MLImuTrackingGetSamples(Instance.Handle, queryFilter,
                    nativeSamplesBuffer, out uint sampleCountPointer, out ulong missedSamplesPointer);

                if (!DidNativeCallSucceed(resultCode))
                    return default;

                if (!DidNativeCallSucceed(NativeBindings.MLImuTrackingClearSamples(Instance.Handle)))
                    return default;

                Sample[] samples = new Sample[sampleCountPointer];

                for (uint i = 0; i < sampleCountPointer; i++)
                {
                    samples[i] = CreateSample(nativeSamplesBuffer[i]);
                }

                return Data.Create(samples, missedSamplesPointer);
            }

            /// <summary>
            ///     Intermediate method for MLImuTrackingAreSamplesCalibrationCorrected.
            /// </summary>
            private bool GetIsCalibrated(Location loc)
            {
                NativeBindings.MLImuTrackingAreSamplesCalibrationCorrected
                (
                    Instance.Handle,
                    (NativeBindings.MLImuSampleType)loc,
                    out bool isCalibrated
                );
                return isCalibrated;
            }
        }

        public partial class ControllableIMU : IMU
        {
            /// <returns> success or failure of operation </returns>
            private bool Enable(SamplingRate rate = SamplingRate._500Hz)
            {
                switch (Location)
                {
                    case Location.LightwearLeft:
                        MLPluginLog.Error("The lightwear IMU cannot be enabled or disabled, and the sampling rate cannot be changed.");
                        return false;

                    case Location.Lightpack:
                        return DidNativeCallSucceed(NativeBindings.MLImuTrackingLightpackEnable(Instance.Handle, (NativeBindings.MLImuSamplingRate)rate));
                }

                return false;
            }

            /// <returns> success or failure of operation </returns>
            private bool Disable()
            {
                switch (Location)
                {
                    case Location.LightwearLeft:
                        return false; // lightwear IMU is always enabled

                    case Location.Lightpack:
                        return DidNativeCallSucceed(NativeBindings.MLImuTrackingLightpackDisable(Instance.Handle));
                }

                return false;
            }
        }

        /// <summary>
        ///     Native Bindings for the ML IMU API
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            ///     The various locations of the IMU on the lightwear and lightpack
            /// </summary>
            public enum MLImuSampleType
            {
                /// <summary>
                ///     Lightpack
                /// </summary>
                MLImuSampleType_Lightpack = 0,

                /// <summary>
                ///     Lightwear left
                /// </summary>
                MLImuSampleType_LightwearLeft = 1,

                MLImuSampleType_Ensure32Bits = 0x7FFFFFFF
            }

            /// <summary>
            ///     The various sampling rates supported by the API.
            /// </summary>
            public enum MLImuSamplingRate
            {
                MLImuSamplingRate_100Hz = 100,

                MLImuSamplingRate_200Hz = 200,

                MLImuSamplingRate_250Hz = 250,

                MLImuSamplingRate_500Hz = 500,

                MLImuSamplingRate_1000Hz = 1000,

                MLImuSamplingRate_Ensure32Bits = 0x7FFFFFFF
            }

            /// <summary>
            ///     IMU internal buffer size, in number of samples. The internal buffer is a
            ///     circular buffer that is implemented most efficiently with a size that is a
            ///     power-of-2. Any buffer passed to MLImuTrackingGetSamples() need not have a
            ///     greater capacity.
            /// </summary>
            public const int MLImuBufferSize = 1024;

            /// <summary>
            ///     Create an IMU Tracker.
            /// </summary>
            /// <param name="out_handle">
            ///     A pointer to an MLHandle which will contain the handle of the IMU tracker. If
            ///     this operation fails, out_handle will be #ML_INVALID_HANDLE.
            /// </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c> if the out_handle parameter was <c> null </c>.
            ///     <c> MLResult_Ok </c> if the tracker was created successfully. <c>
            ///     MLResult_PrivilegeDenied </c> if the application lacks the appropriate
            ///     privilege. <c> MLResult_UnspecifiedFailure </c> if the tracker failed to create successfully.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingCreate(ref ulong out_handle);

            /// <summary> Destroy an IMU Tracker. Requires ImuCapture, LowLatencyLightwear
            /// privileges. </summary> <param name="handle">A handle to an IMU Tracker created by
            /// MLImuTrackingCreate.</param> <returns> <c>MLResult_InvalidParam</c> if the handle
            /// parameter was not a valid and active IMU tracking handle. </returns>
            /// <c>MLResult_Ok</c> if the tracker was successfully destroyed.
            /// <c>MLResult_PrivilegeDenied</c> if the application lacks privilege.
            /// <c>MLResult_UnspecifiedFailure</c> if the tracker was not successfully destroyed. </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingDestroy(ulong handle);

            /// <summary>
            ///     Returns whether the samples from a specified IMU type are calibration-corrected
            ///     or not. Requires ImuCapture, LowLatencyLightwear privileges.
            /// </summary>
            /// <param name="handle"> A handle to an IMU Tracker created by MLImuTrackingCreate. </param>
            /// <param name="sample_type">
            ///     Specifies the type of the IMU samples for which calibration-correction is being queried.
            /// </param>
            /// <param name="out_samples_are_calibration_corrected">
            ///     <c> true </c> if IMU samples supplied by MLImuTrackingGetSamples() will be
            ///     calibration-corrected. The value is platform-specific and will not change for a
            ///     given platform.
            /// </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c> if the handle was not a valid and active IMU
            ///     tracking handle and/or the out_samples_are_calibration_corrected parameterwas
            ///     NULL. <c> MLResult_NotImplemented </c> if the specified sample_type is not
            ///     supported by the device. <c> MLResult_Ok </c> if the function executed
            ///     succesfully. <c> MLResult_PrivilegeDenied </c> if the application lacks
            ///     privilege. <c> MLResult_UnspecifiedFailure </c> if the function failed for an
            ///     unspecified reason.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingAreSamplesCalibrationCorrected(
              ulong handle, MLImuSampleType sample_type, [MarshalAs(UnmanagedType.I1)] out  bool bool__out_samples_are_calibration_corrected);

            /// <summary>
            ///     Return the next batch of IMU samples from a specified IMU type. The samples may
            ///     not be corrected for gravity; the samples are calibration-corrected if the
            ///     specified IMU has been calibrated. Requires ImuCapture, LowLatencyLightwear privileges.
            /// </summary>
            /// <param name="handle"> A handle to an IMU Tracker created by MLImuTrackingCreate. </param>
            /// <param name="query_filter">
            ///     Parameters specifying the type and version of the samples, and the size of the
            ///     out_samples buffer.
            /// </param>
            /// <param name="out_samples">
            ///     Pointer to buffer to be filled with the next batch of IMU samples (MLImuSample).
            ///     The caller owns the buffer and is responsible for allocating and deleting it.
            /// </param>
            /// <param name="out_sample_count"> The number of samples written to out_samples. </param>
            /// <param name="out_missed_sample_count">
            ///     Pointer to an integer to be filled with the number of missed samples due to a
            ///     full buffer since the most recent call. May be <c> null </c>.
            /// </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c> if The handle was not a valid and active IMU
            ///     tracking handle and/or the out_samples or out_samples_count parameter was <c>
            ///     null </c> or the version specified in query_filter is invalid. <c>
            ///     MLResult_NotImplemented </c> if the specified sample_type is not supported by
            ///     the device. <c> MLResult_Ok </c> if the samples were successfully received. <c>
            ///     MLResult_PrivilegeDenied </c> if the application lacks the appropriate
            ///     privileges. <c> MLResult_UnspecifiedFailure </c> if the function failed to
            ///     receive the IMU samples.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingGetSamples(ulong handle,
                                                            MLImuQueryFilter query_filter,
                                                            [MarshalAsAttribute(UnmanagedType.LPArray, SizeConst = NativeBindings.MLImuBufferSize)] MLImuSample[] out_samples,
                                                            out uint out_sample_count,
                                                            out ulong out_missed_sample_count);

            /// <summary>
            ///     Clears the pipeline of current IMU samples for all IMU types. Requires
            ///     ImuCapture, LowLatencyLightwear privileges.
            /// </summary>
            /// <param name="handle"> A handle to an IMU Tracker created by MLImuTrackingCreate. </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c> if the handle was not a valid and active IMU
            ///     tracking handle. <c> MLResult_Ok </c> if the samples were successfully cleared.
            ///     <c> MLResult_PrivilegeDenied </c> if the application lacks the appropriate
            ///     privileges. <c> MLResult_UnspecifiedFailure </c> if the function failed to clear
            ///     the IMU samples.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingClearSamples(ulong handle);

            /// <summary>
            ///     Enable the collection of Lightpack IMU samples. Collect and set the rate of
            ///     production of samples to a specified value. If the Lightpack IMU is not
            ///     currently enabled, it will be enabled. If the Lightpack IMU is currently
            ///     enabled, it will remain enabled. In either case, it will be set to generate IMU
            ///     samples at the specified rate.
            /// </summary>
            /// <param name="handle"> A handle to an IMU Tracker created by MLImuTrackingCreate. </param>
            /// <param name="sampling_rate">
            ///     An enumerator that specifies the sampling rate rate in Hz.
            /// </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c> if the handle parameter was <c> null </c> or
            ///     sampling_rate was not a valid enumerator. <c> MLResult_Ok </c> if the Lightpack
            ///     IMU was successfully enabled and the specified samplng rate set. <c>
            ///     MLResult_PrivilegeDenied </c> if the application lacks the appropriate
            ///     privileges. <c> MLResult_UnspecifiedFailure </c> if failed to enable the
            ///     Lightpack IMU.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingLightpackEnable(ulong handle,
                                                                MLImuSamplingRate sampling_rate);

            /// <summary>
            ///     Disables the collection of Lightpack IMU samples.
            /// </summary>
            /// <param name="handle"> A handle to an IMU Tracker created by MLImuTrackingCreate. </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c> if the handle parameter was <c> null </c>. <c>
            ///     MLResult_Ok </c> if sample collection from the Lightpack IMU was successfully
            ///     disabled. <c> MLResult_PrivilegeDenied </c> if the application lacks the
            ///     appropriate privileges. <c> MLResult_UnspecifiedFailure </c> if failed to
            ///     disable the Lightpack IMU.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLImuTrackingLightpackDisable(ulong handle);

            /// <summary>
            ///     IMU sample.
            ///     The sample is calibration-corrected if calibration data is available.
            ///     The sample is not corrected for gravity.
            ///</summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLImuSample
            {
                public ulong linear_acceleration_timestamp_in_microseconds;
                public ulong rotational_velocity_timestamp_in_microseconds;

                /// <summary>
                ///     Linear acceleration, in m/s^2. Gravity is included.
                /// </summary>
                public MLVec3f linear_acceleration_in_meters_per_second_per_second;

                public MLVec3f rotational_velocity_in_radians_per_second;

                public float temperature_in_celsius;

                public MLImuSampleType sample_type;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLImuQueryFilter
            {
                public uint version;

                public MLImuSampleType sample_type;

                /// <summary>
                ///     The size of the out_samples buffer, in number of samples of type
                ///     MLImuSample. The number of samples copied will be the lesser of sample_count
                ///     and MLImuBufferSize.
                /// </summary>
                public uint sample_count;

                public static MLImuQueryFilter Create(MLImuSampleType sampleType, uint count) =>
                    new MLImuQueryFilter { version = 1, sample_type = sampleType, sample_count = count };
            }
        }
#endif
    }
}
