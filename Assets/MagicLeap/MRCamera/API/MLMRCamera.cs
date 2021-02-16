// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMRCamera.cs" company="Magic Leap">
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
#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// Mixed Reality Camera API, used to capture camera frames that include mixed reality content.
    /// </summary>
    public sealed partial class MLMRCamera : MLAutoAPISingleton<MLMRCamera>
    {
        /// <summary>
        /// Backing field for if the MRCamera is currently connected.
        /// </summary>
        private bool isConnected = false;

        /// <summary>
        /// Backing field for if the MRCamera is currently capturing.
        /// </summary>
        private bool isCapturing = false;

        /// <summary>
        /// Backing field for if the MRCamera is currently prepared for capturing.
        /// </summary>
        private bool isPreparedForCapturing = false;

        /// <summary>
        /// Caches if the MRCamera was capturing before standby or pause.
        /// </summary>
        private bool wasCapturingBeforeStandbyOrPause = false;

        /// <summary>
        /// Caches if the MRCamera was connected before standby or pause.
        /// </summary>
        private bool wasConnectedBeforeStandbyOrPause = false;

        /// <summary>
        /// The current input context used for capturing.
        /// </summary>
        private InputContext inputContext;

        /// <summary>
        /// Delegate used for the OnFrameCapture event.
        /// </summary>
        /// <param name="frame">The camera frame that was captured.</param>
        public delegate void OnFrameCaptureDelegate(MLMRCamera.Frame frame);

        /// <summary>
        /// Delegate used for the OnCaptureComplete event.
        /// </summary>
        public delegate void OnCaptureCompleteDelegate();

        /// <summary>
        /// Delegate used for the OnError event.
        /// </summary>
        /// <param name="result">MLResult of the error that occurred.</param>
        public delegate void OnErrorDelegate(MLResult result);

        /// <summary>
        /// Event used to listen for when a camera frame is captured, invoked on the main thread.
        /// </summary>
        public static event OnFrameCaptureDelegate OnFrameCapture;

        /// <summary>
        /// Event used to listen for when a camera frame is captured, invoked on the same thread as the native callback,
        /// allowing the use of the unmanaged native pointer to the frame data memory.
        /// </summary>
        public static event OnFrameCaptureDelegate OnFrameCapture_NativeCallbackThread;

        /// <summary>
        /// Event used to listen for when capturing is stopped.
        /// </summary>
        public static event OnCaptureCompleteDelegate OnCaptureComplete;

        /// <summary>
        /// Event used to listen for when the API encounters some internal error.
        /// </summary>
        public static event OnErrorDelegate OnError;

        /// <summary>
        /// Output format of the image planes contained in some camera frame.
        /// </summary>
        public enum OutputFormat
        {
            /// <summary>
            /// Unknown format.
            /// </summary>
            Unknown,

            /// <summary>
            /// RGBA32 format, each channel is 8 bits.
            /// </summary>
            RGBA_8888,
        }

        /// <summary>
        /// Render quality of the image planes contained in some camera frame.
        /// </summary>
        public enum RenderQuality
        {
            /// <summary>
            /// Quality of 720p.
            /// </summary>
            q720P = 1,

            /// <summary>
            /// Quality of 1080p.
            /// </summary>
            q1080P = 2
        }

        /// <summary>
        /// Virtual and real content blending modes.
        /// </summary>
        public enum BlendType
        {
            /// <summary>
            /// Alpha blend type, <c>Standard (1 - srcA)</c> blend where virtual pixels are combined over the background real world pixels.
            /// </summary>
            Alpha,

            /// <summary>
            /// Additive Blend Type. It simply adds pixel values of real world and virtual layer.*/
            /// </summary>
            Additive,

            /// <summary>
            /// Hybrid Blend Type. A mix between Alpha blending and Additive blending using a power curve.
            /// In this setting, content with alpha values of 1 is treated as semi opaque, with a fixed amount
            /// of blending against the world camera.Any other alpha values are treated as additive, allowing
            /// the apps to choose how they want to composite content in the final captured output.
            /// </summary>
            Hybrid
        }

#if PLATFORM_LUMIN
        /// <summary>
        /// Gets if the MRCamera is currently connected.
        /// </summary>
        public static bool IsConnected => Instance.isConnected;

        /// <summary>
        /// Gets if the MRCamera is currently capturing.
        /// </summary>
        public static bool IsCapturing => Instance.isCapturing;

        /// <summary>
        /// Starts MLMRCamera.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if lack of privilege(s).
        /// </returns>
        public static MLResult Connect() => MLMRCamera.Connect(InputContext.Create());

        /// <summary>
        /// Starts MLMRCamera with a custom input context object.
        /// </summary>
        /// <param name="context">The input context to capture with.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if lack of privilege(s).
        /// </returns>
        public static MLResult Connect(InputContext context) => MLResult.Create(Instance.MLMRCameraConnect(context));

        /// <summary>
        /// Starts capturing camera frames, API must already be started.
        /// </summary>
        /// <param name="frames">Number of frames to capture. 0 will capture indefinitely.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if lack of privilege(s).
        /// </returns>
        public static MLResult StartCapture(int frames = 0) => MLResult.Create(Instance.MLMRCameraStartCapture(frames));

        /// <summary>
        /// Gets the resolution of the captured frames.
        /// </summary>
        /// <param name="width">Width of the captured frames.</param>
        /// <param name="height">Height of the captured frames.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to internal invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if lack of privilege(s).
        /// </returns>
        public static MLResult GetCaptureSize(out uint width, out uint height) => MLResult.Create(Instance.MLMRCameraGetCaptureSize(out width, out height));

        /// <summary>
        /// Stops capturing camera frames, API must already be started.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if lack of privilege(s).
        /// </returns>
        public static MLResult StopCapture() => MLResult.Create(Instance.MLMRCameraStopCapture());

        /// <summary>
        /// Disconnect MLMRCamera.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if lack of privilege(s).
        /// </returns>
        public static MLResult Disconnect() => MLResult.Create(Instance.MLMRCameraDisconnect());

        /// <summary>
        /// Starts the MLMRCamera API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
        /// </returns>
        protected override MLResult.Code StartAPI() => MLResult.Code.Ok;

        /// <summary>
        /// Stops the MLMRCamera API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if stopped successfully.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericInvalidOperation</c> if the MR camera was not properly stopped.
        /// </returns>
        protected override MLResult.Code StopAPI() => this.MLMRCameraDisconnect();

        /// <summary>
        /// Reacts to when the app is pause/resumed.
        /// </summary>
        /// <param name="pauseStatus">True if app is going into a paused state.</param>
        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);
            if (pauseStatus)
            {
                this.TeardownCamera();
            }
            else
            {
                this.RestartCamera();
            }
        }

        /// <summary>
        /// Sets up the MRCamera and connects it with the given context.
        /// </summary>
        /// <param name="context">The context to connect the MRCamera with.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully initialized the MR camera.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericAlreadyExists</c> if the MR camera was already initialized.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
        /// </returns>
        private MLResult.Code MLMRCameraConnect(InputContext context)
        {
            this.inputContext = context;
            return this.SetupMLMRCamera();
        }

        /// <summary>
        /// Starts capturing with the MRCamera.
        /// </summary>
        /// <param name="frames">The amount of frames to capture, a value of 0 will capture frames indefinitely.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if started successfully.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
        /// </returns>
        private MLResult.Code MLMRCameraStartCapture(int frames = 0)
        {
            MLResult.Code resultCode = MLResult.Code.Ok;
            if (!this.isPreparedForCapturing)
            {
                resultCode = NativeBindings.MLMRCameraPrepareCapture(MagicLeapNativeBindings.InvalidHandle);
                if (!MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraPrepareCapture"))
                {
                    return resultCode;
                }
                this.isPreparedForCapturing = true;
            }
            
            if (!this.isCapturing)
            {
                resultCode = NativeBindings.MLMRCameraStartCapture((uint)frames, false);
                this.isCapturing = MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraStartCapture");
            }

            return resultCode;
        }

        /// <summary>
        /// Gets the capture size of the MRCamera.
        /// </summary>
        /// <param name="width">Width of the capture size.</param>
        /// <param name="height">Height of the capture size.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully returned the current resolution.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if width or height was invalid.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
        /// </returns>
        private MLResult.Code MLMRCameraGetCaptureSize(out uint width, out uint height)
        {
            MLResult.Code resultCode = NativeBindings.MLMRCameraGetCaptureSize(out width, out height);
            MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraGetCaptureSize");
            return resultCode;
        }

        /// <summary>
        /// Stops capturing with the MRCamera.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if started successfully.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
        /// </returns>
        private MLResult.Code MLMRCameraStopCapture()
        {
            MLResult.Code resultCode = MLResult.Code.Ok;
            if (this.isCapturing)
            {
                resultCode = NativeBindings.MLMRCameraStopCapture();
                MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraStopCapture");
                this.isCapturing = false;
            }

            return resultCode;
        }

        /// <summary>
        /// Stops capturing and disconnects the MRCamera.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if stopped successfully.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericInvalidOperation</c> if the MR camera was not properly stopped.
        /// </returns>
        private MLResult.Code MLMRCameraDisconnect()
        {
            MLResult.Code resultCode = MLResult.Code.Ok;
            if (this.isCapturing)
            {
                resultCode = NativeBindings.MLMRCameraStopCapture();
                MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraStopCapture");
                this.isCapturing = false;
                this.wasCapturingBeforeStandbyOrPause = true;
            }

            if (this.isConnected)
            {
                resultCode = NativeBindings.MLMRCameraDisconnect();
                MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraDisconnect");
                this.isConnected = false;
                this.wasConnectedBeforeStandbyOrPause = true;
                this.isPreparedForCapturing = false;
            }

            return resultCode;
        }

        /// <summary>
        /// Connects to the MRCamera and prepares it for capturing.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully initialized the MR camera.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericAlreadyExists</c> if the MR camera was already initialized.
        /// MLResult.Result will be <c>MLResult.Code.MediaGenericNoInit</c> if the MR camera was not properly initialized.
        /// </returns>
        private MLResult.Code SetupMLMRCamera()
        {
            if (this.isConnected)
            {
                return MLResult.Code.Ok;
            }

            NativeBindings.InputContextNative contextNative = new NativeBindings.InputContextNative();
            contextNative.Data = this.inputContext;
            MLResult.Code resultCode = NativeBindings.MLMRCameraConnect(ref contextNative);
            if (!MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraConnect"))
            {
                return resultCode;
            }

            this.isConnected = true;

            NativeBindings.CallbacksNative callbacksNative = NativeBindings.CreateCallbacks();
            resultCode = NativeBindings.MLMRCameraSetCallbacks(ref callbacksNative, IntPtr.Zero);
            if (!MLMRCamera.DidNativeCallSucceed(resultCode, "MLMRCameraSetCallbacks"))
            {
                return resultCode;
            }

            return resultCode;
        }

        /// <summary>
        /// Restarts the MRCamera by reconnecting it and/or recapturing.
        /// </summary>
        private void RestartCamera()
        {
            if (this.wasConnectedBeforeStandbyOrPause)
            {
                this.MLMRCameraConnect(this.inputContext);
            }

            if (this.wasCapturingBeforeStandbyOrPause)
            {
                // TODO : if StartCapture() was originally called with a frame number != 0, then this invocation would be incorrect.
                this.MLMRCameraStartCapture();
            }
        }

        /// <summary>
        /// Method used to subscribe to the lifecycle events. 
        /// </summary>
        private void TeardownCamera()
        {
            this.MLMRCameraDisconnect();
        }
#endif
    }
}
