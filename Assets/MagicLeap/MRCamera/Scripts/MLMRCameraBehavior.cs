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
    public class MLMRCameraBehavior : MonoBehaviour
    {
        [SerializeField]
        private MLMRCamera.InputContext inputContext;

        public delegate void OnNewRenderPlaneDelegate(MLMRCamera.Frame.ImagePlane imagePlane);
        public event OnNewRenderPlaneDelegate OnNewImagePlane;

        private void Start()
        {
#if PLATFORM_LUMIN
            if(!MLMRCamera.IsStarted)
            {
                MLPrivileges.RequestPrivilege(MLPrivileges.Id.CameraCapture);

                if (MLMRCamera.Connect(inputContext).IsOk)
                {
                    MLMRCamera.StartCapture();
                }
            }
#endif
        }
        
        private void OnEnable()
        {
            MLMRCamera.OnFrameCapture += OnFrameCapture;
        }

        private void OnDisable()
        {
            MLMRCamera.OnFrameCapture -= OnFrameCapture;
        }

        private void OnDestroy()
        {
#if PLATFORM_LUMIN
            MLMRCamera.Disconnect();
#endif
        }

        private void OnFrameCapture(MLMRCamera.Frame frame)
        {
            foreach (MLMRCamera.Frame.ImagePlane imagePlane in frame.ImagePlanes)
            {
                OnNewImagePlane?.Invoke(imagePlane);
            }
        }
    }
}
