// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLImageTrackerSettings.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// Supported formats when adding Image Targets to the Image Tracker.
    /// </summary>
    [Obsolete("Obsolete, please use MLImageTracker.ImageFormat instead.", false)]
    public enum MLImageTrackerImageFormat
    {
        /// <summary>
        /// Grayscale format.
        /// </summary>
        Grayscale,

        /// <summary>
        /// RGB format.
        /// </summary>
        RGB,

        /// <summary>
        /// RGBA format.
        /// </summary>
        RGBA
    }

    #if PLATFORM_LUMIN
    /// <summary>
    /// Represents the list of Image Tracker settings.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Obsolete("Deprecated, please use MLImageTracker.Settings instead.", true)]
    public struct MLImageTrackerSettings
    {
        /// <summary>
        /// Maximum number of Image Targets that can be tracked at any given time.
        /// If the tracker is already tracking the maximum number of targets
        /// possible then it will stop searching for new targets which helps
        /// reduce the load on the CPU. For example, if you are interested in
        /// tracking a maximum of x targets simultaneously from a list y (x &lt; y)
        /// targets then set this parameter to x.
        /// The valid range for this parameter is from 1 through 25.
        /// </summary>
        public uint MaxSimultaneousTargets;

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        /// <param name="maxTargets">The max number of targets to track at once.</param>
        /// <returns>An initialized version of this struct.</returns>
        public static MLImageTrackerSettings Create(uint maxTargets)
        {
            return new MLImageTrackerSettings
            {
                MaxSimultaneousTargets = maxTargets
            };
        }

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        /// <param name="copiedSettings">The settings to be copied.</param>
        /// <returns>An initialized version of this struct.</returns>
        public static MLImageTrackerSettings Create(MLImageTrackerNativeBindings.MLImageTrackerSettingsNative copiedSettings)
        {
            return new MLImageTrackerSettings
            {
                MaxSimultaneousTargets = copiedSettings.MaxSimultaneousTargets
            };
        }

        /// <summary>
        /// The equality check to be used for comparing two MLImageTrackerSettings structs.
        /// </summary>
        /// <param name="one">The first struct to compare with the second struct. </param>
        /// <param name="two">The second struct to compare with the first struct. </param>
        /// <returns>True if the two provided structs have the same number of MaxSimultaneousTargets.</returns>
        public static bool operator ==(MLImageTrackerSettings one, MLImageTrackerSettings two)
        {
            return one.MaxSimultaneousTargets == two.MaxSimultaneousTargets;
        }

        /// <summary>
        /// The inequality check to be used for comparing two MLImageTrackerSettings structs.
        /// </summary>
        /// <param name="one">The first struct to compare with the second struct. </param>
        /// <param name="two">The second struct to compare with the first struct. </param>
        /// <returns>True if the two provided structs do not have the same number of MaxSimultaneousTargets.</returns>
        public static bool operator !=(MLImageTrackerSettings one, MLImageTrackerSettings two)
        {
            return one.MaxSimultaneousTargets != two.MaxSimultaneousTargets;
        }

        /// <summary>
        /// The equality check to be used for comparing another object to this one.
        /// </summary>
        /// <param name="obj">The object to compare to this one with. </param>
        /// <returns>True if the the provided object is of the MLImageTrackerSettings type and has the same number of MaxSimultaneousTargets.</returns>
        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MLImageTrackerSettings p = (MLImageTrackerSettings)obj;
            return this.MaxSimultaneousTargets == p.MaxSimultaneousTargets;
        }

        /// <summary>
        /// Gets the hash code to use from MaxSimultaneousTargets.
        /// </summary>
        /// <returns>The hash code returned by MaxSimultaneousTargets.</returns>
        public override int GetHashCode()
        {
            return this.MaxSimultaneousTargets.GetHashCode();
        }
    }
    #endif
}
