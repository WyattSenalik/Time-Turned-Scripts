using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Branching.Testing
{
    public sealed class SetConditionValue : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private ConditionManager conditionMan
        {
            get
            {
                if (m_conditionMan == null)
                {
                    InitializeConditionManagerIfNull();
                }
                return m_conditionMan;
            }
        }

        [SerializeField, Required] private BranchConvoAction m_branchRef = null;
        [SerializeField] private eActivationTiming m_whenToSet = eActivationTiming.Start;
        [SerializeField, ShowIf(nameof(ShowAutoSetValue))] private bool m_onStartCondValue = true;

        private ConditionManager m_conditionMan = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_branchRef, nameof(m_branchRef), this);
            #endregion Asserts
        }
        private void Start()
        {
            InitializeConditionManagerIfNull();
            if (m_whenToSet == eActivationTiming.Start || m_whenToSet == eActivationTiming.Both)
            {
                conditionMan.SetCondition(m_branchRef.conditionID, m_onStartCondValue);
            }
        }


        public void SetManual(bool condValue)
        {
            #region Logs
            //CustomDebug.LogForComponent($"SetManual({nameof(condValue)}={condValue})", this, IS_DEBUGGING);
            #endregion Logs
            if (m_whenToSet == eActivationTiming.Manual || m_whenToSet == eActivationTiming.Both)
            {
                conditionMan.SetCondition(m_branchRef, condValue);
            }
            else
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Tried to manually set Condition Value but 'When To Set' is {m_whenToSet}.", this);
                #endregion Logs
                return;
            }
        }


        private bool ShowAutoSetValue() => m_whenToSet == eActivationTiming.Start || m_whenToSet == eActivationTiming.Both;
        private void InitializeConditionManagerIfNull()
        {
            if (m_conditionMan == null)
            {
                m_conditionMan = ConditionManager.instance;
                #region Asserts
                //CustomDebug.AssertSingletonIsNotNull(m_conditionMan, this);
                #endregion Asserts
            }
        }

        public enum eActivationTiming { Start, Manual, Both }
    }
}