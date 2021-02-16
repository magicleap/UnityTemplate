// %BANNER_BEGIN%
// ---------------------------------------------------------------------
//
// attention EXPERIMENTAL
//
// %COPYRIGHT_BEGIN%
// <copyright file="MLFoundObjects.cs" company="Magic Leap, Inc">
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// Manages calls to the native MLFoundObjects bindings.
    /// </summary>
    public sealed partial class MLFoundObjects : MLAPISingleton<MLFoundObjects>
    {
        /// <summary>
        /// The maximum number of results that the query can return.
        /// </summary>
        public const int MaxQueryResult = 256;

        /// <summary>
        /// The maximum number of results that the API can internally cache.
        /// </summary>
        public const int MaxObjectCache = 1024;

#if PLATFORM_LUMIN
        /// <summary>
        /// Stores all pending found object queries.
        /// </summary>
        private ConcurrentDictionary<ulong, Query> pendingQueries = new ConcurrentDictionary<ulong, Query>();

        /// <summary>
        /// Keeps the queries that were completed on a specific frame.
        /// </summary>
        private List<ulong> completedQueries = new List<ulong>();

        /// <summary>
        /// Keeps the queries that failed on a specific frame.
        /// </summary>
        private List<ulong> errorQueries = new List<ulong>();

        /// <summary>
        /// Stores the found object system tracker.
        /// </summary>
        private ulong handle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// The current settings for the found object tracker.
        /// </summary>
        private Settings settings = Settings.Create();
#endif
        /// <summary>
        /// Delegate used for when unique objects are queried for.
        /// </summary>
        /// <param name="result">The MLResult of trying to update the tracker settings.</param>
        /// <param name="foundObjects">The array of unique found objects currently tracked.</param>
        public delegate void QueryResultsDelegate(MLResult result, FoundObject[] foundObjects);

        /// <summary>
        /// Delegate used for when unique object labels are queried for.
        /// </summary>
        /// <param name="result">The MLResult of trying to update the tracker settings.</param>
        /// <param name="labels">The array of unique found object labels currently tracked.</param>
        public delegate void OnGetUniqueObjectLabelsDelegate(MLResult result, string[] labels);

        /// <summary>
        /// Delegate used for the found object tracker settings are updated.
        /// </summary>
        /// <param name="result">The MLResult of trying to update the tracker settings.</param>
        /// <param name="newSettings">The settings that the tracker now has.</param>
        public delegate void OnUpdateSettingsDelegate(MLResult result, Settings newSettings);

#if PLATFORM_LUMIN
        /// <summary>
        /// Starts the MLFoundObjects API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if failed due to internal error.
        /// </returns>
        public static MLResult Start()
        {
            CreateInstance();
            return MLFoundObjects.BaseStart(true);
        }

        /// <summary>
        /// Request the list of detected found objects.
        /// Callback will never be called while request is still pending.
        /// </summary>
        /// <param name="queryFilter">Filter used to customize query results.</param>
        /// <param name="callback">
        /// Callback used to report query results.
        /// Callback MLResult code will never be <c>MLResult.Code.Pending</c>.
        /// </param>
        /// <returns>
        /// MLResult.Result inside callback will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result inside callback will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result inside callback will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        public static MLResult GetObjectsAsync(Query.Filter queryFilter, QueryResultsDelegate callback)
        {
            if (MLFoundObjects.IsValidInstance())
            {
                // Don't allow null callbacks to be registered.
                if (callback == null)
                {
                    MLPluginLog.Error("MLFoundObjects.GetObjects failed. Reason: Passed input callback is null.");
                    return MLResult.Create(MLResult.Code.InvalidParam);
                }

                MLThreadDispatch.ScheduleWork(() =>
                {
                    _instance.BeginObjectQueryAsync(queryFilter, callback);
                    return true;
                });

                return MLResult.Create(MLResult.Code.Ok);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLFoundObjects.GetObjects failed. Reason: No Instance for MLFoundObjects");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLFoundObjects.GetFoundObjects failed. Reason: No Instance for MLFoundObjects");
            }
        }

        /// <summary>
        /// Updates the settings of the found objects tracker.
        /// </summary>
        /// <param name="newSettings">The new settings to update the found objects tracker with.</param>
        /// <param name="callback">The callback to invoke when the settings have been updated or failed doing so.</param>
        /// <returns>
        /// MLResult.Result inside callback will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result inside callback will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result inside callback will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        public static MLResult UpdateSettingsAsync(Settings newSettings, OnUpdateSettingsDelegate callback = null)
        {
            MLResult result = MLResult.Create(MLResult.Code.Ok);
            MLThreadDispatch.ScheduleWork(() =>
            {
                if (MLFoundObjects.IsValidInstance())
                {
                    NativeBindings.Settings settingsNative = new NativeBindings.Settings();
                    settingsNative.Data = newSettings;

                    MLResult.Code resultCode = NativeBindings.MLFoundObjectTrackerUpdateSettings(_instance.handle, in settingsNative);

                    if (MLResult.IsOK(resultCode))
                    {
                        MLThreadDispatch.ScheduleMain(() =>
                        {
                            result = MLResult.Create(resultCode);
                            _instance.settings = newSettings;
                            callback?.Invoke(result, _instance.settings);
                        });
                    }
                    else
                    {
                        MLPluginLog.ErrorFormat("MLFoundObjects.UpdateSettingsAsync failed to update settings. Reason: {0}", MLResult.CodeToString(resultCode));
                        result = MLResult.Create(MLResult.Code.UnspecifiedFailure, string.Format("MLFoundObjects.UpdateSettingsAsync failed to update settings. Reason: {0}", MLResult.CodeToString(resultCode)));
                        MLThreadDispatch.ScheduleMain(() =>
                        {
                            callback?.Invoke(result, _instance.settings);
                        });
                    }
                }
                else
                {
                    MLPluginLog.ErrorFormat("MLFoundObjects.GetObjects failed. Reason: No Instance for MLFoundObjects");
                    result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLFoundObjects.GetFoundObjects failed. Reason: No Instance for MLFoundObjects");
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        callback?.Invoke(result, _instance.settings);
                    });
                }

                return true;
            });
            return result;
        }

        /// <summary>
        /// Request the list of detected found objects.
        /// Callback will never be called while request is still pending.
        /// </summary>
        /// <param name="callback">
        /// Callback used to report query results.
        /// Callback MLResult code will never be <c>MLResult.Code.Pending</c>.
        /// </param>
        /// <returns>
        /// MLResult.Result inside callback will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result inside callback will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result inside callback will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        public static MLResult GetUniqueObjectLabelsAsync(OnGetUniqueObjectLabelsDelegate callback)
        {
            MLThreadDispatch.ScheduleWork(() =>
            {
                string[] labels;

                MLResult result = MLResult.Create(MLResult.Code.Ok);
                if (MLFoundObjects.IsValidInstance())
                {
                    MLResult.Code resultCode = NativeBindings.MLFoundObjectGetAvailableLabelsCount(_instance.handle, out uint labelCount);

                    result = MLResult.Create(resultCode);
                    labels = new string[labelCount];
                    if (MLResult.IsOK(resultCode))
                    {
                        for (uint i = 0; i < labelCount; ++i)
                        {
                            resultCode = NativeBindings.MLFoundObjectGetUniqueLabel(_instance.handle, i, 20, out string label);

                            if (MLResult.IsOK(resultCode))
                            {
                                labels[i] = label;
                            }
                            else
                            {
                                MLPluginLog.ErrorFormat("MLFoundObjects.GetUniqueObjectLabels failed getting a unique label. Reason: {0}", MLResult.CodeToString(resultCode));
                                result = MLResult.Create(MLResult.Code.UnspecifiedFailure, string.Format("MLFoundObjects.GetUniqueObjectLabels failed getting a unique label. Reason: {0}", MLResult.CodeToString(resultCode)));
                            }
                        }
                    }
                    else
                    {
                        MLPluginLog.ErrorFormat("MLFoundObjects.GetUniqueObjectLabels failed getting the unique label count. Reason: {0}", MLResult.CodeToString(resultCode));
                        result = MLResult.Create(MLResult.Code.UnspecifiedFailure, string.Format("MLFoundObjects.GetUniqueObjectLabels failed getting the unique label count. Reason: {0}", MLResult.CodeToString(resultCode)));
                    }
                }
                else
                {
                    labels = new string[0];
                    MLPluginLog.ErrorFormat("MLFoundObjects.GetUniqueObjectLabelsAsync failed. Reason: No Instance for MLFoundObjects.");
                    result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLFoundObjects.GetUniqueObjectLabelsAsync failed. Reason: No Instance for MLFoundObjects");
                }

                MLThreadDispatch.ScheduleMain(() =>
                {
                    callback?.Invoke(result, labels);
                });

                return true;
            });

            return MLResult.Create(MLResult.Code.Ok);
        }

        /// <summary>
        /// Starts the found object requests, Must be called to start receiving found object results from the underlying system.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        protected override MLResult StartAPI()
        {
            _instance.handle = MagicLeapNativeBindings.InvalidHandle;
            return _instance.CreateTracker();
        }

        /// <summary>
        /// Cleans up unmanaged memory.
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects">Allow complete cleanup of the API.</param>
        protected override void CleanupAPI(bool isSafeToAccessManagedObjects)
        {
            if (isSafeToAccessManagedObjects)
            {
                _instance.pendingQueries.Clear();
            }

            _instance.DestroyNativeTracker();
        }

        /// <summary>
        /// Polls for the result of pending found object requests.
        /// </summary>
        protected override void Update()
        {
            _instance.ProcessPendingQueriesAsync();
        }

        /// <summary>
        /// static instance of the MLFoundObjects class
        /// </summary>
        private static void CreateInstance()
        {
            if (!MLFoundObjects.IsValidInstance())
            {
                MLFoundObjects._instance = new MLFoundObjects();
            }
        }

        /// <summary>
        /// Create a new found object native tracker.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult CreateTracker()
        {
            try
            {
                MLResult.Code resultCode = NativeBindings.MLFoundObjectTrackerCreate(out _instance.handle);
                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLFoundObjects.CreateTracker failed to initialize native tracker. Reason: {0}", resultCode);
                }

                return MLResult.Create(resultCode);
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLFoundObjects.CreateTracker failed. Reason: API symbols not found");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLFoundObjects.CreateTracker failed. Reason: API symbols not found");
            }
        }

        /// <summary>
        /// Begin querying for found objects.
        /// </summary>
        /// <param name="filter">Filter to use for this query.</param>
        /// <param name="callback">Callback used to report query results.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        private MLResult BeginObjectQueryAsync(Query.Filter filter, QueryResultsDelegate callback)
        {
            try
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(_instance.handle))
                {
                    MLPluginLog.Error("MLFoundObjects.BeginObjectQuery failed to request found objects. Reason: Tracker handle is invalid");
                    return MLResult.Create(MLResult.Code.InvalidParam);
                }

                NativeBindings.QueryFilterNative nativeQueryFilter = new NativeBindings.QueryFilterNative();
                nativeQueryFilter.Data = filter;

                MLResult.Code resultCode = NativeBindings.MLFoundObjectQuery(_instance.handle, ref nativeQueryFilter, out ulong queryHandle);
                MLResult result = MLResult.Create(resultCode);

                if (!result.IsOk)
                {
                    MLPluginLog.ErrorFormat("MLFoundObjects.BeginObjectQuery failed to request objects. Reason: {0}", resultCode);
                    return result;
                }

                // Add query to the list of pendingQueries.
                Query query = Query.Create(callback, filter);
                MLFoundObjects._instance.pendingQueries.TryAdd(queryHandle, query);

                return result;
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLFoundObjects.BeginObjectQuery failed. Reason: API symbols not found");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLFoundObjects.BeginObjectQuery failed. Reason: API symbols not found");
            }
        }

        /// <summary>
        /// Process pending requests and call the callback specified in the startup config.
        /// </summary>
        private void ProcessPendingQueriesAsync()
        {
            MLThreadDispatch.ScheduleWork(() =>
            {
                try
                {
                    if (_instance.pendingQueries.Count > 0)
                    {
                        // Process each individual pending query to get updated status.
                        foreach (ulong handle in _instance.pendingQueries.Keys)
                        {
                            // Request the update.
                            MLResult.Code resultCode = NativeBindings.MLFoundObjectGetResultCount(_instance.handle, handle, out uint resultCount);

                            if (MLResult.IsOK(resultCode))
                            {
                                FoundObject[] foundObjects = new FoundObject[resultCount];

                                // For each found object in the query, get it's reference and property values from the API.
                                for (uint objectIndex = 0; objectIndex < resultCount; objectIndex++)
                                {
                                    NativeBindings.FoundObjectNative nativeFoundObject = new NativeBindings.FoundObjectNative();

                                    // Get the object reference from the API.
                                    resultCode = NativeBindings.MLFoundObjectGetResult(_instance.handle, handle, objectIndex, ref nativeFoundObject);

                                    if (MLResult.IsOK(resultCode))
                                    {
                                        Dictionary<string, string> properties = new Dictionary<string, string>();

                                        // Get the object's properties from the API.
                                        if (nativeFoundObject.PropertyCount > 0)
                                        {
                                            NativeBindings.PropertyNative objectProperty = new NativeBindings.PropertyNative();
                                            for (uint propertyIndex = 0; propertyIndex < nativeFoundObject.PropertyCount; propertyIndex++)
                                            {
                                                resultCode = NativeBindings.MLFoundObjectGetProperty(_instance.handle, nativeFoundObject.Id, propertyIndex, ref objectProperty);

                                                if (MLResult.IsOK(resultCode))
                                                {
                                                    properties.Add(new string(objectProperty.Key).Replace("\0", string.Empty).ToLower(), new string(objectProperty.Value).Replace("\0", string.Empty));
                                                }
                                                else
                                                {
                                                    MLPluginLog.ErrorFormat("MLFoundObjects.ProcessPendingQueries failed to get found object property. Reason: {0}", MLResult.CodeToString(resultCode));
                                                }
                                            }
                                        }

                                        FoundObject foundObject = nativeFoundObject.Data;

                                        // Currently the only valid object properties are: label, score
                                        foundObject.Label = properties.ContainsKey("label") ? properties["label"] : string.Empty;
                                        foundObject.Confidence = properties.ContainsKey("score") ? Convert.ToSingle(properties["score"]) : 0f;
                                        foundObjects[objectIndex] = foundObject;
                                    }
                                    else
                                    {
                                        MLPluginLog.ErrorFormat("MLFoundObjects.ProcessPendingQueries failed to get found object. Reason: {0}", MLResult.CodeToString(resultCode));
                                        _instance.errorQueries.Add(handle);
                                    }
                                }

                                if (!_instance.completedQueries.Contains(handle))
                                {
                                    _instance.completedQueries.Add(handle);
                                }

                                Query query = _instance.pendingQueries[handle];

                                // Dispatch list of found objects back to main.
                                MLThreadDispatch.ScheduleMain(() =>
                                {
                                    query.Callback(MLResult.Create(resultCode), foundObjects);
                                });
                            }
                            else
                            {
                                MLPluginLog.ErrorFormat("MLFoundObjects.ProcessPendingQueries failed to query found objects. Reason: {0}", MLResult.CodeToString(resultCode));
                                _instance.errorQueries.Add(handle);
                            }
                        }

                        foreach (ulong handle in _instance.errorQueries)
                        {
                            _instance.pendingQueries.TryRemove(handle, out Query q);
                        }

                        _instance.errorQueries.Clear();

                        foreach (ulong handle in _instance.completedQueries)
                        {
                            _instance.pendingQueries.TryRemove(handle, out Query q);
                        }

                        _instance.completedQueries.Clear();
                    }
                }
                catch (System.EntryPointNotFoundException)
                {
                    MLPluginLog.Error("MLFoundObjects.ProcessPendingQueries failed. Reason: API symbols not found");
                }

                return true;
            });
        }

        /// <summary>
        /// Destroy the found object native tracker.
        /// </summary>
        private void DestroyNativeTracker()
        {
            try
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(_instance.handle))
                {
                    return;
                }

                MLResult.Code resultCode = NativeBindings.MLFoundObjectTrackerDestroy(_instance.handle);
                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLFoundObjects.DestroyNativeTracker failed to destroy found object tracker. Reason: {0}", MLResult.CodeToString(resultCode));
                }

                _instance.handle = MagicLeapNativeBindings.InvalidHandle;
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLFoundObjects.DestroyNativeTracker failed. Reason: API symbols not found");
            }
        }
#endif
    }
}
