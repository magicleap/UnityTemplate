// %BANNERBEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHTBEGIN%
// <copyright file="MLIdentity.cs" company="Magic Leap">
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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// Functionality to query for user profiles.
    /// </summary>
    public class MLIdentity : MLAPISingleton<MLIdentity>
    {
        #if PLATFORM_LUMIN
        /// <summary>
        /// Initializes an instance of MLIdentity.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to an internal error.
        /// </returns>
        public static MLResult Start()
        {
            CreateInstance();
            return MLIdentity.BaseStart();
        }

        /// <summary>
        /// Queries for a profile.
        /// </summary>
        /// <param name="query">The profile query.</param>
        /// <returns>The profile queried for, if it could be found.</returns>
        public static Profile QueryProfile(string query)
        {
            return new Profile();
        }

        #if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Returns <c>MLResult.Code.Ok</c> because no API is started. 
        /// </summary>
        /// <returns><c>MLResult.Code.Ok</c> because no API is started. </returns>
        protected override MLResult StartAPI()
        {
            return MLResult.Create(MLResult.Code.Ok);
        }
        #endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Cleans up unmanaged memory.
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects">Allow complete cleanup of the API.</param>
        protected override void CleanupAPI(bool isSafeToAccessManagedObjects)
        {
        }

        /// <summary>
        /// Update loop.
        /// </summary>
        protected override void Update()
        {
        }

        /// <summary>
        /// Creates an instance of MLIdentity if there is no valid instance.
        /// </summary>
        private static void CreateInstance()
        {
            if (!MLIdentity.IsValidInstance())
            {
                MLIdentity._instance = new MLIdentity();
            }
        }
        #endif

        /// <summary>
        /// Represents an attribute of a user's profile.
        /// Each attribute has a name (represented by key and value).
        /// </summary>
        public struct Attribute
        {
            #if PLATFORM_LUMIN
            /// <summary>
            /// The attribute key.
            /// </summary>
            public Type Key;

            /// <summary>
            /// The attribute Name.
            /// </summary>
            public string Name;

            /// <summary>
            /// The attribute's string value.
            /// </summary>
            public string Value;

            /// <summary>
            /// The attribute is requested.
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool IsRequested;

            /// <summary>
            /// The attribute is granted.
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool IsGranted;
            #endif

            /// <summary>
            /// The different types of attributes there are.
            /// </summary>
            public enum Type
            {
                /// <summary>
                /// An unknown attribute.
                /// </summary>
                Unknown,

                /// <summary>
                /// The given name listed on a profile.
                /// </summary>
                GivenName,

                /// <summary>
                /// The family name listed on a profile.
                /// </summary>
                FamilyName,

                /// <summary>
                /// The email address listed on a profile.
                /// </summary>
                Email,

                /// <summary>
                /// The bio listed on a profile.
                /// </summary>
                Bio,

                /// <summary>
                /// The main phone number listed on a profile.
                /// </summary>
                PhoneNumber,

                /// <summary>
                /// The Avatar2D representation listed on a profile.
                /// </summary>
                Avatar2D,

                /// <summary>
                /// The Avatar3D representation listed on a profile.
                /// </summary>
                Avatar3D,
            }
        }

        /// <summary>
        /// Public representation of MLIdentity.Profile.
        /// Represents a set of attribute of a user's profile.
        /// </summary>
        public class Profile
        {
            #if PLATFORM_LUMIN
            /// <summary>
            /// Array of MLIdentity.Attributes associated with this profile.
            /// </summary>
            private MLIdentity.Attribute[] attributes;

            /// <summary>
            /// The current request for attributes attached to this profile.
            /// </summary>
            private Request request = null;

            /// <summary>
            /// The specific attributes to request for this profile.
            /// </summary>
            [SerializeField, Tooltip("List of attributes you want to retrieve, if not set all attributes listed by MLIdentity.Attribute.Type will be requested")]
            private MLIdentity.Attribute.Type[] requestAttributes = null;

            /// <summary>
            /// Finalizes an instance of the <see cref="MLIdentity.Profile" /> class.
            /// </summary>
            ~Profile()
            {
                this.Cleanup();
            }

            /// <summary>
            /// Gets a value indicating whether the profile is valid.
            /// </summary>
            /// <returns>Will always be true.</returns>
            public bool IsValid
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// Gets the attributes array of associated with this profile.
            /// </summary>
            public MLIdentity.Attribute[] Attributes
            {
                get
                {
                    return this.attributes;
                }
            }

            /// <summary>
            /// Gets the current request for attributes attached to this profile.
            /// A profile may only handle one request at a time.
            /// </summary>
            public Request CurrentRequest
            {
                get
                {
                    return this.request;
                }

                private set
                {
                    if (this.request == null && value != null)
                    {
                        this.request = value;
                        this.ProcessRequest();
                    }
                }
            }

            /// <summary>
            /// Gets the specific attributes to request for this profile.
            /// </summary>
            public MLIdentity.Attribute.Type[] RequestAttributes
            {
                get
                {
                    return this.requestAttributes;
                 }
            }

            /// <summary>
            /// Fetch the specified attributes and callback when result is known.
            /// </summary>
            /// <param name="callback">The callback to notify when the CurrentRequest is complete.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully.
            /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if the operation failed to allocate memory.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if there was an invalid parameter.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if there was a lack of privileges.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if there was an internal error.
            /// MLResult.Result will be <c>MLResult.Code.IdentityFailedToConnectToLocalService</c> if the local service is not running, or it cannot be accessed
            /// MLResult.Result will be <c>MLResult.Code.IdentityFailedToConnectToCloudService</c> if there is no IP connection or the cloud service is not available.
            /// MLResult.Result will be <c>MLResult.Code.IdentityCloudAuthentication</c> if the user does not have the required privileges to use the requesting service.or the refresh token used by the service is invalid.
            /// MLResult.Result will be <c>MLResult.Code.IdentityInvalidInformationFromCloud</c> if the signature verification failed on the information returned by the cloud or a parsing error occurred.
            /// MLResult.Result will be <c>MLResult.Code.IdentityNotLoggedIn</c> if the user is not logged in to the cloud.
            /// MLResult.Result will be <c>MLResult.Code.IdentityExpiredCredentials</c> if the user's credentials have expired.
            /// MLResult.Result will be <c>MLResult.Code.IdentityFailedToGetUserProfile</c> if failed to retrieve attributes of the user's profile.
            /// MLResult.Result will be <c>MLResult.Code.IdentityUnauthorized</c> if the user is not authorized to execute the operation.
            /// MLResult.Result will be <c>MLResult.Code.IdentityCertificateError</c> if the device failed to authenticate the server.
            /// MLResult.Result will be <c>MLResult.Code.IdentityRejectedByCloud</c> if the cloud rejected the operation.
            /// MLResult.Result will be <c>MLResult.Code.IdentityAlreadyLoggedIn</c> if the user is already logged in.
            /// MLResult.Result will be <c>MLResult.Code.IdentityModifyIsNotSupported</c> if the cloud does not support modification of an attribute value.
            /// MLResult.Result will be <c>MLResult.Code.IdentityNetworkError</c> if the device is not connected to a network.
            /// </returns>
            public MLResult Fetch(Request.RequestAttibutesDelegate callback)
            {
                MLResult result;
                if (this.CurrentRequest != null)
                {
                    result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "Already fetching attributes");
                    MLPluginLog.ErrorFormat("MLIdentityProfile.Fetch failed. Reason: {0}", result);
                    return result;
                }

                this.Cleanup();
                this.CurrentRequest = new Request
                {
                    Callback = callback,
                    Result = MLResult.Code.Pending,
                    RequestState = Request.State.REQUESTING_ATTRIB_NAMES
                };
                MLIdentityNativeBindings.RequestAttributeNamesAsync(this.CurrentRequest, this.RequestAttributes, out this.attributes);

                result = MLResult.Create(this.CurrentRequest.Result == MLResult.Code.Pending ? MLResult.Code.Ok : this.CurrentRequest.Result);

                return result;
            }

            /// <summary>
            /// Handles when a request queries the attribute names.
            /// </summary>
            private void ProcessRequest()
            {
                switch (this.CurrentRequest.RequestState)
                {
                    case Request.State.REQUESTING_ATTRIB_NAMES:
                        this.ProcessPendingAttributeNamesRequest();
                        break;

                    case Request.State.ATTRIB_NAMES_READY:
                        MLIdentityNativeBindings.RequestAttributeValuesAsync(this.CurrentRequest, this.Attributes);
                        this.CurrentRequest.RequestState = Request.State.REQUESTING_ATTRIB_VALUES;
                        break;

                    case Request.State.REQUESTING_ATTRIB_VALUES:
                        this.ProcessPendingAttributeValuesRequest();
                        break;

                    case Request.State.DONE:
                        this.ProcessDoneRequest();
                        break;

                    default:
                        break;
                }
            }

            /// <summary>
            /// Handles when a request queries the attribute names.
            /// </summary>
            private void ProcessPendingAttributeNamesRequest()
            {
                this.CurrentRequest.Result = MLIdentityNativeBindings.RequestAttributeNames(this.CurrentRequest, out this.attributes);
                if (this.CurrentRequest.Result != MLResult.Code.Pending && this.CurrentRequest.Result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLIdentityProfile.ProcessPendingAttribNamesRequest failed this.CurrentRequest to retrieve attribute names. Reason: {0}", this.CurrentRequest.Result);
                    this.CurrentRequest.RequestState = Request.State.DONE;
                }
                else if (this.CurrentRequest.Result == MLResult.Code.Ok)
                {
                    this.CurrentRequest.RequestState = Request.State.ATTRIB_NAMES_READY;
                }

                this.ProcessRequest();
            }

            /// <summary>
            /// Handles when a request queries the attribute values.
            /// </summary>
            private void ProcessPendingAttributeValuesRequest()
            {
                this.CurrentRequest.Result = MLIdentityNativeBindings.RequestAttributeValues(this.CurrentRequest, this.attributes);
                if (this.CurrentRequest.Result != MLResult.Code.Pending && this.CurrentRequest.Result != MLResult.Code.Ok)
                {
                    MLPluginLog.ErrorFormat("MLIdentityProfile.ProcessPendingAttribValuesRequest failed this.CurrentRequest to retrieve attribute values. Reason: {0}", this.CurrentRequest.Result);
                    this.CurrentRequest.RequestState = Request.State.DONE;
                }
                else if (this.CurrentRequest.Result == MLResult.Code.Ok)
                {
                    this.CurrentRequest.RequestState = Request.State.DONE;
                }

                this.ProcessRequest();
            }

            /// <summary>
            /// Handles when a request has finished.
            /// </summary>
            private void ProcessDoneRequest()
            {
                Request temp = this.CurrentRequest;
                this.CurrentRequest = null;
                temp.Callback?.Invoke(MLResult.Create(temp.Result));
                temp = null;
            }

            /// <summary>
            /// Cleans up memory.
            /// </summary>
            private void Cleanup()
            {
                if (this.IsValid)
                {
                    MLIdentityNativeBindings.Cleanup();
                }

                this.attributes = null;
            }
            #endif

            /// <summary>
            /// Represents a CurrentRequest for profile attributes.
            /// </summary>
            public class Request
            {
                #if PLATFORM_LUMIN
                /// <summary>
                /// Gets or sets the delegate for the callback to notify when the request has finished.
                /// </summary>
                /// <param name="result">The MLResult of the request.</param>
                public delegate void RequestAttibutesDelegate(MLResult result);
                #endif

                /// <summary>
                /// The different states a request can be in.
                /// </summary>
                public enum State
                {
                    /// <summary>
                    /// The request was just created.
                    /// </summary>
                    CREATED,

                    /// <summary>
                    /// The request is currently querying for attribute names.
                    /// </summary>
                    REQUESTING_ATTRIB_NAMES,

                    /// <summary>
                    /// The request has finished querying for attribute names.
                    /// </summary>
                    ATTRIB_NAMES_READY,

                    /// <summary>
                    /// The request is currently querying for attribute values.
                    /// </summary>
                    REQUESTING_ATTRIB_VALUES,

                    /// <summary>
                    /// The request has finished querying for attribute values.
                    /// </summary>
                    ATTRIB_VALUES_READY,

                    /// <summary>
                    /// The request has finished.
                    /// </summary>
                    DONE
                }

                #if PLATFORM_LUMIN
                /// <summary>
                /// Gets or sets the pending future parameter which if set would mean our request to retrieve attribute names
                /// or values is pending and further requests are prevented.
                /// </summary>
                public MLIdentityNativeBindings.MLInvokeFuture PendingFuture { get; set; }

                /// <summary>
                /// Gets or sets the current state of the request.
                /// </summary>
                public State RequestState { get; set; }

                /// <summary>
                /// Gets or sets the callback to notify when the request has finished.
                /// </summary>
                public RequestAttibutesDelegate Callback { get; set; }

                /// <summary>
                /// Gets or sets the current result code of this request.
                /// </summary>
                public MLResult.Code Result { get; set; }
                #endif
            }
        }
    }
}
