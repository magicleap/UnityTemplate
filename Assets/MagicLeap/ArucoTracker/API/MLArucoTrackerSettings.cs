// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLArucoTrackerSettings.cs" company="Magic Leap">
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

#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// This tracker is used to track square <c>fiducial</c> markers (also known as Augmented Reality Markers).
    /// It allows for rendering virtual content in relation to the location of so called <c>ArUco</c>
    /// markers that can be attached to flat surfaces. Each <c>ArUco</c> marker has a unique ID,
    /// so different markers can be tracked at the same time and are distinguishable as long as they
    /// belong to the same dictionary and have the same size.
    /// </summary>
    public partial class MLArucoTracker
    {
        /// <summary>
        /// The settings used to configure the <c>ArUco</c> tracker.
        /// When creating the <c>ArUco</c> tracker, this list of settings needs to be passed to configure the tracker
        /// properly. The tracker will only output detections of markers that belong to the defined dictionary
        /// and the estimated poses will only be correct if the marker length has been set correctly.
        /// </summary>
        [Serializable]
        public struct Settings
        {
            /// <summary>
            /// Dictionary from which markers shall be tracked.
            /// </summary>
            public DictionaryName Dictionary;

            /// <summary>
            /// The length of the markers that shall be tracked.
            /// The marker length is important to know, because once a marker is detected we can only determine
            /// its 3D position if we know how large it is in real life.
            /// The length of a marker is given in meters and represents the distance between the four dominant
            /// corners of the squared marker.
            /// </summary>
            [Tooltip("The marker length in meters.")]
            public float MarkerLength;

            /// <summary>
            /// <c>ArUco</c> tracker will detect and track markers.
            /// <c>ArUco</c> tracker should be disabled when app is paused and enabled when app resumes.
            /// When enabled, <c>ArUco</c> tracker will gain access to the camera and start tracking images.
            /// When disabled <c>ArUco</c> tracker will release the camera and stop tracking markers.
            /// Internal state of the tracker will be maintained.
            /// </summary>
            public bool Enabled;

            /// <summary>
            /// Creates and returns an initialized version of this struct.
            /// </summary>
            /// <param name="dictionary">The dictionary to use to determine which markers will be tracked.</param>
            /// <param name="markerLength">The length of the markers to be tracked.</param>
            /// <param name="enabled">Determines if the tracker should currently be enabled or disabled.</param>
            /// <returns>An initialized version of this struct.</returns>
            public static Settings Create(DictionaryName dictionary = DictionaryName.DICT_4X4_50, float markerLength = 0.1f, bool enabled = true)
            {
                return new Settings
                {
                    Dictionary = dictionary,
                    MarkerLength = markerLength,
                    Enabled = enabled
                };
            }
        }
    }
}
