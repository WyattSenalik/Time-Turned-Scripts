using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Branching.Testing
{
    /// <summary>
    /// Helper for unregistering conditions via unity events.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UnregisterConditions : MonoBehaviour
    {
        private ConditionManager m_condMan = null;


        private void Start()
        {
            m_condMan = ConditionManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_condMan, this);
            #endregion Asserts
        }


        public void UnregisterAllConditions()
        {
            m_condMan.UnregisterAllConditions();
        }
    }
}