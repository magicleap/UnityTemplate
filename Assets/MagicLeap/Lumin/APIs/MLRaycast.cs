// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLRaycast.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Collections.Generic;

    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// Sends requests to create Rays intersecting world geometry and returns results through callbacks.
    /// </summary>
    public partial class MLRaycast : MLAutoAPISingleton<MLRaycast>
    {
        #if PLATFORM_LUMIN
        /// <summary>
        /// Contains all pending queries.
        /// </summary>
        private readonly Dictionary<ulong, OnRaycastResultDelegate> pendingQueries = new Dictionary<ulong, OnRaycastResultDelegate>();

        /// <summary>
        /// Tracks the latest query added.
        /// </summary>
        private NativeBindings.MLRaycastQueryNative currentQuery = NativeBindings.MLRaycastQueryNative.Create();

        /// <summary>
        /// Stores the ray cast system tracker.
        /// </summary>
        private ulong trackerHandle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Keeps the queries that were completed on a specific frame.
        /// </summary>
        private Dictionary<ulong, NativeBindings.MLRaycastResultNative> completedQueries = new Dictionary<ulong, NativeBindings.MLRaycastResultNative>();

        /// <summary>
        /// Keeps the queries that failed on a specific frame.
        /// </summary>
        private List<ulong> errorQueries = new List<ulong>();

        /// <summary>
        /// Delegate used to convey the result of a ray cast.
        /// </summary>
        /// <param name="state">The state of the ray cast result.</param>
        /// <param name="hitpoint">Where in the world the collision happened.</param>
        /// <param name="normal">Normal to the surface where the ray collided.</param>
        /// <param name="confidence">The confidence of the ray cast result. Confidence is a non-negative value from 0 to 1 where closer to 1 indicates a higher quality.</param>
        /// \internal
        /// CAPI has custom result MLWorldRaycastResultState, thus we expose it instead of MLResult.
        /// \endinternal
        public delegate void OnRaycastResultDelegate(ResultState state, Vector3 hitpoint, Vector3 normal, float confidence);
        #endif

        /// <summary>
        /// Enumeration of ray cast result states.
        /// </summary>
        public enum ResultState
        {
            /// <summary>
            /// The ray cast request failed.
            /// </summary>
            RequestFailed = -1,

            /// <summary>
            /// The ray passed beyond maximum ray cast distance and it doesn't hit any surface.
            /// </summary>
            NoCollision,

            /// <summary>
            /// The ray hit unobserved area. This will on occur when collide_with_unobserved is set to true.
            /// </summary>
            HitUnobserved,

            /// <summary>
            /// The ray hit only observed area.
            /// </summary>
            HitObserved,
        }

        #if PLATFORM_LUMIN

        /// <summary>
        /// Requests a ray cast with the given query parameters.
        /// </summary>
        /// <param name="query">Query parameters describing ray being cast.</param>
        /// <param name="callback">Delegate which will be called when the result of the ray cast is ready.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        public static MLResult Raycast(QueryParams query, OnRaycastResultDelegate callback)
        {
            return Instance.RaycastInternal(query, callback);
        }

        private MLResult RaycastInternal(QueryParams query, OnRaycastResultDelegate callback)
        {
            if (query == null || callback == null)
            {
                MLPluginLog.ErrorFormat("MLRaycast.Raycast failed. Reason: Invalid input parameters.");
                return MLResult.Create(MLResult.Code.InvalidParam);
            }

            this.currentQuery.Position = MLConvert.FromUnity(query.Position);
            this.currentQuery.Direction = MLConvert.FromUnity(query.Direction, true, false);
            this.currentQuery.UpVector = MLConvert.FromUnity(query.UpVector, true, false);
            this.currentQuery.Width = query.Width;
            this.currentQuery.Height = query.Height;
            this.currentQuery.HorizontalFovDegrees = query.HorizontalFovDegrees;
            this.currentQuery.CollideWithUnobserved = query.CollideWithUnobserved;

            ulong queryHandle = MagicLeapNativeBindings.InvalidHandle;
            MLResult.Code resultCode = NativeBindings.MLRaycastRequest(this.trackerHandle, ref this.currentQuery, ref queryHandle);
            if (resultCode != MLResult.Code.Ok)
            {
                MLResult result = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLRaycast.Raycast failed to request a new ray cast. Reason: {0}", result);
                return result;
            }

            this.pendingQueries.Add(queryHandle, callback);
            return MLResult.Create(MLResult.Code.Ok);
        }
#if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Creates a new ray cast tracker.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            this.trackerHandle = MagicLeapNativeBindings.InvalidHandle;

            MLResult.Code resultCode = NativeBindings.MLRaycastCreate(ref this.trackerHandle);
            DidNativeCallSucceed(resultCode, "MLRaycastCreate");

            return resultCode;
        }
        #endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Polls for the result of pending ray cast requests.
        /// </summary>
        protected override void Update()
        {
            foreach (ulong handle in this.pendingQueries.Keys)
            {
                NativeBindings.MLRaycastResultNative raycastResult = NativeBindings.MLRaycastResultNative.Create();

                MLResult.Code resultCode = NativeBindings.MLRaycastGetResult(this.trackerHandle, handle, ref raycastResult);
                if (resultCode == MLResult.Code.Ok)
                {
                    this.completedQueries.Add(handle, raycastResult);
                }
                else if (resultCode != MLResult.Code.Pending)
                {
                    MLPluginLog.ErrorFormat("MLRaycast.Update failed to get raycast result. Reason: {0}", MLResult.CodeToString(resultCode));
                    this.errorQueries.Add(handle);
                }
            }

            foreach (ulong handle in this.errorQueries)
            {
                this.pendingQueries.Remove(handle);
            }
            this.errorQueries.Clear();

            foreach (KeyValuePair<ulong, NativeBindings.MLRaycastResultNative> handle in this.completedQueries)
            {
                // Check if there is a valid hit result.
                bool didHit = handle.Value.State != ResultState.RequestFailed && handle.Value.State != ResultState.NoCollision;

                this.pendingQueries[handle.Key](
                     handle.Value.State,
                    didHit ? MLConvert.ToUnity(handle.Value.Hitpoint) : Vector3.zero,
                    didHit ? MLConvert.ToUnity(handle.Value.Normal, true, false) : Vector3.zero,
                     handle.Value.Confidence);

                this.pendingQueries.Remove(handle.Key);
            }

            this.completedQueries.Clear();
        }

        /// <summary>
        /// Cleans up memory and destroys the ray cast tracker.
        /// </summary>
        protected override MLResult.Code StopAPI()
        {
            this.pendingQueries.Clear();
            this.DestroyNativeTracker();
            return MLResult.Code.Ok;
        }

        /// <summary>
        /// Destroys the native ray cast tracker.
        /// </summary>
        private void DestroyNativeTracker()
        {
            if (NativeBindings.MLHandleIsValid(this.trackerHandle))
            {
                MLResult.Code resultCode = NativeBindings.MLRaycastDestroy(this.trackerHandle);
                if (resultCode != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLRaycast.DestroyNativeTracker failed to destroy raycast tracker. Reason: {0}", NativeBindings.MLGetResultString(resultCode));
                }

                this.trackerHandle = MagicLeapNativeBindings.InvalidHandle;
            }
        }

        /// <summary>
        /// Parameters for a ray cast request.
        /// </summary>
        public class QueryParams
        {
            /// <summary>
            /// Gets or sets where the ray is cast from.
            /// </summary>
            public Vector3 Position { get; set; } = Vector3.zero;

            /// <summary>
            /// Gets or sets the direction of the ray to fire.
            /// </summary>
            public Vector3 Direction { get; set; } = Vector3.forward;

            /// <summary>
            /// Gets or sets the up vector of the ray to fire.  Use (0, 0, 0) to use the up vector of the rig frame.
            /// </summary>
            public Vector3 UpVector { get; set; } = Vector3.zero;

            /// <summary>
            /// Gets or sets the number of horizontal rays. For single point ray cast, set this to 1.
            /// </summary>
            public uint Width { get; set; } = 1;

            /// <summary>
            /// Gets or sets the number of vertical rays. For single point ray cast, set this to 1.
            /// </summary>
            public uint Height { get; set; } = 1;

            /// <summary>
            /// Gets or sets the horizontal field of view, in degrees.
            /// </summary>
            public float HorizontalFovDegrees { get; set; } = 50.0f;

            /// <summary>
            /// Gets or sets a value indicating whether a ray will terminate when encountering an unobserved area and return
            /// a surface or the ray will continue until it ends or hits a observed surface.
            /// </summary>
            public bool CollideWithUnobserved { get; set; } = false;
        }
        #endif
    }
}
