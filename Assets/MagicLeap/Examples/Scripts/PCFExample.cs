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

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Core;

namespace MagicLeap
{
    /// <summary>
    /// Demonstrates how to persist objects dynamically by interfacing with
    /// the MLPersistence.PersistentCoordinateFramesCoordinateFrames API and the BindingsLocalStorage class.
    /// </summary>
    public class PCFExample : MonoBehaviour
    {
        [Serializable]
        public class PrefabMapEntry
        {
            public string Type;
            public GameObject Prefab;
        }

        [SerializeField, Tooltip("Persistent Content to create.")]
        private GameObject _content = null;

        [SerializeField, Tooltip("Controller to use.")]
        private MLControllerConnectionHandlerBehavior _controller = null;

        [SerializeField, Tooltip("Distance in front of Controller to create content.")]
        private float _distance = 0.2f;

        [SerializeField, Tooltip("PCFVisualizer behavior to use when debugging.")]
        private PCFVisualizer _pcfVisualizer = null;

        private List<GameObject> _persistentContentList = new List<GameObject>();

        [SerializeField, Tooltip("Number of frames to perform delete all gesture before executing the deletion.")]
        private int _deleteAllSequenceMinFrames = 60;
        private int _deleteAllSequenceFrameCount = 0;
        private bool _deleteAllInitiated = false;

        [SerializeField, Tooltip("Status Text")]
        private Text _statusText = null;

        #if PLATFORM_LUMIN
        private bool bindingsLoaded = false;
        #endif

        [SerializeField]
        private List<PrefabMapEntry> prefabMapping = new List<PrefabMapEntry>();

        public static int numPersistentContent
        {
            get;
            set;
        }

        public static int numPersistentContentRestored
        {
            get;
            set;
        }

        public static int numTotalPCFs
        {
            get;
            set;
        }

        public static int numSingerUserSingleSessionPCFs
        {
            get;
            set;
        }

        public static int numSingleUserMultiSessionPCFs
        {
            get;
            set;
        }

        public static int numMultiUserMultiSessionPCFs
        {
            get;
            set;
        }

