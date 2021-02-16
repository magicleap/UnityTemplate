// %BANNER_BEGIN%
// ---------------------------------------------------------------------
//
// attention EXPERIMENTAL
//
// %COPYRIGHT_BEGIN%
// <copyright file="MLFoundObjectsQueryFilter.cs" company="Magic Leap, Inc">
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
    public sealed partial class MLFoundObjects
    {
        /// <summary>
        /// Helper structure to store found object query data.
        /// </summary>
        public partial class Query
        {
            /// <summary>
            /// Struct used to compose a query and customize it's results.
            /// </summary>
            [Serializable]
            public struct Filter
            {
                /// <summary>
                /// Filter a query by object label.
                /// </summary>
                [SerializeField, Tooltip("Filter by object label, an empty string will disable this filter.")]
                private string label;

                /// <summary>
                /// Filter a query by confidence.
                /// </summary>
                [SerializeField, Range(0, 1), Tooltip("Filter by confidence, a value of 0 will disable this filter.")]
                private float confidence;

                /// <summary>
                /// Vector3 of where you want the spatial query to originate.
                /// </summary>
                [SerializeField, Tooltip("Vector3 of where you want the spatial query to originate.")]
                private Vector3 center;

                /// <summary>
                /// Vector3 of the max distance you want the spatial query to span relative to the center of the query.
                /// </summary>
                [SerializeField, Tooltip("Vector3 extents of the object where each dimension is defined as max-min.")]
                private Vector3 maxDistance;

                /// <summary>
                /// Maximum number of results. Used to allocate memory.
                /// </summary>
                [SerializeField, Range(0, MLFoundObjects.MaxQueryResult), Tooltip("THe maximum number of results(objects) that the query should return. A value of 0 is the same as the maximum amount.")]
                private int maxResults;

                /// <summary>
                /// Gets the label to filter the query with.
                /// </summary>
                public string Label { get => this.label; }

                /// <summary>
                /// Gets the confidence to filter the query with.
                /// </summary>
                public float Confidence { get => this.confidence; }

                /// <summary>
                /// Gets the center to filter the query with.
                /// </summary>
                public Vector3 Center { get => this.center; }

                /// <summary>
                /// Gets the max distance from the center to filter the query with.
                /// </summary>
                public Vector3 MaxDistance { get => this.maxDistance; }

                /// <summary>
                /// Gets the maximum results the query should return.
                /// </summary>
                public int MaxResults { get => this.maxResults; }

                /// <summary>
                /// Initializes a FoundObjects.Query.Filter struct with the given values.
                /// </summary>
                /// <param name="label">The label to filter the query with..</param>
                /// <param name="confidence">The confidence to filter the query with.</param>
                /// <param name="center">The center to filter the query with.</param>
                /// <param name="maxDistance">The max distance from the center to filter the query with.</param>
                /// <param name="maxResults">The max results the query should return.</param>
                /// <returns>A FoundObjects.Query.Filter struct with the given values.</returns>
                public static Filter Create(string label = "", float confidence = 0f, Vector3 center = default, Vector3 maxDistance = default, int maxResults = int.MaxValue)
                {
                    Filter filter = new Filter();
                    filter.label = label;
                    filter.confidence = confidence;
                    filter.center = center;
                    filter.maxDistance = maxDistance;
                    filter.maxResults = maxResults;
                    return filter;
                }
            }
        }
    }
}
