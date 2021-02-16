// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCDataChannelNativeBindings.cs" company="Magic Leap, Inc">
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
            /// Native bindings for the MLWebRTC.DataChannel class. 
            /// </summary>
            internal class NativeBindings
            {
                /// <summary>
                /// A delegate that describes the requirements of the OnOpened callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnOpenedDelegate(IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnClosed callback.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnClosedDelegate(IntPtr context);

                /// <summary>
                /// A delegate that describes the requirements of the OnMessage callback.
                /// </summary>
                /// <param name="message">The native message object received.</param>
                /// <param name="context">Pointer to a context object.</param>
                public delegate void OnMessageDelegate(in MLWebRTCDataChannelMessage message, IntPtr context);

                /// <summary>
                /// Creates a data channel.
                /// </summary>
                /// <param name="connectionHandle">The connection to associate the data channel with.</param>
                /// <param name="label">The label to give the data channel.</param>
                /// <param name="dataChannelHandle">The handle of the data channel after it is created.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelCreate(ulong connectionHandle, [MarshalAs(UnmanagedType.LPStr)] string label, out ulong dataChannelHandle);

                /// <summary>
                /// Gets the label of a data channel, call MLWebRTCDataChannelReleaseLabelMemory after.
                /// </summary>
                /// <param name="dataChannelHandle">The handle of the data channel.</param>
                /// <param name="label">Pointer to the unmanaged label string.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelGetLabel(ulong dataChannelHandle, out IntPtr label);

                /// <summary>
                /// Releases the memory created when calling MLWebRTCDataChannelGetLabel.
                /// </summary>
                /// <param name="dataChannelHandle">The handle of the data channel.</param>
                /// <param name="label">Pointer to the unmanaged label string.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelReleaseLabelMemory(ulong dataChannelHandle, IntPtr label);

                /// <summary>
                /// Gets if a data channel is open.
                /// </summary>
                /// <param name="dataChannelHandle">The handle of the data channel.</param>
                /// <param name="isOpen">True if the data channel is open.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelIsOpen(ulong dataChannelHandle, [MarshalAs(UnmanagedType.I1)] out bool isOpen);

                /// <summary>
                /// Sets the callbacks for a data channel.
                /// </summary>
                /// <param name="dataChannelHandle">The handle of the data channel.</param>
                /// <param name="callbacks">Native callbacks object to set the data channel with.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelSetEventCallbacks(ulong dataChannelHandle, in MLWebRTCDataChannelEventCallbacks callbacks);

                /// <summary>
                /// Sends a message to a data channel.
                /// </summary>
                /// <param name="dataChannelHandle">The handle of the data channel.</param>
                /// <param name="message">Native message object to send to the data channel.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelSendMessage(ulong dataChannelHandle, in MLWebRTCDataChannelMessage message);

                /// <summary>
                /// Destroys a data channel.
                /// </summary>
                /// <param name="connectionHandle">The handle of the associated connection.</param>
                /// <param name="dataChannelHandle">The handle of the data channel.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                [DllImport(WebRTCDLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLWebRTCDataChannelDestroy(ulong connectionHandle, ulong dataChannelHandle);

                /// <summary>
                /// Sets the callbacks of a data channel.
                /// </summary>
                /// <param name="dataChannel">The data channel to reference for the callbacks.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code SetCallbacks(MLWebRTC.DataChannel dataChannel)
                {
#if PLATFORM_LUMIN
                    dataChannel.gcHandle = GCHandle.Alloc(dataChannel);
                    IntPtr gcHandlePtr = GCHandle.ToIntPtr(dataChannel.gcHandle);

                    NativeBindings.MLWebRTCDataChannelEventCallbacks callbacks = NativeBindings.MLWebRTCDataChannelEventCallbacks.Create(gcHandlePtr);
                    MLResult.Code resultCode = NativeBindings.MLWebRTCDataChannelSetEventCallbacks(dataChannel.Handle, in callbacks);
                    if (!MLResult.IsOK(resultCode))
                    {
                        dataChannel.gcHandle.Free();
                    }

                    return resultCode;
#else
                    return MLResult.Code.Ok;
#endif
                }

                /// <summary>
                /// Sends a string message to a data channel.
                /// </summary>
                /// <param name="dataChannel">Data channel to send the message to.</param>
                /// <param name="message">The string to send.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code SendMessageToDataChannel(MLWebRTC.DataChannel dataChannel, string message)
                {
#if PLATFORM_LUMIN
                    NativeBindings.MLWebRTCDataChannelMessage messageNative = NativeBindings.MLWebRTCDataChannelMessage.Create(message);

                    MLResult.Code resultCode = NativeBindings.MLWebRTCDataChannelSendMessage(dataChannel.Handle, in messageNative);
                    Marshal.FreeHGlobal(messageNative.Data);
                    return resultCode;
#else
                    return MLResult.Code.Ok;
#endif
                }

                /// <summary>
                /// Sends a string message to a data channel.
                /// </summary>
                /// <param name="dataChannel">Data channel to send the message to.</param>
                /// <param name="message">The byte array to send.</param>
                /// <returns>
                /// MLResult.Result will be <c>MLResult.Code.Ok</c> if destroying all handles was successful.
                /// MLResult.Result will be <c>MLResult.Code.MismatchingHandle</c> if an incorrect handle was sent.
                /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
                /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
                /// </returns>
                public static MLResult.Code SendMessageToDataChannel<T>(MLWebRTC.DataChannel dataChannel, T[] message)
                {
#if PLATFORM_LUMIN
                    NativeBindings.MLWebRTCDataChannelMessage messageNative = NativeBindings.MLWebRTCDataChannelMessage.Create(message);

                    MLResult.Code resultCode = NativeBindings.MLWebRTCDataChannelSendMessage(dataChannel.Handle, in messageNative);
                    Marshal.FreeHGlobal(messageNative.Data);

                    return resultCode;
#else
                    return MLResult.Code.Ok;
#endif
                }

#if PLATFORM_LUMIN
                /// <summary>
                /// Native callback that is invoked when a data channel opens.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(OnOpenedDelegate))]
                private static void OnOpened(IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        DataChannel dataChannel = gcHandle.Target as DataChannel;
                        dataChannel?.OnOpened?.Invoke(dataChannel);
                    });
                }

                /// <summary>
                /// Native callback that is invoked when a data channel closes.
                /// </summary>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(OnClosedDelegate))]
                private static void OnClosed(IntPtr context)
                {
                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        DataChannel dataChannel = gcHandle.Target as DataChannel;
                        dataChannel?.OnClosed?.Invoke(dataChannel);
                    });
                }

                /// <summary>
                /// Native callback that is invoked when a data channel receives a message.
                /// </summary>
                /// <param name="message">Message object received.</param>
                /// <param name="context">Pointer to a context object.</param>
                [AOT.MonoPInvokeCallback(typeof(OnMessageDelegate))]
                private static void OnMessage(in NativeBindings.MLWebRTCDataChannelMessage message, IntPtr context)
                {
                    object messageObject = null;
                    bool isBinary = message.IsBinary;
                    if (isBinary)
                    {
                        byte[] bytes = new byte[message.DataSize];
                        Marshal.Copy(message.Data, bytes, 0, (int)message.DataSize);
                        messageObject = bytes;
                    }
                    else
                    {
                        string chars = Marshal.PtrToStringAnsi(message.Data, (int)message.DataSize);
                        messageObject = chars;
                    }

                    MLThreadDispatch.ScheduleMain(() =>
                    {
                        GCHandle gcHandle = GCHandle.FromIntPtr(context);
                        DataChannel dataChannel = gcHandle.Target as DataChannel;
                        if(isBinary)
                        {
                            dataChannel?.OnMessageBinary?.Invoke(dataChannel, messageObject as byte[]);
                        }
                        else
                        {
                            dataChannel?.OnMessageText?.Invoke(dataChannel, messageObject as string);
                        }
                    });
                }
