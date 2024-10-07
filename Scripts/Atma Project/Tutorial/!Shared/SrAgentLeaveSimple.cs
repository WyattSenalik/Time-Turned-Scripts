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

namespace Atma.Tutorial
{
    /// <summary>
    /// Shared leaving logic for tutorial levels for playing an animation for the sr agent to leave and then letting the player move.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SrAgentLeaveSimple : MonoBehavEndpointProgrammedConvoAction
    {
        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;

        [SerializeField, Min(0)] private int m_moverMoveAnimIndex = 1;

        [SerializeField] private string m_movingInpMapName = "Default";

        [SerializeField] private string m_resumeMovingUniqueKey = "faf2c89e-4565-442c-b2ba-7aef1a705249";

        [SerializeField] private bool m_unpauseTimeBeforeMove = false;

        private ConversationSkipper m_convoSkipper = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerMoveSus, nameof(m_playerMoveSus), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            // No need to save the onFinished callback since this is the last action.
            // m_onFinished = onFinished;
            if (m_unpauseTimeBeforeMove)
            {
                m_timeRewinder.CancelRewind();
            }

            m_srAgentMover.PlayMoveAnimation(m_moverMoveAnimIndex);
            if (m_convoSkipper.ShouldSkipDialogue())
            {
                m_srAgentMover.gameObject.SetActive(false);
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            // Last action, so there is no need to end the conversation.
            return false;
        }

        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s move anim with index <see cref="m_moverMoveAnimIndex"/>.
        /// </summary>
        public void OnLeaveAnimEnd()
        {
            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                // Need to wait 1 frame because Dirigo's timedObject was set to stop recording this frame, which needs a frame to take effect (kind of, he just shows at that end time then, so if you rewind all the way after this, you'd see him).
                StartCoroutine(WaitAFrameAfterLeaveAnimEnd());
            }
            else
            {
                RestorePlayerControl();
            }
        }

        private IEnumerator WaitAFrameAfterLeaveAnimEnd()
        {
            yield return null;
            RestorePlayerControl();
        }
        private void RestorePlayerControl()
        {
            // Let the player move again.
            m_playerMoveSus.CancelSuspensionWithUniqueKey(m_resumeMovingUniqueKey);
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);
            // Also restrict time so player can't rewind to before now.
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
        }
    }
}