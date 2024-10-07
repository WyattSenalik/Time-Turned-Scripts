using System;
using UnityEngine;

using NaughtyAttributes;

using Dialogue;
using Dialogue.ConvoActions.Programmed;
using Timed;
using UnityEngine.InputSystem.Extension;
using System.Collections;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.PPBox
{
    [DisallowMultipleComponent]
    public sealed class SrAgentMoveToOtherSidePPBox : MonoBehavEndpointProgrammedConvoAction
    {
        public SkipButton skipButton
        {
            get
            {
                if (m_skipButton == null)
                {
                    m_skipButton = SkipButton.GetInstanceSafe(this);
                }
                return m_skipButton;
            }
        }

        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;
        [SerializeField, Required] private BranchPlayerController m_playerCont = null;

        [SerializeField] private string m_movingInpMapName = "Default";

        [SerializeField] private string m_stopMovingUniqueKey = "ac1f14f6-4a0b-441b-93ee-112a801a60db";

        [SerializeField] private bool m_isFirstPause = false;

        private SkipButton m_skipButton = null;

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

            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.BeforeBegin, this);
            #endregion Asserts
            m_curState = eMoveState.MoveAnim;

            m_srAgentMover.PlayMoveAnimation(0);

            // Reset the skip button
            skipButton.ResetSkipButton();
        }
        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s 1st (0-index) move anim when finished.
        /// </summary>
        public void OnMoveAnimEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.MoveAnim, this);
            #endregion Asserts
            m_curState = eMoveState.WaitingForPlayer;

            // Waiting for player now, so let them move.
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);
            // After a frame, pause time
            StartCoroutine(StartTimeManipAfterDelay());
        }
        /// <summary>
        /// Meant to be called by a TriggerEnter2D when the player reaches the end of the level.
        /// </summary>
        public void SetPlayerHasReachedEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.WaitingForPlayer, this);
            #endregion Asserts
            m_curState = eMoveState.Finished;

            // Restrict the player's movement
            m_inputMapStack.PopInputMap(m_movingInpMapName);
            m_playerMoveSus.SuspendWithUniqueKey(m_stopMovingUniqueKey);

            m_onFinished?.Invoke();
        }
        public override bool Advance(ConvoData convoData)
        {
            if (m_curState == eMoveState.Finished)
            {
                return true;
            }
            return false;
        }


        private IEnumerator StartTimeManipAfterDelay()
        {
            yield return null;
            // Also restrict time so player can't rewind to before now.
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
            yield return null;
            m_playerCont.ForceBeginTimeManipulation(null, m_isFirstPause);
        }


        public enum eMoveState { BeforeBegin, MoveAnim, WaitingForPlayer, Finished }
    }
}