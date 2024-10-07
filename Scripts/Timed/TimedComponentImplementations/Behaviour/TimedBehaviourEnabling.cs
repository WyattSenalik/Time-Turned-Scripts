using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// Keeps track of whether the specified behaviour is enabled or not.
    /// </summary>
    public sealed class TimedBehaviourEnabling : MonoBehaviour
    {
        public TimedBool isEnabled => m_isEnabled;

        [SerializeField, Required] private Behaviour m_behaviour = null;
#if UNITY_EDITOR
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private int m_snapshotsDebug = 0;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private bool m_valueDebug = false;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private bool m_isRecording = false;
#endif

        private TimedBool m_isEnabled = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_behaviour, nameof(m_behaviour), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_isEnabled = new TimedBool(m_behaviour.enabled);
        }
        private void FixedUpdate()
        {
#if UNITY_EDITOR
            m_snapshotsDebug = m_isEnabled.scrapbook.Count;
            m_valueDebug = m_isEnabled.curData;
            m_isRecording = m_isEnabled.isRecording;
#endif
            if (m_isEnabled.isRecording)
            {
                m_isEnabled.curData = m_behaviour.enabled;
            }
            else
            {
                m_behaviour.enabled = m_isEnabled.curData;
            }
        }
    }
}