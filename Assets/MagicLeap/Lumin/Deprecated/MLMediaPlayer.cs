// %BANNERBEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHTBEGIN%
// <copyright file="MLMediaPlayer.cs" company="Magic Leap">
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
// </copyright>
//
// %COPYRIGHTEND%
// ---------------------------------------------------------------------
// %BANNEREND%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Media player script that allows playback of a streaming video (either from file or web URL)
    /// This script will update the main texture parameter of the Renderer attached as a sibling
    /// with the video frame from playback. Audio is also handled through this class and will
    /// playback audio from the file.
    /// </summary>
    public partial class MLMediaPlayer : MonoBehaviour
    {
        #if PLATFORM_LUMIN
        /// <summary>
        /// Invoked when subtitle data is received from the media library.
        /// First parameter is a <c>MLCea608CaptionSegment</c> object that contains all the data given.
        /// </summary>
        [Obsolete("This event has been marked for deprecation, please use OnSubtitle608DataFound instead")]
        public event Subtitle608Delegate OnSubtitleDataFound
        {
            add
            {
                this.OnSubtitle608DataFound += value;
            }

            remove
            {
                this.OnSubtitle608DataFound -= value;
            }
        }
        #endif
    }
}
