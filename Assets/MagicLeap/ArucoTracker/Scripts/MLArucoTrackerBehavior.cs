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
namespace MagicLeap.Core
{
    using UnityEngine;
    using UnityEngine.XR.MagicLeap;

    public class MLArucoTrackerBehavior : MonoBehaviour
    {
        public event MLArucoTracker.Marker.OnStatusChangeDelegate OnMarkerStatusChange = null;
        
        public MLArucoTracker.Marker Marker => _marker;

        [Tooltip("The dictionary of the marker that should be found.")]
        public MLArucoTracker.DictionaryName MarkerDictionary;

        [Tooltip("The id of the marker that should be found.")]
        public int MarkerId;

        private MLArucoTracker.Marker _marker = null;

        void Start()
        {
#if PLATFORM_LUMIN
            MLResult result = MLPrivileges.RequestPrivileges(MLPrivileges.Id.CameraCapture);
#endif
        }

        void Update()
        {
            if (_marker == null)
            {
                #if PLATFORM_LUMIN
                if(MLArucoTracker.TrackerSettings.Dictionary == MarkerDictionary)
                {
                    _marker = MLArucoTracker.GetMarker(MarkerId);
                    if (_marker != null)
                    {
                        OnMarkerStatusChange?.Invoke(_marker, _marker.Status);
                        _marker.OnStatusChange += OnMarkerStatusChange;
                    }
                }
                #endif
            }
            else
            {
                transform.position = _marker.Position;
                transform.rotation = _marker.Rotation;
            }
        }

        void OnDestroy()
        {
            #if PLATFORM_LUMIN
            if (_marker != null)
            {
                _marker.OnStatusChange -= OnMarkerStatusChange;
            }

            #endif
        }
    }
}
