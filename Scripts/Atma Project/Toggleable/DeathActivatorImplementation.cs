using Helpers.Events;
using Helpers.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atma
{
    public class DeathActivatorImplementation : MonoBehaviour, IActivator
    {
        private const bool IS_DEBUGGING = false;

        public IReadOnlyList<GameObject> linkedObjects => m_linkedObjects;
        public bool isActive { get; private set; } = false;
        public IEventPrimer onActivated => m_onActivated;
        public IEventPrimer onDeactivated => m_onDeactivated;

        [SerializeField] private GameObject[] m_linkedObjects = new GameObject[0];
        [SerializeField] private bool m_shouldActivateOnEnable = true;

        [SerializeField] private MixedEvent m_onActivated = new MixedEvent();
        [SerializeField] private MixedEvent m_onDeactivated = new MixedEvent();

        private IToggleable[] m_toggleables = null;

        private void Awake()
        {
            //CustomDebug.AssertSerializeFieldIsNotNull(m_linkedObjects, nameof(m_linkedObjects), this);

            m_toggleables = new IToggleable[m_linkedObjects.Length];
            for (int i = 0; i < m_linkedObjects.Length; ++i)
            {
                m_toggleables[i] = m_linkedObjects[i].GetIComponentSafe<IToggleable>(this);
            }
        }
        private void OnEnable()
        {
            if (m_shouldActivateOnEnable)
            {
                Deactivate();
            }
        }
        private void OnDisable()
        {
            if (m_shouldActivateOnEnable)
            {
                Activate();
            }
        }

        public void Activate()
        {
            if (isActive) { return; }
            isActive = true;

            foreach (IToggleable t_toggleable in m_toggleables)
            {
                t_toggleable.Activate();
            }
            #region Logs
            //CustomDebug.LogForComponent("Pressure Plate Activated", this, IS_DEBUGGING);
            #endregion Logs

            m_onActivated.Invoke();
        }

        public void Deactivate()
        {
            // Already deactivated.
            if (!isActive) { return; }
            isActive = false;

            foreach (IToggleable t_toggleable in m_toggleables)
            {
                t_toggleable.Deactivate();
            }
            #region Logs
            //CustomDebug.LogForComponent("Pressure Plate Deactivated", this, IS_DEBUGGING);
            #endregion Logs

            m_onDeactivated.Invoke();
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
