// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright company="Magic Leap">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://id.magicleap.com/terms/developer
//
// </copyright>
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;
using UnityEngine;
using MagicLeap.Core;

namespace MagicLeap
{
    /// <summary>
    /// Demonstrates how to detect and visualize found objects.
    /// </summary>
    [RequireComponent(typeof(MLFoundObjectsBehavior))]
    public class FoundObjectExample : MonoBehaviour
    {
        [SerializeField, Tooltip("The prefab used for the detected found objects.")]
        private GameObject _outlinePrefab = null;

        private MLFoundObjectsBehavior _foundObjectBehavior = null;
        private Dictionary<System.Guid, GameObject> _foundObjects = new Dictionary<System.Guid, GameObject>();

        private void Awake()
        {
            _foundObjectBehavior = GetComponent<MLFoundObjectsBehavior>();

            if (_outlinePrefab == null)
            {
                Debug.LogError("Error: FoundObjectExample._outlinePrefab is not set, disabling script.");
                enabled = false;
                return;
            }

            _foundObjectBehavior.OnQueryFoundObjectsResult += HandleOnQueryFoundObjectsResult;
        }

        private void HandleOnQueryFoundObjectsResult(System.Guid id, Vector3 position, Quaternion rotation, Vector3 extents, List<KeyValuePair<string, string>> properties)
        {
            GameObject objectInstance = null;
            FoundObjectVisual objectVisual = null;

            // Obtain a reference to the found object visual GameObject.
            if (_foundObjects.ContainsKey(id))
            {
                _foundObjects.TryGetValue(id, out objectInstance);
            }
            else
            {
                objectInstance = Instantiate(_outlinePrefab);
            }

            if (objectInstance == null)
            {
                Debug.LogError("Error: FoundObjectExample.HandleOnQueryFoundObjectsResult failed to obtain an object instance.");
                return;
            }

            // Obtain the found object visual script.
            objectVisual = objectInstance.GetComponent<FoundObjectVisual>();

            if (objectVisual == null)
            {
                Debug.LogError("Error: FoundObjectExample.HandleOnQueryFoundObjectsResult failed to obtain the object visual script.");
                return;
            }

            string label = "Default";

            // Obtain the first property label if it exists.
            if(properties != null && properties.Count > 0)
            {
                label = properties[0].Value;
            }

            // Update the found object visual with the new properties.
            objectVisual.UpdateVisual(position, rotation, extents, label);
        }
    }
}
