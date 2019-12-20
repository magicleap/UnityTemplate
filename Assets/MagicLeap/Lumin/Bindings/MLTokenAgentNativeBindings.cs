// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLTokenAgentNativeBindings.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591

namespace UnityEngine.XR.MagicLeap.Native
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A class for retrieving the client credentials.
    /// See ml_token_agent.h for additional comments
    /// </summary>
    public class MLTokenAgentNativeBindings : MagicLeapNativeBindings
    {
        /// <summary>
        /// MLIdentity library name.
        /// </summary>
        private const string MLIdentityDll = "ml_identity";

        /// <summary>
        /// Initializes a new instance of the <see cref="MLTokenAgentNativeBindings" /> class.
        /// </summary>
        protected MLTokenAgentNativeBindings()
        {
        }

        /// <summary>
        /// A blocking function that accesses the cloud and returns a pointer to a structure
        /// containing the users credentials and tokens for a particular service(Audience).
        /// </summary>
        /// <param name="outCredentials">A pointer to the client credentials.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the out_credentials was 0 (null).
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if the operation failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if the caller does not have the ClientCredentialsRead privilege.
        /// MLResult.Result will be <c>MLResult.Code.TokenAgent*</c> if a token specific failure occurred during the operation.
        /// </returns>
        [DllImport(MLIdentityDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLTokenAgentGetClientCredentials(ref IntPtr outCredentials);

        /// <summary>
        /// Invokes the MLTokenAgentGetClientCredentials() function asynchronously (in a different thread).
        /// </summary>
        /// <param name="outFuture">A pointer to a MLInvokeFuture pointer which provides the ability to poll for completion and
        /// to retrieve the credentials returned by MLTokenAgentGetClientCredentials().</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the profile or out_future were 0 (null).
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if the operation failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if the caller does not have the ClientCredentialsRead privilege.
        /// MLResult.Result will be <c>MLResult.Code.TokenAgent*</c> if a token specific failure occurred during the operation.
        /// </returns>
        [DllImport(MLIdentityDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLTokenAgentGetClientCredentialsAsync(ref IntPtr outFuture);

        /// <summary>
        /// Having made a call to MLTokenAgentGetClientCredentialsAsync(), the user can call MLTokenAgentGetClientCredentialsWait()
        /// to detect whether the asynchronous call completed and if so, to retrieve the credentials in out_credentials.
        /// </summary>
        /// <param name="future">The pointer returned by the MLTokenAgentGetClientCredentialsAsync() function.</param>
        /// <param name="msecTimeout">The timeout in milliseconds.</param>
        /// <param name="outCredentials">The location in memory where the pointer to the credentials
        /// structure allocated by the library will be copied, if the asynchronous call completed,
        /// or 0 (null) if not.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully before the timeout elapsed.
        /// MLResult.Result will be <c>MLResult.Code.Pending</c> if the timeout elapsed before the asynchronous call completed.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the future or out_credentials were 0 (null).
        /// MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if the operation failed to allocate memory.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if the caller does not have the ClientCredentialsRead privilege.
        /// MLResult.Result will be <c>MLResult.Code.TokenAgent*</c> if a token specific failure occurred during the operation.
        /// </returns>
        [DllImport(MLIdentityDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLTokenAgentGetClientCredentialsWait(
            IntPtr future,
            uint msecTimeout,
            ref IntPtr outCredentials);

        /// <summary>
        /// MLTokenAgentReleaseClientCredentials() releases all resources associated with the
        /// client credentials structure that was returned by the library.
        /// </summary>
        /// <param name="credentials">A pointer to the client credentials to release.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the operation completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the credentials was 0 (null).
        /// </returns>
        [DllImport(MLIdentityDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLTokenAgentReleaseClientCredentials(IntPtr credentials);

        /// <summary>
        /// Gets the result string for a MLResult.Code.
        /// </summary>
        /// <param name="result">The MLResult.Code to be requested.</param>
        /// <returns>A pointer to the result string.</returns>
        [DllImport(MLIdentityDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MLTokenAgentGetResultString(MLResult.Code result);

        /// <summary>
        /// Internal raw representation of C API's MLTokenAgentClientCredentials.
        /// MLTokenAgentClientCredentials represents the credentials and tokens of the User-Audience pair
        /// that is associated with the calling service.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ClientCredentialsNative
        {
            /// <summary>
            /// The credentials that can be used to access a particular service (Audience).
            /// </summary>
            public MLTokenAgent.Credentials Credentials;

            /// <summary>
            /// The tokens that can be used to manage the user profile for a particular Audience.
            /// </summary>
            public MLTokenAgent.Tokens Tokens;
        }
    }
}

#endif
