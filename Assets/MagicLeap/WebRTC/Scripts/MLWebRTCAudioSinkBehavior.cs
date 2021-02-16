// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    public class MLWebRTCAudioSinkBehavior : MonoBehaviour
    {
        public MLWebRTC.AudioSink AudioSink 
        { 
            get
            {
                return audioSink;
            }
            
            set
            {
                if (audioSink != null)
                {
#if PLATFORM_LUMIN
                    audioSink.ResetPosition();
#endif
                }
                audioSink = value;
                if (audioSink != null)
                {
                    audioSink.SetPosition(transform.position);
                }
            }
        }

        private MLWebRTC.AudioSink audioSink;
        void Awake()
        {
#if PLATFORM_LUMIN
            MLResult result = MLPrivileges.RequestPrivileges(MLPrivileges.Id.Internet, MLPrivileges.Id.LocalAreaNetwork, MLPrivileges.Id.CameraCapture, MLPrivileges.Id.AudioCaptureMic);

            if (result.Result != MLResult.Code.PrivilegeGranted)
            {
                Debug.LogError("MLPrivileges failed to grant all needed privileges.");
                enabled = false;
            }

            audioSink = MLWebRTC.AudioSink.Create(out  result);
            audioSink.SetPosition(transform.position);
#endif
        }

        void Update()
        {
            if (transform.hasChanged && audioSink != null)
            {
                audioSink.SetPosition(transform.position);
                transform.hasChanged = false;
            }
        }
    }
}
