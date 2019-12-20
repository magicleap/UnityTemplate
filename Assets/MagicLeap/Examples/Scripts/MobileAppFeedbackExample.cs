// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://id.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System;

namespace MagicLeap
{
      /// <summary>
    /// This provides visual feedback for the status and keyboard data of the mobile app controller.
    /// </summary>
    public class MobileAppFeedbackExample : MonoBehaviour
    {
        [SerializeField, Tooltip("The mobile app visualizer to use for displaying keyboard text.")]
        private MobileAppControllerVisualizer _mobileAppVisualizer = null;

        [SerializeField, Tooltip("The status text that will display input.")]
        private Text _statusText = null;

        void Awake()
        {
            if (_mobileAppVisualizer == null)
            {
                Debug.LogError("Error: MobileAppFeedbackExample._mobileAppVisualizer is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_statusText == null)
            {
                Debug.LogError("Error: MobileAppFeedbackExample._statusText is not set, disabling script.");
                enabled = false;
                return;
            }
        }

        void Update()
        {
            _statusText.text = string.Format("<color=#dbfb76><b>{0}</b></color>\n{1}: {2}\n\n",
            LocalizeManager.GetString("Controller Data"),
            LocalizeManager.GetString("Status"),
            LocalizeManager.GetString(ControllerStatus.Text));

            _statusText.text += string.Format("<color=#dbfb76><b>{0}</b></color>\n{1}: {2}",
                LocalizeManager.GetString("Keyboard Data"),
                LocalizeManager.GetString("Input"),
                LocalizeManager.GetString(_mobileAppVisualizer.KeyboardText));
        }
    }
}
