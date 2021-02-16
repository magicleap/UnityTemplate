// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTC.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
#if PLATFORM_LUMIN
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC : MLAutoAPISingleton<MLWebRTC>
    {
        /// <summary>
        /// The name of the WebRTC DLL.
        /// </summary>
        private const string WebRTCDLL = "ml_webrtc";

#if PLATFORM_LUMIN

        /// <summary>
        /// Contains all the unique Id's of the media streams for this connection.
        /// </summary>
        private HashSet<string> uniqueMediaStreamIds = new HashSet<string>();

        /// <summary>
        /// List of all local tracks created in one session.
        /// </summary>
        private List<MLWebRTC.MediaStream.Track> localTracks = new List<MLWebRTC.MediaStream.Track>();

        /// <summary>
        /// List of  all sinks created in one session.
        /// </summary>
        private List<MLWebRTC.Sink> sinks = new List<MLWebRTC.Sink>();

        /// <summary>
        /// List of  all connections created in one session.
        /// </summary>
        private List<MLWebRTC.PeerConnection> connections = new List<MLWebRTC.PeerConnection>();

        /// <summary>
        /// Used to determine when video sinks are currently being processed.
        /// </summary>
        internal bool IsProcessingSinks { get; private set; }

        /// <summary>
        /// Calls Disconnect(), destroys the WebRTC sinks and WebRTC instance.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
        /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
        /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
        /// </returns>
        protected override MLResult.Code StopAPI()
        {
            // Destroy connections.
            PeerConnection[] remainingConnections = new PeerConnection[connections.Count];
            // PeerConnection.Destroy() removes the connection from Instance.connections list.
            connections.CopyTo(remainingConnections);
            foreach (PeerConnection connection in remainingConnections)
            {
                connection.Destroy();
            }

            // Destroy sinks.
            Sink[] remainingSinks = new Sink[sinks.Count];
            // Sink.Destroy() removes the sink from Instance.sinks list.
            sinks.CopyTo(remainingSinks);
            foreach (Sink sink in remainingSinks)
            {
                sink.Destroy();
            }

            // Destroy local tracks.
            MediaStream.Track[] remainingTracks = new MediaStream.Track[localTracks.Count];
            // MediaStream.Track.Destroy() removes the track from Instance.localTracks list.
            localTracks.CopyTo(remainingTracks);
            foreach (MediaStream.Track localTracks in remainingTracks)
            {
                localTracks.DestroyLocal();
            }

            Instance.connections.Clear();
            Instance.sinks.Clear();
            Instance.localTracks.Clear();

            MLResult.Code resultCode = NativeBindings.MLWebRTCInstanceDestroy();
            return resultCode; 
        }

        /// <summary>
        /// Creates a WebRTC instance.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the MLWebRTC instance was created successfully.
        /// </returns>
        protected override MLResult.Code StartAPI()
        {
            NativeBindings.MLWebRTCDebugUtils debugUtils = NativeBindings.MLWebRTCDebugUtils.Create();
            MLResult.Code resultCode = NativeBindings.MLWebRTCInstanceCreateWithDebugUtils(in debugUtils);
            return resultCode;
        }

        /// <summary>
        /// Update loop used to process any connection events.
        /// </summary>
        protected override void Update()
        {
            for (int i = 0; i < Instance.connections.Count; ++i)
            {
                MLWebRTC.PeerConnection connection = Instance.connections[i];
                if (MagicLeapNativeBindings.MLHandleIsValid(connection.Handle))
                {
                    // Polls for connection events.
                    DidNativeCallSucceed(MLWebRTC.PeerConnection.NativeBindings.MLWebRTCConnectionProcessEvents(connection.Handle), "MLWebRTCConnectionProcessEvents()");
                }
            }

            if(!IsProcessingSinks)
            {
                _ = ProcessVideoSinksAsync();
            }
        }

        /// <summary>
        /// Updates each active video sink.
        /// </summary>
        /// <returns>A completed task indicating all video sinks have finished updating.</returns>
        private Task UpdateVideoSinks()
        {
            for (int i = 0; i < Instance.sinks.Count; ++i)
            {
                MLWebRTC.Sink sink = Instance.sinks[i];
                if (sink.Type != MediaStream.Track.Type.Video)
                {
                    continue;
                }
                ((MLWebRTC.VideoSink)sink).UpdateVideoSink();
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Updates each video sink on a seperate thread and then calls InvokeOnNewFrame() to propagate any new frames to the main thread.
        /// </summary>
        private async Task ProcessVideoSinksAsync()
        {
            IsProcessingSinks = true;

            // Runs the UpdateVideoSinks on another thread and awaits the result.
            await Task.Run(UpdateVideoSinks);
            for (int i = 0; i < Instance.sinks.Count; ++i)
            {
                MLWebRTC.Sink sink = Instance.sinks[i];
                if (sink.Type != MediaStream.Track.Type.Video)
                {
                    continue;
                }
                ((MLWebRTC.VideoSink)sink).InvokeOnNewFrame();
            }
            IsProcessingSinks = false;
        }
#endif
    }
}
