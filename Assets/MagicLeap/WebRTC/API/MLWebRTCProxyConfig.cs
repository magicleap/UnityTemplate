// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebRTCIceServer.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MLWebRTC class contains the API to interface with the
    /// WebRTC C API.
    /// </summary>
    public partial class MLWebRTC
    {
        /// <summary>
        /// Protocol for the forward proxy.
        /// </summary>
        public enum ProxyType
        {
            Https,
            Socks5
        }

        /// <summary>
        /// Class that represents an ice server used by the MLWebRTC API.
        /// </summary>
        public partial struct ProxyConfig
        {
            /// <summary>
            /// Gets the type of this proxy config.
            /// </summary>
            public ProxyType Type { get; private set; }

            /// <summary>
            /// Gets the proxy server address.
            /// </summary>
            public string HostAddress { get; private set; }

            /// <summary>
            /// Gets the proxy server port.
            /// </summary>
            public int HostPort { get; private set; }

            /// <summary>
            /// Gets the username to authenticate the proxy server.
            /// </summary>
            public string Username { get; private set; }

            /// <summary>
            /// Gets the password to authenticate the proxy server.
            /// </summary>
            public string Password { get; private set; }

            /// <summary>
            /// Whether the proxy will be auto-detected.
            /// </summary>
            public bool AutoDetect { get; private set; }

            /// <summary>
            /// Gets url to use for downloading proxy config.
            /// </summary>
            public string AutoConfigUrl { get; private set; }

            /// <summary>
            /// Gets urls which should bypass the proxy.
            /// </summary>
            public string BypassList { get; private set; }

            /// <summary>
            /// Factory method used to create a new IceServer object.
            /// </summary>
            /// <param name="uri">The uri of the ice server.</param>
            /// <param name="userName">The username to log into the ice server.</param>
            /// <param name="password">The password to log into the ice server.</param>
            /// <returns>An ice candidate object with the given handle.</returns>
            public static ProxyConfig Create(ProxyType type, string hostAddress, int hostPort, string userName = null, string password = null, bool autoDetect = false, string autoConfigUrl = null, string bypassList = null)
            {
                ProxyConfig proxyConfig = new ProxyConfig()
                {
                    Type = type,
                    HostAddress = hostAddress,
                    HostPort = hostPort,
                    Username = userName,
                    Password = password,
                    AutoDetect = autoDetect,
                    AutoConfigUrl = autoConfigUrl,
                    BypassList = bypassList
                };

                return proxyConfig;
            }

            public override string ToString()
            {
                return $"Type: {Type}, Host: {HostAddress}:{HostPort}, Auth: {Username}:{Password},\nAutoDetect: {AutoDetect}, AutoConfigUrl: {AutoConfigUrl}, BypassList: {BypassList}";
            }
        }
    }
}
