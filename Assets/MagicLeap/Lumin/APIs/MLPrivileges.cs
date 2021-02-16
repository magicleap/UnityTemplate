// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLPrivileges.cs" company="Magic Leap, Inc">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Functionality to validate or query privileges from the system.
    /// </summary>
    public sealed partial class MLPrivileges : MLAutoAPISingleton<MLPrivileges>
    {
        #if PLATFORM_LUMIN
        /// <summary>
        /// A dictionary of pending privilege requests.
        /// </summary>
        private Dictionary<MLPrivileges.Id, RequestPrivilegeQuery> currentRequests = new Dictionary<MLPrivileges.Id, RequestPrivilegeQuery>();

        /// <summary>
        /// The callback delegate for privilege requests.
        /// </summary>
        /// <param name="result">The result code of the privilege callback request.</param>
        /// <param name="id">The privilege id that was requested.</param>
        public delegate void CallbackDelegate(MLResult result, MLPrivileges.Id id);
        #endif

        /// <summary>
        /// Privilege Types:
        /// AutoGranted - Once it is included in the manifest the application is granted the privilege.
        /// Sensitive - Must be requested at runtime, as well as in the manifest, because it requires user consent on first use.
        /// Reality - Must be requested at runtime and every time the application gains focus, as well as in the manifest, because it requires user consent every active session.
        /// </summary>
        public enum Id : uint
        {
            /// <summary>
            /// An invalid Id.
            /// </summary>
            Invalid = 0,

            /// <summary>
            /// Full read and search access to address book contacts.
            /// Type: Sensitive
            /// </summary>
            AddressBookRead = 1,

            /// <summary>
            /// Ability to add, modify and delete address book contacts.
            /// Type: Sensitive
            /// </summary>
            AddressBookWrite = 2,

            /// <summary>
            /// Query battery status/percentage.
            /// Type: AutoGranted
            /// </summary>
            BatteryInfo = 18,

            /// <summary>
            /// Take pictures and videos using camera.
            /// Type: Reality
            /// </summary>
            CameraCapture = 26,

            /// <summary>
            /// Access dense map.
            /// Type: AutoGranted
            /// </summary>
            WorldReconstruction = 33,

            /// <summary>
            /// Use the in-app purchase mechanism.
            /// Type: AutoGranted
            /// </summary>
            InAppPurchase = 42,

            /// <summary>
            /// Open a microphone stream of the users voice or the ambient surroundings.
            /// Type: Reality
            /// </summary>
            AudioCaptureMic = 49,

            /// <summary>
            /// Provision and use DRM certificates.
            /// Type: AutoGranted
            /// </summary>
            DrmCertificates = 51,

            /// <summary>
            /// Access Low Latency data from the <c>Lightwear</c>.
            /// Type: AutoGranted
            /// </summary>
            LowLatencyLightwear = 59,

            /// <summary>
            /// Access the internet.
            /// Type: AutoGranted
            /// </summary>
            Internet = 96,

            /// <summary>
            /// Bluetooth Adapter User
            /// Type: AutoGranted
            /// </summary>
            BluetoothAdapterUser = 106,

            /// <summary>
            /// Bluetooth <c>Gatt</c> Client Write
            /// Type: AutoGranted
            /// </summary>
            BluetoothGattWrite = 108,

            /// <summary>
            /// Read user profile attributes.
            /// Type: AutoGranted
            /// </summary>
            IdentityRead = 113,

            /// <summary>
            /// Download in the background.
            /// Type: AutoGranted
            /// </summary>
            BackgroundDownload = 120,

            /// <summary>
            /// Upload in the background.
            /// Type: AutoGranted
            /// </summary>
            BackgroundUpload = 121,

            /// <summary>
            /// Get power information.
            /// Type: AutoGranted
            /// </summary>
            PowerInfo = 150,

            /// <summary>
            /// Access other entities on the local network.
            /// Type: Sensitive
            /// </summary>
            LocalAreaNetwork = 171,

            /// <summary>
            /// Receive voice input.
            /// Type: Reality
            /// </summary>
            VoiceInput = 173,

            /// <summary>
            /// Connect to Background Music Service.
            /// Type: AutoGranted
            /// </summary>
            ConnectBackgroundMusicService = 192,

            /// <summary>
            /// Register with Background Music Service.
            /// Type: AutoGranted
            /// </summary>
            RegisterBackgroundMusicService = 193,

            /// <summary>
            /// Read found objects from Passable World.
            /// Type: AutoGranted
            /// </summary>
            PcfRead = 201,

            /// <summary>
            /// Post notifications for users to see, dismiss own notifications, listen for own notification events.
            /// Type: AutoGranted
            /// </summary>
            NormalNotificationsUsage = 208,

            /// <summary>
            /// Access Music Service functionality.
            /// Type: AutoGranted
            /// </summary>
            MusicService = 218,

            /// <summary>
            /// Access controller pose data.
            /// Type: AutoGranted
            /// </summary>
            ControllerPose = 263,

            /// <summary>
            /// Subscribe to gesture hand mask and config data.
            /// Type: AutoGranted
            /// </summary>
            GesturesSubscribe = 268,

            /// <summary>
            /// Set/Get gesture configuration.
            /// Type: AutoGranted
            /// </summary>
            GesturesConfig = 269,

            /// <summary>
            /// Access a manually selected subset of contacts from address book.
            /// Type: AutoGranted
            /// </summary>
            AddressBookBasicAccess = 305,

            /// <summary>
            /// Access hand mesh features.
            /// Type: AutoGranted
            /// </summary>
            HandMesh = 315,

            /// <summary>
            /// Get coarse location of the device.
            /// Type: Sensitive
            /// </summary>
            CoarseLocation = 323,

            /// <summary>
            /// Ability to initiate invites to Social connections.
            /// Type: AutoGranted
            /// </summary>
            SocialConnectionsInvitesAccess = 329,

            /// <summary>
            /// SDK access CV related info from <c>graph_pss</c>.
            /// Type: Reality
            /// </summary>
            ComputerVision = 343,

            /// <summary>
            /// Get <c>Wifi</c> status to application.
            /// Type: AutoGranted
            /// </summary>
            WifiStatusRead = 344,

            /// <summary>
            /// Access ACP connection APIs.
            /// Type: Reality
            /// </summary>
            ConnectionAccess = 350,

            /// <summary>
            /// Access ACP connection audio API.
            /// Type: Reality
            /// </summary>
            ConnectionAudioCaptureStreaming = 351,

            /// <summary>
            /// Access ACP connection video API.
            /// Type: Reality
            /// </summary>
            ConnectionVideoCaptureStreaming = 352,

            /// <summary>
            /// Request a secure browser window.
            /// Type: AutoGranted
            /// </summary>
            SecureBrowserWindow = 357,

            /// <summary>
            /// Access to Bluetooth adapter from external app
            /// Type: AutoGranted
            /// </summary>
            BluetoothAdapterExternalApp = 362,

            /// <summary>
            /// Get fine location of the device.
            /// Type: Reality
            /// </summary>
            FineLocation = 367,

            /// <summary>
            /// Select access to social connections.
            /// Type: AutoGranted
            /// </summary>
            SocialConnectionsSelectAccess = 372,

            /// <summary>
            /// Access found object data from object-recognition pipeline.
            /// Type: Sensitive
            /// </summary>
            ObjectData = 394,

            /// <summary>
            /// Allow applications to capture lightwear and lightpack IMU samples
            /// Type: AutoGranted
            /// </summary>
            ImuCapture = 395
        }

        /// <summary>
        /// Privilege ids that need to be requested at runtime in addition
        /// to being specified in the app manifest. Descriptions for each
        /// value can be found in MLPrivilege.Id enum. This is the subset of
        /// runtime privileges applicable to Unity apps.
        /// </summary>
        public enum RuntimeRequestId : uint
        {
            /// <summary>
            /// The Audio Capture Microphone privilege.
            /// </summary>
            AudioCaptureMic = MLPrivileges.Id.AudioCaptureMic,

            /// <summary>
            /// The Camera Capture privilege.
            /// </summary>
            CameraCapture = MLPrivileges.Id.CameraCapture,

            /// <summary>
            /// The Local Area Network privilege.
            /// </summary>
            LocalAreaNetwork = MLPrivileges.Id.LocalAreaNetwork,

            /// <summary>
            /// The obsolete PCF Read privilege.
            /// </summary>
            [System.Obsolete("PcfRead/PwFoundObjRead are now AutoGranted and do not need to be requested at runtime.", true)]
            PcfRead = MLPrivileges.Id.PcfRead,

            /// <summary>
            /// The Address Book Read privilege.
            /// </summary>
            AddressBookRead = MLPrivileges.Id.AddressBookRead,

            /// <summary>
            /// The Address Book Write privilege.
            /// </summary>
            AddressBookWrite = MLPrivileges.Id.AddressBookWrite,

            /// <summary>
            /// The Coarse Location privilege.
            /// </summary>
            CoarseLocation = MLPrivileges.Id.CoarseLocation,

            /// <summary>
            /// The Computer Vision privilege.
            /// </summary>
            ComputerVision = MLPrivileges.Id.ComputerVision,

            /// <summary>
            /// The Fine Location privilege.
            /// </summary>
            FineLocation = MLPrivileges.Id.FineLocation,

            /// <summary>
            /// Access ACP connection audio API.
            /// Type: Reality
            /// </summary>
            ConnectionAudioCaptureStreaming = MLPrivileges.Id.ConnectionAudioCaptureStreaming,

            /// <summary>
            /// Access ACP connection video API.
            /// Type: Reality
            /// </summary>
            ConnectionVideoCaptureStreaming = MLPrivileges.Id.ConnectionVideoCaptureStreaming,

            /// <summary>
            /// Access found object data from object-recognition pipeline.
            /// Type: Sensitive
            /// </summary>
            ObjectData = MLPrivileges.Id.ObjectData
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Checks whether the application has the specified privileges.
        /// This does not solicit consent from the end-user.
        /// </summary>
        /// <param name="privilegeId">The privilege to check for access.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        public static MLResult CheckPrivilege(MLPrivileges.Id privilegeId)
        {
            return MLResult.Create(Instance.CheckPrivilegeInternal(privilegeId));
        }

        /// <summary>
        /// Requests the specified privilege. This may possibly solicit consent from the end-user.
        /// </summary>
        /// <param name="privilegeId">The privilege to request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        public static MLResult RequestPrivilege(MLPrivileges.Id privilegeId)
        {
            return MLResult.Create(Instance.RequestPrivilegeInternal(privilegeId));
        }

        /// <summary>
        /// Request the specified privilege asynchronously. This may solicit consent from the end-user.
        /// This async override uses Tasks instead of a callback.
        /// </summary>
        /// <param name="privilegeId">The privilege to request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        public static async Task<MLResult> RequestPrivilegeAsync(MLPrivileges.Id privilegeId)
        {
            return await Instance.RequestPrivilegeAsyncInternal(privilegeId);
        }

        /// <summary>
        /// Requests the specified privileges. This may possibly solicit consent from the end-user.
        /// </summary>
        /// <param name="privilegeIds">The privileges to request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        public static MLResult RequestPrivileges(params MLPrivileges.Id[] privilegeIds)
        {
#if PLATFORM_LUMIN
            MLResult result = new MLResult();

            foreach (MLPrivileges.Id privilegeId in privilegeIds)
            {
                result = CheckPrivilege(privilegeId);
                if (result.Result == MLResult.Code.PrivilegeGranted)
                {
                    continue;
                }

                result = RequestPrivilege(privilegeId);
                if (result.Result != MLResult.Code.PrivilegeGranted)
                {
                    return result;
                }
            }
#endif
            return result;
        }

        /// <summary>
        /// Request the specified privileges asynchronously. This may possibly solicit consent from the end-user.
        /// </summary>
        /// <param name="privilegeIds">The privileges to request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>        
        public static async Task<MLResult> RequestPrivilegesAsync(params MLPrivileges.Id[] privilegeIds)
        {
            foreach (MLPrivileges.Id privilegeId in privilegeIds)
            {
                Task<MLResult> task = await RequestPrivilegeAsync(privilegeId);
                if (task.Result.Result != MLResult.Code.PrivilegeGranted)
                {
                    return task.Result;
                }
            }

            return MLResult.Create(MLResult.Code.PrivilegeGranted);
        }

        /// <summary>
        /// Gets the result string for a MLResult.Code.
        /// </summary>
        /// <param name="resultCode">The MLResult.Code to be requested.</param>
        /// <returns>A pointer to the result string.</returns>
        internal static IntPtr GetResultString(MLResult.Code resultCode)
        {
            return Instance.GetResultStringInternal(resultCode);
        }

        /// <summary>
        /// Starts the Privileges, Must be called to start checking for privileges at runtime.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the privilege system startup succeeded.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system failed to startup.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            return NativeBindings.MLPrivilegesStartup(); 
        }

        /// <summary>
        /// Cleans objects and stops the API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the privilege system shutdown succeeded.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system failed to shutdown.
        /// </returns>
        protected override MLResult.Code StopAPI()
        {
            Instance.currentRequests.Clear();
            return NativeBindings.MLPrivilegesShutdown();
        }

        /// <summary>
        /// Polls for the result of pending privileges requests.
        /// </summary>
        protected override void Update()
        {
            this.ProcessPendingQueries();
        }

        /// <summary>
        /// Checks whether the application has the specified privileges.
        /// This does not solicit consent from the end-user.
        /// </summary>
        /// <param name="privilegeId">The privilege to check for access.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        private MLResult.Code CheckPrivilegeInternal(MLPrivileges.Id privilegeId)
        {
            return NativeBindings.MLPrivilegesCheckPrivilege(privilegeId);
        }

        /// <summary>
        /// Requests the specified privileges. This may possibly solicit consent from the end-user.
        /// </summary>
        /// <param name="privilegeId">The privilege to request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        private MLResult.Code RequestPrivilegeInternal(MLPrivileges.Id privilegeId)
        {
            return NativeBindings.MLPrivilegesRequestPrivilege(privilegeId);
        }

        /// <summary>
        /// Request the specified privilege asynchronously. This may solicit consent from the end-user.
        /// This async override uses Tasks instead of a callback.
        /// </summary>
        /// <param name="privilegeId">The privilege to request.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeGranted</c> if the privilege is granted.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeNotGranted</c> if the privilege is denied.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the privilege system was not started.
        /// </returns>
        private async Task<MLResult> RequestPrivilegeAsyncInternal(MLPrivileges.Id privilegeId)
        {
            var taskCompletionSource = new TaskCompletionSource<MLResult>();
            if (this.currentRequests.ContainsKey(privilegeId))
            {
                return MLResult.Create(MLResult.Code.Ok);
            }

            IntPtr newRequest = IntPtr.Zero;
            MLResult.Code resultCode = NativeBindings.MLPrivilegesRequestPrivilegeAsync(privilegeId, ref newRequest);
            if (!MLResult.IsOK(resultCode))
            {
                return MLResult.Create(resultCode);
            }

            RequestPrivilegeQuery newQuery = new RequestPrivilegeQuery((result, id) => taskCompletionSource.SetResult(result), newRequest, privilegeId);
            this.currentRequests.Add(privilegeId, newQuery);
            return await taskCompletionSource.Task;
        }

        /// <summary>
        /// Process pending requests and call the callback specified in the startup config.
        /// </summary>
        private void ProcessPendingQueries()
        {
            try
            {
                foreach (var pending in this.currentRequests.OrderByDescending(x => x.Key))
                {
                    MLResult.Code result = NativeBindings.MLPrivilegesRequestPrivilegeTryGet(pending.Value.Request);
                    if (result != MLResult.Code.Pending)
                    {
                        pending.Value.Result = MLResult.Create(result);
                        pending.Value.Callback(pending.Value.Result, pending.Key);

                        this.currentRequests.Remove(pending.Key);
                    }
                }
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLPrivileges.ProcessPendingQueries failed. Reason: API symbols not found");
            }
        }

        /// <summary>
        /// Gets the result string for a MLResult.Code.
        /// </summary>
        /// <param name="resultCode">The MLResult.Code to be requested.</param>
        /// <returns>A pointer to the result string.</returns>
        private IntPtr GetResultStringInternal(MLResult.Code resultCode)
        {
            return NativeBindings.MLPrivilegesGetResultString(resultCode);
        }

        /// <summary>
        /// Wrapper for the Async Request
        /// </summary>
        private class RequestPrivilegeQuery
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RequestPrivilegeQuery"/> class.
            /// </summary>
            /// <param name="callback">The callback that should receive the notification.</param>
            /// <param name="request">A pointer to the request.</param>
            /// <param name="privilege">The privilege Id.</param>
            public RequestPrivilegeQuery(CallbackDelegate callback, IntPtr request, MLPrivileges.Id privilege)
            {
                this.Callback = callback;
                this.Result = MLResult.Create(MLResult.Code.Pending);
                this.Request = request;
                this.PrivilegeId = privilege;
            }

            /// <summary>
            /// Gets or sets the query result callback.
            /// </summary>
            public CallbackDelegate Callback { get; set; }

            /// <summary>
            /// Gets or sets the requested privilege id.
            /// </summary>
            public MLPrivileges.Id PrivilegeId { get; set; }

            /// <summary>
            /// Gets or sets The result.
            /// </summary>
            public MLResult Result { get; set; }

            /// <summary>
            /// Gets or sets the Async request <c>IntPtr</c>.
            /// </summary>
            public IntPtr Request { get; set; }
        }
        #endif
    }
}
