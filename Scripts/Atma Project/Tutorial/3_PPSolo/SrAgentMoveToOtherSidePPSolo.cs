using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Atma.Settings;
using Dialogue;
using Dialogue.ConvoActions.Programmed;
using Timed;
using UnityEngine.InputSystem.Extension;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.PPSolo
{
    [DisallowMultipleComponent]
    public sealed class SrAgentMoveToOtherSidePPSolo : MonoBehavEndpointProgrammedConvoAction
    {
        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private BulletController m_enemy1BulletCont = null;
        [SerializeField, Required] private BulletController m_enemy2BulletCont = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;
        [SerializeField, Required] private BranchPlayerController m_playerCont = null;

        [SerializeField] private string m_movingInpMapName = "Default";

        [SerializeField, Min(0.0f)] private float m_timeAfterEnemy1FireToBeginMoveTwoAnim = 0.0f;
        [SerializeField, Min(0.0f)] private float m_timeAfterEnemy2FireToBeginMoveTwoAnim = 0.0f;

        [SerializeField] private string m_stopMovingUniqueKey = "19dc77fc-e6aa-4b18-939b-9262b572b372";

        private ConversationSkipper m_convoSkipper = null;
        private SkipButton m_skipButton = null;
        private ePPSoloMoveState m_curState = ePPSoloMoveState.BeforeBegin;

        private Action m_onFinished = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_enemy1BulletCont, nameof(m_enemy1BulletCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_enemy2BulletCont, nameof(m_enemy2BulletCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerMoveSus, nameof(m_playerMoveSus), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerCont, nameof(m_playerCont), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
            m_skipButton = SkipButton.GetInstanceSafe(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_enemy1BulletCont?.onFireBullet.ToggleSubscription(OnEnemy1BulletFired, false);
            m_enemy2BulletCont?.onFireBullet.ToggleSubscription(OnEnemy2BulletFired, false);
        }

        // For the below functions, they are sorted by order in which they occur, not by StyleGuide rules

        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;

            if (m_convoSkipper.ShouldSkipDialogue())
            {
                StartMoveThreeAnim();
                m_skipButton.ResetSkipButton();
                return;
            }

            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.BeforeBegin, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.MoveOneAnim;

            m_srAgentMover.PlayMoveAnimation(0);
        }
        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s 1st (0-index) move anim when finished.
        /// </summary>
        public void OnMoveAnimOneEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.MoveOneAnim, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.BulletWaitOne;

            m_enemy1BulletCont.onFireBullet.ToggleSubscription(OnEnemy1BulletFired, true);
        }
        private void OnEnemy1BulletFired()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.BulletWaitOne, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.AfterBulletWaitOne;

            m_enemy1BulletCont.onFireBullet.ToggleSubscription(OnEnemy1BulletFired, false);

            Invoke(nameof(StartMoveTwoAnim), m_timeAfterEnemy1FireToBeginMoveTwoAnim);
        }

        private void StartMoveTwoAnim()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.AfterBulletWaitOne, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.MoveTwoAnim;

            m_srAgentMover.PlayMoveAnimation(1);
        }
        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s 2nd (1-index) move anim when finished.
        /// </summary>
        public void OnMoveAnimTwoEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.MoveTwoAnim, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.BulletWaitTwo;

            m_enemy2BulletCont.onFireBullet.ToggleSubscription(OnEnemy2BulletFired, true);
        }
        private void OnEnemy2BulletFired()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.BulletWaitTwo, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.AfterBulletWaitTwo;

            m_enemy2BulletCont.onFireBullet.ToggleSubscription(OnEnemy2BulletFired, false);

            Invoke(nameof(StartMoveThreeAnim), m_timeAfterEnemy2FireToBeginMoveTwoAnim);
        }

        private void StartMoveThreeAnim()
        {
            m_curState = ePPSoloMoveState.MoveThreeAnim;

            m_srAgentMover.PlayMoveAnimation(2);
        }
        /// <summary>
        /// Meant to be called by <see cref="m_srAgentMover"/>'s 3rd (2-index) move anim when finished.
        /// </summary>
        public void OnMoveAnimThreeEnd()
        {
            m_curState = ePPSoloMoveState.WaitingForPlayer;

            // Waiting for player now, so let them move.
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);
            // Pause time for player (wait a frame first)
            StartCoroutine(ForceBeginTimeManipulationCoroutine());
        }
        /// <summary>
        /// Meant to be called by a TriggerEnter2D when the player reaches the end of the level.
        /// </summary>
        public void SetPlayerHasReachedEnd()
        {
            #region Asserts
            //CustomDebug.AssertEnumIs(m_curState, ePPSoloMoveState.WaitingForPlayer, this);
            #endregion Asserts
            m_curState = ePPSoloMoveState.Finished;

            // Restrict the player's movement
            m_inputMapStack.PopInputMap(m_movingInpMapName);
            m_playerMoveSus.SuspendWithUniqueKey(m_stopMovingUniqueKey);

            m_onFinished?.Invoke();
        }

        public override bool Advance(ConvoData convoData)
        {
            if (m_curState == ePPSoloMoveState.Finished)
            {
                return true;
            }
            return false;
        }


        private IEnumerator ForceBeginTimeManipulationCoroutine()
        {
            // Wait 1 frame
            yield return null;
            // Also restrict time so player can't rewind to before now.
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
            // Wait 1 more frame
            yield return null;
            m_playerCont.ForceBeginTimeManipulation();
        }


        public enum ePPSoloMoveState { BeforeBegin, MoveOneAnim, BulletWaitOne, AfterBulletWaitOne, MoveTwoAnim, BulletWaitTwo, AfterBulletWaitTwo, MoveThreeAnim, WaitingForPlayer, Finished }
    }
}