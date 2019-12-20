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
    /// <summary>
    /// MLIdentityAttribute represents an attribute of a user's profile
    /// (for instance: name, address, email). Each attribute has a name (represented by key and value).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    [Obsolete("Use MLIdentity.Attribute instead.", true)]
    public struct MLIdentityAttribute
    {
        /// <summary>
        /// The enum value.
        /// </summary>
        public MLIdentityAttributeKey Key;
        /// <summary>
        /// Attribute Name.
        /// </summary>
        public string Name;
        /// <summary>
        /// Attribute's string value. Call GetValueAs* functions to get
        /// the value of the attribute as other types (eg. int, float etc )
        /// </summary>
        public string Value;

        /// <summary>
        /// The Attribute is requested.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool IsRequested;

        /// <summary>
        /// The Attribute is granted.
        /// </summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool IsGranted;
    }

    /// <summary>
    /// Internal raw representation of C API's MLIdentityProfile.
    /// MLIdentityProfile represents a set of attribute of a user's profile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Obsolete("Use MLIdentityNativeBindings.ProfileNative instead.", true)]
    internal struct InternalMLIdentityProfile
    {
        /// <summary>
        /// attributeCount is the number of MLIdentityAttribute structures pointed
        /// to by attributePtrs.
        /// </summary>
        public uint AttributeCount;

        /// <summary>
        /// attributePtrs is an array of MLIdentityAttributes containing user profile
        /// </summary>
        public IntPtr AttributePtrs;
    }


    /// <summary>
    /// MLInvokeFuture represents a type which is opaque (incomplete) to users of this library.
    /// A pointer to an MLInvokeFuture is returned by the Async function.
    /// Users pass it to the Wait function to determine if the asynchronous method has
    /// completed and to retrieve the result if it has.
    /// </summary>
    [Obsolete("Use MLIdentityNativeBindings.MLInvokeFuture instead.", true)]
    public class MLInvokeFuture
    {
        // This pointer is marked internal to avoid exposing it to the user, however it cannot
        // be private as other objects (within this assembly) use it and require access to it.
        internal IntPtr _invokeFuturePtr;
    }
    #endif

    /// <summary>
    /// MLIdentityAttributeKey identifies an attribute in a user profile.
    /// Attributes that were not known at the time the library was built, are marked as Unknown.
    /// </summary>
    [Obsolete("Use MLIdentity.Attribute.Keys instead.", true)]
    public enum MLIdentityAttributeKey
    {
        //// TODO: Need more information on these values.
        Unknown,
        GivenName,
        FamilyName,
        Email,
        Bio,
        PhoneNumber,
        Avatar2D,
        Avatar3D,
    }
}
