// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLFoundObjectsNativeBindings.cs" company="Magic Leap, Inc">
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
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// Manages calls to the native MLFoundObjects bindings.
    /// </summary>
    public sealed partial class MLFoundObjects
    {
        /// <summary>
        /// Native bindings for MLFoundObjects.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// MLFoundObject library name.
            /// </summary>
            private const string MLFoundObjectDll = "ml_perception_client";

            /// <summary>
            /// The string value for the native property, label.
            /// </summary>
            private const string PropertyKeyLabel = "label";

            /// <summary>
            /// The string value for the native property, score.
            /// </summary>
            private const string PropertyKeyScore = "score";

            /// <summary>
            /// The maximum label size.
            /// </summary>
            private const uint MaxLabelSize = 64;

            /// <summary>
            /// The maximum property size.
            /// </summary>
            private const uint MaxPropertyKeySize = 64;

            /// <summary>
            /// The maximum property value size.
            /// </summary>
            private const uint MaxPropertyValueSize = 64;

            /// <summary>
            /// Initializes a new instance of the <see cref="NativeBindings"/> class.
            /// </summary>
            protected NativeBindings()
            {
            }

            /// <summary>
            /// The type of found object.
            /// </summary>
            public enum FoundObjectType
            {
                /// <summary>
                /// It's Invalid
                /// </summary>
                None = -1,

                /// <summary>
                /// It's an Object
                /// </summary>
                Object,
            }

            /// <summary>
            /// Create a found object query tracker.
            /// </summary>
            /// <param name="handle">A pointer to the handle of the found object query tracker.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectTrackerCreate(out ulong handle);

            /// <summary>
            /// Update the tracker settings.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="settings">A pointer of the found object tracker settings.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectTrackerUpdateSettings(ulong handle, in Settings settings);

            /// <summary>
            /// Create a new Found Object Query.
            /// Creates a new query for requesting found objects. Query criteria is
            /// specified by filling out the QueryFilterNative object.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="query">A pointer to the query filter applied to the search.</param>
            /// <param name="queryHandle">A pointer to the handle of the query.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if a privilege is missing or was denied.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectQuery(ulong handle, ref QueryFilterNative query, out ulong queryHandle);

            /// <summary>
            /// Gets the result count of a query.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="queryHandle">The handle to the active query.</param>
            /// <param name="numResults">A pointer to the number of max results from the query.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if a privilege is missing or was denied.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectGetResultCount(ulong handle, ulong queryHandle, out uint numResults);

            /// <summary>
            /// Get the result of a submitted query.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="queryHandle">The handle to the active query.</param>
            /// <param name="index">The index of the found object result.</param>
            /// <param name="foundObject">A pointer to the structure that will contain the found object information.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectGetResult(ulong handle, ulong queryHandle, uint index, ref FoundObjectNative foundObject);

            /// <summary>
            /// Gets the property information for a found object ID by index.
            /// This is not the data for a property. This is a MLFoundObjectProperty. If the
            /// property has a data size greater than zero and you would like to get the data you
            /// will have to call MLFoundObjectRequestPropertyData and then MLFoundObjectGetPropertyData.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="id">The universally unique identifier of the found object.</param>
            /// <param name="index">The index of the found object.</param>
            /// <param name="property">A pointer to the property of the found object.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectGetProperty(ulong handle, MLUUID id, uint index, ref PropertyNative property);

            /// <summary>
            /// Returns the count for all the unique labels for your current area.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="uniqueLabelCount">A count of all the unique labels in the area.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectGetAvailableLabelsCount(ulong handle, out uint uniqueLabelCount);

            /// <summary>
            /// Returns the unique label by index for your current area.
            /// Each found object has a label. To facilitate better understanding of the
            /// environment, you can get all the unique labels in the area. This is used for
            /// discovering what is available in the users area. Unique labels have the
            /// potential to change and expand as the area is explored.
            /// </summary>
            /// <param name="handle">A handle to the FoundObject query tracker.</param>
            /// <param name="uniqueLabelIndex">The index of the unique label you are fetching.</param>
            /// <param name="bufferSize">The size of the buffer for the label.</param>
            /// <param name="label">A pointer to the label of the found object.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
            public static extern MLResult.Code MLFoundObjectGetUniqueLabel(ulong handle, uint uniqueLabelIndex, uint bufferSize, [MarshalAs(UnmanagedType.LPStr)] out string label);

            /// <summary>
            /// Releases the resources assigned to the tracker.
            /// </summary>
            /// <param name="handle">A handle to the found object query tracker.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid handle was returned.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was provided.
            /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the operation failed with an unspecified error.
            /// </returns>
            [DllImport(MLFoundObjectDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLFoundObjectTrackerDestroy(ulong handle);

            /// <summary>
            /// Contains property information for the found object.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct PropertyNative
            {
                /// <summary>
                /// Key for an objects property. Type is string. Max size is defined by MLFoundObject_MaxPropertyKeySize.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxPropertyKeySize)]
                public char[] Key;

                /// <summary>
                /// Value for an objects property. Type is string. Max size is defined by MLFoundObject_MaxPropertyValueSize.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxPropertyValueSize)]
                public char[] Value;

                /// <summary>
                /// Last time this object was updated in UTC time. Not filled out when creating a query.
                /// </summary>
                public ulong LastUpdateEpochTimeNs;

                /// <summary>
                /// Sets the Key member of this object to the given string value.
                /// </summary>
                public string KeyData
                {
                    set
                    {
                        if (value != null)
                        {
                            this.Key = new char[MaxPropertyKeySize];
                            char[] keyToUse = value.ToCharArray();
                            for (int i = 0; i < keyToUse.Length; ++i)
                            {
                                this.Key[i] = keyToUse[i];
                            }
                        }
                    }
                }

                /// <summary>
                /// Sets the Value member of this object to the given string value.
                /// </summary>
                public string ValueData
                {
                    set
                    {
                        if (value != null)
                        {
                            this.Value = new char[MaxPropertyValueSize];
                            char[] valueToUse = value.ToCharArray();
                            for (int i = 0; i < valueToUse.Length; ++i)
                            {
                                this.Value[i] = valueToUse[i];
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Struct used to compose a query.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct QueryFilterNative
            {
                /// <summary>
                /// Version of the structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Valid ID for a found object. This can be obtained from sources such as a prior session.
                /// </summary>
                public MLUUID Id;

                /// <summary>
                /// Properties to be used as filters for the query.
                /// </summary>
                /// const FoundObjectProperty *properties;
                public IntPtr Properties;

                /// <summary>
                /// Number of attributes.
                /// </summary>
                public uint PropertiesCount;

                /// <summary>
                /// Vector3 float of where you want the spatial query to originate.
                /// </summary>
                public MLVec3f Center;

                /// <summary>
                /// Vector3 float of the max distance you want the spatial query to span relative to the center of the query.
                /// </summary>
                public MLVec3f MaxDistance;

                /// <summary>
                /// Maximum number of results. Used to allocate memory.
                /// </summary>
                public uint MaxResults;

                /// <summary>
                /// Sets a MLFoundObjects.Query.Filter with the given object's values.
                /// </summary>
                /// <param name="filter">A pointer to the found object query filter.</param>
                public MLFoundObjects.Query.Filter Data
                {
                    /// <summary>
                    /// Sets a MLFoundObjects.Query.Filter object with the given object's values.
                    /// </summary>
                    set
                    {
                        this.Version = 1;
                        this.Properties = IntPtr.Zero;

                        // Sets up the native properties array to pass into this native query filter.
                        if (value.Label != string.Empty)
                        {
                            PropertyNative[] properties = new PropertyNative[2];

                            PropertyNative propertyNative = new PropertyNative();
                            propertyNative.KeyData = PropertyKeyLabel;
                            propertyNative.ValueData = value.Label;
                            properties[this.PropertiesCount++] = propertyNative;

                            propertyNative = new PropertyNative();
                            propertyNative.KeyData = PropertyKeyScore;
                            propertyNative.ValueData = value.Confidence.ToString();
                            properties[this.PropertiesCount++] = propertyNative;

                            if (this.PropertiesCount > 0)
                            {
                                IntPtr arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf<PropertyNative>() * (int)this.PropertiesCount);
                                IntPtr walkPtr = arrayPtr;

                                for (int i = 0; i < this.PropertiesCount; ++i)
                                {
                                    Marshal.StructureToPtr(properties[i], walkPtr, true);
                                    walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<PropertyNative>());
                                }

                                this.Properties = arrayPtr;
                            }
                        }

                        this.Center = MLConvert.FromUnity(value.Center);
                        this.MaxDistance = MLConvert.FromUnity(value.MaxDistance);
                        this.MaxResults = (uint)value.MaxResults;
                    }
                }
            }

            /// <summary>
            /// Struct to represent a Found Object.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct FoundObjectNative
            {
                /// <summary>
                /// Gets the identifier of the Found Object.
                /// </summary>
                public MLUUID Id { get; private set; }

                /// <summary>
                /// Gets the number of properties.
                /// </summary>
                public uint PropertyCount { get; private set; }

                /// <summary>
                /// Gets the center position of found object.
                /// </summary>
                public MLVec3f Position { get; private set; }

                /// <summary>
                /// Gets the rotation of found object.
                /// </summary>
                public MLQuaternionf Rotation { get; private set; }

                /// <summary>
                /// Gets the Vector3 extents of the object where each dimension is defined as max-min.
                /// </summary>
                public MLVec3f Size { get; private set; }

                /// <summary>
                /// Gets the data from this object and uses it to initialize another object.
                /// </summary>
                public MLFoundObjects.FoundObject Data
                {
                    get
                    {
                        FoundObject fo = new FoundObject();
                        fo.Id = MLConvert.ToUnity(this.Id);
                        fo.Position = MLConvert.ToUnity(this.Position);
                        fo.Rotation = MLConvert.ToUnity(this.Rotation);
                        fo.Size = MLConvert.ToUnity(this.Size);
                        return fo;
                    }
                }
            }

            /// <summary>
            /// Settings for the found object tracker.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct Settings
            {
                /// <summary>
                /// Gets the version of the structure.
                /// </summary>
                public uint Version { get; private set; }

                /// <summary>
                /// Gets the max result returned by a query.
                /// </summary>
                public uint MaxQueryResult { get; private set; }

                /// <summary>
                /// Gets the maximum number of found objects to be stored.
                /// </summary>
                public uint MaxObjectCache { get; private set; }

                /// <summary>
                /// Sets a Settings object with the given object's values.
                /// </summary>
                public MLFoundObjects.Settings Data
                {
                    set
                    {
                        this.Version = 1;
                        this.MaxQueryResult = (uint)Mathf.Clamp(value.MaxQueryResult, 0, MLFoundObjects.MaxQueryResult);
                        this.MaxObjectCache = (uint)Mathf.Clamp(value.MaxObjectCache, 0, MLFoundObjects.MaxObjectCache);
                    }
                }
            }
        }
    }
}
#endif
