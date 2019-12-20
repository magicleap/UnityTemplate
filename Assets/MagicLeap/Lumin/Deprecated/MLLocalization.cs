// %BANNERBEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHTBEGIN%
// <copyright file="MLLocalization.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHTEND%
// ---------------------------------------------------------------------
// %BANNEREND%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;

    /// <summary>
    /// MLLocalization provides the Language and Country set at the system level.
    /// </summary>
    [Obsolete("Use MLLocale instead.")]
    public sealed class MLLocalization : MLLocale
    {
    }
}

#endif
