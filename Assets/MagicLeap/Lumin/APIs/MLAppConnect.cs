// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLAppConnect.cs" company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    #if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
    #endif

    /// <summary>
    /// Manages App Connections.
    /// </summary>cle
    public sealed class MLAppConnect : MLAPISingleton<MLAppConnect>
    {
        #if PLATFORM_LUMIN

        /// <summary>
        /// Connection Name Key.
        /// Mandatory key in the key-value list returned by #MLAppConnectInviteCallback().
        /// An application invited to a new connection is supposed to obtain the connection name by reading
        /// the value associated with this key.
        /// </summary>
        public const string ConnectionNameKey = "connection_name";

        /// <summary>
        /// Maximum number of participants in a session.
        /// </summary>
        public const uint ConnectionMaxParticipants = 10;

        /// <summary>
        /// The internal handle attached to this instance of MLAppConnect invite receiving registration.
        /// </summary>
        private static ulong connectionHandle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// The internal handle attached to this instance of MLAppConnect invite receiving registration.
        /// </summary>
        private static ulong inviteHandle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// The internal event handle.
        /// </summary>
        private static ulong eventHandle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Default byte that gets populated with new incoming data.
        /// </summary>
        private static byte[] defaultByteArray;

        /// <summary>
        /// Default MLImage that gets populated with new incoming video frame.
        /// </summary>
        private static MLImageNativeBindings.MLImageNative cachedNativeImage = MLImageNativeBindings.MLImageNative.Create();

        /// <summary>
        /// Prevents a default instance of the <see cref="MLAppConnect"/> class from being created.
        /// </summary>
        private MLAppConnect()
        {
            ConnectionName = null;
        }

        /// <summary>
        /// ACP Invite callback function.
        /// </summary>
        /// <param name="connection_name">Name of the connection.</param>
        public delegate void OnInviteDelegate(string connection_name);

        /// <summary>
        /// Connection callback function.
        /// </summary>
        /// <param name="result">The result status of the connection.</param>
        /// <param name="connectionName">Name of the connection.</param>
        public delegate void OnConnectionDelegate(MLResult.Code result, string connectionName);

        /// <summary>
        /// Event callback for any changes inside the registered connection relating to microphone, pipe user or connection
        /// </summary>
        /// <param name="eventType">>Type of ACP Event.</param>
        public delegate void OnEventDelegate(EventType eventType);

        /// <summary>
        /// Users Event Callback for the following Events:
        /// User Exited.
        /// User Joined.
        /// User Invite.
        /// User Invite Aborted.
        /// </summary>
        /// <param name="eventType">>Type of User Event.</param>
        /// <param name="userInfo">The user structure containing more information regarding the event.</param>
        public delegate void OnUsersEventDelegate(EventType eventType, UserEventInfo userInfo);

        /// <summary>
        /// Pipe Event Callback for the following Events:
        /// Pipe Created.
        /// Pipe Creation Failed.
        /// Pipe Deleted.
        /// Large Data Sent.
        /// Pipe Blocked
        /// Pipe Unblocked
        /// </summary>
        /// <param name="eventType">Type of Pipe Event.</param>
        /// <param name="pipeInfo">The pipe structure containing more information regarding the event</param>
        public delegate void OnPipeEventDelegate(EventType eventType, PipeEventInfo pipeInfo);

        /// <summary>
        /// Microphone Event Callback for the following Events:
        /// Microphone Unmuted.
        /// Microphone Muted.
        /// </summary>
        /// <param name="eventType">Type of Microphone Event.</param>
        /// <param name="micInfo">The microphone structure containing more information</param>
        public delegate void OnMicrophoneEventDelegate(EventType eventType, MicrophoneEventInfo micInfo);

        /// <summary>
        /// Connection Event Callback for the following Events:
        /// Connection Created.
        /// Connection Deleted.
        /// Connection Failed.
        /// </summary>
        /// <param name="eventType">Type of Connection Event.</param>
        /// <param name="connectionInfo">The connection structure containing more information</param>
        public delegate void OnConnectionEventDelegate(EventType eventType, ConnectionEventInfo connectionInfo);

        /// <summary>
        /// Microphone Event Callback for the following Events:
        /// Microphone Unmuted.
        /// Microphone Muted.
        /// </summary>
        public static event OnPipeEventDelegate OnPipeEvent
        {
            add
            {
                MLAppConnectNativeBindings.OnPipeEvent += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnPipeEvent -= value;
            }
        }

        /// <summary>
        /// Connection Event Callback for the following Events:
        /// Connection Created.
        /// Connection Deleted.
        /// Connection Failed.
        /// </summary>
        public static event OnConnectionEventDelegate OnConnectionEvent
        {
            add
            {
                MLAppConnectNativeBindings.OnConnectionEvent += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnConnectionEvent -= value;
            }
        }

        /// <summary>
        /// Users Event Callback for the following Events:
        /// User Exited.
        /// User Joined.
        /// User Invite.
        /// User Invite Aborted.
        /// </summary>
        public static event OnUsersEventDelegate OnUsersEvent
        {
            add
            {
                MLAppConnectNativeBindings.OnUsersEvent += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnUsersEvent -= value;
            }
        }

        /// <summary>
        /// Microphone Event Callback for the following Events:
        /// Microphone Unmuted.
        /// Microphone Muted.
        /// </summary>
        public static event OnMicrophoneEventDelegate OnMicrophoneEvent
        {
            add
            {
                MLAppConnectNativeBindings.OnMicrophoneEvent += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnMicrophoneEvent -= value;
            }
        }

        /// <summary>
        /// On connection event.
        /// </summary>
        public static event OnConnectionDelegate OnConnection
        {
            add
            {
                MLAppConnectNativeBindings.OnConnection += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnConnection -= value;
            }
        }

        /// <summary>
        /// Connection Invite event.
        /// </summary>
        public static event OnInviteDelegate OnInvite
        {
            add
            {
                MLAppConnectNativeBindings.OnInvite += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnInvite -= value;
            }
        }

        /// <summary>
        /// Event Info.
        /// </summary>
        public static event OnEventDelegate OnEvent
        {
            add
            {
                MLAppConnectNativeBindings.OnEvent += value;
            }

            remove
            {
                MLAppConnectNativeBindings.OnEvent -= value;
            }
        }

#endif

        /// <summary>
        /// Type of connection to be created.
        /// </summary>
        public enum ConnectionType
        {
            /// <summary>
            /// Unknown connection type.
            /// </summary>
            UnknownConnectionType,

            /// <summary>
            /// Loopback Connection.
            /// </summary>
            Loopback,

            /// <summary>
            /// Connection with default WebRTC.
            /// </summary>
            Default
        }

        /// <summary>
        /// The type of media pipe to send/receive data with remote users.
        /// </summary>
        public enum PipeType
        {
            /// <summary>
            /// Unknown Pipe type.
            /// </summary>
            UnknownPipeType,

            /// <summary>
            /// Pipe type for Audio transfer.
            /// </summary>
            AudioPipe,

            /// <summary> Pipe type for Data transfer.
            /// </summary>
            DataPipe,

            /// <summary>
            /// Pipe type for Video transfer.
            /// </summary>
            VideoPipe
        }

        /// <summary>
        /// The reliability of the pipe.
        /// </summary>
        public enum PipeReliability
        {
            /// <summary>
            /// Unknown reliability.
            /// </summary>
            UnknownPipeReliability,

            /// <summary>
            /// Reliable data transfer.
            /// </summary>
            Reliable,

            /// <summary>
            /// Unreliable data transfer.
            /// </summary>
            Unreliable
        }

        /// <summary>
        /// The data ordering of the pipe.
        /// </summary>
        public enum PipeOrdering
        {
            /// <summary>
            /// Unknown order.
            /// </summary>
            UnknownPipeOrdering,

            /// <summary>
            /// Data transfer in order.
            /// </summary>
            Ordered,

            /// <summary>
            /// Data transfer not in order.
            /// </summary>
            NotOrdered
        }

        /// <summary>
        /// The direction of data flow in a pipe.
        /// </summary>
        public enum PipeDirection
        {
            /// <summary>
            /// Pipe direction, Unknown.
            /// </summary>
            UnknownPipeDirection,

            /// <summary>
            /// Pipe direction, <c>UniDirection</c> out.
            /// </summary>
            UniDirectionalOut,

            /// <summary>
            /// Pipe direction, <c>UniDirection</c> in.
            /// </summary>
            UniDirectionalIn,

            /// <summary>
            /// Pipe direction, BiDirection.
            /// </summary>
            BiDirectional,
        }

        /// <summary>
        /// The priority of the pipe.
        /// </summary>
        public enum Priority
        {
            /// <summary>
            /// priority, High.
            /// </summary>
            High,

            /// <summary>
            /// priority, Normal.
            /// </summary>
            Normal,

            /// <summary>
            /// priority, Low.
            /// </summary>
            Low
        }

        /// <summary>
        /// The AppConnect event types that can be reported to client applications.
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// Unknown Event.
            /// </summary>
            UnknownEvent,

            /// <summary>
            /// Connection Created.
            /// </summary>
            ConnectionCreated,

            /// <summary>
            /// Connection Creation Failed.
            /// </summary>
            ConnectionFailed,

            /// <summary>
            /// Connection Deleted.
            /// </summary>
            ConnectionDeleted,

            /// <summary>
            /// Pipe Created.
            /// </summary>
            PipeCreated,

            /// <summary>
            /// Pipe Creation Failed.
            /// </summary>
            PipeFailed,

            /// <summary>
            /// Pipe Deleted.
            /// </summary>
            PipeDeleted,

            /// <summary>
            /// Pipe Blocked.
            /// </summary>
            PipeBlocked,

            /// <summary>
            /// Pipe Unblocked.
            /// </summary>
            PipeUnblocked,

            /// <summary>
            /// Pipe received large data packet(s).
            /// </summary>
            PipeLargeData,

            /// <summary>
            /// Microphone Muted.
            /// </summary>
            MicMuted,

            /// <summary>
            /// Microphone Unmuted.
            /// </summary>
            MicUnmuted,

            /// <summary>
            /// Contact joined the connection.
            /// </summary>
            UserJoined,

            /// <summary>
            /// Contact left the connection.
            /// </summary>
            UserExited,

            /// <summary>
            /// A new user got invited.
            /// </summary>
            UserInvite,

            /// <summary>
            /// User decided to abort the "invite dialog".
            /// </summary>
            UsersInviteAbort,
        }

        /// <summary>
        /// Video format.
        /// </summary>
        public enum VideoFormat
        {
            /// <summary>
            /// RGBA format.
            /// </summary>
            RGBA,

            /// <summary>
            /// I420 video format.
            /// </summary>
            I420
        }