        /// <summary>
        /// Validates fields.
        /// </summary>
        void Awake()
        {
            if (_content == null || _content.GetComponent<PersistentBall>() == null)
            {
                Debug.LogError("Error: PCFExample._content is not set or is missing PersistentBall behavior, disabling script.");
                enabled = false;
                return;
            }

            if (_controller == null)
            {
                Debug.LogError("Error: PCFExample._controller is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_statusText == null)
            {
                Debug.LogError("Error: PCFExample._statusText is not set, disabling script.");
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// Starts APIs, registers to MLInput events, and restores content.
        /// </summary>
        void Start()
        {
            #if PLATFORM_LUMIN
            MLResult result = MLPersistentCoordinateFrames.Start();
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: PCFExample failed starting MLPersistentCoordinateFrames, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }


            MLPersistentCoordinateFrames.PCF.OnStatusChange += HandleOnPCFStatusChange;
            MLPersistentCoordinateFrames.OnLocalized += HandleOnLocalized;

            MLInput.OnControllerButtonDown += HandleControllerButtonDown;
            MLInput.OnControllerTouchpadGestureStart += HandleTouchpadGestureStart;
            MLInput.OnControllerTouchpadGestureContinue += HandleTouchpadGestureContinue;
            MLInput.OnControllerTouchpadGestureEnd += HandleTouchpadGestureEnd;
            #endif
        }

        /// <summary>
        /// Updates the status text inside the UI.
        /// </summary>
        void Update()
        {
            string exampleStatus = "";

            if (_deleteAllInitiated)
            {
                if (_deleteAllSequenceFrameCount < _deleteAllSequenceMinFrames)
                {
                    exampleStatus = string.Format("<color=yellow>{0} {1:P} {2}.</color>",
                        LocalizeManager.GetString("DeleteAllSequence"),
                        LocalizeManager.GetString("Complete"),
                        (float)(_deleteAllSequenceFrameCount) / _deleteAllSequenceMinFrames);
                }
            }

            _statusText.text = string.Format("<color=#dbfb76><b>{0}</b></color>\n{1}: {2}\n\n",
                LocalizeManager.GetString("ControllerData"),
                LocalizeManager.GetString("Status"),
                LocalizeManager.GetString(ControllerStatus.Text));

            #if PLATFORM_LUMIN
            _statusText.text += string.Format("<color=#dbfb76><b>{0}</b></color>\n{1}: {2}\n{3}\n",
                LocalizeManager.GetString("ExampleData"),
                LocalizeManager.GetString("Status"),
                LocalizeManager.GetString(MLPersistentCoordinateFrames.IsLocalized ? "LocalizedToMap" : "NotLocalizedToMap"),
                LocalizeManager.GetString(exampleStatus));
            #endif

            _statusText.text += string.Format("{0}: {1}\n\n", LocalizeManager.GetString("RestoredContent"), numPersistentContentRestored);

            _statusText.text += string.Format("{0}: {1}\n\n", LocalizeManager.GetString("CreatedContent"), numPersistentContent);

            if (PCFVisualizer.IsVisualizing)
            {
                _statusText.text += string.Format("{0}: {1}\n", LocalizeManager.GetString("Total PCFCount"), numTotalPCFs);
                _statusText.text += string.Format("{0}: {1}\n", LocalizeManager.GetString("Singer-Single PCFCount"), numSingerUserSingleSessionPCFs);
                _statusText.text += string.Format("{0}: {1}\n", LocalizeManager.GetString("Single-Multi PCFCount"), numSingleUserMultiSessionPCFs);
                _statusText.text += string.Format("{0}: {1}\n", LocalizeManager.GetString("Multi-Multi PCFCount"), numMultiUserMultiSessionPCFs);

            }
        }
        /// <summary>
        /// Handler for pcf status changes.
        /// When PCFs have their status changed it is broadcasted by the MLPersistentCoordinateFrames.PCF class.
        /// </summary>
        /// <param name="pcfStatus">The new status of the incoming pcf.</param>
        /// <param name="pcf">The incoming pcf that has changed.</param>
        private void HandleOnPCFStatusChange(MLPersistentCoordinateFrames.PCF.Status pcfStatus, MLPersistentCoordinateFrames.PCF pcf)
        {
            if (pcfStatus == MLPersistentCoordinateFrames.PCF.Status.Created || pcfStatus == MLPersistentCoordinateFrames.PCF.Status.Regained)
            {
                ++numTotalPCFs;

                #if PLATFORM_LUMIN
                switch (pcf.Type)
                {
                    case MLPersistentCoordinateFrames.PCF.Types.SingleUserSingleSession:
                    {
                        ++numSingerUserSingleSessionPCFs;
                        break;
                    }

                    case MLPersistentCoordinateFrames.PCF.Types.SingleUserMultiSession:
                    {
                        ++numSingleUserMultiSessionPCFs;
                        break;
                    }

                    case MLPersistentCoordinateFrames.PCF.Types.MultiUserMultiSession:
                    {
                        ++numMultiUserMultiSessionPCFs;
                        break;
                    }
                }
                #endif
            }
        }

        /// <summary>
        /// Stops the MLPersistentCoordinateFrames API and unregisters from events and
        /// </summary>
        void OnDestroy()
        {
            #if PLATFORM_LUMIN
            MLPersistentCoordinateFrames.PCF.OnStatusChange -= HandleOnPCFStatusChange;
            MLPersistentCoordinateFrames.Stop();
            MLHeadTracking.Stop();
            MLInput.OnControllerButtonDown -= HandleControllerButtonDown;
            MLInput.OnControllerTouchpadGestureStart -= HandleTouchpadGestureStart;
            MLInput.OnControllerTouchpadGestureContinue -= HandleTouchpadGestureContinue;
            MLInput.OnControllerTouchpadGestureEnd -= HandleTouchpadGestureEnd;
            #endif
        }

        /// <summary>
        /// Handle for when localization is gained or lost.
        /// Attempts to read all the locally stored bindings when localized and resets the counters when localization is lost.
        /// </summary>
        /// <param name="localized"> Map Events that happened. </param>
        private void HandleOnLocalized(bool localized)
        {
            #if PLATFORM_LUMIN
            if (localized)
            {
                ReadAllStoredBindings();
            }
            else
            {
                numPersistentContent = 0;
                numPersistentContentRestored = 0;
                numTotalPCFs = 0;
                numSingerUserSingleSessionPCFs = 0;
                numSingleUserMultiSessionPCFs = 0;
                numMultiUserMultiSessionPCFs = 0;
            }
            #endif
        }

        /// <summary>
        /// Reads all stored persistent bindings.
        /// Give the persistent content the stored binding's object id to try and have the binidng restored by MLPersistentCoordinateFrames.
        /// If the binding is restored correctly then the persistent content will retain it's pose from the last known launch.
        /// Unsubscribe from OnLocalized event so this will only run on application launch.
        /// </summary>
        private void ReadAllStoredBindings()
        {
            #if PLATFORM_LUMIN
            if (bindingsLoaded)
            {
                return;
            }

            TransformBinding.storage.LoadFromFile();

            List<TransformBinding> allBindings = TransformBinding.storage.Bindings;

            List<TransformBinding> deleteBindings = new List<TransformBinding>();

            foreach (TransformBinding storedBinding in allBindings)
            {
                // try to get the pcf with the stored cfuid form last session
                MLResult result = MLPersistentCoordinateFrames.FindPCFByCFUID(storedBinding.PCF.CFUID, out MLPersistentCoordinateFrames.PCF pcf);

                if (result.IsOk)
                {
                    PrefabMapEntry prefabMapEntry = prefabMapping.Find(entry => entry.Type == storedBinding.PrefabType);

                    if (prefabMapEntry != null)
                    {
                        GameObject gameObj = Instantiate(prefabMapEntry.Prefab, Vector3.zero, Quaternion.identity);
                        PersistentBall persistentContent = gameObj.GetComponent<PersistentBall>();
                        persistentContent.BallTransformBinding = storedBinding;
                        persistentContent.BallTransformBinding.Bind(pcf, gameObj.transform, true);
                        _persistentContentList.Add(gameObj);
                        numPersistentContentRestored++;
                    }
                }
                else
                {
                    deleteBindings.Add(storedBinding);
                }
            }

            foreach (TransformBinding storedBinding in deleteBindings)
            {
                storedBinding.UnBind();
            }

            bindingsLoaded = true;
            #endif
        }


        /// <summary>
        /// Instantiates a new object with MLPCFPersistentContent. The MLPCFPersistentContent is
        /// responsible for restoring and saving itself.
        /// </summary>
        /// <param name="position">Position to spawn the content at.</param>
        /// <param name="rotation">Rotation to spawn the content at.</param>
        private void CreateContent(Vector3 position, Quaternion rotation)
        {
            GameObject gameObj = Instantiate(_content, position, rotation);
            #if PLATFORM_LUMIN
            MLPersistentCoordinateFrames.FindClosestPCF(transform.position, out MLPersistentCoordinateFrames.PCF pcf);
            PersistentBall persistentContent = gameObj.GetComponent<PersistentBall>();
            persistentContent.BallTransformBinding = new TransformBinding(gameObj.GetInstanceID().ToString(), "Ball");
            persistentContent.BallTransformBinding.Bind(pcf, gameObj.transform);
            #endif
            numPersistentContent++;
            _persistentContentList.Add(gameObj);
        }

        /// <summary>
        /// Listens for Bumper and Home Tap.
        /// </summary>
        /// <param name="controllerId">Id of the incoming controller.</param>
        /// <param name="button">Enum of the button that was pressed.</param>
        private void HandleControllerButtonDown(byte controllerId, MLInput.Controller.Button button)
        {
            if (!_controller.IsControllerValid(controllerId))
            {
                return;
            }

            if (button == MLInput.Controller.Button.Bumper)
            {
                Vector3 position = _controller.transform.position + _controller.transform.forward * _distance;
                CreateContent(position, _controller.transform.rotation);
            }
            else if (button == MLInput.Controller.Button.HomeTap)
            {
                _pcfVisualizer.Toggle();
            }
        }

        /// <summary>
        /// Handler when touchpad gesture begins.
        /// </summary>
        /// <param name="controllerId">Id of the incoming controller.</param>
        /// <param name="touchpadGesture">Class of which gesture was started.</param>
        private void HandleTouchpadGestureStart(byte controllerId, MLInput.Controller.TouchpadGesture touchpadGesture)
        {
            if (!_controller.IsControllerValid(controllerId))
            {
                return;
            }

            #if PLATFORM_LUMIN
            if (touchpadGesture.Type == MLInput.Controller.TouchpadGesture.GestureType.RadialScroll)
            {
                _deleteAllInitiated = true;
                _deleteAllSequenceFrameCount = 0;
            }
            #endif
        }

        /// <summary>
        /// Handler when touchpad gesture continues.
        /// </summary>
        /// <param name="controllerId">Id of the incoming controller.</param>
        /// <param name="touchpadGesture">Class of which gesture was continued.</param>
        private void HandleTouchpadGestureContinue(byte controllerId, MLInput.Controller.TouchpadGesture touchpadGesture)
        {
            if (!_controller.IsControllerValid(controllerId))
            {
                return;
            }

            if (_deleteAllInitiated)
            {
                #if PLATFORM_LUMIN
                if (touchpadGesture.Type == MLInput.Controller.TouchpadGesture.GestureType.RadialScroll)
                {
                    ++_deleteAllSequenceFrameCount;
                    if (_deleteAllSequenceFrameCount >= _deleteAllSequenceMinFrames)
                    {
                        _deleteAllInitiated = false;
                        foreach(GameObject content in _persistentContentList)
                        {
                            Destroy(content);
                        }
                        _persistentContentList.Clear();
                    }
                }
                #endif
            }
        }

        /// <summary>
        /// Handler when touchpad gesture ends.
        /// </summary>
        /// <param name="controllerId">Id of the incoming controller.</param>
        /// <param name="touchpadGesture">Class of which gesture was ended.</param>
        private void HandleTouchpadGestureEnd(byte controllerId, MLInput.Controller.TouchpadGesture touchpadGesture)
        {
            if (!_controller.IsControllerValid(controllerId))
            {
                return;
            }

            if (_deleteAllInitiated)
            {
                #if PLATFORM_LUMIN
                if (touchpadGesture.Type == MLInput.Controller.TouchpadGesture.GestureType.RadialScroll &&
                    _deleteAllSequenceFrameCount < _deleteAllSequenceMinFrames)
                {
                    _deleteAllInitiated = false;
                }
                #endif
            }
        }
    }
}
