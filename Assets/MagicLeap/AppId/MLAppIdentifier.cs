// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAppIdentifier.cs" company="Magic Leap, Inc">
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

    /// <summary>
    /// MLAppIdentifier API used for querying specific application information.
    /// </summary>
    public partial class MLAppIdentifier : MLAutoAPISingleton<MLAppIdentifier>
    {
        /// <summary>
        /// The type of identification.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Represents no Identifier.
            /// </summary>
            None,

            /// <summary>
            /// Represents an App Instance Identifier.
            /// </summary>
            AppInstanceId
        }

        /// <summary>
        /// The max size of the string that a type of identifier can have.
        /// </summary>
        public enum MaxSize
        {
            /// <summary>
            /// Max size for the AppInstance identifier string.
            /// </summary>
            AppInstanceId = 255
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Get the app instance identifier.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        public static string GetAppInstanceId()
        {
            return Instance.MLIdentifierGetResult(Type.AppInstanceId).Identifier;
        }

        /// <summary>
        /// Starts the API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        protected override MLResult.Code StartAPI() => NativeBindings.MLIdentifierCreate(ref this.Handle);

        /// <summary>
        /// Stops the API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        protected override MLResult.Code StopAPI() => NativeBindings.MLIdentifierDestroy(this.Handle);

        /// <summary>
        /// Get the results of an identifier requested. This function can be used to poll results from the Identifier instance.
        /// </summary>
        /// <param name="identifierType">The type of identifier to query for.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        private Info MLIdentifierGetResult(Type identifierType)
        {
            NativeBindings.MLIdentifierParams queryParams = NativeBindings.MLIdentifierParams.Create(identifierType);
            MLResult.Code resultCode = NativeBindings.MLIdentifierGetResult(this.Handle, ref queryParams, out NativeBindings.MLIdentifierData data);

            if (!MLAppIdentifier.DidNativeCallSucceed(resultCode))
            {
                throw new Exception($"NativeBindings.MLIdentifierGetResult failed, reason: {resultCode}");
            }

            Info resultInfo = Info.Create(data.Id, data.IdentifierType);
            return resultInfo;
        }
#endif
        /// <summary>
        /// Struct representing the type of info for an identifier.
        /// </summary>
        private struct Info
        {
            /// <summary>
            /// The identifier's value.
            /// </summary>
            public string Identifier;

            /// <summary>
            /// The type of identifier.
            /// </summary>
            public Type IdentifierType;

            /// <summary>
            /// Create and initialize an Info struct object.
            /// </summary>
            /// <param name="id">The identifier to use.</param>
            /// <param name="type">THe type of identifier to use.</param>
            /// <returns>An initialized struct object.</returns>
            public static Info Create(string id, Type type)
            {
                Info appInfo = new Info();
                appInfo.Identifier = id;
                appInfo.IdentifierType = type;
                return appInfo;
            }

            /// <summary>
            /// Override to display the contents of the struct as a string.
            /// </summary>
            /// <returns>A string representation of this struct.</returns>
            public override string ToString()
            {
                return $"Identifier: {this.Identifier},  IdentifierType: {this.IdentifierType}";
            }
        }
    }
}
