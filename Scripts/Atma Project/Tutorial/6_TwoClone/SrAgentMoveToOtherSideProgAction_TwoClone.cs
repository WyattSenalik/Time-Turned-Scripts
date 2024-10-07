using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Dialogue;
using Dialogue.ConvoActions.Programmed;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.TwoClone
{
    /// <summary>
    /// Moves SrAgent to other side of room and then starts the dialogue.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SrAgentMoveToOtherSideProgAction_TwoClone : MonoBehavEndpointProgrammedConvoAction
    {
        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;

        private Action m_onFinished = null;
        private eMoveState m_curState = eMoveState.BeforeBegin;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            #endregion Asserts
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;

            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.BeforeBegin, this);
            #endregion Asserts
            m_curState = eMoveState.MoveAnim;

            m_srAgentMover.PlayMoveAnimation(0);
        }
        public override bool Advance(ConvoData convoData)
        {
            if (m_curState == eMoveState.Finished)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s 1st (0-index) move anim when finished.
        /// </summary>
        public void OnMoveAnimEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eMoveState.MoveAnim, this);
            #endregion Asserts
            m_curState = eMoveState.Finished;

            StartCoroutine(StartRewindAfterDelay());
        }

        private IEnumerator StartRewindAfterDelay()
        {
            // Set earliest rewind time to now
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
            yield return null;
            // Freeze time so that dialogue isn't taking up time.
            m_timeRewinder.StartRewind();

            m_onFinished?.Invoke();
        }



        public enum eMoveState { BeforeBegin, MoveAnim, Finished }
    }
}