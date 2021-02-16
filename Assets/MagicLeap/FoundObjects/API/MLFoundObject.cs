// %BANNER_BEGIN%
// ---------------------------------------------------------------------
//
// attention EXPERIMENTAL
//
// %COPYRIGHT_BEGIN%
// <copyright file="MLFoundObject.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
namespace UnityEngine.XR.MagicLeap
{
    using System;

    /// <summary>
    /// Manages calls to the native MLFoundObjects bindings.
    /// </summary>
    public partial class MLFoundObjects
    {
        /// <summary>
        /// Contains information about the found object.
        /// </summary>
        public struct FoundObject
        {
            /// <summary>
            /// Gets the identifier of the found object.
            /// </summary>
            public Guid Id
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the center position of found object.
            /// </summary>
            public Vector3 Position
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the rotation of found object.
            /// </summary>
            public Quaternion Rotation
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the Vector3 extents of the object where each dimension is defined as max-min.
            /// </summary>
            public Vector3 Size
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the label of the object, at the moment can be one of the following: ["couch", "chair", "table", "poster", "screen"].
            /// </summary>
            public string Label
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets value between [0,1] with 1 indicating high confidence.
            /// </summary>
            public float Confidence
            {
                get;
                internal set;
            }
        }
    }
}
