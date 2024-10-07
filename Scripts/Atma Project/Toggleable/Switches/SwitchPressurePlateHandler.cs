using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Timed;
using Helpers.Extensions;

// Original Authors - Jack Dekko
// Heavily Tweaked by Wyatt Senalik
//
// A lot of code has moved to OncePressurePlate and HoldPressurePlate.
//
// Kept this class alive so that we don't have to go through every instance of PP in every level
// and re-specify the linked objects. Bryce will be doing this eventually anyway. No need to do it twice.
// Also kept because then we can continue to use the tool Eslis made.

namespace Atma
{
    /// <summary>
    /// Shared class for <see cref="OncePressurePlate"/> and <see cref="HoldPressurePlate"/> that holds
    /// linked objects and has functions for activating and deactivating the pressure plate.
    /// </summary>
    public sealed class SwitchPressurePlateHandler : TimedRecorder, IActivator
    {
        private const bool IS_DEBUGGING = false;

        [ShowNativeProperty]
        public bool isActive { get; private set; } = false;
        public IReadOnlyList<GameObject> linkedObjects => m_linkedObjects;
        public IEventPrimer onActivated => m_onActivated;
        public IEventPrimer onDeactivated => m_onDeactivated;

        [SerializeField] private bool m_invertSignal = false;
        [SerializeField] private GameObject[] m_linkedObjects = new GameObject[1];
        [SerializeField, Tag] private string[] m_activateTags = new string[] { "Player", "Clone", "PickUp", "PressurePlateInteractable" };

        [SerializeField] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))] private string m_activeAnimBoolParamName = "Active";

        [SerializeField, Required, BoxGroup("Physics")] private Collider2D m_ppTriggerCollider = null;
        [SerializeField, BoxGroup("Physics")] private ContactFilter2D m_checkContactFilter = new ContactFilter2D();

        [SerializeField, BoxGroup("Events")] private MixedEvent m_onActivated = new MixedEvent();
        [SerializeField, BoxGroup("Events")] private MixedEvent m_onDeactivated = new MixedEvent();

        private IToggleable[] m_toggleables = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppTriggerCollider, nameof(m_ppTriggerCollider), this);
            #endregion Asserts

            m_toggleables = new IToggleable[m_linkedObjects.Length];
            for (int i = 0; i < m_linkedObjects.Length; ++i)
            {
                m_toggleables[i] = m_linkedObjects[i].GetIComponentSafe<IToggleable>(this);
            }
        }


        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            // Need to do this now that we are using an animator (which doesn't allow its params to be set when not recording).
            if (m_animator != null)
            {
                StartCoroutine(ShortDelayCorout());
                IEnumerator ShortDelayCorout()
                {

                    m_animator.SetBool(m_activeAnimBoolParamName, isActive);
                    yield return null;
                    m_animator.SetBool(m_activeAnimBoolParamName, isActive);
                }
            }
        }


        public bool IsValidTag(GameObject obj)
        {
            foreach (string t_tag in m_activateTags)
            {
                if (obj.CompareTag(t_tag))
                {
                    return true;
                }
            }
            return false;
        }
        public void Activate()
        {
            if (m_invertSignal)
            {
                // Already activated.
                //if (!isActive) { return; }
                isActive = false;

                DeactivateToggleables();
            }
            else
            {
                // Already deactivated.
                //if (isActive) { return; }
                isActive = true;

                ActiveToggleables();
            }

            m_onActivated.Invoke();
            if (m_animator != null)
            {
                m_animator.SetBool(m_activeAnimBoolParamName, isActive);
            }
        }
        public void Deactivate()
        {
            if (m_invertSignal)
            {
                // Already activated.
                //if (isActive) { return; }
                isActive = true;

                ActiveToggleables();
            }
            else
            {
                // Already deactivated.
                //if (!isActive) { return; }
                isActive = false;

                DeactivateToggleables();
            }

            m_onDeactivated.Invoke();
            if (m_animator != null)
            {
                m_animator.SetBool(m_activeAnimBoolParamName, isActive);
            }
        }
        public bool WouldActivateHaveEffect()
        {
            if (isActive)
            {
                if (m_invertSignal)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (m_invertSignal)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool WouldDeactivateHaveEffect() => !WouldActivateHaveEffect();

        public int CheckForOverlap(List<Collider2D> overlapResults)
        {
            return m_ppTriggerCollider.OverlapCollider(m_checkContactFilter, overlapResults);
        }

        private void ActiveToggleables()
        {
            foreach (IToggleable t_toggleable in m_toggleables)
            {
                t_toggleable.Activate();
            }
            #region Logs
            //CustomDebug.LogForComponent("Pressure Plate Activated", this, IS_DEBUGGING);
            #endregion Logs
        }
        private void DeactivateToggleables()
        {
            foreach (IToggleable t_toggleable in m_toggleables)
            {
                t_toggleable.Deactivate();
            }
            #region Logs
            //CustomDebug.LogForComponent("Pressure Plate Deactivated", this, IS_DEBUGGING);
            #endregion Logs
        }


        #region Editor
#if UNITY_EDITOR
        public void AddGameObjectToLinkedObjects(IToggleable obj)
        {
            List<GameObject> t_objList = new List<GameObject>();
            foreach (GameObject linkedObj in m_linkedObjects)
            {
                t_objList.Add(linkedObj);
            }
            t_objList.Add((obj as MonoBehaviour).gameObject);
            m_linkedObjects = t_objList.ToArray();
        }

        public void RemoveGameObjectFromLinkedObjects(IToggleable obj)
        {
            List<GameObject> t_gameObj = new List<GameObject>();
            List<IToggleable> t_doors = new List<IToggleable>();
            foreach (GameObject gameObj in m_linkedObjects)
            {
                if (gameObj.TryGetComponent(out IToggleable doorComp))
                    t_doors.Add(doorComp);
            }
            if (t_doors.Contains(obj))
                t_doors.Remove(obj);
            foreach (IToggleable door in t_doors)
            {
                t_gameObj.Add((door as MonoBehaviour).gameObject);
            }
            m_linkedObjects = t_gameObj.ToArray();
        }
#endif
        #endregion Editor
    }
}
