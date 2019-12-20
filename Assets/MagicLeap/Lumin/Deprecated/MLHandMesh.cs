// %BANNERBEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHTBEGIN%
// <copyright file="MLHandMesh.cs" company="Magic Leap">
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
    /// Structure to represent a hand mesh.
    /// </summary>
    [Obsolete("Deprecated, please MLHandMeshing.Meshing.Mesh instead.", false)]
    public struct MLHandMesh
    {
        /// <summary>
        /// Gets the array of mesh blocks.
        /// </summary>
        public MLHandMeshBlock[] MeshBlock { get; internal set; }
    }

    /// <summary>
    /// Structure to represent a mesh block.
    /// </summary>
    [Obsolete("Deprecated, please MLHandMeshing.Meshing.Mesh.Block instead.", false)]
    public struct MLHandMeshBlock
    {
        /// <summary>
        /// Gets the array of vertex positions relative to the origin.
        /// This can be directly plugged into Mesh.vertices.
        /// </summary>
        public Vector3[] Vertex { get; internal set; }

        /// <summary>
        /// Gets the array of indices. Guaranteed to be a multiple of 3. Every 3 indices creates a triangles.
        /// This can be directly plugged into Mesh.triangles.
        /// </summary>
        public int[] Index { get; internal set; }
    }
}

#endif
