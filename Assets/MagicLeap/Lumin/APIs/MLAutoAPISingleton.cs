// %BANNER_BEGIN% 
// --------------------------------------------------------------------- 
// %COPYRIGHT_BEGIN%
// <copyright file="MLAPISingleton.cs" company="Magic Leap, Inc">
//     Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// </copyright>
// %COPYRIGHT_END%
// --------------------------------------------------------------------- 
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Internal;
#endif

    /// <summary>
    ///     Place this attribute on a child of MLAutoAPISingleton to prevent its initialization
    ///     until the Magic Leap XR package is loaded.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireXRLoader : Attribute { }

    /// <summary>
    ///     MLAutoAPISingleton class contains a template for singleton APIs
    /// </summary>
    /// <typeparam name="T"> The type of the class to create a singleton for. </typeparam>
    public abstract class MLAutoAPISingleton<T> where T : MLAutoAPISingleton<T>, new()
    {
        /// <summary>
        ///     The native handle ID for this API instance. Will be invalid until the API is started.
        /// </summary>
#if PLATFORM_LUMIN
        protected ulong Handle = Native.MagicLeapNativeBindings.InvalidHandle;
#else
        protected ulong Handle = 0;
#endif

        private static readonly bool requiresXRLoader = typeof(T).GetCustomAttribute<RequireXRLoader>() != null;

#if PLATFORM_LUMIN
        protected static T Instance
        {
            get
            {
                if (instance == null)
                    Init();

                return instance;
            }
        }

        /// <summary>
        ///     This is the only way to initialize this class.
        /// </summary>
        private static void Init()
        {
            allowInit = true;
            instance = new T();
            allowInit = false;

            if (requiresXRLoader && !MLDevice.IsReady())
            {
                MLPluginLog.Error($"Magic Leap XR Loader is not initialized, and the {typeof(T).Name} API must be started afterwards.");
                return;
            }
            else
                instance.StartInternal();
        }

        /// <summary>
        ///     Indicates if the API has started successfully.
        /// </summary>
        public static bool IsStarted { get; private set; } = false;

        protected readonly string DllNotFoundError = $"Failed to start {typeof(T).Name} API. This API is only available on device or when running inside the Unity editor with Zero Iteration enabled.";

        private static T instance = null;
        private static bool allowInit = false;

        private PerceptionHandle perceptionHandle;

        /// <summary>
        ///     DO NOT USE THIS! This class cannot be instantiated manually. 
        /// </summary>
        protected MLAutoAPISingleton()
        {
            if (!allowInit)
                throw new InvalidInstanceException($"Manually creating an instance of {typeof(T).Name} is not supported. You should use {typeof(T).Name}.Instance instead.");
        }

        /// <summary>
        ///     Do API-specific creation/initialization of ML resources for this API, such as
        ///     creating trackers, etc. Called automatically the first time <c>Instance</c> is accessed.
        ///     Error checking on the return value is performed in the base class.
        /// </summary>
        protected abstract MLResult.Code StartAPI();

        /// <summary>
        ///     API-specific cleanup. Will be called whenever MLDevice is destroyed
        ///     (at the latest, when the application is shutting down).
        ///     Error checking on the return value is performed in the base class.
        /// </summary>
        protected abstract MLResult.Code StopAPI();

        /// <summary>
        ///     Update function that will run once per Unity frame.
        /// </summary>
        protected virtual void Update() { }

        /// <summary>
        ///     Callback sent to all MagicLeap APIs on application pause.
        /// </summary>
        /// <param name="pauseStatus"> True if the application is paused, else False. </param>
        protected virtual void OnApplicationPause(bool pauseStatus) { }

        private void StartInternal()
        {
            MLPluginLog.Debug($"Initializing {typeof(T).Name} API...");

            if (DidNativeCallSucceed(StartAPI(), $"{typeof(T).Name} Start"))
            {
                IsStarted = true;
                MLDevice.RegisterUpdate(instance.Update);
                MLDevice.RegisterApplicationPause(instance.OnApplicationPause);
                MLDevice.RegisterDestroy(instance.StopInternal);

                instance.perceptionHandle = PerceptionHandle.Acquire();
                MLPluginLog.Debug($"{typeof(T).Name} API initialized.");
            }
        }

        private void StopInternal()
        {
            if (IsStarted)
            {
                MLDevice.UnregisterUpdate(this.Update);
                MLDevice.UnregisterApplicationPause(this.OnApplicationPause);
                MLDevice.UnregisterDestroy(instance.StopInternal);

                MLResult.Code resultCode = instance.StopAPI();

                if (DidNativeCallSucceed(resultCode, $"{typeof(T).Name} Stop"))
                    MLPluginLog.Debug($"{typeof(T).Name} API stopped successfully");

                if (perceptionHandle.active)
                    perceptionHandle.Dispose();

                IsStarted = false;
            }
        }

        /// <summary>
        ///     Checks native code results for failure.
        /// </summary>
        /// <param name="resultCode"> The result of the native function call. </param>
        /// <param name="functionName"> The name of the native function. </param>
        /// <param name="successCase">
        ///     Predicate delegate for determining when a call was successful.
        ///     Defaults to a check against <c>MLResult.IsOK(resultCode)</c>.
        /// </param>
        /// <param name="showError">
        ///     Should the default error message be displayed
        ///     if the <c>resultCode</c> is not expected?
        /// </param>
        /// <returns>
        ///     <c>true</c> if the result of <c>successCase</c> matches <c>resultCode</c>.
        ///     <c>false</c> otherwise.
        ///  </returns>
        protected static bool DidNativeCallSucceed(MLResult.Code resultCode, string functionName = "A native function", Predicate<MLResult.Code> successCase = null, bool showError = true)
        {
            bool success = successCase != null ? successCase(resultCode) : MLResult.IsOK(resultCode);

            if (!success && showError)
                    MLPluginLog.ErrorFormat($"{functionName} in the Magic Leap API failed. Reason: {MLResult.CodeToString(resultCode)} ");

            return success;
        }
#endif
    }
}
