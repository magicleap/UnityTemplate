// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLAppIdentifierNativeBindings.cs" company="Magic Leap, Inc">
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
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLAppIdentifier API used for querying specific application information.
    /// </summary>
    public partial class MLAppIdentifier
    {
        /// <summary>
        /// Native bindings for the MLAppIdentifier API.
        /// </summary>
        internal class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Internal DLL used by the API.
            /// </summary>
            private const string MLAppIdentifierDll = "ml_settings";
                        
            /// <summary>
            /// Creates an identifier instance.
            /// </summary>
            /// <param name="idHandle">The API identifier instance handle.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
            /// </returns>
            [DllImport(MLAppIdentifierDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLIdentifierCreate(ref ulong idHandle);

            /// <summary>
            /// Destroys an identifier instance.
            /// </summary>
            /// <param name="idHandle">The API identifier instance handle.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
            /// </returns>
            [DllImport(MLAppIdentifierDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLIdentifierDestroy(ulong idHandle);

            /// <summary>
            /// Get the results for the identifier requested. This function can be used to poll results from the identifier instance.
            /// </summary>
            /// <param name="idHandle">The API identifier instance handle.</param>
            /// <param name="queryParams">The query parameters for which identifier to query for.</param>
            /// <param name="identifierData">The data of the requested identifier.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
            /// </returns>
            [DllImport(MLAppIdentifierDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLIdentifierGetResult(ulong idHandle, ref MLIdentifierParams queryParams, out MLIdentifierData identifierData);
            
            /// <summary>
            /// Represents the result of an identifier query To get this the API MLIdentifierGetResult needs to be polled
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLIdentifierData
            {
                /// <summary>
                /// enum representing the type of identifier
                /// </summary>
                public Type IdentifierType;

                /// <summary>
                /// The C string carrying identifier data which is UTF-8 and null terminated.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)MaxSize.AppInstanceId + 1)]
                public string Id;
            }
                        
            /// <summary>
            /// The parameters for requesting an identifier.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLIdentifierParams
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Type of the identifier requested.
                /// </summary>
                public Type IdentifierTypeRequested;

                /// <summary>
                /// Creates and returns an initialized version of this struct.
                /// </summary>
                /// <param name="identifierType">The type of identifier.</param>
                /// <returns>An initialized version of this struct.</returns>
                public static MLIdentifierParams Create(Type identifierType)
                {
                    return new MLIdentifierParams()
                    {
                        Version = 1,
                        IdentifierTypeRequested = identifierType
                    };
                }
            }
        }
    }
}

#endif
