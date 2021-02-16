// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCMediaStreamTrack.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Collections.Generic;
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
        /// Class that represents a media stream object.
        /// </summary>
        public partial class MediaStream
        {
            /// <summary>
            /// Struct that represents a media stream track object.
            /// </summary>
            public partial class Track
            {
                /// <summary>
                /// Caches if the track was enabled before a lifestyle event change.
                /// </summary>
                protected bool wasEnabledBeforeLifecycleStateChange = false;

                /// <summary>
                /// List of streams associated with this track.
                /// </summary>
                private List<MediaStream> streams = new List<MediaStream>();

                /// <summary>
                /// Initializes a new instance of the <see cref="Track" /> class.
                /// </summary>
                internal Track()
                {
#if PLATFORM_LUMIN
                    UnityEngine.Lumin.Lifecycle.deviceStandbyEvent += this.HandleDeviceStandby;
                    UnityEngine.Lumin.Lifecycle.deviceRealityEvent += this.HandleDeviceReality;
                    UnityEngine.Lumin.Lifecycle.deviceActiveEvent += this.HandleDeviceActive;
                    MLDevice.RegisterApplicationPause(this.OnApplicationPause);
#endif
                }

                /// <summary>
                /// Defines the types of tracks that can exist.
                /// </summary>
                public enum Type
                {
                    /// <summary>
                    /// Audio track type.
                    /// </summary>
                    Audio,

                    /// <summary>
                    /// Video track type.
                    /// </summary>
                    Video
                }

                /// <summary>
                /// Defines the natively supported video types.
                /// </summary>
                public enum AudioType
                {
                    /// <summary>
                    /// No audio source.
                    /// </summary>
                    None,

                    /// <summary>
                    /// Natively uses Microphone as audio source.
                    /// </summary>
                    Microphone
                }

                /// <summary>
                /// Defines the natively supported video types.
                /// Recommended to use app defined video sources in production, with sample sources provided
                /// as MLCameraVideoSource and MLMRCameraVideoSource in the UnityEngine.XR.MagicLeap namespace
                /// since those sources provide more information about and control over various error cases and
                /// handle special cases like app pause/resume and device standby/reality/active.
                /// </summary>
                public enum VideoType
                {
                    /// <summary>
                    /// No video source.
                    /// </summary>
                    None,

                    /// <summary>
                    /// Natively uses MLCamera as video source.
                    /// </summary>
                    MLCamera,

                    /// <summary>
                    /// Natively uses MLMRCamera as video source.
                    /// </summary>
                    MLMRCamera
                }

                /// <summary>
                /// Gets the list of streams associated with this track.
                /// </summary>
                public List<MediaStream> Streams { get => this.streams; }

                /// <summary>
                /// Gets or sets a value indicating whether the track is local or not.
                /// </summary>
                public bool IsLocal { get; protected set; }

                /// <summary>
                /// Gets the string that determines the type of track this is.
                /// </summary>
                public Type TrackType { get; internal set; }

                /// <summary>
                /// Gets a reference to the associated connection of the track.
                /// </summary>
                public MLWebRTC.PeerConnection ParentConnection { get; internal set; }

                /// <summary>
                /// Gets or sets the native handle of the track (source).
                /// </summary>
                internal ulong Handle { get; set; }

#if PLATFORM_LUMIN
                /// <summary>
                /// Creates an initialized Track object.
                /// </summary>
                /// <param name="result">The MLResult object of the inner platform call(s).</param>
                /// <returns> An initialized Track object.</returns>
                public static Track CreateAudioTrackFromMicrophone(out MLResult result)
                {
                    Track track = null;
                    ulong handle = MagicLeapNativeBindings.InvalidHandle;
                    MLResult.Code resultCode = MLResult.Code.Ok;

                    resultCode = Source.NativeBindings.MLWebRTCSourceCreateLocalSourceForMicrophone(out handle);

                    if (!DidNativeCallSucceed(resultCode, "MLWebRTCSourceCreateLocalSourceForMicrophone()"))
                    {
                        result = MLResult.Create(resultCode);
                        return track;
                    }

                    track = new Track()
                    {
                        Handle = handle,
                        TrackType = Type.Audio,
                        IsLocal = true,
                    };

                    MLWebRTC.Instance.localTracks.Add(track);
                    result = MLResult.Create(resultCode);
                    return track;
                }

                /// <summary>
                /// Creates an initialized Track object.
                /// Recommended to use app defined video sources in production, with sample sources provided
                /// as MLCameraVideoSource and MLMRCameraVideoSource in the UnityEngine.XR.MagicLeap namespace
                /// since those sources provide more information about and control over various error cases and
                /// handle special cases like app pause/resume and device standby/reality/active.
                /// </summary>
                /// <param name="videoType">The type of video source to use.</param>
                /// <param name="result">The MLResult object of the inner platform call(s).</param>
                /// <param name="inputContext">The InputContext object to start the MLMRCamera API with.</param>
                /// <returns> An initialized Track object.</returns>
                public static Track CreateVideoTrack(VideoType videoType, out MLResult result, MLMRCamera.InputContext? inputContext = null)
                {
                    Track track = null;
                    ulong handle = MagicLeapNativeBindings.InvalidHandle;
                    MLResult.Code resultCode = MLResult.Code.Ok;

                    switch (videoType)
                    {
                        case VideoType.MLCamera:
                            {
                                resultCode = Source.NativeBindings.MLWebRTCSourceCreateLocalSourceForCamera(out handle);
                                DidNativeCallSucceed(resultCode, "MLWebRTCSourceCreateLocalSourceForCamera");
                                break;
                            }

                        case VideoType.MLMRCamera:
                            {
                                MLMRCamera.NativeBindings.InputContextNative inputContextNative = new MLMRCamera.NativeBindings.InputContextNative
                                {
                                    Data = inputContext ?? MLMRCamera.InputContext.Create()
                                };
                                resultCode = Source.NativeBindings.MLWebRTCSourceCreateLocalSourceForMRCamera(in inputContextNative, out handle);
                                DidNativeCallSucceed(resultCode, "MLWebRTCSourceCreateLocalSourceForMRCamera");
                                break;
                            }
                    }

                    if (!MLResult.IsOK(resultCode))
                    {
                        result = MLResult.Create(resultCode);
                        return track;
                    }

                    track = new Track()
                    {
                        Handle = handle,
                        TrackType = Type.Video,
                        IsLocal = true,
                    };

                    MLWebRTC.Instance.localTracks.Add(track);
                    result = MLResult.Create(resultCode);
                    return track;
                }
#endif
                /// <summary>
                /// Gets if a track is currently enabled or not.
                /// </summary>
                /// <param name="isEnabled">True if the track is enabled.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if an invalid parameter was passed.
                /// </returns>
                public MLResult GetEnabled(out bool isEnabled)
                {
#if PLATFORM_LUMIN
                    if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                    {
                        isEnabled = false;
                        return MLResult.Create(MLResult.Code.InvalidParam, "Source handle is invalid.");
                    }

                    MLResult.Code resultCode = Source.NativeBindings.MLWebRTCSourceIsEnabled(this.Handle, out isEnabled);
                    DidNativeCallSucceed(resultCode, "MLWebRTCSourceIsEnabled()");
                    return MLResult.Create(resultCode);
#else
                    isEnabled = false;
                    return new MLResult();
#endif
                }

                /// <summary>
                /// Sets a track to be enabled or disabled.
                /// </summary>
                /// <param name="isEnabled">True if the track should be enabled.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// </returns>
                public MLResult SetEnabled(bool isEnabled)
                {
#if PLATFORM_LUMIN
                    if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                    {
                        return MLResult.Create(MLResult.Code.InvalidParam, "Source handle is invalid.");
                    }

                    MLResult result = this.GetEnabled(out bool trackEnabled);
                    if (!result.IsOk)
                    {
                        return result;
                    }

                    if (trackEnabled != isEnabled)
                    {
                        MLResult.Code resultCode = Source.NativeBindings.MLWebRTCSourceSetEnabled(this.Handle, isEnabled);
                        DidNativeCallSucceed(resultCode, "MLWebRTCSourceSetEnabled");
                        result = MLResult.Create(resultCode);
                    }

                    return result;
#else
                    return new MLResult();
#endif
                }

                /// <summary>
                /// Destroys the Track and its associated media source.
                /// </summary>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultInstanceNotCreated</c> if MLWebRTC instance was not created.
                /// MLResult.Result will be <c>MLResult.Code.WebRTCResultMismatchingHandle</c> if an incorrect handle was sent.
                /// </returns>                
                public virtual MLResult DestroyLocal()
                {
#if PLATFORM_LUMIN
                    if (!this.IsLocal)
                    {
                        return MLResult.Create(MLResult.Code.InvalidParam, "Souce is not local.");
                    }

                    if (!MagicLeapNativeBindings.MLHandleIsValid(this.Handle))
                    {
                        return MLResult.Create(MLResult.Code.InvalidParam, "Source handle is invalid.");
                    }

                    this.Cleanup();

                    MLResult.Code resultCode = MLResult.Code.Ok;

                    // If this is a local source then we need to explicitly remove it from it's associated connections.
                    foreach (MLWebRTC.PeerConnection connection in Instance.connections)
                    {
                        if (connection == this.ParentConnection)
                        {
                            connection.RemoveLocalTrack(this);
                            break;
                        }
                    }

                    MLWebRTC.Instance.localTracks.Remove(this);

                    resultCode = Source.NativeBindings.MLWebRTCSourceDestroy(this.Handle);
                    DidNativeCallSucceed(resultCode, "MLWebRTCSourceDestroy()");
                    this.Handle = MagicLeapNativeBindings.InvalidHandle;
                    this.ParentConnection = null;

                    return MLResult.Create(resultCode);
#else
                    return new MLResult();
#endif
                }

                /// <summary>
                /// Unsubscribes from pause and lifecycle events.
                /// </summary>
                internal void Cleanup()
                {
                    // While local tracks can be unsubscribed in the DestroyLocal(), the only place to do that for remote tracks is in the finalizer.
#if PLATFORM_LUMIN
                    UnityEngine.Lumin.Lifecycle.deviceStandbyEvent -= this.HandleDeviceStandby;
                    UnityEngine.Lumin.Lifecycle.deviceRealityEvent -= this.HandleDeviceReality;
                    UnityEngine.Lumin.Lifecycle.deviceActiveEvent -= this.HandleDeviceActive;
                    MLDevice.UnregisterApplicationPause(this.OnApplicationPause);
#endif
                }

                /// <summary>
                /// Sets this track to enabled during the lifecycle active event.
                /// Override for custom behavior.
                /// </summary>
                protected virtual void HandleDeviceActive()
                {
                    if (this.wasEnabledBeforeLifecycleStateChange)
                    {
                        this.SetEnabled(true);
                    }
                }

                /// <summary>
                /// Disables this track during the lifecycle standby event.
                /// Override for custom behavior.
                /// </summary>
                protected virtual void HandleDeviceStandby()
                {
                    this.GetEnabled(out this.wasEnabledBeforeLifecycleStateChange);
                    if (this.wasEnabledBeforeLifecycleStateChange)
                    {
                        this.SetEnabled(false);
                    }
                }

                /// <summary>
                /// Disables this track during the lifecycle reality events.
                /// Override for custom behavior.
                /// </summary>
                protected virtual void HandleDeviceReality()
                {
                    HandleDeviceStandby();
                }

                /// <summary>
                /// Reacts to when the application is paused (after subscribing to MLDevice).
                /// Override for custom behavior.
                /// </summary>
                /// <param name="pause">True if paused.</param>
                protected virtual void OnApplicationPause(bool pause)
                {
                    if (pause)
                    {
                        this.HandleDeviceStandby();
                    }
                    else
                    {
                        this.HandleDeviceActive();
                    }
                }
            }
        }
    }
}
