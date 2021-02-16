// %BANNER_BEGIN%
// ---------------------------------------------------------------------
//
// attention EXPERIMENTAL
//
// %COPYRIGHT_BEGIN%
// <copyright file="MLFoundObjectsSettings.cs" company="Magic Leap, Inc">
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
        /// Struct representing the settings of the MLFoundObjects tracker.
        /// </summary>
        [Serializable]
        public struct Settings
        {
            /// <summary>
            /// The maximum results(objects) that the query should return.
            /// </summary>
            [SerializeField, Range(0, MLFoundObjects.MaxQueryResult), Tooltip("The max results(objects) that the query should return.")]
            private uint maxQueryResult;

            /// <summary>
            /// The max objects that the API should store.
            /// </summary>
            [SerializeField, Range(0, MLFoundObjects.MaxObjectCache), Tooltip("The maximum number of found objects to be stored by the API.")]
            private uint maxObjectCache;

            /// <summary>
            /// Gets the maximum results(objects) that the query should return.
            /// </summary>
            public int MaxQueryResult { get => (int)this.maxQueryResult; }

            /// <summary>
            /// Gets the maximum number of found objects to be stored.
            /// </summary>
            public int MaxObjectCache { get => (int)this.maxObjectCache; }

            /// <summary>
            /// Initializes a MLFoundObjects.Settings struct with the provided values.
            /// </summary>
            /// <param name="maxQueryResult">The max results the query should return.</param>
            /// <param name="maxObjectCache">The max objects the API should store.</param>
            /// <returns>A MLFoundObjects.Settings struct with the provided values.</returns>
            public static Settings Create(int maxQueryResult = MLFoundObjects.MaxQueryResult, int maxObjectCache = MLFoundObjects.MaxObjectCache)
            {
                Settings settings = new Settings();
                settings.maxQueryResult = (uint)maxQueryResult;
                settings.maxObjectCache = (uint)maxObjectCache;
                return settings;
            }
        }
    }
}
