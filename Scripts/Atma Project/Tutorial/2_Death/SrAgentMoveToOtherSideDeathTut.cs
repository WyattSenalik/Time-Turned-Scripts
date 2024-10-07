using System;
using UnityEngine;

using NaughtyAttributes;

using Dialogue.ConvoActions.Programmed;
using Dialogue;
using Helpers.Events;
using Timed;
using UnityEngine.InputSystem.Extension;
using Atma.Settings;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Death
{
    /// <summary>
    /// Scripted event for the Rewind/Death Tutorial (Scene 3 as of writing this) for when the Sr Agent goes to the other side of the room.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SrAgentMoveToOtherSideDeathTut : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;

        public IEventPrimer onWaitingForPlayerBegin => m_onWaitingForPlayerBegin;
        public IEventPrimer onAdvancing => m_onAdvancing;

        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private BulletController m_enemyBulletCont = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;

        [SerializeField] private string m_movingInpMapName = "Default";

        [SerializeField, Min(0.0f)] private float m_timeAfterFireToBeginMoveTwoAnim = 0.0f;

        [SerializeField] private string m_stopMovingUniqueKey = "54747e70193e4dfeb9dc072529ae1160";

        [SerializeField] private MixedEvent m_onWaitingForPlayerBegin = new MixedEvent();
        [SerializeField] private MixedEvent m_onAdvancing = new MixedEvent();

        private Action m_onFinished = null;
        private eDeathTutMoveState m_curState = eDeathTutMoveState.BeforeBegin;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_enemyBulletCont, nameof(m_enemyBulletCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerMoveSus, nameof(m_playerMoveSus), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_enemyBulletCont.onFireBullet.ToggleSubscription(OnBulletFired, true);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_enemyBulletCont?.onFireBullet.ToggleSubscription(OnBulletFired, false);
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;

            if (ConversationSkipper.instance.ShouldSkipDialogue())
            {
                StartMoveTwoAnim();
                return;
            }

            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eDeathTutMoveState.BeforeBegin, this);
            #endregion Asserts
            m_curState = eDeathTutMoveState.MoveOneAnim;

            m_srAgentMover.PlayMoveAnimation(0);
        }
        public override bool Advance(ConvoData convoData)
        {
            if (m_curState == eDeathTutMoveState.Finished)
            {
                m_onAdvancing.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Should be called by <see cref="SrAgentMover"/>'s 1st (0-index) move anim.
        /// </summary>
        public void OnMoveOneAnimEnd()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(OnMoveOneAnimEnd), this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eDeathTutMoveState.MoveOneAnim, this);
            #endregion Asserts
            m_curState = eDeathTutMoveState.WaitingForBullet;
        }
        /// <summary>
        /// Should be called by <see cref="SrAgentMover"/>'s 2nd (1-index) move anim.
        /// </summary>
        public void OnMoveTwoAnimEnd()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(OnMoveTwoAnimEnd), this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eDeathTutMoveState.MoveTwoAnim, this);
            #endregion Asserts
            m_curState = eDeathTutMoveState.WaitingForPlayer;

            // Change player input map to let them move.
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);
            // Make it so the player isn't allowed to rewind to before when this anim ended.
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);

            m_onWaitingForPlayerBegin.Invoke();
        }
        public void SetPlayerReachedEndOfRoom()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(SetPlayerReachedEndOfRoom), this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, eDeathTutMoveState.WaitingForPlayer, this);
            #endregion Asserts
            m_curState = eDeathTutMoveState.Finished;

            // Make is so the player can't move again
            m_inputMapStack.PopInputMap(m_movingInpMapName);
            m_playerMoveSus.SuspendWithUniqueKey(m_stopMovingUniqueKey);

            m_onFinished?.Invoke();
        }


        private void OnBulletFired()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(OnBulletFired), this, IS_DEBUGGING);
            #endregion Logs
            if (m_curState != eDeathTutMoveState.WaitingForBullet) { return; }
            m_curState = eDeathTutMoveState.WaitingAfterBullet;

            Invoke(nameof(StartMoveTwoAnim), m_timeAfterFireToBeginMoveTwoAnim);
        }
        private void StartMoveTwoAnim()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(StartMoveTwoAnim), this, IS_DEBUGGING);
            #endregion Logs
            m_curState = eDeathTutMoveState.MoveTwoAnim;

            m_srAgentMover.PlayMoveAnimation(1);
        }


        public enum eDeathTutMoveState { BeforeBegin, MoveOneAnim, WaitingForBullet, WaitingAfterBullet, MoveTwoAnim, WaitingForPlayer, Finished }
    }
}