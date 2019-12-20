// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;

namespace MagicLeap
{
    /// <summary>
    /// Maintains the found object visual by updating the transform and text label.
    /// </summary>
    public class FoundObjectVisual : MonoBehaviour
    {
        [SerializeField, Tooltip("The transform of the bounding box outline mesh.")]
        private Transform _outline = null;

        [SerializeField, Tooltip("The text used for the object label.")]
        private TextMesh _label = null;

        public void UpdateVisual(Vector3 position, Quaternion rotation, Vector3 extents, string label = "Found Object")
        {
            _outline.position = position;
            _outline.rotation = rotation;

            // Need to test scale value. Unknown units.
            _outline.localScale = extents / 100;

            _label.text = label;
        }
    }
}
