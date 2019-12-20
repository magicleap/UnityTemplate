// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPCF.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using UnityEngine;
    using System;

    ///<summary>
    /// MLPCF class is an abstraction for representing anchor points, called Persistent
    /// Coordinate Frames or PCFs, in the real world. PCFs cannot be created, modified or
    /// destroyed from the app level. Rather, we query the OS for any PCFs it has stored
    /// and query again to determine the PCF location, if it is within the vicinity.
    ///</summary>
    [Serializable]
    [Obsolete("Please use MLPersistentCoordinateFrames.PCF instead.", false)]
    public class MLPCF : MLPersistentCoordinateFrames.PCF
    {
        [Obsolete("Please use Rotation instead.", false)]
        public Quaternion Orientation
        {
            get
            {
                return Rotation;
            }
        }

        #pragma warning disable 67
        [Obsolete("No longer used, please use MLPersistentCoordinateFrames.PCF.OnStatusChange instead.", true)]
        public static event System.Action<MLPCF> OnCreate;

        [Obsolete("No longer used, please use MLPersistentCoordinateFrames.PCF.OnStatusChange or MLPersistentCoordinateFrames.PCF.IBinding instead.", true)]
        public event System.Action OnUpdate;

        [Obsolete("No longer used, please use MLPersistentCoordinateFrames.PCF.OnStatusChange or MLPersistentCoordinateFrames.PCF.IBinding instead.", true)]
        public event System.Action OnLost;

        [Obsolete("No longer used, please use MLPersistentCoordinateFrames.PCF.OnStatusChange or MLPersistentCoordinateFrames.PCF.IBinding instead.", true)]
        public event System.Action OnRegain;
        #pragma warning restore 67
    }
}

#endif
