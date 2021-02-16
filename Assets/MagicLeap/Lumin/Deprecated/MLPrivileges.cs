// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLPrivileges.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;

    /// <summary>
    /// Functionality to validate or query privileges from the system.
    /// </summary>
    public sealed partial class MLPrivileges
    {
        [System.Obsolete("MLPrivileges API is now automatically started and stopped. There is no need to call the Start() and Stop() methods for this API, and they are now deprecated. See https://developer.magicleap.com/learn/guides/auto-api-changes for more info.", false)]
        public static MLResult Start()
        {
            return MLResult.Create(MLResult.Code.Ok);
        }

        [System.Obsolete("MLPrivileges API is now automatically started and stopped. There is no need to call the Start() and Stop() methods for this API, and they are now deprecated. See https://developer.magicleap.com/learn/guides/auto-api-changes for more info.", false)]
        public static void Stop()
        {
        }

        /// <summary>
        /// Request the specified privileges. This may solicit consent from the end-user.
        /// Note: The asynchronous callback occurs within the main thread.
        /// </summary>
        /// <param name="privilegeId">The privilege to request.</param>
        /// <param name="callback">Callback to be executed when the privilege request has completed.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the privilege request is in progress.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the callback is null.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// Callback MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// Callback MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// </returns>
        [Obsolete("Please use Task<MLResult> RequestPrivilegeAsync instead.", false)]
        public static MLResult RequestPrivilegeAsync(MLPrivileges.Id privilegeId, CallbackDelegate callback)
        {
            return Instance.RequestPrivilegeAsyncInternal(privilegeId, callback);
        }

        private MLResult RequestPrivilegeAsyncInternal(MLPrivileges.Id privilegeId, CallbackDelegate callback)
        {
            if (callback == null)
            {
                return MLResult.Create(MLResult.Code.InvalidParam, "MLPrivileges.RequestPrivilegeAsync failed. Reason: Must send a valid callback.");
            }

            if (!this.currentRequests.ContainsKey(privilegeId))
            {
                IntPtr newRequest = IntPtr.Zero;

                MLResult.Code resultCode = NativeBindings.MLPrivilegesRequestPrivilegeAsync(privilegeId, ref newRequest);
                if (resultCode == MLResult.Code.Ok)
                {
                    RequestPrivilegeQuery newQuery = new RequestPrivilegeQuery(callback, newRequest, privilegeId);
                    this.currentRequests.Add(privilegeId, newQuery);
                }

                return MLResult.Create(resultCode);
            }
            else
            {
                return MLResult.Create(MLResult.Code.Ok);
            }
        }


    }
}

#endif
