//%BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLIMU.cs" company="Magic Leap, Inc">
//     Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
//%BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    ///     API for interacting with the IMU on a Magic Leap device.
    /// </summary>
    public sealed partial class MLIMU : MLAutoAPISingleton<MLIMU>
    {
#if PLATFORM_LUMIN
        /// <summary>
        ///     The lightpack IMU hardware may be directly controlled via this API.
        /// </summary>
        public static ControllableIMU LightpackIMU => Instance.lightpackIMU;

        /// <summary>
        ///     Although its data can be read normally, the lightwear IMU is always enabled and
        ///     cannot be disabled.
        /// </summary>
        public static IMU LightwearLeftIMU => Instance.lightwearIMU;

        /// <summary>
        ///     The various sampling rates supported by the API.
        /// </summary>
        public enum SamplingRate
        {
            _100Hz = 100,

            _200Hz = 200,

            _250Hz = 250,

            _500Hz = 500,

            _1000Hz = 1000,
        }

        /// <summary>
        ///     The various locations of the IMU on the lightwear and lightpack
        /// </summary>
        public enum Location
        {
            /// <summary>
            ///     Lightpack
            /// </summary>
            Lightpack = 0,

            /// <summary>
            ///     Lightwear left
            /// </summary>
            LightwearLeft = 1,
        }

        private readonly ControllableIMU lightpackIMU = new ControllableIMU(Location.Lightpack);
        private readonly IMU lightwearIMU = new IMU(Location.LightwearLeft);

        /// <summary>
        ///     All the data returned from a query to an IMU
        /// </summary>
        public struct Data
        {
            public Sample[] Samples;

            /// <summary>
            ///     While IMU sampling is enabled, results are written to an internal buffer at the
            ///     rate defined in the MLIMU.Settings. If this buffer hasn't been queried recently
            ///     enough via <c> GetCurrentIMUData() </c>, the buffer will fill and samples will
            ///     be dropped in a FIFO manner. This property indicates the number of dropped
            ///     samples since the last time the IMU was queried. If you are polling every frame
            ///     and you see too many dropped samples, try reducing the sampling rate.
            /// </summary>
            public ulong NumDroppedSamples;

            public override string ToString()
            {
                string result = $"Dropped Samples: {NumDroppedSamples}\nSamples Found: {Samples.Length}\nFirst 3 Samples:\n";
                for (int i = 0; i < 3; i++)
                {
                    if (Samples.Length > i)
                        result += Samples[i] + "\n\n";
                    else
                        result += "NONE\n\n";
                }
                return result;
            }

            internal static Data Create(Sample[] samples, ulong numDroppedSamples) =>
                new Data()
                {
                    Samples = samples,
                    NumDroppedSamples = numDroppedSamples
                };
        }

        /// <summary>
        ///     An IMU that can be enabled/disabled and have its sampling rate adjusted
        /// </summary>
        public sealed partial class ControllableIMU : IMU
        {
            public bool IsEnabled
            {
                get => isEnabled;
                set
                {
                    if (value == isEnabled)
                        return;

                    if (value)
                        isEnabled = Enable(SampleRate);
                    else
                        isEnabled = !Disable();
                }
            }

            public SamplingRate SampleRate = SamplingRate._1000Hz;
            private bool isEnabled;

            internal ControllableIMU(Location loc) : base(loc) { }

            public override string ToString() =>
                "==" + Location.GetName(typeof(Location), Location) + "=="
                + $"\nEnabled: {IsEnabled}"
                + $"\nSample Rate: {SampleRate}"
                + $"\nCalibration: {IsCalibrated}"
                + $"\n{GetCurrentData()}";
        }

        public partial class IMU
        {
            /// <summary>
            ///     Returns the calibration status of this IMU.
            /// </summary>
            public bool IsCalibrated => GetIsCalibrated(Location);

            public readonly Location Location;

            internal IMU(Location loc)
            {
                Location = loc;
            }

            public Data GetCurrentData() => GetCurrentDataInternal();

            public override string ToString() =>
                "==" + Location.GetName(typeof(Location), Location) + "=="
                + $"\nCalibration: {IsCalibrated}"
                + $"\n{GetCurrentData()}";
        }

        /// <summary>
        ///     A single data point sampled from an IMU.
        /// </summary>
        public struct Sample
        {
            /// <summary>
            ///     Timestamp of the linear velocity measurement in ms
            /// </summary>
            public ulong LinearAccelerationTimestamp;

            /// <summary>
            ///     Timestamp of the rotational velocity measurement in ms
            /// </summary>
            public ulong RotationalVelocityTimestamp;

            /// <summary>
            ///     Linear acceleration, in m/s^2. Gravity is included.
            /// </summary>
            public Vector3 LinearAcceleration;

            /// <summary>
            ///     Rotational velocity in rad/s.
            /// </summary>
            public Vector3 RotationalVelocity;

            /// <summary>
            ///     Temperature in Celcius
            /// </summary>
            public float Temperature;

            /// <summary>
            ///     Which IMU on the device was this measurement was taken from?
            /// </summary>
            public Location IMULocation;

            public override string ToString() =>
                $"Linear Accel (timestamp): x={LinearAcceleration.x.ToString("F2")}, y={LinearAcceleration.y.ToString("F2")}, z={LinearAcceleration.z.ToString("F2")} ({LinearAccelerationTimestamp}ms)\n"
                + $"Rotational Vel (timestamp): x={RotationalVelocity.x.ToString("F2")}, y={RotationalVelocity.y.ToString("F2")}, z={RotationalVelocity.z.ToString("F2")} ({RotationalVelocityTimestamp}ms)\n"
                + $"Temperature: {Temperature.ToString("F2")}";
        }
#endif
    }

}
