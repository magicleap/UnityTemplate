// %BANNERBEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHTBEGIN%
// <copyright file="MLHandMeshing.cs" company="Magic Leap">
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
    /// The MLHandMeshing API is used to request for the mesh information of the hands.
    /// </summary>
    public sealed partial class MLHandMeshing : MLAPISingleton<MLHandMeshing>
    {
        /// <summary>
        /// Hand Mesh Request Callback delegate.
        /// </summary>
        /// <param name="result">Result of request.</param>
        /// <param name="meshData">Mesh Data of the request if result is ok. Otherwise, meshData.MeshBlock is null.</param>
        [Obsolete("Deprecated, please use RequestHandMeshCallback instead.", true)]
        public delegate void HandMeshRequestCallback(MLResult result, MLHandMesh meshData);

        /// <summary>
        /// Requests for the hand mesh and executes the callback when the request is done.
        /// </summary>
        /// <param name="callback">Callback to execute when the request is done.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// </returns>
        [Obsolete("Deprecated, please use RequestHandMesh with the RequestHandMeshCallback delegate instead.", true)]
        public static MLResult RequestHandMesh(HandMeshRequestCallback callback)
        {
            return MLResult.Create(MLResult.Code.UnspecifiedFailure);
        }
    }
}

#endif
