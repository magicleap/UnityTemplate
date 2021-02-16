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

#if PLATFORM_LUMIN
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core.StarterKit
{
    /// <summary>
    /// Starterkit class for practical use of MLFoundObjects.
    /// </summary>
    public static class MLFoundObjectsStarterKit
    {
        /// <summary>
        /// Use this to determine when you are allowed to Query for found objects again.
        /// </summary>
        public static bool IsQuerying
        {
            get;
            private set;
        }

        /// <summary>
        /// Used to set IsQuerying to false.
        /// </summary>
        private static MLFoundObjects.QueryResultsDelegate queryCallback = (MLResult result, MLFoundObjects.FoundObject[] foundObjects) => { IsQuerying = false; };

        /// <summary>
        /// Used to cache the last MLResult.
        /// </summary>
        private static MLResult result;

        /// <summary>
        /// Starts up MLFoundObjects.
        /// </summary>
        public static MLResult Start()
        {
            #if PLATFORM_LUMIN
            result = MLPrivilegesStarterKit.RequestPrivileges(MLPrivileges.Id.ObjectData);
            if (result.Result != MLResult.Code.PrivilegeGranted)
            {
                Debug.LogErrorFormat("Error: MLFoundObjectsStarterKit failed requesting privileges. Reason: {0}", result);
                return result;
            }

            result = MLFoundObjects.Start();
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: MLFoundObjectsStarterKit failed starting MLFoundObjects. Reason: {0}", result);
            }
            return result;
            #endif
        }

        /// <summary>
        /// Stops MLFoundObjects if it has been started.
        /// </summary>
        public static void Stop()
        {
            if (MLFoundObjects.IsStarted)
            {
                MLFoundObjects.Stop();
            }
        }

        /// <summary>
        /// Function used to query for found objects present in the real world.
        /// </summary>
        /// <param name="queryFilter">Filter used to customize query results.</param>
        /// <param name="callback">The function to call when the query is done.</param>
        public static MLResult QueryFoundObjectsAsync(MLFoundObjects.Query.Filter queryFilter, MLFoundObjects.QueryResultsDelegate callback)
        {
            if (MLFoundObjects.IsStarted)
            {
                if (IsQuerying)
                {
                    return MLResult.Create(MLResult.Code.UnspecifiedFailure, "A previous query is still in progress.");
                }

                callback += queryCallback;
                result = MLFoundObjects.GetObjectsAsync(queryFilter, callback);
                IsQuerying = result.IsOk;

                if (!result.IsOk)
                {
                    callback = null;
                    Debug.LogErrorFormat("Error: MLFoundObjectsStarterKit.QueryFoundObjects failed. Reason: {0}", result);
                }
            }

            else
            {
                Debug.LogError("Error: MLFoundObjectsStarterKit.QueryFoundObjects failed because MLFoundObjects was not started.");
                result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLFoundObjects was not started");
            }

            return result;
        }
    }
}
#endif
