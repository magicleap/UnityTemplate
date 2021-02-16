// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLEyes.cs" company="Magic Leap, Inc">
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

    /// <summary>
    /// Functionality to validate or query privileges from the system.
    /// </summary>
    public sealed partial class MLEyes
    {
        [System.Obsolete("MLEyes API is now automatically started and stopped. There is no need to call the Start() and Stop() methods for this API, and they are now deprecated. See https://developer.magicleap.com/learn/guides/auto-api-changes for more info.", false)]
        public static MLResult Start()
        {
            return MLResult.Create(MLResult.Code.Ok);
        }

        [System.Obsolete("MLEyes API is now automatically started and stopped. There is no need to call the Start() and Stop() methods for this API, and they are now deprecated. See https://developer.magicleap.com/learn/guides/auto-api-changes for more info.", false)]
        public static void Stop()
        {
        }
    }
}

#endif
