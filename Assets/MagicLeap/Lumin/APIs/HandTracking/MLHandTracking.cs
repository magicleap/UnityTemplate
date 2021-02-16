// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLHandTracking.cs" company="Magic Leap, Inc">
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

    /// <summary>
    /// MLHandTracking is the entry point for all the hand tracking data
    /// including gestures, hand centers and key points for both hands.
    /// </summary>
    public partial class MLHandTracking : MLAutoAPISingleton<MLHandTracking>
    {
#if PLATFORM_LUMIN
        /// <summary>
        /// The object to manage when different key poses start and end.
        /// </summary>
        private KeyposeManager keyposeManager;

        /// <summary>
        /// Right hand class used to get right hand specific data.
        /// </summary>
        private Hand right;

        /// <summary>
        /// Left hand class used to get left hand specific data.
        /// </summary>
        private Hand left;

        /// <summary>
        /// List for left hand devices.
        /// </summary>
        private List<InputDevice> leftHandDevices = new List<InputDevice>();

        /// <summary>
        /// List for right hand devices.
        /// </summary>
        private List<InputDevice> rightHandDevices = new List<InputDevice>();
#endif

        /// <summary>
        /// Static key pose types which are available when both hands are separated.
        /// </summary>
        public enum HandKeyPose
        {
            /// <summary>
            /// Index finger.
            /// </summary>
            Finger,

            /// <summary>A
            /// A closed fist.
            /// </summary>
            Fist,

            /// <summary>
            /// A pinch.
            /// </summary>
            Pinch,

            /// <summary>
            /// A closed fist with the thumb pointed up.
            /// </summary>
            Thumb,

            /// <summary>
            /// An L shape
            /// </summary>
            L,

            /// <summary>
            /// An open hand.
            /// </summary>
            OpenHand = 5,

            /// <summary>
            /// A pinch with all fingers, except the index finger and the thumb, extended out.
            /// </summary>
            Ok,

            /// <summary>
            /// A rounded 'C' alphabet shape.
            /// </summary>
            C,

            /// <summary>
            /// No pose was recognized.
            /// </summary>
            NoPose,

            /// <summary>
            /// No hand was detected. Should be the last pose.
            /// </summary>
            NoHand
        }

        /// <summary>
        /// Configured level for key points filtering of key points and hand centers.
        /// </summary>
        public enum KeyPointFilterLevel
        {
            /// <summary>
            /// Default value, no filtering is done, the points are raw.
            /// </summary>
            Raw,

            /// <summary>
            /// Some smoothing at the cost of latency.
            /// </summary>
            Smoothed,

            /// <summary>
            /// Predictive smoothing, at higher cost of latency.
            /// </summary>
            ExtraSmoothed
        }

        /// <summary>
        /// Configured level of filtering for static poses.
        /// </summary>
        public enum PoseFilterLevel
        {
            /// <summary>
            /// Default value, no filtering, the poses are raw.
            /// </summary>
            Raw,

            /// <summary>
            /// Some robustness to flicker at some cost of latency.
            /// </summary>
            Robust,

            /// <summary>
            /// More robust to flicker at higher latency cost.
            /// </summary>
            ExtraRobust
        }

        /// <summary>
        /// Represents if a hand is the right or left hand.
        /// </summary>
        public enum HandType
        {
            /// <summary>
            /// Left hand.
            /// </summary>
            Left,

            /// <summary>
            /// Right hand.
            /// </summary>
            Right
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Gets the key pose manager of the instance.
        /// </summary>
        public static KeyposeManager KeyPoseManager => Instance.keyposeManager;

        /// <summary>
        /// Gets the left hand.
        /// </summary>
        public static Hand Left => Instance.left;

        /// <summary>
        /// Gets the right hand.
        /// </summary>
        public static Hand Right => Instance.right;


        #if !DOXYGENSHOULDSKIPTHIS
        /// <summary>
        /// Start tracking hands with all key poses disabled.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to initialize the native hand tracker.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            this.left = new Hand(MLHandTracking.HandType.Left);
            this.right = new Hand(MLHandTracking.HandType.Right);

            // Initialize KeyPoseManager, to register the gesture subsystem.
            this.keyposeManager = new KeyposeManager(Left, Right);

            try
            {
                // Attempt to start the tracker & validate.
                NativeBindings.SetHandGesturesEnabled(true);
                if (!NativeBindings.IsHandGesturesEnabled())
                {
                    MLPluginLog.Error("MLHandTracking.StartAPI failed to initialize the native hand tracker.");
                    return MLResult.Code.UnspecifiedFailure;
                }
            }
            catch (EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLHandTracking.StartAPI failed. Reason: API symbols not found.");
                return MLResult.Code.UnspecifiedFailure;
            }

            return MLResult.Code.Ok;
        }

        /// <summary>
        /// Cleans up API and unmanaged memory.
        /// </summary>
        protected override MLResult.Code StopAPI()
        {
            // The KeyPoseManager object will not receive any more updates from Left or Right hands.
            this.keyposeManager.Dispose();
            this.keyposeManager = null;
            this.left = null;
            this.right = null;
            NativeBindings.SetHandGesturesEnabled(false);
            return MLResult.Code.Ok;
        }
        #endif // DOXYGENSHOULDSKIPTHIS

        /// <summary>
        /// Updates the key pose state based on the provided snapshot.
        /// </summary>
        protected override void Update()
        {
            this.leftHandDevices.Clear();

            #if UNITY_2019_3_OR_NEWER
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Left, this.leftHandDevices);
            #else
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, this.leftHandDevices);
            #endif

            if (this.leftHandDevices.Count > 0 && this.leftHandDevices[0].isValid)
            {
                this.left.Update(this.leftHandDevices[0]);
            }

            this.rightHandDevices.Clear();

            #if UNITY_2019_3_OR_NEWER
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Right, this.rightHandDevices);
            #else
            InputDevices.GetDevicesAtXRNode(XRNode.RightHand, this.rightHandDevices);
            #endif

            if (this.rightHandDevices.Count > 0 && this.rightHandDevices[0].isValid)
            {
                this.right.Update(this.rightHandDevices[0]);
            }
        }
        #endif
    }
}
