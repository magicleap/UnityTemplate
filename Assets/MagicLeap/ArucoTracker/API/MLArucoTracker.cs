// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLArucoTracker.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// This tracker is used to track square <c>fiducial</c> markers (also known as Augmented Reality Markers).
    /// It allows for rendering virtual content in relation to the location of so called <c>ArUco</c>
    /// markers that can be attached to flat surfaces. Each <c>ArUco</c> marker has a unique ID,
    /// so different markers can be tracked at the same time and are distinguishable as long as they
    /// belong to the same dictionary and have the same size.
    /// </summary>
    public sealed partial class MLArucoTracker : MLAutoAPISingleton<MLArucoTracker>
    {
        /// <summary>
        /// Map used to keep track of all the markers that have been tracked.
        /// </summary>
        private Dictionary<int, Marker> mapMarkers = new Dictionary<int, Marker>();

        /// <summary>
        /// The settings that the <c>ArUco</c> tracker will use.
        /// </summary>
        private Settings settings;

#if PLATFORM_LUMIN
        /// <summary>
        /// An event that's invoked when a marker's status has changed.
        /// </summary>
        public static event Marker.OnStatusChangeDelegate OnMarkerStatusChange
        {
            add => Instance.OnMarkerStatusChangeInternal += value;
            remove => Instance.OnMarkerStatusChangeInternal -= value;
        }
#endif

        /// <summary>
        /// An event that's invoked when a marker's status has changed.
        /// </summary>
        private event Marker.OnStatusChangeDelegate OnMarkerStatusChangeInternal = delegate { };

        /// <summary>
        /// Supported pre-defined dictionary names.
        /// <c>ArUco</c> Tracker supports pre-defined dictionary names.
        /// Some of these dictionaries can be looked up and markers can be generated for them here:
        /// <c>http://chev.me/arucogen/</c>
        /// </summary>
        public enum DictionaryName
        {
            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_4X4_50 = 0,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_4X4_100,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_4X4_250,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_4X4_1000,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_5X5_50,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_5X5_100,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_5X5_250,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_5X5_1000,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_6X6_50,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_6X6_100,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_6X6_250,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_6X6_1000,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_7X7_50,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_7X7_100,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_7X7_250,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_7X7_1000,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 1024 IDs.
            /// </summary>
            DICT_ARUCO_ORIGINAL
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Gets the tracker settings.
        /// </summary>
        public static MLArucoTracker.Settings TrackerSettings => Instance.settings;

        /// <summary>
        /// Attempts to get the marker with the provided id if it has been tracked.
        /// </summary>
        /// <param name="id">The id of the marker to look for.</param>
        /// <returns>The Marker object with the provided id if it's been tracked and null if hasn't been tracked yet.</returns>
        public static Marker GetMarker(int id)
        {
            if (Instance.mapMarkers.ContainsKey(id))
            {
                return Instance.mapMarkers[id];
            }

            return null;
        }

        /// <summary>
        /// Sets the <c>ArUco</c> tracker's settings to match the provided settings.
        /// </summary>
        /// <param name="newSettings">The new settings to update the tracker with.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully updated the <c>ArUco</c> tracker settings.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to update the settings due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if failed to update the settings due to lack of privilege(s).
        /// </returns>
        public static Task UpdateSettings(MLArucoTracker.Settings newSettings) => Instance.UpdateSettingsInternal(newSettings);

        /// <summary>
        /// Initializes the <c>ArUco</c> tracker.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            MLResult result = MLPrivileges.RequestPrivilege(MLPrivileges.Id.CameraCapture);
            if (result.Result != MLResult.Code.PrivilegeGranted)
            {
                return result.Result;
            }

            return NativeBindings.CreateTracker(this.settings);
        }

        /// <summary>
        /// Initializes the <c>ArUco</c> tracker.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult.Code StopAPI() => NativeBindings.MLArucoTrackerDestroy(Instance.Handle);

        /// <summary>
        /// Updates the map of markers and updates each detected marker.
        /// </summary>
        protected override void Update()
        {
            if(this.settings.Enabled)
            {
                _ = this.GetResultsAsync();
            }
        }

        /// <summary>
        /// Gets the results of the <c>ArUco</c> tracker on a worker thread and reports results back to the main thread.
        /// </summary>
        /// <returns>An async task.</returns>
        private async Task GetResultsAsync()
        {
            await Task.Run(() => NativeBindings.GetResults(out NativeBindings.MLArucoTrackerResultNative[] resultsArray));

            if(!this.settings.Enabled)
            {
                NativeBindings.MapTrackerResults.Clear();
                return;
            }

            //// Update existing markers
            foreach (Marker marker in this.mapMarkers.Values)
            {
                Marker.TrackingStatus previousStatus = marker.Status;
                if (NativeBindings.MapTrackerResults.ContainsKey(marker.Id))
                {
                    marker.UpdateMarker(Marker.TrackingStatus.Tracked, NativeBindings.MapTrackerResults[marker.Id].ReprojectionError);
                    if (previousStatus != marker.Status)
                    {
                        this.OnMarkerStatusChangeInternal(marker, marker.Status);
                    }

                    NativeBindings.MapTrackerResults.Remove(marker.Id);
                }
                else
                {
                    marker.UpdateMarker(Marker.TrackingStatus.NotTracked, 0f);
                    if (previousStatus != marker.Status)
                    {
                        this.OnMarkerStatusChangeInternal(marker, marker.Status);
                    }
                }
            }

            //// Add any new markers
            foreach (NativeBindings.MLArucoTrackerResultNative trackerResult in NativeBindings.MapTrackerResults.Values)
            {
                Marker newMarker = new Marker(trackerResult.Id, trackerResult.CFUID);
                this.mapMarkers.Add(newMarker.Id, newMarker);
                newMarker.UpdateMarker(Marker.TrackingStatus.Tracked, trackerResult.ReprojectionError);
                this.OnMarkerStatusChangeInternal(newMarker, newMarker.Status);
            }

            NativeBindings.MapTrackerResults.Clear();
        }

        /// <summary>
        /// Sets the <c>ArUco</c> tracker's settings to match the provided settings.
        /// </summary>
        /// <param name="newSettings">The new settings to update the tracker with.</param>
        /// <returns>A task that updates settings.</returns>
        private Task UpdateSettingsInternal(MLArucoTracker.Settings newSettings)
        {
            if (MLArucoTracker.DidNativeCallSucceed(NativeBindings.UpdateSettings(newSettings)))
            {
                this.settings = newSettings;
            }

            return Task.CompletedTask;
        }

#endif
    }
}
