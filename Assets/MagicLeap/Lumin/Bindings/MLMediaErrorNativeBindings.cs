// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLMediaErrorNativeBindings.cs" company="Magic Leap">
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

    /// <summary>
    /// See ml_media_error.h for additional comments.
    /// </summary>
    public class MLMediaErrorNativeBindings : MagicLeapNativeBindings
    {
        /// <summary>
        /// MLMediaError library name
        /// </summary>
        private const string MLMediaErrorDLL = "ml_mediaerror";

        /// <summary>
        /// Gets a readable version of the result code as an ASCII string.
        /// </summary>
        /// <param name="result">The MLResult.Code that should be converted.</param>
        /// <returns>ASCII string containing a readable version of the result code.</returns>
        [DllImport(MLMediaErrorDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MLMediaResultGetString(MLResult.Code result);
    }
}

#endif
