// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMRCameraInputContext.cs" company="Magic Leap">
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

#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// Mixed Reality Camera API, used to capture camera frames that include mixed reality content.
    /// </summary>
    public partial class MLMRCamera
    {
        /// <summary>
        /// Struct representing the input context to pass into the MLMRCamera API when initializing.
        /// </summary>
        [Serializable]
        public struct InputContext
        {
            /// <summary>
            /// Quality that the capture should have.
            /// </summary>
            [SerializeField, Tooltip("Resolution that the capture should have.")]
            private RenderQuality quality;

            /// <summary>
            /// Blend type that the capture should have.
            /// </summary>
            [SerializeField, Tooltip("Affects how content is rendered, use Additive to display virtual content.")]
            private BlendType blendType;

            /// <summary>
            /// Stabilization that the capture should have.
            /// </summary>
            [SerializeField, Tooltip("Stablization that the capture should have.")]
            private bool stabilization;

            /// <summary>
            /// Gets the quality of the input context.
            /// </summary>
            public RenderQuality Quality { get => this.quality; }

            /// <summary>
            /// Gets the blend type of the input context.
            /// </summary>
            public BlendType BlendType { get => this.blendType; }

            /// <summary>
            /// Gets a value indicating whether the capture should have stabilization.
            /// </summary>
            public bool Stabilization { get => this.stabilization; }

            /// <summary>
            /// Creates and returns an initialized version of this struct.
            /// </summary>
            /// <param name="quality">Quality to pass into the input context.</param>
            /// <param name="blendType">Blend type to pass into the input context.</param>
            /// <param name="stabilization">Stabilization to pass into the input context.</param>
            /// <returns>An initialized version of this struct.</returns>
            public static InputContext Create(RenderQuality quality = RenderQuality.q1080P, BlendType blendType = BlendType.Additive, bool stabilization = false)
            {
                InputContext context = new InputContext();
                context.quality = quality;
                context.blendType = blendType;
                context.stabilization = stabilization;
                return context;
            }
        }
    }
}
