// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLArucoTrackerMarker.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
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
        /// Represents a marker that is tracked by the <c>ArUco</c> tracker.
        /// Markers have an id and pose.
        /// </summary>
        public class Marker
        {
            /// <summary>
            /// The id of the marker.
            /// </summary>
            private int id = -1;

            /// <summary>
            /// Indicates whether the marker is currently detected by the tracker or not.
            /// A marker that has a status of NotTracked may contian unreliable pose and reprojection error data.
            /// </summary>
            private TrackingStatus status = TrackingStatus.NotTracked;

            /// <summary>
            /// The pose of the marker that contains position and rotation data.
            /// </summary>
            private Pose pose = new Pose();

            /// <summary>
            /// The reprojection error of this marker detection in degrees.
            /// A high reprojection error means that the estimated pose of the marker doesn't match well with
            /// the 2D detection on the processed video frame and thus the pose might be inaccurate.The error
            /// is given in degrees, signifying by how much either camera or marker would have to be moved or
            /// rotated to create a perfect reprojection.
            /// </summary>
            private float reprojectionError = 0;

            #if PLATFORM_LUMIN
            /// <summary>
            /// The CFUID of the marker that's used to query the pose with.
            /// </summary>
            private MagicLeapNativeBindings.MLCoordinateFrameUID cfuid;
            #endif

            /// <summary>
            /// Handle used for subscribing to the OnStatusChangeDelegate event.
            /// </summary>
            /// <param name="marker">The reference to the marker that has changed.</param>
            /// <param name="status">The current status of the marker.</param>
            public delegate void OnStatusChangeDelegate(Marker marker, Marker.TrackingStatus status);

            /// <summary>
            /// An event that's invoked when a marker has been found or lost.
            /// </summary>
            public event OnStatusChangeDelegate OnStatusChange = delegate { };

            #if PLATFORM_LUMIN
            /// <summary>
            /// Initializes a new instance of the <see cref="MLArucoTracker.Marker" /> class.
            /// </summary>
            /// <param name="id">The id of this marker.</param>
            /// <param name="cfuid">The CFUID associated with this marker.</param>
            public Marker(int id, MagicLeapNativeBindings.MLCoordinateFrameUID cfuid)
            {
                this.id = id;
                this.cfuid = cfuid;
            }
            #endif

            /// <summary>
            /// Identifies the tracking status of the marker.
            /// </summary>
            public enum TrackingStatus
            {
                /// <summary>
                /// The marker is tracked and can have it's pose found.
                /// </summary>
                Tracked,

                /// <summary>
                /// The marker is not tracked and can not have it's pose found, it's current pose data may be inaccurate.
                /// </summary>
                NotTracked
            }

            /// <summary>
            /// Gets the id of the marker.
            /// </summary>
            public int Id
            {
                get
                {
                    return this.id;
                }
            }


            /// <summary>
            /// Gets the tracking status of the marker.
            /// </summary>
            public TrackingStatus Status
            {
                get
                {
                    return this.status;
                }
            }

            /// <summary>
            /// Gets the position of the marker.
            /// </summary>
            public Vector3 Position
            {
                get
                {
                    return this.pose.position;
                }
            }

            /// <summary>
            /// Gets the rotation of the marker.
            /// </summary>
            public Quaternion Rotation
            {
                get
                {
                    return this.pose.rotation;
                }
            }

            /// <summary>
            /// Gets the reprojection error of the marker.
            /// </summary>
            public float ReprojectionError
            {
                get
                {
                    return this.reprojectionError;
                }
            }

            /// <summary>
            /// String representation of the marker.
            /// </summary>
            /// <returns>A string representation of the marker. </returns>
            /// 
#if PLATFORM_LUMIN
            public override string ToString() => $"Id: {this.id},  Status: {this.status}, Pose: {this.pose}, ReprojectionError: {this.reprojectionError}, CFUID: {this.cfuid}";
#endif

            /// <summary>
            /// Sets the marker's status, reprojection error, and updates it's pose data.
            /// </summary>
            internal void UpdateMarker(TrackingStatus status, float reprojectionError)
            {
#if PLATFORM_LUMIN
                TrackingStatus previousStatus = this.Status;

                this.status = status;
                this.reprojectionError = reprojectionError;

                if (this.Status == TrackingStatus.Tracked)
                {
                    MagicLeapNativeBindings.UnityMagicLeap_TryGetPose(this.cfuid, out this.pose);
                }

                if (previousStatus != this.Status)
                {
                    OnStatusChange(this, this.Status);
                }
#endif
            }
        }
    }
}
