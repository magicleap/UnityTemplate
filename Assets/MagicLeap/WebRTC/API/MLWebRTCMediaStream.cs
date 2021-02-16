// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCMediaStream.cs" company="Magic Leap, Inc">
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

    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Class that represents a media stream object.
        /// </summary>
        public partial class MediaStream
        {
            /// <summary>
            /// The active tracks mapped by MLWebRTC.MediaStream.Track.Type.
            /// </summary>
            private Dictionary<Track.Type, MediaStream.Track> activeTracks = new Dictionary<Track.Type, Track>
            {
                { Track.Type.Audio, null},
                { Track.Type.Video, null}
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="MediaStream" /> class.
            /// </summary>
            private MediaStream()
            {
            }

            /// <summary>
            /// Gets the list of tracks associated with this media stream.
            /// </summary>
            public List<MediaStream.Track> Tracks { get; private set; }

            /// <summary>
            /// Gets all video tracks.
            /// </summary>
            public List<MediaStream.Track> VideoTracks { get => Tracks.FindAll(track => track.TrackType == Track.Type.Video); }

            /// <summary>
            /// Gets all audio tracks.
            /// </summary>
            public List<MediaStream.Track> AudioTracks { get => Tracks.FindAll(track => track.TrackType == Track.Type.Audio); }

            /// <summary>
            /// Gets the active video track.
            /// </summary>
            public MediaStream.Track ActiveVideoTrack { get => activeTracks[Track.Type.Video]; }

            /// <summary>
            /// Gets the active audio track.
            /// </summary>
            public MediaStream.Track ActiveAudioTrack { get => activeTracks[Track.Type.Audio]; }

            /// <summary>
            /// Gets the connections associated with this media stream.
            /// Remote media streams will have just 1 connection, while local streams can have more than one,
            /// depending on the app setup.
            /// </summary>
            public HashSet<MLWebRTC.PeerConnection> ParentConnections { get; internal set; } = new HashSet<PeerConnection>();

            /// <summary>
            /// Gets the id of this media stream.
            /// </summary>
            public string Id { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the media stream is local or not.
            /// </summary>
            public bool IsLocal { get; private set; }

            /// <summary>
            /// Creates an initialized local MediaStream object and it's tracks with the given video type.
            /// Recommended to use CreateWithAppDefinedVideoTrack() in production, with sample video sources
            /// provided as MLCameraVideoSource and MLMRCameraVideoSource in the UnityEngine.XR.MagicLeap namespace
            /// since those sources provide more information about and control over various error cases and
            /// handle special cases like app pause/resume and device standby/reality/active.
            /// </summary>
            /// <param name="connection">Connection to use.</param>
            /// <param name="id">The id to give the media stream.</param>
            /// <param name="videoType">The type of video to use.</param>
            /// <returns> An initialized MediaStream object.</returns>
            public static MediaStream CreateWithBuiltInTracks(string id, Track.VideoType videoType, Track.AudioType audioType)
            {
#if PLATFORM_LUMIN
                MediaStream mediaStream = Create(id);
                if (mediaStream == null)
                {
                    return null;
                }

                MediaStream.Track videoTrack = (videoType != Track.VideoType.None) ? MediaStream.Track.CreateVideoTrack(videoType, out MLResult result) : null;
                MediaStream.Track audioTrack = (audioType != Track.AudioType.None) ? MediaStream.Track.CreateAudioTrackFromMicrophone(out result) : null;

                if (videoTrack != null)
                {
                    mediaStream.AddLocalTrack(videoTrack);
                    mediaStream.SelectTrack(videoTrack);
                }
                if (audioTrack != null)
                {
                    mediaStream.AddLocalTrack(audioTrack);
                    mediaStream.SelectTrack(audioTrack);
                }

                return mediaStream;
#else
                return null;
#endif
            }

            /// <summary>
            /// Creates an initialized local MediaStream object and it's tracks with the given video source.
            /// </summary>
            /// <param name="connection">Connection to use.</param>
            /// <param name="id">The id to give the media stream.</param>
            /// <param name="appDefinedVideoSource">The defined video source to use.</param>
            /// <returns> An initialized MediaStream object.</returns>
            public static MediaStream CreateWithAppDefinedVideoTrack(string id, MLWebRTC.AppDefinedVideoSource appDefinedVideoSource, Track.AudioType audioType)
            {
#if PLATFORM_LUMIN
                MediaStream mediaStream = Create(id);
                if (mediaStream == null)
                {
                    return null;
                }

                mediaStream.AddLocalTrack(appDefinedVideoSource);
                mediaStream.SelectTrack(appDefinedVideoSource);

                if (audioType != Track.AudioType.None)
                {
                    MediaStream.Track audioTrack = MediaStream.Track.CreateAudioTrackFromMicrophone(out MLResult result);
                    mediaStream.AddLocalTrack(audioTrack);
                    mediaStream.SelectTrack(audioTrack);
                }

                return mediaStream;
#else
                return null;
#endif
            }

            /// <summary>
            /// Creates an initialized local MediaStream object.
            /// </summary>
            /// <param name="id">The id to give the media stream.</param>
            /// <returns> An initialized MediaStream object.</returns>
            public static MediaStream Create(string id)
            {
#if PLATFORM_LUMIN
                if (MLWebRTC.Instance.uniqueMediaStreamIds.Contains(id))
                {
                    MLPluginLog.ErrorFormat("Media stream id '{0}' already exists.", id);
                    return null;
                }

                MediaStream mediaStream = Create(null, id);
                mediaStream.IsLocal = true;
                MLWebRTC.Instance.uniqueMediaStreamIds.Add(id);
                return mediaStream;
#else
                return null;
#endif
            }

            /// <summary>
            /// Creates an initialized MediaStream object.
            /// </summary>
            /// <param name="connection">The connection to associate the media stream with.</param>
            /// <param name="id">The id to give the media stream.</param>
            /// <returns> An initialized MediaStream object.</returns>
            internal static MediaStream Create(MLWebRTC.PeerConnection connection, string id)
            {
                MediaStream mediaStream = new MediaStream();
                mediaStream.Tracks = new List<Track>();
                mediaStream.Id = id;
                mediaStream.ParentConnections.Add(connection);
                return mediaStream;
            }

            /// <summary>
            /// Adds a local track to the media stream.
            /// </summary>
            /// <param name="connection">The connection to use.</param>
            /// <param name="track">The local track to add.</param>
            /// <returns>
            public MLResult AddLocalTrack(MLWebRTC.MediaStream.Track track)
            {
#if PLATFORM_LUMIN
                MLResult result = MLResult.Create(MLResult.Code.Ok);
                if (this.Tracks.Contains(track))
                {
                    return result;
                }

                if (track.IsLocal)
                {
                    this.Tracks.Add(track);
                    track.Streams.Add(this);
                }

                return result;
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Removes a local track from the media stream.
            /// </summary>
            /// <param name="connection">The connection to use.</param>
            /// <param name="track">The local track to add.</param>
            /// <returns>
            public MLResult RemoveLocalTrack(MLWebRTC.MediaStream.Track track)
            {
#if PLATFORM_LUMIN
                MLResult result = MLResult.Create(MLResult.Code.Ok);
                if (!this.Tracks.Contains(track))
                {
                    return result;
                }

                if (track.IsLocal)
                {
                    this.Tracks.Remove(track);
                    track.Streams.Remove(this);
                }

                return result;
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Sets the given track as the active track of it's kind and enables it.
            /// </summary>
            /// <param name="track">The track to make active.</param>
            public MLResult SelectTrack(MediaStream.Track track)
            {
#if PLATFORM_LUMIN
                if (track == null)
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Track is null.");
                }

                MediaStream.Track currentActiveTrack = activeTracks[track.TrackType];
                
                if(currentActiveTrack != null)
                {
                    UnSelectTrack(currentActiveTrack);
                }
                
                activeTracks[track.TrackType] = track;
                MLResult result = track.SetEnabled(true);

                return result;
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Sets the given track to longer be the active track of it's kind and disables.
            /// </summary>
            /// <param name="track">The track to make inactive.</param>
            public MLResult UnSelectTrack(MediaStream.Track track)
            {
#if PLATFORM_LUMIN
                if (track == null)
                {
                    return MLResult.Create(MLResult.Code.InvalidParam, "Track is null.");
                }

                MLResult result = MLResult.Create(MLResult.Code.Ok);

                MediaStream.Track currentActiveTrack = activeTracks[track.TrackType];

                if (currentActiveTrack == track)
                {
                    result = currentActiveTrack.SetEnabled(false);
                    activeTracks[track.TrackType] = null;
                }

                return result;
#else
                return new MLResult();
#endif
            }

            /// <summary>
            /// Destroys this stream and it's associated tracks.
            /// </summary>
            public void DestroyLocal()
            {
                if (!IsLocal)
                {
                    return;
                }

                foreach(Track track in this.Tracks)
                {
                    track.DestroyLocal();
                }
#if PLATFORM_LUMIN
                MLWebRTC.Instance.uniqueMediaStreamIds.Remove(this.Id);
#endif
            }
        }
    }
}
