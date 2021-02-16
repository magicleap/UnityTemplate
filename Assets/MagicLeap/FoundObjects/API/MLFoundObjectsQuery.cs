// %BANNER_BEGIN%
// ---------------------------------------------------------------------
//
// attention EXPERIMENTAL
//
// %COPYRIGHT_BEGIN%
// <copyright file="MLFoundObjectsQuery.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Manages calls to the native MLFoundObjects bindings.
    /// </summary>
    public sealed partial class MLFoundObjects
    {
        /// <summary>
        /// Helper class to store found object query data.
        /// </summary>
        public partial class Query
        {
            /// <summary>
            /// Gets the query results callback.
            /// </summary>
            public QueryResultsDelegate Callback
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the filter applied to the query.
            /// </summary>
            public Filter QueryFilter
            {
                get;
                private set;
            }

            /// <summary>
            /// Initializes a FoundObjects.Query class with the given values.
            /// </summary>
            /// <param name="callback">The callback that should be invoked.</param>
            /// <param name="queryFilter">The filter applied to the query.</param>
            /// <returns>A FoundObjects.Query class with the given values.</returns>
            public static Query Create(QueryResultsDelegate callback, Filter queryFilter)
            {
                Query q = new Query();
                q.Callback = callback;
                q.QueryFilter = queryFilter;
                return q;
            }
        }
    }
}