#endif

                /// <summary>
                /// The native representation of the MLWebRTC data channel message.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCDataChannelMessage
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Pointer to the message in unmanaged memory.
                    /// </summary>
                    public IntPtr Data;

                    /// <summary>
                    /// Describes how large the unmanaged data is.
                    /// </summary>
                    public ulong DataSize;

                    /// <summary>
                    /// Determines if the message is a string or byte array.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool IsBinary;

                    /// <summary>
                    /// Creates an initialized MLWebRTCDataChannelMessage object.
                    /// </summary>
                    /// <param name="message">The string message to send.</param>
                    /// <returns>An initialized MLWebRTCDataChannelMessage object.</returns>
                    public static MLWebRTCDataChannelMessage Create(string message)
                    {
                        MLWebRTCDataChannelMessage channelMessage = new MLWebRTCDataChannelMessage();
                        channelMessage.Version = 1;
                        channelMessage.Data = Marshal.StringToHGlobalAnsi(message);
                        channelMessage.DataSize = (ulong)message.Length;
                        channelMessage.IsBinary = false;
                        return channelMessage;
                    }

                    /// <summary>
                    /// Creates an initialized MLWebRTCDataChannelMessage object.
                    /// </summary>
                    /// <param name="message">The byte array message to send.</param>
                    /// <returns>An initialized MLWebRTCDataChannelMessage object.</returns>
                    public static MLWebRTCDataChannelMessage Create<T>(T[] message)
                    {
                        MLWebRTCDataChannelMessage channelMessage = new MLWebRTCDataChannelMessage();
                        channelMessage.Version = 1;
                        int typeSize = Marshal.SizeOf(typeof(T));
                        int unmanagedByteArrayLength = (typeSize * message.Length);

                        channelMessage.Data = Marshal.AllocHGlobal(unmanagedByteArrayLength);
                        IntPtr next = channelMessage.Data;
                        for (int i = 0; i < message.Length; ++i)
                        {
                            Marshal.StructureToPtr(message[i], next, false);
                            next = IntPtr.Add(next, typeSize);
                        }

                        channelMessage.DataSize = (ulong)unmanagedByteArrayLength;
                        channelMessage.IsBinary = true;
                        return channelMessage;
                    }
                }

                /// <summary>
                /// The native representation of the MLWebRTC data channel callback events.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLWebRTCDataChannelEventCallbacks
                {
                    /// <summary>
                    /// Version of the struct.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// Context pointer used with the dataChannelCounter field to determine which data channel that callbacks are associated with.
                    /// </summary>
                    public IntPtr Context;

                    /// <summary>
                    /// Native OnOpen event.
                    /// </summary>
                    public OnOpenedDelegate OnOpen;

                    /// <summary>
                    /// Native OnClosed event.
                    /// </summary>
                    public OnClosedDelegate OnClosed;

                    /// <summary>
                    /// Native OnMessage event.
                    /// </summary>
                    public OnMessageDelegate OnMessage;

                    /// <summary>
                    /// Creates an initialized MLWebRTCDataChannelEventCallbacks object.
                    /// </summary>
                    /// <param name="context">The context object to use.</param>
                    /// <returns>An initialized MLWebRTCDataChannelEventCallbacks object.</returns>
                    public static MLWebRTCDataChannelEventCallbacks Create(IntPtr context)
                    {
                        MLWebRTCDataChannelEventCallbacks callbacks = new MLWebRTCDataChannelEventCallbacks();
                        callbacks.Version = 1;
#if PLATFORM_LUMIN
                        callbacks.OnOpen = DataChannel.NativeBindings.OnOpened;
                        callbacks.OnClosed = DataChannel.NativeBindings.OnClosed;
                        callbacks.OnMessage = DataChannel.NativeBindings.OnMessage;
#endif
                        
                        callbacks.Context = context;
                        return callbacks;
                    }
            }
            }
        }
    }
}
