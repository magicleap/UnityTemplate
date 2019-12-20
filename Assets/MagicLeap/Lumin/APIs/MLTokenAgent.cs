// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLTokenAgent.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// A class for retrieving the client credentials.
    /// </summary>
    public sealed partial class MLTokenAgent
    {
        /// <summary>
        /// Only available on device error message.
        /// </summary>
        private const string DLLNotFoundError = "MLTokenAgent API is currently available only on device.";

        /// <summary>
        /// GetClientCredentials is a blocking function that accesses the cloud and
        /// returns a MLTokenAgentClientCredentials structure containing the users credentials and
        /// tokens for a particular service (Audience).
        /// The library deduces the Audience being requested from the name of the calling service.
        /// </summary>
        /// <param name="outCredentials">
        /// MLTokenAgentClientCredentials output reference to a structure which was created
        /// with all its fields by the library. It must only be released using
        /// ReleaseClientCredentials.
        /// </param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.TokenAgent*</c> if a token specific failure occurred during the operation.
        /// </returns>
        public static MLResult GetClientCredentials(out ClientCredentials outCredentials)
        {
            outCredentials = new ClientCredentials();

            try
            {
                MLResult.Code resultCode = MLTokenAgentNativeBindings.MLTokenAgentGetClientCredentials(ref outCredentials.ClientCredentialsPtr);
                MLResult result = MLResult.Create(resultCode);
                if (result.IsOk)
                {
                    outCredentials.ClientCredentialsNative = (MLTokenAgentNativeBindings.ClientCredentialsNative)Marshal.PtrToStructure(outCredentials.ClientCredentialsPtr, typeof(MLTokenAgentNativeBindings.ClientCredentialsNative));
                }

                return result;
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(DLLNotFoundError);
                throw;
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLTokenAgent.GetClientCredentials failed. Reason: API symbols not found.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLTokenAgent.GetClientCredentials failed. Reason: API symbols not found.");
            }
        }

        /// <summary>
        /// GetClientCredentialsAsync invokes the GetClientCredentials
        /// function asynchronously (in a different thread).
        /// </summary>
        /// <param name="outFuture">
        /// A pointer to an MLInvokeFuture pointer which provides the means to poll for completion and
        /// to retrieve the credentials returned by MLTokenAgentGetClientCredentials.
        /// This pointer will be freed by the library before returning from the first (and last)
        /// call to MLTokenAgentGetClientCredentialsWait, after the asynchronous call completed, that is
        /// after MLTokenAgentGetClientCredentialsWait returns any value that is not MLResult.Pending.
        /// </param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.TokenAgent*</c> if a token specific failure occurred during the operation.
        /// </returns>
        public static MLResult GetClientCredentialsAsync(ref IntPtr outFuture)
        {
            try
            {
                MLResult.Code resultCode = MLTokenAgentNativeBindings.MLTokenAgentGetClientCredentialsAsync(ref outFuture);
                return MLResult.Create(resultCode);
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(DLLNotFoundError);
                throw;
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLTokenAgent.GetClientCredentialsAsync failed. Reason: API symbols not found.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLTokenAgent.GetClientCredentialsAsync failed. Reason: API symbols not found.");
            }
        }

        /// <summary>
        /// Having made a call to MLTokenAgentGetClientCredentialsAsync, the user can call
        /// MLTokenAgentGetClientCredentialsWait to detect whether the asynchronous call completed and if
        /// so, to retrieve the credentials in outCredentials. The call to MLTokenAgentGetClientCredentialsWait
        /// blocks until either <c>msecTimeout</c> <c>msec</c> elapses or the asynchronous function completes.
        /// </summary>
        /// <param name="future">
        /// MLInvokeFuture returned by the GetClientCredentialsAsync function
        /// </param>
        /// <param name="msecTimeout">
        /// The timeout in milliseconds.
        /// </param>
        /// <param name="outCredentials">
        /// Output reference where the credentials structure created
        /// by the library will be copied, if the asynchronous call completed.
        /// The location pointed to by out_credentials was set to the address of the
        /// MLTokenAgentClientCredentials structure allocated by the library.
        /// The returned credentials must be released when no longer needed by calling
        /// MLTokenAgentReleaseClientCredentials
        /// If any other value is returned, the location pointed to by outCredentials is set to
        /// 0 (null).
        /// </param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully before the timeout elapsed.
        /// MLResult.Result will be <c>MLResult.Code.Pending</c> if the timeout elapsed before the operation completed.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.TokenAgent*</c> if a token specific failure occurred during the operation.
        /// </returns>
        /// <remarks>
        /// Note:
        /// 1. To poll without blocking use <c>msecTimeout</c> = 0
        /// 2. After the function returns true, future is freed by the library and must not be used again in a call to MLTokenAgentGetClientCredentialsWait.
        /// </remarks>
        public static MLResult GetClientCredentialsWait(
            MLIdentityNativeBindings.MLInvokeFuture future,
            uint msecTimeout,
            out ClientCredentials outCredentials)
        {
            outCredentials = new ClientCredentials();

            try
            {
                // Attempt to get data if available.
                MLResult.Code resultCode = MLTokenAgentNativeBindings.MLTokenAgentGetClientCredentialsWait(future.InvokeFuturePtr, msecTimeout, ref outCredentials.ClientCredentialsPtr);
                MLResult result = MLResult.Create(resultCode);

                // If it succeeded, copy any modifications made to the credentials in unmanaged memory by the TokenAgent API to managed memory.
                if (result.IsOk)
                {
                    outCredentials.ClientCredentialsNative = (MLTokenAgentNativeBindings.ClientCredentialsNative)Marshal.PtrToStructure(outCredentials.ClientCredentialsPtr, typeof(MLTokenAgentNativeBindings.ClientCredentialsNative));
                }

                return result;
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(DLLNotFoundError);
                throw;
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLTokenAgent.GetClientCredentialsWait failed. Reason: API symbols not found.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLTokenAgent.GetClientCredentialsWait failed. Reason: API symbols not found.");
            }
        }

        /// <summary>
        /// ReleaseClientCredentials releases all resources associated with the
        /// MLTokenAgentClientCredentials structure that was returned by the library.
        /// </summary>
        /// <param name="credentials">
        /// MLTokenAgentClientCredentials reference to a structure received using one of:
        /// GetClientCredentials or GetClientCredentialsWait.
        /// </param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully before the timeout elapsed.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// </returns>
        public static MLResult ReleaseClientCredentials(ref ClientCredentials credentials)
        {
            try
            {
                MLResult result;
                if (credentials.Valid)
                {
                    // Make the native call with correct internal parameter.
                    MLResult.Code resultCode = MLTokenAgentNativeBindings.MLTokenAgentReleaseClientCredentials(credentials.ClientCredentialsPtr);
                    result = MLResult.Create(resultCode);
                    if (!result.IsOk)
                    {
                        MLPluginLog.WarningFormat("MLTokenAgent.ReleaseClientCredentials failed request to release client credentials. Reason: {0}", result);
                    }
                }
                else
                {
                    result = MLResult.Create(MLResult.Code.InvalidParam, "Invalid credentials parameter");
                }

                // Null the local representation of these pointers as they should have been freed by the call above.
                credentials.ClientCredentialsPtr = IntPtr.Zero;

                return result;
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error(DLLNotFoundError);
                throw;
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLTokenAgent.ReleaseClientCredentials failed. Reason: API symbols not found.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLTokenAgent.ReleaseClientCredentials failed. Reason: API symbols not found.");
            }
        }

        /// <summary>
        /// The credentials that can be used to for a user to access a particular service (Audience).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Credentials
        {
            /// <summary>
            /// Null terminated string allocated and released by the library.
            /// </summary>
            public string AccessKeyId;

            /// <summary>
            /// Null terminated string allocated and released by the library.
            /// </summary>
            public string SecretKey;

            /// <summary>
            /// Null terminated string allocated and released by the library.
            /// </summary>
            public string SessionToken;
        }

        /// <summary>
        /// MLTokenAgentTokens contains tokens that are used to read and modify the user profile.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Tokens
        {
            /// <summary>
            /// The idToken contains information from the user profile.
            /// It is a null terminated string that is allocated and released by the library.
            /// </summary>
            public string IdToken;

            /// <summary>
            /// The accessToken is the token that can be used to modify attributes of the user profile.
            /// It is a null terminated string that is allocated and released by the library.
            /// </summary>
            public string AccessToken;
        }

        /// <summary>
        /// Public representation of MLTokenAgentClientCredentials.
        /// MLTokenAgentClientCredentials represents the credentials and tokens of the User-Audience pair
        /// that is associated with the calling service.
        /// </summary>
        public struct ClientCredentials
        {
            /// <summary>
            /// Internal pointer used to associate the the managed version of this structure
            /// with the unmanaged pointer that is used by the TokenAgent API to communicate with us.
            /// </summary>
            internal IntPtr ClientCredentialsPtr;

            /// <summary>
            /// Internal convenience structure which represents the marshaled unmanaged structure
            /// to communicate with the TokenAgent API.
            /// </summary>
            internal MLTokenAgentNativeBindings.ClientCredentialsNative ClientCredentialsNative;

            /// <summary>
            /// Gets a value indicating whether the client credentials are valid.
            /// </summary>
            public bool Valid
            {
                get
                {
                    return this.ClientCredentialsPtr != IntPtr.Zero;
                }
            }

            /// <summary>
            /// Gets or sets the client credentials.
            /// </summary>
            public Credentials Credentials
            {
                get
                {
                    return this.ClientCredentialsNative.Credentials;
                }

                set
                {
                    this.ClientCredentialsNative.Credentials = value;
                }
            }

            /// <summary>
            /// Gets or sets the tokens for the client credentials.
            /// </summary>
            public Tokens Tokens
            {
                get
                {
                    return this.ClientCredentialsNative.Tokens;
                }

                set
                {
                    this.ClientCredentialsNative.Tokens = value;
                }
            }
        }
    }
}

#endif
