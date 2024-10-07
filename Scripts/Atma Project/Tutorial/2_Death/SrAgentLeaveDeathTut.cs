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

namespace Atma.Tutorial.Death
{
    /// <summary>
    /// Scripted event for the Rewind/Death Tutorial (Scene 3 as of writing this) for when the Sr Agent leaves the room.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SrAgentLeaveDeathTut : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;

        [SerializeField] private string m_movingInpMapName = "Default";
        [SerializeField] private string m_resumeMovingUniqueKey = "54747e70193e4dfeb9dc072529ae1160";

        private ConversationSkipper m_convoSkipper = null;

        private Action m_onFinished = null;
        private bool m_isAnimFinished = false;


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
            m_onFinished = onFinished;

            if (m_convoSkipper.ShouldSkipDialogue())
            {
                gameObject.SetActive(false);
                RestorePlayerControlAndFinish();
            }
            else
            {
                m_srAgentMover.PlayMoveAnimation(2);
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            return m_isAnimFinished;
        }

        /// <summary>
        /// Should be called from by <see cref="m_srAgentMover"/>'s 3rd (2-index) move.
        /// </summary>
        public void OnLeaveAnimEnd()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(OnLeaveAnimEnd), this, IS_DEBUGGING);
            #endregion Logs
            // Need to wait 1 frame because Dirigo's timedObject was set to stop recording this frame, which needs a frame to take effect (kind of, he just shows at that end time then, so if you rewind all the way after this, you'd see him).
            StartCoroutine(WaitAFrameAfterLeaveAnimEnd());
        }


        private IEnumerator WaitAFrameAfterLeaveAnimEnd()
        {
            yield return new WaitForFixedUpdate();
            RestorePlayerControlAndFinish();
        }

        private void RestorePlayerControlAndFinish()
        {
            // Set the earliest time to now so that the player can't rewind to before Sr Agent started talking. Also so we don't have to record Sr Agent's movements (not hard but could be annoying if we add a collider to him).
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);

            // Allow the player to move again.
            m_playerMoveSus.CancelSuspensionWithUniqueKey(m_resumeMovingUniqueKey);
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);

            m_isAnimFinished = true;
            m_onFinished?.Invoke();
        }
    }
}