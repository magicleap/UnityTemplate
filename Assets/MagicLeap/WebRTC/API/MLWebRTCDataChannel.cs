// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCDataChannel.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;
    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a data channel used by the MLWebRTC API.
        /// </summary>
        public partial class DataChannel
        {
            /// <summary>
            /// The handle for this managed object.
            /// </summary>
            private GCHandle gcHandle;

            /// <summary>
            /// Gets or sets the handle of the data channel.
            /// </summary>
            internal ulong Handle { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="DataChannel" /> class.
            /// </summary>
            internal DataChannel()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DataChannel" /> class.
            /// </summary>
            /// <param name="handle">The handle to give the data channel.</param>
            internal DataChannel(ulong handle)
            {
                this.Handle = handle;
            }

            /// <summary>
            /// Delegate that describes the requirements of the OnOpened callback.
            /// </summary>
            /// <param name="dataChannel">The data channel associated with the event.</param>
            public delegate void OnOpenedDelegate(MLWebRTC.DataChannel dataChannel);

            /// <summary>
            /// Delegate that describes the requirements of the OnClosed callback.
            /// </summary>
            /// <param name="dataChannel">The data channel associated with the event.</param>
            public delegate void OnClosedDelegate(MLWebRTC.DataChannel dataChannel);

            /// <summary>
            /// Delegate that describes the requirements of the OnMessageText callback.
            /// </summary>
            /// <param name="dataChannel">The data channel associated with the event.</param>
            /// <param name="message">The message string.</param>
            public delegate void OnMessageTextDelegate(MLWebRTC.DataChannel dataChannel, string message);

            /// <summary>
            /// Delegate that describes the requirements of the OnMessageBinary callback.
            /// </summary>
            /// <param name="dataChannel">The data channel associated with the event.</param>
            /// <param name="message">The message byte array.</param>
            public delegate void OnMessageBinaryDelegate(MLWebRTC.DataChannel dataChannel, byte[] message);

            /// <summary>
            /// Event invoked for when a data channel opens.
            /// </summary>
            public event OnOpenedDelegate OnOpened = delegate { };

            /// <summary>
            /// Event invoked for when a data channel closes.
            /// </summary>
            public event OnClosedDelegate OnClosed = delegate { };

            /// <summary>
            /// Event invoked for when a data channel receives a text message.
            /// </summary>
            public event OnMessageTextDelegate OnMessageText = delegate { };

            /// <summary>
            /// Event invoked for when a data channel receives a binary message.
            /// </summary>
            public event OnMessageBinaryDelegate OnMessageBinary = delegate { };

            /// <summary>
            /// Gets the label of the data channel.
            /// </summary>
            public string Label { get; internal set; }

            /// <summary>
            /// Gets the connection associated with the data channel.
            /// </summary>
            public PeerConnection ParentConnection { get; internal set; }

#if PLATFORM_LUMIN
            /// <summary>
            /// Creates an initialized DataChannel object.
            /// </summary>
            /// <param name="connection">The connection to create the data channel with.</param>
            /// <param name="label">The label to give the data channel.</param>
            /// <param name="result">The MLResult object of the inner platform call(s).</param>
            /// <returns> An initialized DataChannel object.</returns>
            public static DataChannel CreateLocal(MLWebRTC.PeerConnection connection, out MLResult result, string label = "local")
            {
                DataChannel dataChannel = null;

                if (connection == null)
                {
                    result = MLResult.Create(MLResult.Code.InvalidParam, "PeerConnection is null.");
                    return dataChannel;
                }

                ulong dataChannelHandle = MagicLeapNativeBindings.InvalidHandle;
                MLResult.Code resultCode = NativeBindings.MLWebRTCDataChannelCreate(connection.Handle, label, out dataChannelHandle);
                if (!DidNativeCallSucceed(resultCode, "MLWebRTCDataChannelCreate()"))
                {
                    result = MLResult.Create(resultCode);
                    return dataChannel;
                }

                dataChannel = new DataChannel(dataChannelHandle)
                {
                    Label = label,
                    ParentConnection = connection
                };

                resultCode = NativeBindings.SetCallbacks(dataChannel);

                if (!DidNativeCallSucceed(resultCode, "MLWebRTCDataChannelSetCallbacks()"))
                {
                    result = MLResult.Create(resultCode);
                    return dataChannel;
                }

                connection.LocalDataChannels.Add(dataChannel);
                result = MLResult.Create(resultCode);
                return dataChannel;
            }

            /// <summary>
            /// Sends a string message to a data channel.
            /// </summary>
            /// <param name="message">The string to send.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultDataChannelIsClosed</c> if data channel is closed.
            /// </returns>
            public MLResult SendMessage(string message)
            {
                MLResult.Code resultCode = NativeBindings.SendMessageToDataChannel(this, message);
                DidNativeCallSucceed(resultCode, "MLWebRTCDataChannel.SendMessageToDataChannel()");
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Sends a byte array message to a data channel.
            /// </summary>
            /// <param name="message">The byte array to send.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultDataChannelIsClosed</c> if data channel is closed.
            /// </returns>
            public MLResult SendMessage<T>(T[] message)
            {
                MLResult.Code resultCode = NativeBindings.SendMessageToDataChannel(this, message);
                DidNativeCallSucceed(resultCode, "MLWebRTCDataChannel.SendMessageToDataChannel()");
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Checks if the data channel is currently open.
            /// </summary>
            /// <param name="open">True if data channel is open.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
            /// </returns>
            public MLResult IsOpen(out bool open)
            {
                bool isOpen = false;
                MLResult.Code resultCode = NativeBindings.MLWebRTCDataChannelIsOpen(this.Handle, out isOpen);
                DidNativeCallSucceed(resultCode, "MLWebRTCDataChannelIsOpen()");
                open = isOpen;
                MLResult result = MLResult.Create(resultCode);
                return result;
            }

            /// <summary>
            /// Destroys the data channel object.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public MLResult Destroy()
            {
                if (this.ParentConnection == null || !MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                {
                    return MLResult.Create(MLResult.Code.InvalidParam);
                }

                MLResult.Code resultCode = NativeBindings.MLWebRTCDataChannelDestroy(this.ParentConnection.Handle, this.Handle);
                if (DidNativeCallSucceed(resultCode, "MLWebRTCDataChannelDestroy()"))
                {
                    this.Handle = MagicLeapNativeBindings.InvalidHandle;
                    this.ParentConnection = null;
                }

                this.gcHandle.Free();
                return MLResult.Create(resultCode);
            }
#endif
        }
    }
}
