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
#if PLATFORM_LUMIN
using MagicLeap.Core.StarterKit;
#endif
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    /// <summary>
    /// Starts the MLFoundObjectsToolkit and queries for found objects.
    /// </summary>
    public class MLFoundObjectsBehavior : MonoBehaviour
    {
        [SerializeField, Tooltip("When enabled this behaviour will continuously query for new objects.")]
        private bool _autoQuery = true;

        [Tooltip("Query frequency in seconds.")]
        private float queryFrequency = 3.0f;

        [SerializeField]
        private MLFoundObjects.Query.Filter queryFilter = MLFoundObjects.Query.Filter.Create();

#if PLATFORM_LUMIN
        private Timer queryTimer;
#endif

        public delegate void OnFoundObjectsDelegate(MLFoundObjects.FoundObject[] foundObjects);

        /// <summary>
        /// Event for when a query has completed.
        /// </summary>
        public event OnFoundObjectsDelegate OnFoundObjects = delegate { };

        /// <summary>
        /// Starts up MLFoundObjectsToolkit.
        /// </summary>
        void Start()
        {
#if PLATFORM_LUMIN
            MLFoundObjectsStarterKit.Start();
            queryTimer = new Timer(queryFrequency);
#endif
        }

        /// <summary>
        /// Clean up.
        /// </summary>
        void OnDestroy()
        {
#if PLATFORM_LUMIN
            MLFoundObjectsStarterKit.Stop();
#endif
        }

        /// <summary>
        /// Obtains the latest found object query.
        /// </summary>
        void Update()
        {
#if PLATFORM_LUMIN
            if (_autoQuery && queryTimer.LimitPassed)
            {
                QueryFoundObjects();
                queryTimer.Reset();
            }
#endif
        }

        /// <summary>
        /// Requests a new query if one is not currently active.
        /// </summary>
        /// <returns>Will return true if the request was successful.</returns>
        public bool QueryFoundObjects()
        {
#if PLATFORM_LUMIN
            if (MLFoundObjects.IsStarted)
            {
                MLFoundObjectsStarterKit.QueryFoundObjectsAsync(queryFilter, HandleOnFoundObjects);
                return true;
            }
#endif

            return false;
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Handles when an array of found objects is returned by the API.
        /// </summary>
        /// <param name="result">MLResult of the query.</param>
        /// <param name="foundObjects">Array of found objects returned by the query.</param>
        private void HandleOnFoundObjects(MLResult result, MLFoundObjects.FoundObject[] foundObjects)
        {
            if(result.IsOk)
            {
                OnFoundObjects?.Invoke(foundObjects);
            }
            else
            {
                Debug.LogFormat("MLFoundObjects failed to find any objects. Reason: {0}", MLResult.CodeToString(result.Result));
            }
        }
#endif
    }
}
