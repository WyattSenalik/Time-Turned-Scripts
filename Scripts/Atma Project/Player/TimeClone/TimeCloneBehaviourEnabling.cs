using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
using Timed;
using Timed.TimedComponentImplementations;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// TimeClone behaviour for enabling behaviours
    /// </summary>
    [RequireComponent(typeof(TimeClone))]
    public sealed class TimeCloneBehaviourEnabling : MonoBehaviour
    {
        [SerializeField, Required] private Behaviour m_behaviour = null;

        [SerializeField] private bool m_doesPlayerHaveMultipleOfCopyFromBehaviour = false;
        [SerializeField, Min(0), ShowIf(nameof(m_doesPlayerHaveMultipleOfCopyFromBehaviour))] private int m_componentIndex = 0;

        private TimeClone m_clone = null;
        private TimedBool m_copiedEnableData = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_behaviour, nameof(m_behaviour), this);
            #endregion Asserts
            m_clone = this.GetComponentSafe<TimeClone>();
            m_clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (m_clone != null)
            {
                m_clone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }
        private void FixedUpdate()
        {
            if (m_copiedEnableData == null) { return; }
            m_behaviour.enabled = m_copiedEnableData.curData;
        }


        private void Initialize()
        {
            TimedBehaviourEnabling[] t_ogBehavEnabling = m_clone.cloneData.originalPlayerObj.GetComponents<TimedBehaviourEnabling>();
            TimedBehaviourEnabling t_correspondingBehavEnabling = t_ogBehavEnabling[m_componentIndex];

            m_copiedEnableData = new TimedBool(t_correspondingBehavEnabling.isEnabled.scrapbook, true);
        }
    }
}