#if PLATFORM_LUMIN

        /// <summary>
        /// Gets or sets Name of the connection used either used to create the connection or join an existing one.
        /// The field will be populated once a connection has been successfully created.
        /// </summary>
        public static string ConnectionName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Determines if the connection has been joined or created.
        /// </summary>
        public static bool IsConnectionActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Determines if the connection was created by the current user.
        /// </summary>
        public static bool IsOwner { get; set; }

        /// <summary>
        /// Start the MLConnections API. This will register and set callbacks for receiving an ACP invite and creating a ACP connection.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully started.
        /// </returns>
        public static MLResult Start()
        {
            CreateInstance();
            return MLAppConnect.BaseStart();
        }

        /// <summary>
        /// Create connection.
        /// Initial step to create a new connection.
        /// Invoking this function will start by displaying the "invite dialog" interface in the "initiator device".
        /// The user will be able to invite one or more of their users to the connection.Each one of the invited
        /// user will then receive a system notification to be accepted or declined.
        /// A connection callback (registered by #MLAppConnectRegisterConnectionCallback()) should be
        /// set up before calling this function.
        /// </summary>
        /// <param name="connectionName">Name of the connection</param>
        /// <param name="type">The connection type.</param>
        /// <param name="connectionPriority">The priority of the connection.</param>
        /// <param name="maxParticipants">Max number of participants allowed in the connection.</param>
        /// <param name="lockUsers">Set boolean for other participants to not have access rights to invite other users.</param>
        /// <param name="lockPipes">Set boolean for other participants to not have access rights to create pipes.</param>
        /// <returns>
        /// MLResult_Ok if successfully created a connection, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateConnection(string connectionName, ConnectionType type, Priority connectionPriority, uint maxParticipants, bool lockUsers, bool lockPipes)
        {
            ConnectionName = connectionName;
            ConnectionProperties connectionProperties = ConnectionProperties.Create();
            connectionProperties.Type = type;
            connectionProperties.Priority = connectionPriority;
            connectionProperties.MaxParticipants = maxParticipants;
            connectionProperties.LockContacts = lockUsers;
            connectionProperties.LockPipes = lockPipes;

            if (MagicLeapNativeBindings.MLHandleIsValid(inviteHandle))
            {
                MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCreateConnection(connectionName, ref connectionProperties);
                if (!MLResult.IsOK(resultCode))
                {
                    MLResult resultError = MLResult.Create(resultCode);

                    MLPluginLog.ErrorFormat("MLAppConnect.CreateConnection failed to create a new connection, Reason: {0}", resultError);

                    return resultError;
                }

                return MLResult.Create(resultCode);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLAppConnect.CreateConnection failed to create a new connection, Reason: InviteHandle is invalid");

                return MLResult.Create(MLResult.Code.UnspecifiedFailure, " Reason: InviteHandle is invalid");
            }
        }

        /// <summary>
        /// Create connection using the  default connection properties.
        /// Initial step to create a new connection.
        /// Invoking this function will start by displaying the "invite dialog" interface in the "initiator device".
        /// The user will be able to invite one or more of their users to the connection.Each one of the invited
        /// user will then receive a system notification to be accepted or declined.
        /// A connection callback (registered by #MLAppConnectRegisterConnectionCallback()) should be
        /// set up before calling this function.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully created a connection, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateConnection()
        {
            ConnectionProperties connectionProperties = ConnectionProperties.Create();

            if (MagicLeapNativeBindings.MLHandleIsValid(inviteHandle))
            {
                MLResult.Code resultCode;
                uint attemptCount = 1;
                do
                {
                    ConnectionName = "connctionid" + attemptCount.ToString();
                    resultCode = MLAppConnectNativeBindings.MLAppConnectCreateConnection(ConnectionName, ref connectionProperties);
                    attemptCount++;
                }
                while (resultCode == MLResult.Code.AppConnectConnectionExists && attemptCount < 15);

                if (!MLResult.IsOK(resultCode))
                {
                    MLResult resultError = MLResult.Create(resultCode);
                    MLPluginLog.ErrorFormat("MLAppConnect.CreateConnection failed to create a new connection, Reason: {0}", resultError);
                    return resultError;
                }

                return MLResult.Create(resultCode);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLAppConnect.CreateConnection failed to create a new connection, Reason: InviteHandle is invalid");

                return MLResult.Create(MLResult.Code.UnspecifiedFailure, " Reason: InviteHandle is invalid");
            }
        }

        /// <summary>
        /// Invite users.
        /// This function is similar #MLAppConnectCreateConnection(), but works with an ongoing connection.
        /// It will also display the "invite dialog" and allow the user to invite more contacts.
        /// A connection event callback (registered by #MLAppConnectRegisterEventCallback()) should be
        /// set up before calling this function.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successful, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult InviteUsers()
        {
            if (MagicLeapNativeBindings.MLHandleIsValid(connectionHandle))
            {
                MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectInviteUsers(ConnectionName);

                if (!MLResult.IsOK(resultCode))
                {
                    MLResult resultError = MLResult.Create(resultCode);

                    MLPluginLog.ErrorFormat("MLAppConnect.InviteUsers failed to create a new connection, Reason: {0}", resultError);

                    return resultError;
                }

                return MLResult.Create(resultCode);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLAppConnect.InviteUsers failed to create a new connection, Reason: EventHandle is invalid");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, " Reason: EventnHandle is invalid");
            }
        }

        /// <summary>
        /// Co-join connection.
        /// Once a user is invited to join some specific connection, a system notification will be displayed
        /// and the user will be able to accept or decline.
        /// If the invite is accepted, the corresponding app will load and be ready to join the connection.
        /// The callback #MLAppConnectInviteCallback will then be triggered and bring any information related
        /// to the connection. In special, the "connection name" used by <c>#MLAppConnectCojoinConnection()</c>.
        /// A connection callback (registered by <c>#MLAppConnectRegisterConnectionCallback())</c> should be
        /// set up before calling this function.
        /// </summary>
        /// <returns>
        /// MLAppConnectResult_Ok if successfully unblocked a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CojoinConnection()
        {
            if (MagicLeapNativeBindings.MLHandleIsValid(connectionHandle))
            {
                MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCojoinConnection(ConnectionName);

                if (!MLResult.IsOK(resultCode))
                {
                    MLResult resultError = MLResult.Create(resultCode);

                    MLPluginLog.ErrorFormat("MLAppConnect.CojoinConnection failed to create a new connection, Reason: {0}", resultError);

                    return resultError;
                }

                return MLResult.Create(resultCode);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLAppConnect.CojoinConnection failedto create a new connection, Reason: ConnectionHandle is invalid");

                return MLResult.Create(MLResult.Code.UnspecifiedFailure, " Reason: ConnectionHandle is invalid");
            }
        }

        /// <summary>
        /// Delete connection.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully deleted a connection, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult DeleteConnection()
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCojoinConnection(ConnectionName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.CreateConnection failed to create a new connection, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Create data pipe.
        /// Creates a "general purpose" data pipe.The functions #MLAppConnectSendData() and #MLAppConnectReadData() should
        /// be used to write to and read from this pipe.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to create.</param>
        /// <param name="dataPipe">The data pipe properties for this pipe.</param>
        /// <returns>
        /// MLResult_Ok if successfully created a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateDataPipe(string pipeName, DataPipeProperties dataPipe)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCreateDataPipe(ConnectionName, pipeName, ref dataPipe);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.CreateDataPipe failed to create a new data pipe, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Create data pipe.
        /// Creates a "general purpose" data pipe.The functions #MLAppConnectSendData() and #MLAppConnectReadData() should
        /// be used to write to and read from this pipe.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to create.</param>
        /// <returns>
        /// MLResult_Ok if successfully created a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateDataPipe(string pipeName)
        {
            DataPipeProperties dataPipe = DataPipeProperties.Create();

            return CreateDataPipe(pipeName, dataPipe);
        }

        /// <summary>
        /// Create video pipe.
        /// Creates a "video specialized" data pipe.The functions #MLAppConnectSendVideoFrame() and
        /// #MLAppConnectReadVideoFrame() should be used to write to and read from this pipe.
        /// </summary>
        /// <param name="pipeName">pipe_name The name of the pipe to create </param>
        /// <param name="videoPipe">Properties of the video pipe to be set</param>
        /// <returns>
        ///  MLResult_Ok if successfully created a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateVideoPipe(string pipeName, VideoPipeProperties videoPipe)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCreateVideoPipe(ConnectionName, pipeName, ref videoPipe);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.CreateVideePipe failed to create a new video pipe, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Create video pipe.
        /// Creates a "video specialized" data pipe.The functions #MLAppConnectSendVideoFrame() and
        /// #MLAppConnectReadVideoFrame() should be used to write to and read from this pipe.
        /// </summary>
        /// <param name="pipeName">pipe_name The name of the pipe to create. </param>
        /// <returns>
        ///  MLResult_Ok if successfully created a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateVideoPipe(string pipeName)
        {
            VideoPipeProperties videoPipe = VideoPipeProperties.Create();
            return CreateVideoPipe(pipeName, videoPipe);
        }

        /// <summary>
        /// This function should be called to delete a "data pipe" or a "video pipe" or an "audio pipe".
        /// This function should NOT be used to delete "video capture pipe" or "microphone audio pipe".
        /// For these, use <c>#MLAppConnectDeleteVideoCapturePipe()</c> and <c>#MLAppConnectDeleteMicAudioPipe()</c> instead.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to delete.</param>
        /// <returns>
        /// MLResult_Ok if successfully deleted the pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult DeletePipe(string pipeName)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectDeletePipe(ConnectionName, pipeName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.DeletePipe failed to delete pipe, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Create video capture pipe.
        /// Creates a "video capture specialized" data pipe.
        /// No function should be used to write to this pipe.It is managed and fed automatically with frames.
        /// from the device's camera.
        /// The other peers of the connection should use #MLAppConnectReadVideoFrame() to read the video frames
        /// from this pipe.From the receiver side, a "video capture pipe" is no different than a "regular video pipe".
        /// </summary>
        /// <param name="videoPipe">The name of the pipe to delete.</param>
        /// <returns>
        /// MLResult_Ok if successfully created a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateVideeCapturePipe(VideoPipeProperties videoPipe)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCreateVideoCapturePipe(ConnectionName, ref videoPipe);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.CreateVideeCapturePipe failed to create a new video capture pipe, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Create video capture pipe.
        /// Creates a "video capture specialized" data pipe.
        /// No function should be used to write to this pipe.It is managed and fed automatically with frames.
        /// from the device's camera.
        /// The other peers of the connection should use #MLAppConnectReadVideoFrame() to read the video frames
        /// from this pipe.From the receiver side, a "video capture pipe" is no different than a "regular video pipe".
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully created a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult CreateVideoCapturePipe()
        {
            VideoPipeProperties videoPipe = VideoPipeProperties.Create();
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCreateVideoCapturePipe(ConnectionName, ref videoPipe);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.CreateVideeCapturePipe failed to create a new video capture pipe, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Delete video capture pipe.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully deleted a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult DeleteVideoCapturePipe()
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectDeleteVideoCapturePipe(ConnectionName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.DeleteVideoCapturePipe failed to delete video capture pipe, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Create microphone audio pipe.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully created a pipe, error code from <c>#MLAppConnectResult</c> otherwise.
        /// </returns>
        public static MLResult CreateMicAudioPipe()
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectCreateMicAudioPipe(ConnectionName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.CreateMicAudioPipe failed to create an audio pipe, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Delete the audio pipe for the (internally managed) microphone from the connection..
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully deleted a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult DeleteMicAudioPipe()
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectDeleteMicAudioPipe(ConnectionName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.DeleteMicAudioPipe failed to delete a audio pipe, Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Mute the microphone for the connection.
        /// </summary>
        /// <returns>
        /// MLResult_Ok if successfully muted a pipe, error code from #MLAppConnectResult otherwise
        /// </returns>
        public static MLResult MuteMic()
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectMuteMic(ConnectionName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.MuteMic failed to mute mic audio, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// UnMute the microphone for the connection.
        /// </summary>
        /// <returns>
        /// LResult_Ok if successfully muted a pipe, error code from <c>#MLAppConnectResult</c> otherwise
        /// </returns>
        public static MLResult UnmuteMic()
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectUnmuteMic(ConnectionName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnmuteMic failed to unmute mic audio, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Check if the microphone is muted for the the connection.
        /// </summary>
        /// <param name="isMuted">out: if the Microphone is muted.</param>
        /// <returns>
        /// MLResult_Ok if successfully returned the value, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult IsMicMuted(out bool isMuted)
        {
            isMuted = false;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectIsMicMuted(ConnectionName, ref isMuted);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.IsMicMuted failed to detect if mic is muted, Reason: {0}", resultError);
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Mute user.
        /// Mute audio from another user.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <returns>
        /// MLResult_Ok if successfully returned the value, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult MuteUser(string user)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectMuteUser(ConnectionName, user);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.MuteUser failed to mute user, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Mute user.
        /// Mute audio from another user.
        /// </summary>
        /// <param name="user">The user name.</param>
        /// <returns>
        /// MLResult_Ok if successfully returned the value, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult UnmuteUser(string user)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectUnmuteUser(ConnectionName, user);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnmuteMic failed to unmute user, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Send a byte array.
        /// This function should be called to send data over a pipe.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to send the data.</param>
        /// <param name="byteArray">The data to be sent.</param>
        /// <returns>
        /// MLResult_Ok if successfully sent data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult SendData(string pipeName, byte[] byteArray)
        {
            IntPtr data = Marshal.AllocHGlobal(byteArray.Length);
            Marshal.Copy(byteArray, 0, data, byteArray.Length);
            Debug.LogErrorFormat("After Marshal Copy byte array lengnth: {0}", byteArray.Length);
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectSendData(ConnectionName, pipeName, data, (ulong)byteArray.Length);
            Debug.LogErrorFormat("Size of data: {0}", Marshal.SizeOf(data));
            Marshal.FreeHGlobal(data);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.SendData failed to send data, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Send Large data over a pipe.
        /// Send Large data(>IP MTU) to a pipe of this connection.
        /// </summary>
        /// <param name="pipeName">Name of the pipe</param>
        /// <param name="byteArray">The name of the pipe to send the data.</param>
        /// <returns>
        /// MLResult_Ok if successfully sent data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult SendLargeData(string pipeName, byte[] byteArray)
        {
            IntPtr data = Marshal.AllocHGlobal(byteArray.Length);
            Marshal.Copy(byteArray, 0, data, byteArray.Length);
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectSendLargeData(ConnectionName, pipeName, data, (ulong)byteArray.Length);
            Marshal.FreeHGlobal(data);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.SendLargeData failed to send large data, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Read data.
        /// This function will block the execution until at least #min_bytes are read from the pipe.
        /// #min_bytes can also be set to 0 and this function will return immediately with or without
        /// any incoming data.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to read the data.</param>
        /// <param name="byteArray">The data read from the pipe.</param>
        /// <param name="minBytes">The minimum size of the data that needs to read before returning.</param>
        /// <returns>
        /// MLResult_Ok if successfully read data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult GetData(string pipeName, out byte[] byteArray, uint minBytes = 0)
        {
            IntPtr data = IntPtr.Zero;
            ulong size = 0;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectReadData(ConnectionName, pipeName, minBytes, ref data, ref size);
            byteArray = null;
            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.Getdata failed to read data, Reason: {0}", resultError);
                return resultError;
            }
            else
            {
                // null check result check
                if (size > 0)
                {
                    byteArray = new byte[size];
                    Marshal.Copy(data, byteArray, 0, (int)size);
                    resultCode = MLAppConnectNativeBindings.MLAppConnectFreeData(data);
                }

                if (!MLResult.IsOK(resultCode))
                {
                    MLResult resultError = MLResult.Create(resultCode);
                    MLPluginLog.ErrorFormat("MLAppConnect.GetData failed to free data after a successful read, Reason: {0}", resultError);
                    return resultError;
                }
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Read Large.
        /// [non-blocking; ownership of memory is transferred to the client]
        /// Read (large) data from a pipe of this connection. Due to fragmentation and reassembly
        /// of large datagrams when calling this API there is no guarantee that this datagram is received
        /// in order with data that is sent using the Send API, MLAppConnectSendLarge().
        /// </summary>
        /// <param name="pipeName">The name of the pipe to read the data.</param>
        /// <param name="byteArray">Address of data read from the pipe.</param>
        /// <param name="packetsRemaining">The number of packets remaining.</param>
        /// <returns>
        /// MLResult_Ok if successfully read data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult GetLargeData(string pipeName, out byte[] byteArray, out ulong packetsRemaining)
        {
            IntPtr data = IntPtr.Zero;
            ulong size = 0;
            packetsRemaining = 0;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectReadLargeData(ConnectionName, pipeName, ref data, ref size, ref packetsRemaining);

            byteArray = null;

            if (!MLResult.IsOK(resultCode))
            {
                byteArray = null;
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.GetLargeData failed to read Data, Reason: {0}", resultError);
                return resultError;
            }
            else
            {
                // null check result check
                if (size > 0)
                {
                    byteArray = new byte[size];
                    Marshal.Copy(data, byteArray, 0, (int)size);
                    resultCode = MLAppConnectNativeBindings.MLAppConnectFreeLargeData(data);
                }

                if (!MLResult.IsOK(resultCode))
                {
                    MLResult resultError = MLResult.Create(resultCode);
                    MLPluginLog.ErrorFormat("MLAppConnect.GetLargeData failed to free large data after a successful read, Reason: {0}", resultError);
                    return resultError;
                }
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Send video frame.
        /// This function should be called to send video data over a pipe.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to send the data.</param>
        /// <param name="videoFrame">The video frame.</param>
        /// <param name="imageType">The Image type of the texture.</param>
        /// <returns>
        /// MLResult_Ok if successfully sent data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult SendVideoFrame(string pipeName, Texture2D videoFrame, MLImageNativeBindings.MLImageType imageType = MLImageNativeBindings.MLImageType.RGBA32)
        {
            byte[] imageData = MLTextureUtils.ConvertToByteArray(videoFrame, out int numChannels);
            MLImageNativeBindings.MLImageNative nativeImage = new MLImageNativeBindings.MLImageNative
            {
                ImageType = imageType,
                Alignment = 1,
                Width = (uint)videoFrame.width,
                Height = (uint)videoFrame.height,
                Image = Marshal.AllocHGlobal(imageData.Length)
            };
            Marshal.Copy(imageData, 0, nativeImage.Image, imageData.Length);
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectSendVideoFrame(ConnectionName, pipeName, ref nativeImage);
            Marshal.FreeHGlobal(nativeImage.Image);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.SendVideoFrame failed to send frame, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Read a video frame from a video pipe and Free data buffers allocated by MLAppConnectReadVideoFrame().
        /// This will fill up out_video_frame structure with data related to the video frame.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to read the data.</param>
        /// <param name="videoFrame">[out] New Video frame read as a Texture 2D</param>
        /// <returns>
        /// MLResult_Ok if successfully read data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult GetVideoFrame(string pipeName, out Texture2D videoFrame)
        {
            videoFrame = null;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectReadVideoFrame(ConnectionName, pipeName, false, out cachedNativeImage);

            if (!MLResult.IsOK(resultCode))
            {
                cachedNativeImage.Image = IntPtr.Zero;
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.GetVideoFrame failed to read frame, Reason: {0}", resultError);
                return resultError;
            }
            else if (cachedNativeImage.Image != IntPtr.Zero)
            {
                TextureFormat texFormat = cachedNativeImage.ImageType == MLImageNativeBindings.MLImageType.RGB24 ? TextureFormat.RGB24 : TextureFormat.RGBA32;
                videoFrame = new Texture2D((int)cachedNativeImage.Width, (int)cachedNativeImage.Height, texFormat, false, false);

                uint size = cachedNativeImage.Width * cachedNativeImage.Height;
                size = cachedNativeImage.ImageType == MLImageNativeBindings.MLImageType.RGB24 ? (size * 3) : (size * 4);

                videoFrame.LoadRawTextureData(cachedNativeImage.Image, (int)size);

                videoFrame.Apply();

                resultCode = MLAppConnectNativeBindings.MLAppConnectFreeVideoFrame(ref cachedNativeImage);

                if (!MLResult.IsOK(resultCode))
                {
                    cachedNativeImage.Image = IntPtr.Zero;
                    MLResult resultError = MLResult.Create(resultCode);
                    MLPluginLog.ErrorFormat("MLAppConnect.GetVideoFrame failed to free video frame after a successful read, Reason: {0}", resultError);
                    return resultError;
                }
            }
            else
            {
                Debug.LogErrorFormat(" Video Frame Read result = {0}", MLResult.Create(resultCode));
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Block a pipe in the connection (block sending and/or block data).
        /// </summary>
        /// <param name="pipeName">An unique identifier for the pipe.</param>
        /// <returns>
        /// MLResult_Ok if successfully read data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult BlockPipe(string pipeName)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectBlockPipe(ConnectionName, pipeName);
            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.BlockPipe failed to block pipe, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Unblock a pipe in the connection (resume sending and/or receiving data).
        /// </summary>
        /// <param name="pipeName">An unique identifier for the pipe.</param>
        /// <returns>
        /// MLAppConnectResult_Ok if successfully unblocked a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult UnblockPipe(string pipeName)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectBlockPipe(ConnectionName, pipeName);
            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnblockPipe failed to ublock a pipe, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Check if Pipe if blocked.
        /// </summary>
        /// <param name="pipeName"> An unique identifier for the pipe.</param>
        /// <param name="pipeBlocked">Indicates if the pipe is blocked or not.</param>
        /// <returns>
        /// MLAppConnectResult_Ok if successfully unblocked a pipe, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult IsPipeBlocked(string pipeName, out bool pipeBlocked)
        {
            pipeBlocked = false;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectIsPipeBlocked(ConnectionName, pipeName, ref pipeBlocked);
            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.IsPipeBlocked failed to detect if pipe is blocked, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Unblocks a blocked pipe.
        /// See #MLAppConnectReadData(), #MLAppConnectReadLargeData() and #MLAppConnectReadVideoFrame() for more details
        /// on why and how a pipe can get blocked.
        /// </summary>
        /// <param name="pipeName">The pipe name.</param>
        /// <returns>
        /// MLResult_Ok if successfully unblocks read data, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult UnblockRead(string pipeName)
        {
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectUnblockRead(ConnectionName, pipeName);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnblockPipe failed to unblock read, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Get Participants.
        /// Get a list of participants in an ongoing connection.
        /// </summary>
        /// <param name="participantList">The list of participants in this connection.</param>
        /// <returns>
        /// MLResult_Ok if successfully able to get the participants
        /// error code from #MLAppConnectResult otherwise
        /// </returns>
        public static MLResult GetParticipantList(out string[] participantList)
        {
            IntPtr unmanagedParticipantList = IntPtr.Zero;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectGetParticipants(ConnectionName, out unmanagedParticipantList, out uint count);

            IntPtr[] participantIntPtrArray = new IntPtr[count];
            participantList = new string[count];

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnblockPipe failed to unblock read, Reason: {0}", resultError);
                return resultError;
            }

            Marshal.Copy(unmanagedParticipantList, participantIntPtrArray, 0, (int)count);

            for (int i = 0; i < count; i++)
            {
                participantList[i] = Marshal.PtrToStringAnsi(participantIntPtrArray[i]);
            }

            resultCode = MLAppConnectNativeBindings.MLAppConnectFreeParticipants(unmanagedParticipantList, count);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnblockPipe failed to unblock read, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Get a list of strings with the pipe names.
        /// </summary>
        /// <param name="pipeNames">The list of pipe names.</param>
        /// <returns>
        /// MLResult_Ok if successfully returned the value, error code from #MLAppConnectResult otherwise.
        /// </returns>
        public static MLResult GetPipeNames(out string[] pipeNames)
        {
            IntPtr unmanagedPipeList = IntPtr.Zero;
            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectGetPipeNames(ConnectionName, out unmanagedPipeList, out uint count);

            IntPtr[] pipeIntPtrArray = new IntPtr[count];
            pipeNames = new string[count];

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnblockPipe failed to unblock read, Reason: {0}", resultError);
                return resultError;
            }

            Marshal.Copy(unmanagedPipeList, pipeIntPtrArray, 0, (int)count);

            for (int i = 0; i < count; i++)
            {
                pipeNames[i] = Marshal.PtrToStringAnsi(pipeIntPtrArray[i]);
            }

            resultCode = MLAppConnectNativeBindings.MLAppConnectFreePipeNames(unmanagedPipeList, count);

            if (!MLResult.IsOK(resultCode))
            {
                MLResult resultError = MLResult.Create(resultCode);
                MLPluginLog.ErrorFormat("MLAppConnect.UnblockPipe failed to unblock read, Reason: {0}", resultError);
                return resultError;
            }

            return MLResult.Create(resultCode);
        }

        /// <summary>
        /// Register to Event Callback to receive ACP Events.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>
        /// <para>MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if the registration resource allocation failed.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a given argument is invalid.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully initialized.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if there was an unexpected failure.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.ConnectionsAlreadyRegistered</c> if this is a duplicate registration.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.ConnectionsNetworkFailure</c> if communication to the network failed.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.ConnectionsSystemFailure</c> if there was system failure.</para>
        /// </returns>
        public static MLResult RegisterEventCallback(string connectionName)
        {
            MLAppConnectNativeBindings.EventCallback eventCallbacks = MLAppConnectNativeBindings.EventCallback.Create();

            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectRegisterEventCallback(connectionName, ref eventCallbacks, ref eventHandle);
            MLPluginLog.ErrorFormat("MLAppConnect.OnConnection Attempt: {0}", connectionName);
            if (!MLResult.IsOK(resultCode) || !MagicLeapNativeBindings.MLHandleIsValid(inviteHandle))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.OnConnection failed in RegisterEventCallback() to register to receive events. Handle is invalid or Reason: {0}", resultError);

                return resultError;
            }

            MLPluginLog.ErrorFormat("MLAppConnect.OnConnection resilt {0}", MLResult.Create(resultCode));
            return MLResult.Create(resultCode);
        }

#if !DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Called by MLAPISingleton to start the API
        /// </summary>
        /// <returns>
        /// <para>MLResult.Result will be <c>MLResult.Code.AllocFailed</c> if the registration resource allocation failed.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a given argument is invalid.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully initialized.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if there was an unexpected failure.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.ConnectionsAlreadyRegistered</c> if this is a duplicate registration.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.ConnectionsNetworkFailure</c> if communication to the network failed.</para>
        /// <para>MLResult.Result will be <c>MLResult.Code.ConnectionsSystemFailure</c> if there was system failure.</para>
        /// </returns>
        protected override MLResult StartAPI()
        {
            MLAppConnectNativeBindings.ConnectionCallback connectionCallbacks = MLAppConnectNativeBindings.ConnectionCallback.Create();

            MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectRegisterConnectionCallback(ref connectionCallbacks, ref connectionHandle);

            if (!MLResult.IsOK(resultCode) || !MagicLeapNativeBindings.MLHandleIsValid(connectionHandle))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.Start failed in StartAPI() to register to receive connection callbacks. Handle is invalid or Reason: {0}", resultError);

                return resultError;
            }

            MLAppConnectNativeBindings.InviteCallback inviteCallbacks = MLAppConnectNativeBindings.InviteCallback.Create();

            resultCode = MLAppConnectNativeBindings.MLAppConnectRegisterInviteCallback(ref inviteCallbacks, ref inviteHandle);

            if (!MLResult.IsOK(resultCode) || !MagicLeapNativeBindings.MLHandleIsValid(inviteHandle))
            {
                MLResult resultError = MLResult.Create(resultCode);

                MLPluginLog.ErrorFormat("MLAppConnect.Start failed in StartAPI() to register to receive invites. Handle is invalid or Reason: {0}", resultError);

                return resultError;
            }

            return MLResult.Create(resultCode);
        }
#endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Update loop.
        /// </summary>
        protected override void Update()
        {
        }

        /// <summary>
        /// Called by MLAPISingleton on destruction
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects">Used for cleanup.</param>
        protected override void CleanupAPI(bool isSafeToAccessManagedObjects)
        {
            if (ConnectionName != null)
            {
                DeleteConnection();
            }

            if (MagicLeapNativeBindings.MLHandleIsValid(eventHandle))
            {
                MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectUnregisterEventCallback(ConnectionName, ref eventHandle);

                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLAppConnect.Stop failed in CleapupAPI() to shutdown registration to receive invites. Reason: {0}", MLResult.CodeToString(resultCode));
                }
            }

            if (MagicLeapNativeBindings.MLHandleIsValid(connectionHandle))
            {
                MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectUnregisterConnectionCallback(connectionHandle);

                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLAppConnect.Stop failed in CleapupAPI() to shutdown registration to receive invites. Reason: {0}", MLResult.CodeToString(resultCode));
                }
            }

            if (MagicLeapNativeBindings.MLHandleIsValid(inviteHandle))
            {
                MLResult.Code resultCode = MLAppConnectNativeBindings.MLAppConnectUnregisterInviteCallback(inviteHandle);

                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLAppConnect.Stop failed in CleapupAPI() to shutdown registration to receive invites. Reason: {0}", MLResult.CodeToString(resultCode));
                }
            }

            eventHandle = MagicLeapNativeBindings.InvalidHandle;
            connectionHandle = MagicLeapNativeBindings.InvalidHandle;
            inviteHandle = MagicLeapNativeBindings.InvalidHandle;
        }

        /// <summary>
        /// Creates an Instance of the MLAPPConnect class.
        /// </summary>
        private static void CreateInstance()
        {
            if (!MLAppConnect.IsValidInstance())
            {
                MLAppConnect._instance = new MLAppConnect();
            }
        }

        /// <summary>
        /// The connection properties of a AppConnect connection.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ConnectionProperties
        {
            /// <summary>Version of this structure. </summary>
            public uint Version;

            /// <summary>The connection type to create a connection. </summary>
            public MLAppConnect.ConnectionType Type;

            /// <summary>The priority level of this connection. </summary>
            public MLAppConnect.Priority Priority;

            /// <summary>Max participants per connection. It may be any number between 2 and #MLAppConnectConnectionMaxParticipants </summary>
            public uint MaxParticipants;

            /// <summary>Lock remote connections from inviting contacts. </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool LockContacts;

            /// <summary>Lock remote connections from creating/deleting pipes. </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool LockPipes;

            /// <summary>
            /// Initializes a new instance of ConnectionProperties.
            /// </summary>
            /// <returns>
            /// Instance of MLAppConnectConnectionProperties object.
            /// </returns>
            public static ConnectionProperties Create()
            {
                return new ConnectionProperties()
                {
                    Version = 8,
                    Type = MLAppConnect.ConnectionType.Default,
                    Priority = MLAppConnect.Priority.Normal,
                    MaxParticipants = ConnectionMaxParticipants,
                    LockContacts = false,
                    LockPipes = false
                };
            }
        }

        /// <summary>
        /// Pipe Properties
        /// </summary>
        public struct PipeProperties
        {
            /// <summary>
            /// The pipe type to create a pipe.
            /// </summary>
            public PipeType Type;

            /// <summary>
            /// The directionality (bi-direction, <c>uni-direction</c> in, out) of the pipe.
            /// </summary>
            public PipeDirection Direction;

            /// <summary>
            /// The priority level of this pipe (relative to connection).
            /// </summary>
            public Priority Priority;
        }

        /// <summary>
        /// Data Pipe Properties.
        /// </summary>
        public struct DataPipeProperties
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// The base pipe properties.
            /// </summary>
            public PipeProperties BasePipe;

            /// <summary>
            /// The reliability requirements of the pipe.
            /// </summary>
            public PipeReliability Reliability;

            /// <summary>
            /// The packet ordering requirements of the pipe.
            /// </summary>
            public PipeOrdering Ordering;

            /// <summary>
            /// The buffer size (memory allocation) for the pipe's queue(s).
            /// </summary>
            public long BufferSize;

            /// <summary>
            /// Initializes a new instance of ConnectionProperties.
            /// </summary>
            /// <returns>
            /// Instance of MLAppConnectConnectionProperties object.
            /// </returns>
            public static DataPipeProperties Create()
            {
                return new DataPipeProperties()
                {
                    Version = 8,
                    BasePipe = new PipeProperties()
                    {
                        Type = PipeType.DataPipe,
                        Direction = PipeDirection.BiDirectional,
                        Priority = Priority.Normal
                    },
                    Reliability = PipeReliability.Reliable,
                    Ordering = PipeOrdering.Ordered,
                    BufferSize = 4096
                };
            }
        }

        /// <summary>
        /// Video data properties.
        /// </summary>
        public struct VideoPipeProperties
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// The base pipe properties.
            /// </summary>
            public PipeProperties BasePipe;

            /// <summary>
            /// Preferred width of the frame.
            /// </summary>
            public uint Width;

            /// <summary>
            /// Preferred height of the frame.
            /// </summary>
            public uint Height;

            /// <summary>
            /// Maximum width supported by the pipe.
            /// It must be a multiple of 16.
            /// </summary>
            public uint MaxWidth;

            /// <summary>
            /// Maximum height supported by the pipe.
            /// It must be a multiple of 16.
            /// </summary>
            public uint MaxHeight;

            /// <summary>
            /// Rate at which frames are send in this pipe.
            /// </summary>
            public uint Framerate;

            /// <summary>
            /// Video format.
            /// This is ignored for video capture pipe.
            /// </summary>
            public VideoFormat Format;

            /// <summary>
            /// Initializes a new instance of VideoPipeProperties.
            /// </summary>
            /// <returns>
            /// Instance of MLAppConnectVideoPipeProperties object.
            /// </returns>
            public static VideoPipeProperties Create()
            {
                return new VideoPipeProperties()
                {
                    Version = 8,
                    BasePipe = new PipeProperties()
                    {
                        Type = PipeType.VideoPipe,
                        Direction = PipeDirection.BiDirectional,
                        Priority = Priority.Normal
                    },
                    Width = 640,
                    Height = 480,
                    MaxWidth = 640,
                    MaxHeight = 480,
                    Framerate = 10,
                    Format = VideoFormat.RGBA
                };
            }
        }

        /// <summary>
        /// Connection info in a connection callback MLAppConnectConnectionCallback().
        /// this structure contains unmanaged data and is for  internal use only.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ConnectionInfo
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Name of the connection for which the callback is received.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string ConnectionName;

            /// <summary>
            /// Result of an operation performed.
            /// The values are defined in enum #MLAppConnectResult.
            /// </summary>
            public MLResult.Code Result;
        }

        /// <summary>
        /// Information about a connection from the connection event callback (#MLAppConnectEventCallback()).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ConnectionEventInfo
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Type of a connection.
            /// </summary>
            public MLAppConnect.ConnectionType ConnectioType;

            /// <summary>
            /// Status of a connection.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Status;
        }

        /// <summary>
        /// Information about a pipe from the connection event callback (#MLAppConnectEventCallback()).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PipeEventInfo
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Name of a pipe.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string PipeName;

            /// <summary>
            /// Owner of a pipe.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string PipeOwner;

            /// <summary>
            /// Type of a pipe.
            /// </summary>
            public MLAppConnect.PipeType PipeType;

            /// <summary>
            /// Direction of a pipe.
            /// </summary>
            public MLAppConnect.PipeDirection PipeDirection;

            /// <summary>
            /// Status of the pipe.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Status;
        }

        /// <summary>
        /// Information about a user from the connection event callback (#MLAppConnectEventCallback()).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct UserEventInfo
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Name of the user for which the changes have occurred.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string UserName;

            /// <summary>
            /// Status of the user.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Status;
        }

        /// <summary>
        /// Information about a user from the connection event callback (#MLAppConnectEventCallback()).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MicrophoneEventInfo
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Name of the user for which the changes have occurred.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string UserName;
        }

        /// <summary>
        /// This can be used to create key-value maps
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyValue
        {
            /// <summary>
            /// Key of the pair.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Key;

            /// <summary>
            /// Value of the pair.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Value;
        }

        /// <summary>
        /// Event info.
        /// Top level structure with information from the connection event callback (#MLAppConnectEventCallback()).
        /// It contains child structures with specific information for each type of event.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct EventInfo
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Event type.
            /// </summary>
            public EventType EventName;

            /// <summary>
            /// Connection name.
            /// </summary>
            public string ConnectionName;

            /// <summary>
            /// Connection Information.
            /// </summary>
            public ConnectionEventInfo ConnectionInfo;

            /// <summary>
            /// Pipe Information.
            /// </summary>
            public PipeEventInfo PipeInfo;

            /// <summary>
            /// User Information.
            /// </summary>
            public UserEventInfo UserInfo;

            /// <summary>
            /// Microphone Information.
            /// </summary>
            public MicrophoneEventInfo MicrophoneInfo;
        }
#endif
    }
}
