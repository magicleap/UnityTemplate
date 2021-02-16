// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLArucoTrackerNativeBindings.cs" company="Magic Leap">
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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLArucoTracker manages the Aruco Tracker system.
    /// Arudo Tracker enables experiences that recognize Aruco images (image targets)
    /// in the physical world. It provides the position and
    /// orientation of the aruco image targets in the physical world.
    /// </summary>
    public sealed partial class MLArucoTracker
    {
        private class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// The map of native results, updated every time MLArucoTracker.Update() is called.
            /// </summary>
            public static Dictionary<int, MLArucoTrackerResultNative> MapTrackerResults = new Dictionary<int, MLArucoTrackerResultNative>();

            /// <summary>
            /// Helper method that the MLArucoTracker API uses to create the tracker's internal handle.
            /// </summary>
            /// <param name="settings">The settings to configure the tracker with.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully created ArUco tracker.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to create ArUco tracker due to invalid handle.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if failed to create tracker due to lack of privilege(s).
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to create the ArUco tracker due to an internal error.
            /// </returns>
            public static MLResult.Code CreateTracker(MLArucoTracker.Settings settings)
            {
                MLArucoTrackerSettingsNative nativeSettings = MLArucoTrackerSettingsNative.Create();
                nativeSettings.Data = settings;
                MLResult.Code resultCode = NativeBindings.MLArucoTrackerCreate(in nativeSettings, out ulong trackerHandle);
                Instance.Handle = trackerHandle;
                return resultCode;
            }

            /// <summary>
            /// Helper method that the MLArucoTracker API uses to update the tracker's settings.
            /// </summary>
            /// <param name="settings">The settings to update the tracker with.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully updated the ArUco Tracker settings.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to update the settings due to invalid settings.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if failed to update the settings due to lack of privilege(s).
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to update the settings due to an internal error.
            /// </returns>
            public static MLResult.Code UpdateSettings(MLArucoTracker.Settings settings)
            {
                MLArucoTrackerSettingsNative nativeSettings = MLArucoTrackerSettingsNative.Create();
                nativeSettings.Data = settings;
                MLResult.Code resultCode = NativeBindings.MLArucoTrackerUpdateSettings(Instance.Handle, in nativeSettings);
                return resultCode;
            }

            /// <summary>
            /// Helper method that the MLArucoTracker API uses to marshal the results recieved from the device.
            /// Results are released after being queried for.
            /// </summary>
            /// <param name="trackerResults">The array of marker results found by the device.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully fetched and returned all detections.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to return detection data due to an invalid resultArray.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to return detections due to an internal error.
            /// </returns>
            public static MLResult.Code GetResults(out MLArucoTrackerResultNative[] trackerResultsArray)
            {
                MLResult.Code resultCode = (NativeBindings.MLArucoTrackerGetResult(Instance.Handle, out MLArucoTrackerResultArrayNative resultsArray));
                if (!MLArucoTracker.DidNativeCallSucceed(resultCode))
                {
                    trackerResultsArray = new MLArucoTrackerResultNative[0];
                    return resultCode;
                }

                trackerResultsArray = new MLArucoTrackerResultNative[resultsArray.Count];

                for (int i = 0; i < resultsArray.Count; i++)
                {
                    IntPtr offsetPtr = Marshal.ReadIntPtr(new IntPtr(resultsArray.Detections.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)));
                    MLArucoTrackerResultNative trackerResult = (MLArucoTrackerResultNative)Marshal.PtrToStructure(offsetPtr, typeof(MLArucoTrackerResultNative));
                    trackerResultsArray[i] = trackerResult;
                }

                resultCode = NativeBindings.MLArucoTrackerReleaseResult(ref resultsArray);

                foreach (NativeBindings.MLArucoTrackerResultNative trackerResult in trackerResultsArray)
                {
                    if (!NativeBindings.MapTrackerResults.ContainsKey(trackerResult.Id))
                    {
                        NativeBindings.MapTrackerResults.Add(trackerResult.Id, trackerResult);
                    }
                }

                if (!MLArucoTracker.DidNativeCallSucceed(resultCode))
                {
                    return resultCode;
                }

                return resultCode;
            }

            /// <summary>
            /// Creates an ArUco tracker.
            /// </summary>
            /// <param name="settings">The list of settings to configure the tracker with.</param>
            /// <param name="handle">A pointer to an MLHandle to the newly created ArUco Tracker. If this operation fails, handle will be ML_INVALID_HAND</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully created ArUco tracker.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to create ArUco tracker due to invalid handle.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if failed to create tracker due to lack of privilege(s).
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to create the ArUco tracker due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLArucoTrackerCreate(in MLArucoTrackerSettingsNative settings, out ulong handle);

            /// <summary>
            /// Updates the ArUco Tracker with new settings.
            /// </summary>
            /// <param name="handle">The handle to the ArUco tracker created by MLArucoTrackerCreate().</param>
            /// <param name="settings">The list of new ArUco tracker settings to use.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully updated the ArUco Tracker settings.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to update the settings due to invalid settings.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if failed to update the settings due to lack of privilege(s).
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to update the settings due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLArucoTrackerUpdateSettings(ulong handle, in MLArucoTrackerSettingsNative settings);

            /// <summary>
            /// Destroys an ArUco tracker.
            /// </summary>
            /// <param name="handle">The handle to the ArUco tracker created by MLArucoTrackerCreate().</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully destroyed the ArUco tracker.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to destroy the tracker due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLArucoTrackerDestroy(ulong handle);

            /// <summary>
            /// Gets the results for Aurco Tracking.
            /// </summary>
            /// <param name="handle">The handle to the ArUco Tracker created by MLArucoTrackerCreate().</param>
            /// <param name="resultArray">The array of detections to be freed.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully fetched and returned all detections.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to return detection data due to an invalid resultArray.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to return detections due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLArucoTrackerGetResult(ulong handle, out MLArucoTrackerResultArrayNative resultArray);

            /// <summary>
            /// Releases the resources for the results array.
            /// </summary>
            /// <param name="resultArray">The array of detections to be freed.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully freed data structure.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to free structure due to invalid data.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to free data due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLArucoTrackerReleaseResult(ref MLArucoTrackerResultArrayNative resultArray);

            /// <summary>
            /// The native structure that holds Image Tracker settings.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLArucoTrackerSettingsNative
            {
                /// <summary>
                /// Version of this struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Dictionary from which markers shall be tracked.
                /// </summary>
                public MLArucoTracker.DictionaryName Dictionary;

                /// <summary>
                /// The length of the markers that shall be tracked.
                /// The marker length is important to know, because once a marker is detected we can only determine
                /// its 3D position if we know how large it is in real life.
                /// The length of a marker is given in meters and represents the distance between the four dominant
                /// corners of the squared marker.
                /// </summary>
                public float MarkerLength;

                /// <summary>
                /// ArUco tracker will detect and track markers.
                /// ArUco Tracker should be disabled when app is paused and enabled when app resumes.
                /// When enabled, ArUco Tracker will gain access to the camera and start tracking images.
                /// When disabled ArUco Tracker will release the camera and stop tracking markers.
                /// Internal state of the tracker will be maintained.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public bool EnableMarkerTracking;

                /// <summary>
                /// Sets the native structures from the user facing properties.
                /// </summary>
                public MLArucoTracker.Settings Data
                {
                    get
                    {
                        return MLArucoTracker.Settings.Create(this.Dictionary, this.MarkerLength, this.EnableMarkerTracking);
                    }

                    set
                    {
                        this.Dictionary = value.Dictionary;
                        this.MarkerLength = value.MarkerLength;
                        this.EnableMarkerTracking = value.Enabled;
                    }
                }

                /// <summary>
                /// Initializes default values for QueryFilterNative.
                /// </summary>
                /// <returns> An initialized version of this struct.</returns>
                public static MLArucoTrackerSettingsNative Create()
                {
                    return new MLArucoTrackerSettingsNative()
                    {
                        Version = 1u,
                        Dictionary = MLArucoTracker.DictionaryName.DICT_5X5_250,
                        MarkerLength = 0.1f,
                        EnableMarkerTracking = true
                    };
                }
            }

            /// <summary>
            /// Represents the result for a single marker.
            /// A list of these detections will be returned by the ArUco Tracker, after processing a video frame succesfully.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLArucoTrackerResultNative
            {
                /// <summary>
                /// Unique marker ID as defined in selected MLArucoDictionaryName.
                /// </summary>
                public int Id;

                /// <summary>
                /// MLCoordinateFrameUID of the marker.
                /// This should be passed to the MLSnapshotGetTransform() function to get the 6 DOF pose of the marker.
                /// </summary>
                public MLCoordinateFrameUID CFUID;

                /// <summary>
                /// The reprojection error of this marker detection in degrees.
                /// A high reprojection error means that the estimated pose of the marker doesn't match well with
                /// the 2D detection on the processed video frame and thus the pose might be inaccurate.The error
                /// is given in degrees, signifying by how much either camera or marker would have to be moved or
                /// rotated to create a perfect reprojection.
                /// </summary>
                public float ReprojectionError;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLArucoTrackerResultArrayNative
            {
                /// <summary>
                /// Pointer to an array of pointers for MLArucoTrackerResult.
                /// </summary>
                public IntPtr Detections;

                /// <summary>
                /// Number of markers being tracked.
                /// </summary>
                public uint Count;
            }
        }
    }
}
#endif
