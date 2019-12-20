// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLImageTrackerNativeBindings.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap.Native
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap;

    /// <summary>
    /// The native bindings to the Image Tracking API.
    /// See ml_image_tracking.h for additional comments
    /// </summary>
    public partial class MLImageTrackerNativeBindings : MagicLeapNativeBindings
    {
        /// <summary>
        /// Creates and adds a new target to the image tracker from an array.
        /// Supported formats: Grayscale, RGB, and RGBA, should be specified using the MLImageTracker.ImageFormat enum.
        /// </summary>
        /// <param name="trackerHandle">Handle to the created Image Tracker created by MLImageTrackerCreate().</param>
        /// <param name="targetSettings">List of the settings to be used for the new target.</param>
        /// <param name="imageData">Pointer to the array of pixel data.</param>
        /// <param name="width">Width of the image.</param>
        /// <param name="height">Height of the image.</param>
        /// <param name="format"> Specifies the image format.</param>
        /// <param name="targetHandle">A handle to the created Image Target.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the target was added to the Image Tracker successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if there were any invalid arguments or if there was an internal error.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        [Obsolete("Obsolete, please use MLImageTrackerAddTargetFromArray with MLImageTracker.Target.Settings and MLImageTracker.ImageFormat instead.", true)]
        public static extern MLResult.Code MLImageTrackerAddTargetFromArray(ulong trackerHandle, ref MLImageTrackerTargetSettings targetSettings, [MarshalAs(UnmanagedType.LPArray)] byte[] imageData, uint width, uint height, MLImageTrackerImageFormat format, ref ulong targetHandle);

        /// <summary>
        /// Updates the settings of an Image Target that is already added to the Image Tracker system.
        /// </summary>
        /// <param name="trackerHandle">Handle to the created Image Tracker created by MLImageTrackerCreate().</param>
        /// <param name="targetHandle">Handle to the Image Target whose settings needs to be updated.</param>
        /// <param name="targetSettings">List of the updated settings to be used for the new target.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if there were invalid targetSettings
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the target's settings were updated successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if there was an internal error.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        [Obsolete("Obsolete, please yse MLImageTrackerAddTargetFromArray with MLImageTracker.Target.Settings instead.", true)]
        public static extern MLResult.Code MLImageTrackerUpdateTargetSettings(ulong trackerHandle, ulong targetHandle, ref MLImageTrackerTargetSettings targetSettings);
    }
}

#endif
