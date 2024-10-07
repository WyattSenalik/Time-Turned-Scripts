using System;
using UnityEngine;

using NaughtyAttributes;

using Dialogue;
using Dialogue.ConvoActions.Programmed;
using Timed;
using UnityEngine.InputSystem.Extension;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Clone
{
    /// <summary>
    /// Shared moving logic for tutorial levels for playing an animation for the sr agent to get to the other side of the level.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SrAgentMoveToOtherSideSimple : MonoBehavEndpointProgrammedConvoAction
    {
        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;
        [SerializeField, Required] private BranchPlayerController m_playerCont = null;

        [SerializeField, Min(0)] private int m_moverMoveAnimIndex = 0;

        [SerializeField] private string m_switchMovingInpMapName = "Default";
        [SerializeField] private string m_popMovingInpMapName = "Default";

        [SerializeField] private string m_stopMovingUniqueKey = "faf2c89e-4565-442c-b2ba-7aef1a705249";

        [SerializeField] private bool m_setEarliestTimeToBeginningOfAction = false;
        [SerializeField] private bool m_setEarliestTimeToEndOfAnim = true;
        [SerializeField] private bool m_startTimeManipAfterEndOfAnim = false;
        [SerializeField, ShowIf(nameof(m_startTimeManipAfterEndOfAnim))] private bool m_isFirstPause = false;

        [SerializeField] private bool m_autoFinishAfterAnimation = false;

        [SerializeField] private bool m_pauseTimeAfterFinsh = false;

        private eMoveState m_curState = eMoveState.BeforeBegin;
        private Action m_onFinished = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerMoveSus, nameof(m_playerMoveSus), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerCont, nameof(m_playerCont), this);
            #endregion Asserts
        }

        // For the below functions, they are sorted by order in which they occur, not by StyleGuide rules

        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;

            if (m_setEarliestTimeToBeginningOfAction)
            {
                m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
            }

            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.BeforeBegin, this);
            #endregion Asserts
            m_curState = eMoveState.MoveAnim;

            m_srAgentMover.PlayMoveAnimation(m_moverMoveAnimIndex);
        }
        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s move anim with index <see cref="m_moverMoveAnimIndex"/>.
        /// </summary>
        public void OnMoveAnimEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.MoveAnim, this);
            #endregion Asserts
            m_curState = eMoveState.WaitingForGoAhead;

            // Waiting for player now, so let them move.
            m_inputMapStack.SwitchInputMap(m_switchMovingInpMapName);
            if (m_setEarliestTimeToEndOfAnim)
            {
                // Also restrict time so player can't rewind to before now.
                m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
            }
            if (m_startTimeManipAfterEndOfAnim)
            {
                StartCoroutine(StartTimeManipAfterDelay());
            }
            if (m_autoFinishAfterAnimation)
            {
                SetFinished();
            }
        }
        /// <summary>
        /// Meant to be called by a TriggerEnter2D when the player reaches the end of the level, usually.
        /// </summary>
        public void SetFinished()
        {
            StartCoroutine(FinishAfterDelay());
        }
        public override bool Advance(ConvoData convoData)
        {
            if (m_curState == eMoveState.Finished)
            {
                return true;
            }
            return false;
        }

        private IEnumerator FinishAfterDelay()
        {
            yield return null;
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.WaitingForGoAhead, this);
            #endregion Asserts
            m_curState = eMoveState.Finished;

            // Restrict the player's movement
            m_inputMapStack.PopInputMap(m_popMovingInpMapName);
            m_playerMoveSus.SuspendWithUniqueKey(m_stopMovingUniqueKey);

            if (m_pauseTimeAfterFinsh)
            {
                m_timeRewinder.StartRewind();
            }

            m_onFinished?.Invoke();
        }
        private IEnumerator StartTimeManipAfterDelay()
        {
            yield return null;
            m_playerCont.ForceBeginTimeManipulation(null, m_isFirstPause);
        }


        public enum eMoveState { BeforeBegin, MoveAnim, WaitingForGoAhead, Finished }
    }
}
