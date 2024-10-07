using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Helpers.Events.GameEventSystem;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class LifetimeManager : MonoBehaviour
    {
        [SerializeField, Required] private GameObject m_lifetimeUIElePrefab = null;

        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneCreatedEventIdentifierSO m_cloneCreatedEventID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneDeletedEventIdentifierSO m_cloneDeletedEventID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneDisconnectedEventIdentifierSO m_cloneDisconnectedEventID = null;

        [SerializeField] private RectTransform[] m_uiEleParents = new RectTransform[0];

        private SubManager<ICloneCreatedContext> m_cloneCreatedSubMan = null;
        private SubManager<ICloneDeletedContext> m_cloneDeletedSubMan = null;
        private SubManager<ICloneDisconnectedContext> m_cloneDisconnectedSubMan = null;

        private readonly List<ILifetimeUIElement> m_spawnedElements = new List<ILifetimeUIElement>();

        private ITimeRewinder m_timeRewinder = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedEventID, nameof(m_cloneCreatedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDeletedEventID, nameof(m_cloneDeletedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDisconnectedEventID, nameof(m_cloneDisconnectedEventID), this);
            #endregion Asserts
        }
        private void Start()
        {
            // Clone Created
            m_cloneCreatedSubMan = new SubManager<ICloneCreatedContext>(m_cloneCreatedEventID, OnCloneCreated);
            m_cloneCreatedSubMan.Subscribe();
            // Clone Deleted
            m_cloneDeletedSubMan = new SubManager<ICloneDeletedContext>(m_cloneDeletedEventID, OnCloneDeleted);
            m_cloneDeletedSubMan.Subscribe();
            // Clone Disconnected
            m_cloneDisconnectedSubMan = new SubManager<ICloneDisconnectedContext>(m_cloneDisconnectedEventID, OnCloneDisconnected);
            m_cloneDisconnectedSubMan.Subscribe();

            PlayerSingleton t_player = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            #endregion Asserts
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            foreach (ILifetimeUIElement t_uiEle in m_spawnedElements)
            {
                t_uiEle.enabled = true;
            }
        }
        private void OnDisable()
        {
            foreach (ILifetimeUIElement t_uiEle in m_spawnedElements)
            {
                t_uiEle.enabled = false;
            }
        }
        private void OnDestroy()
        {
            m_cloneCreatedSubMan?.Unsubscribe();
            m_cloneDeletedSubMan?.Unsubscribe();
            m_cloneDisconnectedSubMan?.Unsubscribe();
        }


        private void OnCloneCreated(ICloneCreatedContext context)
        {
            TimeClone t_timeClone = context.timeClone;
            TimeCloneInitData t_cloneData = t_timeClone.cloneData;
            int t_chargeIndex = t_cloneData.occupyingCharge;

            // Determine what it's parent will be.
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(t_chargeIndex, m_uiEleParents, this);
            #endregion Asserts
            RectTransform t_parent = m_uiEleParents[t_chargeIndex];
            // Create new LifetimeUIElement
            GameObject t_uiEleObj = Instantiate(m_lifetimeUIElePrefab, t_parent);
            ILifetimeUIElement t_lifetimeUIEle = t_uiEleObj.GetComponent<ILifetimeUIElement>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(t_lifetimeUIEle, t_uiEleObj, this);
            #endregion Asserts
            t_lifetimeUIEle.Initialize(t_cloneData.spawnTime, t_cloneData.farthestTime, t_chargeIndex, m_timeRewinder);
            m_spawnedElements.Add(t_lifetimeUIEle);
            t_lifetimeUIEle.enabled = enabled;
        }
        private void OnCloneDeleted(ICloneDeletedContext context)
        {
            DestroyLifetimeForClone(context.timeClone);
        }
        private void OnCloneDisconnected(ICloneDisconnectedContext context)
        {
            DestroyLifetimeForClone(context.timeClone);
        }

        private void DestroyLifetimeForClone(TimeClone clone)
        {
            TimeCloneInitData t_cloneData = clone.cloneData;

            // Search and destroy the existing UI element for this clone.
            for (int i = 0; i < m_spawnedElements.Count; ++i)
            {
                ILifetimeUIElement t_uiEle = m_spawnedElements[i];
                if (t_uiEle.chargeIndex == t_cloneData.occupyingCharge &&
                    t_uiEle.startTime == t_cloneData.spawnTime &&
                    t_uiEle.endTime == t_cloneData.farthestTime)
                {
                    Destroy(t_uiEle.gameObject);
                    m_spawnedElements.RemoveAt(i);

                    break;
                }
            }
        }
    }
}