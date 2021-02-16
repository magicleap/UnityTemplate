// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCSink.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
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
        /// Class that represents a audio sink used by the MLWebRTC API.
        /// </summary>
        public abstract class Sink
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Sink" /> class.
            /// </summary>
            protected Sink()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Sink" /> class.
            /// </summary>
            /// <param name="handle">The handle of the sink.</param>
            protected Sink(ulong handle)
            {
                this.Handle = handle;
            }

            /// <summary>
            /// Gets or sets the reference to the stream.
            /// </summary>
            public MLWebRTC.MediaStream Stream { get; protected set; }

            /// <summary>
            /// Gets the type of the sink.
            /// </summary>
            public MediaStream.Track.Type Type { get; internal set; }

            /// <summary>
            /// Gets the handle of the sink.
            /// </summary>
            internal ulong Handle { get; private set; }

            /// <summary>
            /// Abstract method for destroying the sink.
            /// </summary>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            public abstract MLResult Destroy();

            /// <summary>
            /// Abstract method for setting the sink's track.
            /// </summary>
            /// <param name="track">The track to set the sink to.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
            /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
            /// </returns>
            protected abstract MLResult SetTrack(MLWebRTC.MediaStream.Track track);

            /// <summary>
            /// Used to invalidate handle by inherited classes.
            /// </summary>
            protected void InvalidateHandle()
            {
#if PLATFORM_LUMIN
                this.Handle = MagicLeapNativeBindings.InvalidHandle;
#endif
            }
        }
    }
}
