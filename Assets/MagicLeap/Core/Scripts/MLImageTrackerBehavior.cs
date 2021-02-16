// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    /// <summary>
    /// MLImageTrackerBehavior encapsulates the functionality to track images.
    /// </summary>
    [AddComponentMenu("XR/MagicLeap/MLImageTrackerBehavior")]
    public class MLImageTrackerBehavior : MonoBehaviour
    {
        /// <summary>
        /// Cached tracking status.
        /// </summary>
        private MLImageTracker.Target.TrackingStatus status;

        /// <summary>
        /// Image that needs to be tracked.
        /// Do not resize the image, the aspect ratio of the image provided here
        /// and the printed image should be the same. Set the "Non Power of 2"
        /// property of Texture2D to none.
        /// </summary>
        [Tooltip("Texture2D  of image that needs to be tracked. Do not change the aspect ratio of the image, it should be the same as the printed image. Set the \"Non Power of 2\" property of Texture2D to \"none\".")]
        public Texture2D image;

        /// <summary>
        /// Set this to true if the position of this image target in the physical
        /// world is fixed and its surroundings are planar (ex: walls, floors, tables, etc).
        /// </summary>
        [Tooltip("Set this to true if the position of this image target in the physical world is fixed and its surroundings are planar (ex: walls, floors, tables, etc).")]
        public bool isStationary;

        /// <summary>
        /// Set this to true if the behavior should automatically move the attached game object.
        /// </summary>
        [Tooltip("Set this to true if the behavior should automatically move the attached game object.")]
        public bool autoUpdate;

        /// <summary>
        /// Longer dimension of the printed image target in scene units.
        /// If width is greater than height, it is the width, height otherwise.
        /// </summary>
        [Tooltip("Longer dimension of the printed image target in scene units. If width is greater than height, it is the width, height otherwise.")]
        public float longerDimensionInSceneUnits;

        /// <summary>
        /// Whether or not this object is currently being tracked.
        /// </summary>
        public bool IsTracking
        {
            get
            {
                if(_imageTarget == null)
                {
                    return false;
                }

                return (this.status == MLImageTracker.Target.TrackingStatus.Tracked || this.status == MLImageTracker.Target.TrackingStatus.Unreliable);
            }
        }

        /// <summary>
        /// The current this.status of the tracking state.
        /// </summary>
        public MLImageTracker.Target.TrackingStatus TrackingStatus
        {
            get
            {
                if (_imageTarget == null)
                {
                    return MLImageTracker.Target.TrackingStatus.NotTracked;
                }

                return this.status;
            }
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Delegate for status updates.
        /// </summary>
        public delegate void StatusUpdate(MLImageTracker.Target target, MLImageTracker.Target.Result result);

        /// <summary>
        /// Occurs when an existing image target is found.
        /// The this.status of the MLImageTracker.Target.Result will indicate if tracking is unreliable.
        /// </summary>
        public event StatusUpdate OnTargetFound = delegate { };

        /// <summary>
        /// Occurs when the image target is lost.
        /// </summary>
        public event StatusUpdate OnTargetLost = delegate { };

        /// <summary>
        /// Occurs when the result gets updated for the image target and happens once every frame.
        /// </summary>
        public event StatusUpdate OnTargetUpdated = delegate { };
#endif

        /// <summary>
        /// Holds reference to the image target inside MLImageTracker.Target.
        /// </summary>
        private MLImageTracker.Target _imageTarget = null;

        /// <summary>
        /// Starts the image tracker and adds the image target to the tracking system.
        /// </summary>
        void Start()
        {
            AddTarget();
        }

        /// <summary>
        /// Removes the image target from the tracking system and then stops the starter kit.
        /// </summary>
        void OnDestroy()
        {
#if PLATFORM_LUMIN
            MLImageTracker.RemoveTarget(gameObject.GetInstanceID().ToString());
#endif
        }

        /// <summary>
        /// Adds a new image target to be tracked.
        /// </summary>
        private void AddTarget()
        {
#if PLATFORM_LUMIN
            _imageTarget = MLImageTracker.AddTarget(gameObject.GetInstanceID().ToString(), image, longerDimensionInSceneUnits, HandleAllTargetStatuses, isStationary);

            if (_imageTarget == null)
            {
                Debug.LogErrorFormat("MLImageTrackerBehavior.AddTarget failed to add target {0} to the image tracker.", gameObject.name);
                return;
            }
#endif
        }

        /// <summary>
        /// Get the longer dimension of the Image Target.
        /// This should not be called before the image target is added to the tracker system.
        /// </summary>
        /// <param name="longerDimension">longer dimension of the image target in scene units.</param>
        /// <returns> true if the dimension was successfully fetched, false otherwise.</returns>
        public bool GetTargetLongerDimension(out float longerDimension)
        {
            if (_imageTarget == null)
            {
                Debug.LogError("MLImageTrackerBehavior.GetTargetLongerDimension failed to get the longer dimension of the image target. Reason: Invalid image target.");
                longerDimension = 0;
                return false;
            }

#if PLATFORM_LUMIN
            longerDimension = _imageTarget.GetTargetLongerDimension();
#else
            longerDimension = 0.0f;
#endif

            return true;
        }

        /// <summary>
        /// Set the longer dimension of the Image Target.
        /// This method can be used to change the dimension of the image target at runtime.
        /// This should not be called before the image target is added to the tracker system.
        /// </summary>
        /// <param name="longerDimension">longer dimension of the image target in scene units.</param>
        /// <returns/>
        public MLResult SetTargetLongerDimension(float longerDimension)
        {
            MLResult result;

#if PLATFORM_LUMIN
            if (_imageTarget == null)
            {
                result = MLResult.Create(MLResult.Code.InvalidParam, "Invalid image target");
                Debug.LogErrorFormat("MLImageTrackerBehavior.SetTargetLongerDimension failed to set the longer dimension of the image target. Reason: {0}", result);
                return result;
            }

            result = _imageTarget.SetTargetLongerDimension(longerDimension);
            if (result.IsOk)
            {
                longerDimensionInSceneUnits = longerDimension;
            }
#endif

            return result;
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Handles all the image target's this.status updates. This is called every frame.
        /// </summary>
        private void HandleAllTargetStatuses(MLImageTracker.Target target, MLImageTracker.Target.Result result)
        {
            if (result.Status != this.status)
            {
                this.status = result.Status;

                if (this.status == MLImageTracker.Target.TrackingStatus.Tracked || this.status == MLImageTracker.Target.TrackingStatus.Unreliable)
                {
                    OnTargetFound(target, result);
                }

                else
                {
                    OnTargetLost(target, result);
                }
            }
            else
            {
                OnTargetUpdated(target, result);
            }

            if (autoUpdate)
            {
                transform.position = result.Position;
                transform.rotation = result.Rotation;
            }
        }
#endif
    }
}
