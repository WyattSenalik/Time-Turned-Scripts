using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using Helpers.Events;

using Timed;
using NaughtyAttributes;
using Helpers.Physics.Custom2DInt.NavAI;
using Helpers.Extensions;
// Original Authors - Jack Dekko
// Tweaked by Wyatt Senalik (7 Nov. 2022, 3 Oct. 2023, & 7 Dec. 2023)

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class Door : TimedRecorder, IToggleable
    {
        public IEventPrimer onOpen => m_onOpen;
        public IEventPrimer onClose => m_onClose;
        public bool isOn => m_isOn.curData;
        public bool isToggleInteractable => m_isToggleInteractable;
        public GameObject[] linkedObjects => m_linkedObjects;

        [SerializeField, FormerlySerializedAs("isToggleInteractable")]
        private bool m_isToggleInteractable = false;
        [SerializeField, FormerlySerializedAs("linkedObjects")]
        private GameObject[] m_linkedObjects = new GameObject[0];
        [SerializeField] private bool m_invertOpenClose = false;
        [SerializeField] private bool m_startOpen = false;
        [SerializeField] private bool m_isExitDoor = false;
        [SerializeField] private MixedEvent m_onOpen = new MixedEvent();
        [SerializeField] private MixedEvent m_onClose = new MixedEvent();

        [SerializeField] private PushableBoxCollider m_doorWallCollider = null;

        [SerializeField, BoxGroup("Sounds")] private SoundRecorder m_activateSoundRecorder = null;
        [SerializeField, BoxGroup("Sounds")] private SoundRecorder m_deactivateSoundRecorder = null;

        [SerializeField] private bool m_isDebugging = false;
        [SerializeField, ReadOnly] private bool m_isOnDebug = false;
        [SerializeField, ReadOnly] private bool m_isOnDesiredDebug = false;
        [SerializeField, ReadOnly] private bool m_isRecordingDebug = false;

        private IActivator[] m_activators = null;

        private BoxPhysicsManager m_boxPhysMan = null;

        private Collider2D m_collider = null;

        private TimedBool m_isOn = null;
        private TimedBool m_desiredOnValue = null;

        private bool m_isCloseDoorOncePlayerStepsOffCoroutActive = false;
        private Coroutine m_closeDoorOncePlayerStepsOffCorout = null;


        protected override void Awake()
        {
            base.Awake();
            // Not required to have a collider.
            m_collider = GetComponent<Collider2D>();

            m_activators = new IActivator[m_linkedObjects.Length];
            for (int i = 0; i < m_linkedObjects.Length; ++i)
            {
                m_activators[i] = m_linkedObjects[i].GetIComponentSafe<IActivator>(this);
            }
        }
        private IEnumerator Start()
        {
            m_boxPhysMan = BoxPhysicsManager.instance;

            m_isOn = new TimedBool(m_invertOpenClose ? !m_startOpen : m_startOpen, false);
            m_desiredOnValue = new TimedBool(m_invertOpenClose ? !m_startOpen : m_startOpen, false);
            ToggleOpenClose(m_startOpen, false, true);

            UpdateNavGraphTileState();
            yield return null;
            UpdateNavGraphTileState();
        }
        private void Update()
        {
            m_isOnDebug = m_isOn.curData;
            m_isOnDesiredDebug = m_desiredOnValue.curData;
            m_isRecordingDebug = m_isOn.isRecording;
        }


        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            // Need to wait a frame because we need to wait for Player's (or clone's) Rigidbody2D to turn back on before making is playing standing on check.
            StartCoroutine(CheckAfterFrame());
            IEnumerator CheckAfterFrame()
            {
                yield return null;

                // If we want the door to close, but its not closed
                if (!m_desiredOnValue.curData && m_isOn.curData)
                {
                    // If the player is standing on it, then start the coroutine
                    if (IsPlayerCurrentlyStandingOnOpenDoor())
                    {
                        StartCloseDoorAfterPlayerStepsOffCorout();
                    }
                    else
                    {
                        // Will close the door whether inverted or not
                        ToggleOpenClose(m_invertOpenClose);
                    }
                }

                UpdateNavGraphTileState();
            }
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            // Don't check constantly while paused if the player is standing on the door.
            StopCloseDoorAfterPlayerStepsOffCorout();
        }


        public void Activate()
        {
            if (m_isOn == null) { return; }

            // TODO: Change component to generic shared class between toggleables (not worth it at this point)
            if (isToggleInteractable)
            {
                bool t_openOrClose = !m_invertOpenClose; // True when we dont want to invert, false when we do.
                if (t_openOrClose == m_isOn.curData) { return; }
                
                if (AreAllLinkedObjectsActive())
                {
                    // Open object
                    ToggleOpenClose(true);
                }
            }
        }
        public void Deactivate()
        {
            if (m_isOn == null) { return; }

            // TODO: Change component to generic shared class between toggleables (not worth it at this point)
            if (isToggleInteractable)
            {
                bool t_openOrClose = m_invertOpenClose; // False when we dont want to invert, true when we do.
                if (t_openOrClose == m_isOn.curData) { return; }

                if (!AreAllLinkedObjectsActive())
                {
                    // Close object
                    ToggleOpenClose(false);
                }
            }
        }


        private void ToggleOpenClose(bool openOrClose, bool shouldPlaySound = true, bool executeEvenIfSame = false)
        {
            if (!isRecording) { return; }
            if (m_desiredOnValue == null || m_isOn == null) { return; }

            if (m_invertOpenClose)
            {
                openOrClose = !openOrClose;
            }
            m_desiredOnValue.curData = openOrClose;
            if (m_isOn.curData == openOrClose && !executeEvenIfSame) { return; }

            /// Coroutine
            // If is currently on (open) and we want to turn it off (close), but the player is standing on the door. Put off turning off (close) the door until the player is no longer standing on the door.
            if (!openOrClose)
            {
                // If door is currently on (open) and player is standing on it, then start the monitor coroutine.
                if (m_isOn.curData && IsPlayerCurrentlyStandingOnOpenDoor())
                {
                    StartCloseDoorAfterPlayerStepsOffCorout();
                    return;
                }
            }
            // If we want to turn on (open) the door, stop the close door after player steps off coroutine (if its running).
            else
            {
                StopCloseDoorAfterPlayerStepsOffCorout();
            }

            #region Logs
            //CustomDebug.LogForComponent($"Toggling door ({openOrClose}).", this, m_isDebugging);
            #endregion Logs

            /// Sound and Event
            if (openOrClose)
            {
                if (shouldPlaySound && m_activateSoundRecorder != null)
                {
                    m_activateSoundRecorder.Play();
                }

                m_onOpen.Invoke();
            }
            else
            {
                if (shouldPlaySound && m_deactivateSoundRecorder != null)
                {
                    m_deactivateSoundRecorder.Play();
                }

                m_onClose.Invoke();
            }

            /// Collider
            if (m_collider != null)
            {
                m_collider.enabled = !openOrClose;
            }

            m_isOn.curData = openOrClose;

            /// Turning on Nav Mesh Tiles
            UpdateNavGraphTileState();
        }
        private bool AreAllLinkedObjectsActive()
        {
            bool t_allActivated = true;
            foreach (IActivator t_activator in m_activators)
            {
                if (!t_activator.isActive)
                {
                    t_allActivated = false;
                    break;
                }
            }
            return t_allActivated;
        }
        private void DoActionForEachLinkedObject(Action<IActivator> forEachActivator)
        {
            foreach (IActivator t_activator in m_activators)
            {
                forEachActivator.Invoke(t_activator);
            }
        }
        private bool IsPlayerCurrentlyStandingOnOpenDoor()
        {
            if (m_isExitDoor)
            {
                return false;
            }
            return m_boxPhysMan.IsTherePusherOrBoxOnDoor(m_doorWallCollider);
        }
        private void StartCloseDoorAfterPlayerStepsOffCorout()
        {
            // No need to start the coroutine if its already active.
            if (m_isCloseDoorOncePlayerStepsOffCoroutActive) { return; }
            m_closeDoorOncePlayerStepsOffCorout = StartCoroutine(CloseDoorAfterPlayerStepsOffCorout());
        }
        private void StopCloseDoorAfterPlayerStepsOffCorout()
        {
            if (m_isCloseDoorOncePlayerStepsOffCoroutActive)
            {
                StopCoroutine(m_closeDoorOncePlayerStepsOffCorout);
                m_isCloseDoorOncePlayerStepsOffCoroutActive = false;
            }
        }
        private IEnumerator CloseDoorAfterPlayerStepsOffCorout()
        {
            m_isCloseDoorOncePlayerStepsOffCoroutActive = true;

            while (IsPlayerCurrentlyStandingOnOpenDoor() || !isRecording)
            {
                yield return null;
            }
            // Close door (if inverted, sends true (which gets inverted to false). If not inverted, sends false).
            ToggleOpenClose(m_invertOpenClose);

            m_isCloseDoorOncePlayerStepsOffCoroutActive = false;
        }

        private void UpdateNavGraphTileState()
        {
            NavGraphInt2D t_navGraph = NavGraphInt2D.instance;
            if (!m_isExitDoor && t_navGraph != null)
            {
                t_navGraph.SetStateOfTileThatContainsFloatPosition(transform.position, m_isOn.curData);
            }
        }


#if UNITY_EDITOR
        public void AddGameObjectToLinkedObjects(IActivator obj)
        {
            List<GameObject> t_objList = new List<GameObject>();
            foreach (GameObject t_linkedObj in m_linkedObjects)
            {
                t_objList.Add(t_linkedObj);
            }
            t_objList.Add((obj as MonoBehaviour).gameObject);
            m_linkedObjects = t_objList.ToArray();
        }

        public void RemoveGameObjectFromLinkedObjects(IActivator obj)
        {
            List<GameObject> t_objList = new List<GameObject>();
            List<IActivator> t_activatorList = new List<IActivator>();
            foreach (GameObject t_linkedObj in m_linkedObjects)
            {
                if (t_linkedObj.TryGetComponent(out IActivator t_activator))
                {
                    t_activatorList.Add(t_activator);
                }
            }
            if (t_activatorList.Contains(obj))
            {
                t_activatorList.Remove(obj);
            }
            foreach (IActivator t_activator in t_activatorList)
            {
                t_objList.Add((t_activator as MonoBehaviour).gameObject);
            }
            m_linkedObjects = t_objList.ToArray();
        }
#endif
    }
}