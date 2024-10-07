using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Extension;

using NaughtyAttributes;

using Dialogue;
using Dialogue.ConvoActions.Branching;
using Dialogue.ConvoActions.Programmed;
using Timed;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    [DisallowMultipleComponent]
    public sealed class SrAgentMoveAfterRewindCloneTut : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private MovementSuspender m_moveSus = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private CloneManager m_cloneMan = null;

        [SerializeField, Required] private SwitchPressurePlateHandler m_ppHandler = null;
        [SerializeField, Required] private BranchConvoAction m_wasCloneCreatedCorrectlyBranchAction = null;

        [SerializeField] private string m_restrictedInpMapName = "Restricted";
        [SerializeField] private string m_movingInpMapName = "Default";

        [SerializeField]
        private string m_resumeMovingUniqueKey = "148556bd7fd948b08ea74c36ba6761c2";

        private ConditionManager m_condMan = null;
        private SkipButton m_skipButton = null;
        private ConversationSkipper m_convoSkipper = null;

        private Action m_onFinished = null;
        private Coroutine m_waitCorout = null;

        private bool m_isPressurePlateActive = false;
        private bool m_isMoveAfterCloneAnimFin = false;
        private bool m_hasCalledFin = false;


        protected override void Awake() 
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_moveSus, nameof(m_moveSus), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneMan, nameof(m_cloneMan), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppHandler, nameof(m_ppHandler), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_wasCloneCreatedCorrectlyBranchAction, nameof(m_wasCloneCreatedCorrectlyBranchAction), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_condMan = ConditionManager.instance;
            m_skipButton = SkipButton.GetInstanceSafe(this);
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            ToggleSubscriptions(false);
        }

        // For the below functions, they are sorted by order in which they occur, not by StyleGuide rules

        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;
            ToggleSubscriptions(true);

            // Resume time because it is currently paused.
            m_timeRewinder.CancelRewind();

            m_srAgentMover.PlayMoveAnimation(1);

            // Switch input maps to be restricted while agent moves.
            m_inputMapStack.SwitchInputMap(m_restrictedInpMapName);
            // Reset skipping
            m_skipButton.ResetSkipButton();
        }
        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s 2nd (1-index) move anim when its finished.
        /// </summary>
        public void OnMoveAfterCloneAnimEnd()
        {
            // Need to protect against recalling this because if player rewinds to "earliest time" this will be called again because the call was recorded by the TimedAnimator somehow.
            if (m_isMoveAfterCloneAnimFin) { return; }
            m_isMoveAfterCloneAnimFin = true;

            // Pop the restricted map cause agent is done moving.
            m_inputMapStack.PopInputMap(m_restrictedInpMapName);
            // Let player move again
            m_moveSus.CancelSuspensionWithUniqueKey(m_resumeMovingUniqueKey);
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);

            // Restrict rewind time to not rewind past this point
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);

            if (m_isPressurePlateActive)
            {
                #region Logs
                //CustomDebug.LogForComponent($"MoveAnimEnd: InvokeOnFinished", this, IS_DEBUGGING);
                #endregion Logs
                InvokeOnFinished();
            }
            else
            {
                #region Logs
                //CustomDebug.LogForComponent($"MoveAnimEnd: StartCorout", this, IS_DEBUGGING);
                #endregion Logs
                m_waitCorout = StartCoroutine(WaitUntilPPActiveOrCloneDisappear());
            }
        }
        private IEnumerator WaitUntilPPActiveOrCloneDisappear()
        {
            yield return new WaitUntil(() =>
            {
                return m_cloneMan.GetAmountClonesActiveAtTime(m_cloneMan.curTime) <= 0;
            });
            yield return new WaitForFixedUpdate();

            #region Logs
            //CustomDebug.LogForComponent($"Fin waiting for clone to disappear", this, IS_DEBUGGING);
            #endregion Logs

            InvokeOnFinished();
        }
        private void InvokeOnFinished()
        {
            if (m_convoSkipper.ShouldSkipDialogue())
            {
                Finish();
            }
            else
            {
                StartCoroutine(InvokeOnFinishAfterAFrame());
            }
        }
        private IEnumerator InvokeOnFinishAfterAFrame()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Finish();
        }
        private void Finish()
        {
            if (!m_hasCalledFin)
            {
                m_hasCalledFin = true;

                // Pause time to let dirigo speak
                m_timeRewinder.StartRewind();
                m_inputMapStack.PopInputMap(m_movingInpMapName);

                // Kill the wait coroutine if it was active
                if (m_waitCorout != null)
                {
                    StopCoroutine(m_waitCorout);
                }

                m_onFinished?.Invoke();
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            return m_hasCalledFin;
        }

        // For the above functions, they are sorted by order in which they occur, not by StyleGuide rules

        private void ToggleSubscriptions(bool cond)
        {
            m_ppHandler?.onActivated.ToggleSubscription(OnPressurePlateActivated, cond);
            m_ppHandler?.onDeactivated.ToggleSubscription(OnPressurePlateDeactivated, cond);
        }
        #region EventDriven
        private void OnPressurePlateActivated()
        {
            m_isPressurePlateActive = true;

            if (m_isMoveAfterCloneAnimFin && m_condMan.GetCondition(m_wasCloneCreatedCorrectlyBranchAction))
            {
                #region Asserts
                //CustomDebug.LogForComponent($"PPActive: InvokeOnFinished.", this, IS_DEBUGGING);
                #endregion Asserts
                InvokeOnFinished();
            }
        }
        private void OnPressurePlateDeactivated()
        {
            m_isPressurePlateActive = false;
        }
        #endregion EventDriven
    }
}
