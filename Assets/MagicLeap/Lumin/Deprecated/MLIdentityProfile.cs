
// %BANNERBEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHTBEGIN%
// <copyright file="MLLocation.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHTEND%
// ---------------------------------------------------------------------
// %BANNEREND%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif


    /// <summary>
    /// Public represenation of MLIdentityProfile.
    /// MLIdentityProfile represents a set of attribute of a user's profile.
    /// </summary>
    [Obsolete("Use MLIdentity.Profile instead.", true)]
    public class MLIdentityProfile : MonoBehaviour
    {
        #if PLATFORM_LUMIN
        public delegate void CallbackDelegate(MLResult result);
        #endif

        private class Request
        {
            public enum State
            {
                CREATED,
                REQUESTING_ATTRIB_NAMES,
                ATTRIB_NAMES_READY,
                REQUESTING_ATTRIB_VALUES,
                ATTRIB_VALUES_READY,
                DONE
            }

            #if PLATFORM_LUMIN
            //pending future parameter which if set would mean our request to retrieve attribute names
            //or values is pending and further requests are prevented
            public MLInvokeFuture PendingFuture;

            public State RequestState = State.CREATED;

            public CallbackDelegate Callback;

            public MLResult.Code Result;
            #endif
        }

        #if PLATFORM_LUMIN
        /// <summary>
        /// Public property to check if internal pointer and hence this profile is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _profilePtr != IntPtr.Zero;
            }
        }

        /// <summary>
        /// Array of MLIdentityAttributes containing user profile.
        /// </summary>
        public MLIdentityAttribute[] Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Optionally specific attributes to request for this profile.
        /// </summary>
        public MLIdentityAttributeKey[] RequestAttributes;

        /// <summary>
        /// Internal pointer used to associate the the managed version of this structure
        /// with the unmanaged pointer that is used by the Identity API to communicate with us.
        /// </summary>
        private IntPtr _profilePtr;

        /// <summary>
        /// Internal convenience structure which represents the marshaled unmanaged structure
        /// to communicate with the Identity API.
        /// </summary>
        private InternalMLIdentityProfile _profile;

        /// <summary>
        /// Request class used to keep track of pending requests for identity data.
        /// </summary>
        private Request _request;

        /// <summary>
        /// Fetch the specified attributes and callback when result is known.
        /// </summary>
        /// <param name="callback">
        /// MLResult.Result in callback will be MLResult.Code.Ok if successful.
        /// MLResult.Result will be MLResult.Code.InvalidParam if failed due to invalid null internal parameter.
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if failed due to internal error.
        /// MLResult.Result will be MLResult.Code.AllocFailed if failed to allocate memory.
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if caller does not have the IdentityRead privilege.
        /// MLResult.Result will be MLResult.Code.Identity* if an identity specific failure occurred during the operation.
        /// </param>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Pending if successful (request will be pending).
        /// MLResult.Result will be MLResult.Code.InvalidParam if failed due to invalid null internal parameter.
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if failed due to internal error.
        /// MLResult.Result will be MLResult.Code.AllocFailed if failed to allocate memory.
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if caller does not have the IdentityRead privilege.
        /// MLResult.Result will be MLResult.Code.Identity* if an identity specific failure occurred during the operation.
        /// </returns>
        public MLResult Fetch(CallbackDelegate callback)
        {
            MLResult result;
            if (_request != null)
            {
                result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "Already fetching attributes");
                MLPluginLog.ErrorFormat("MLIdentityProfile.Fetch failed. Reason: {0}", result);
                return result;
            }
            // Create a request
            _request = new Request();
            _request.Callback = callback;
            _request.Result = MLResult.Code.Pending;
            _request.RequestState = Request.State.REQUESTING_ATTRIB_NAMES;
            RequestAttributeNamesAsync(_request);

            // TODO: Map pending resultCode to OK for return value.
            result = MLResult.Create(_request.Result);

            return result;
        }

        private void Update()
        {
            if (_request != null)
            {
                ProcessRequest();
            }
        }

        private void ProcessRequest()
        {
            switch (_request.RequestState)
            {
                case Request.State.REQUESTING_ATTRIB_NAMES:
                    ProcessPendingAttribNamesRequest();
                    break;

                case Request.State.ATTRIB_NAMES_READY:
                    RequestAttributeValuesAsync(_request);
                    _request.RequestState = Request.State.REQUESTING_ATTRIB_VALUES;
                    break;

                case Request.State.REQUESTING_ATTRIB_VALUES:
                    ProcessPendingAttribValuesRequest();
                    break;

                case Request.State.DONE:
                    ProcessDoneRequest();
                    break;

                default:
                    //TODO: add error log that this state is unhandled
                    break;
            }
        }

        private void ProcessPendingAttribNamesRequest()
        {
            _request.Result = RequestAttributeNamesWait(_request);
            if (_request.Result != MLResult.Code.Pending && _request.Result != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLIdentityProfile.ProcessPendingAttribNamesRequest failed request to retrieve attribute names. Reason: {0}", _request.Result);
                _request.RequestState = Request.State.DONE;
            }
            else if (_request.Result == MLResult.Code.Ok)
            {
                if (_profilePtr == IntPtr.Zero || _profile.AttributePtrs == IntPtr.Zero)
                {
                    _request.RequestState = Request.State.DONE;
                    _request.Result = MLResult.Code.IdentityInvalidInformationFromCloud;
                    MLPluginLog.ErrorFormat("MLIdentityProfile.ProcessPendingAttribNamesRequest failed request to retrieve attribute names. Reason: {0}", _request.Result);
                }
                else
                {
                    _request.RequestState = Request.State.ATTRIB_NAMES_READY;
                }
            }
        }

        private void ProcessPendingAttribValuesRequest()
        {
            _request.Result = RequestAttributeValuesWait(_request);
            if (_request.Result != MLResult.Code.Pending && _request.Result != MLResult.Code.Ok)
            {
                MLPluginLog.ErrorFormat("MLIdentityProfile.ProcessPendingAttribValuesRequest failed request to retrieve attribute values. Reason: {0}", _request.Result);
                _request.RequestState = Request.State.DONE;
            }
            else if (_request.Result == MLResult.Code.Ok)
            {
                if (_profilePtr == IntPtr.Zero || _profile.AttributePtrs == IntPtr.Zero)
                {
                    _request.RequestState = Request.State.DONE;
                    _request.Result = MLResult.Code.IdentityInvalidInformationFromCloud;
                    MLPluginLog.ErrorFormat("MLIdentityProfile.ProcessPendingAttribValuesRequest failed request to retrieve attribute values. Reason: {0}", _request.Result);
                }
                else
                {
                    _request.RequestState = Request.State.DONE;
                }
            }
        }

        private void ProcessDoneRequest()
        {
            Request temp = _request;
            _request = null;
            if (temp.Callback != null)
            {
                temp.Callback(MLResult.Create(temp.Result));
            }
            temp = null;
        }

        [Obsolete("Use GetResultString(MLResult.Code) instead.", true)]
        private static string GetResultString(MLResultCode result)
        {
            return "This function is deprecated. Use MLIdentity.GetResultString(MLResult.Code) instead.";
        }

        private static string GetResultString(MLResult.Code result)
        {
            return Marshal.PtrToStringAnsi(MLIdentityNativeBindings.MLIdentityGetResultString(result));
        }

        private void RequestAttributeNamesAsync(Request request)
        {
            // Make sure we cleanup any previous profile data.
            Cleanup();

            if (RequestAttributes != null && RequestAttributes.Length > 0)
            {
                //_request.Result = MLIdentityNativeBindings.MLIdentityGetKnownAttributeNames(RequestAttributes, (uint)RequestAttributes.Length, ref _profilePtr);
                if (_request.Result == MLResult.Code.Ok)
                {
                    _profile = (InternalMLIdentityProfile)Marshal.PtrToStructure(_profilePtr, typeof(InternalMLIdentityProfile));

                    Attributes = new MLIdentityAttribute[_profile.AttributeCount];
                    for (int i = 0; i < _profile.AttributeCount; i++)
                    {
                        IntPtr offsetPtr = Marshal.ReadIntPtr(new IntPtr(_profile.AttributePtrs.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)));
                        Attributes[i] = (MLIdentityAttribute)Marshal.PtrToStructure(offsetPtr, typeof(MLIdentityAttribute));

                        // Set this to true so that we can request the value.
                        Attributes[i].IsRequested = true;
                    }
                    _request.Result = MLResult.Code.Pending;
                    _request.RequestState = Request.State.ATTRIB_NAMES_READY;
                }
                else
                {
                    MLPluginLog.ErrorFormat("MLIdentityProfile.RequestAttributeNamesAsync failed to get known attributes names. Reason: {0}", _request.Result);
                    _request.RequestState = Request.State.DONE;
                }
            }
            else
            {
                request.PendingFuture = new MLInvokeFuture();
                MLResult.Code getAttributeNamesAsyncResult = MLIdentityNativeBindings.MLIdentityGetAttributeNamesAsync(ref request.PendingFuture._invokeFuturePtr);

                if (getAttributeNamesAsyncResult == MLResult.Code.PrivilegeDenied)
                {
                    MLPluginLog.Warning("MLIdentityProfile.RequestAttributeNamesAsync failed request for attribute names. Reason: Caller does not have IdentityRead Privilege.");
                }
                else if (getAttributeNamesAsyncResult != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLIdentityProfile.RequestAttributeNamesAsync failed request for attribute names. Reason: {0}", getAttributeNamesAsyncResult);
                }
            }
        }

        private MLResult.Code RequestAttributeNamesWait(Request request)
        {
            MLInvokeFuture future = request.PendingFuture;

            // Attempt to get data if available, 0 is passed as a timeout to immediately return and never wait for results.
            MLResult.Code result = MLIdentityNativeBindings.MLIdentityGetAttributeNamesWait(future._invokeFuturePtr, 0, ref _profilePtr);

            // If it succeeded, copy any modifications made to the profile in unmanaged memory by the Identity API to managed memory.
            if (result == MLResult.Code.Ok)
            {
                _profile = (InternalMLIdentityProfile)Marshal.PtrToStructure(_profilePtr, typeof(InternalMLIdentityProfile));

                Attributes = new MLIdentityAttribute[_profile.AttributeCount];
                for (int i = 0; i < _profile.AttributeCount; i++)
                {
                    IntPtr offsetPtr = Marshal.ReadIntPtr(new IntPtr(_profile.AttributePtrs.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)));

                    // Write the unmanaged copy back onto the managed memory we were passed in.
                    Attributes[i] = (MLIdentityAttribute)Marshal.PtrToStructure(offsetPtr, typeof(MLIdentityAttribute));

                    // Set this to true so that we can request the value.
                    Attributes[i].IsRequested = true;
                }
            }

            return result;
        }

        private void RequestAttributeValuesAsync(Request request)
        {
            request.PendingFuture = new MLInvokeFuture();

            // Copy any modifications made to the profile in managed memory to the unmanaged memory used by the Identity API.
            for (int i = 0; i < Attributes.Length; i++)
            {
                IntPtr offsetPtr = Marshal.ReadIntPtr(new IntPtr(_profile.AttributePtrs.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)));

                // Write the managed copy back onto the unmanaged memory we were originally given for it.
                Marshal.StructureToPtr(Attributes[i], offsetPtr, false);
            }

            MLResult.Code requestAttributeValuesAsyncResult = MLIdentityNativeBindings.MLIdentityRequestAttributeValuesAsync(_profilePtr, ref request.PendingFuture._invokeFuturePtr);

            if (requestAttributeValuesAsyncResult != MLResult.Code.Ok)
            {
                MLPluginLog.WarningFormat("MLIdentityProfile.RequestAttributeValuesAsync failed request for attribute values async. Reason: {0}", requestAttributeValuesAsyncResult);
            }
        }

        private MLResult.Code RequestAttributeValuesWait(Request request)
        {
            MLInvokeFuture future = request.PendingFuture;

            // Attempt to get data if available, 0 is passed as a timeout to immediately return and never wait for results.
            MLResult.Code result = MLIdentityNativeBindings.MLIdentityRequestAttributeValuesWait(future._invokeFuturePtr, 0);

            // If it succeeded, copy any modifications made to the profile in unmanaged memory by the Identity API to managed memory.
            if (result == MLResult.Code.Ok)
            {
                for (int i = 0; i < _profile.AttributeCount; i++)
                {
                    IntPtr offsetPtr = Marshal.ReadIntPtr(new IntPtr(_profile.AttributePtrs.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)));

                    // Write the unmanaged copy back onto the managed memory we were passed in.
                    Attributes[i] = (MLIdentityAttribute)Marshal.PtrToStructure(offsetPtr, typeof(MLIdentityAttribute));
                }
            }

            return result;
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        /// ReleaseUserProfile releases all resources associated with the
        private void Cleanup()
        {
            if (IsValid)
            {
                // Make the native call with correct internal parameter.
                MLResult.Code releaseUserProfileResult = MLIdentityNativeBindings.MLIdentityReleaseUserProfile(_profilePtr);

                if (releaseUserProfileResult != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLIdentityProfile.Cleanup failed request to release user profile. Reason: {0}", releaseUserProfileResult);
                }
            }

            // Null the local representation of these pointers as they should have been freed by the call above.
            _profilePtr = IntPtr.Zero;
            _profile.AttributePtrs = IntPtr.Zero;
            _profile.AttributeCount = 0;
            Attributes = null;
        }
        #endif
    }
}
