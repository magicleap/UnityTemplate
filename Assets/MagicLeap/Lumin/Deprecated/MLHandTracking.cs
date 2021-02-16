// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLHandTracking.cs" company="Magic Leap, Inc">
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
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// Manages the input state for controllers, MCA and tablet devices.
    /// </summary>
    public partial class MLHandTracking
    {
        [System.Obsolete("MLHandTracking API is now automatically started and stopped. There is no need to call the Start() and Stop() methods for this API, and they are now deprecated. See https://developer.magicleap.com/learn/guides/auto-api-changes for more info.", false)]
        public static MLResult Start()
        {
            return MLResult.Create(MLResult.Code.Ok);
        }

        [System.Obsolete("MLHandTracking API is now automatically started and stopped. There is no need to call the Start() and Stop() methods for this API, and they are now deprecated. See https://developer.magicleap.com/learn/guides/auto-api-changes for more info.", false)]
        public static void Stop()
        {
        }
    }
}
#endif
