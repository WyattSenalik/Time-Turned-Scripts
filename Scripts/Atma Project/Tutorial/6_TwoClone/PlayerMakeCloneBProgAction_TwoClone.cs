using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Dialogue;
using Dialogue.ConvoActions.Branching;
using Dialogue.ConvoActions.Programmed;
using Helpers.Events.GameEventSystem;
using Timed;
using UnityEngine.InputSystem.Extension;

using static Atma.Tutorial.TwoClone.PlayerMakeCloneAOrBothClonesProgAction_TwoClone;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.TwoClone
{
    /// <summary>
    /// Programmed action for the player when after they are prompted to make a clone. Allows the player to make 1 or 2 clones, only advancing when a clone steps on a pressure plate or one of the clones disappears.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerMakeCloneBProgAction_TwoClone : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField, Required] private PlayerMakeCloneAOrBothClonesProgAction_TwoClone m_playerMakeCloneAProgAction = null;

        [SerializeField, Required] private BranchConvoAction m_isCloneBCorrectBA = null;
        [SerializeField, Required]
        private CloneCreatedEventIdentifierSO m_cloneCreatedEventIDSO = null;

        [SerializeField] private string m_moveInpMap = "Default";
        [SerializeField] private string m_resumeMoveUniqueKey = "c3014f79a35e47949a27ab1c7d90627d";
        [SerializeField] private string m_suspendMoveUniqueKey = "edb3262cc92241d48206e00c864680c1";

        [SerializeField, Tag] private string m_playerTag = "Player";

        private ConditionManager m_condMan = null;
        private SkipButton m_skipButton = null;

        private TimeRewinder m_timeRewinder = null;
        private InputMapStack m_inpMapStack = null;
        private MovementSuspender m_moveSus = null;
        private CloneManager m_cloneMan = null;

        private Door m_exitDoor = null;

        private SwitchPressurePlateHandler m_cloneBPPHandler = null;
        private SwitchPressurePlateHandler m_cloneAPPHandler = null;
        private HoldPressurePlate m_cloneBHoldPP = null;
        private HoldPressurePlate m_cloneAHoldPP = null;

        private TimeClone m_cloneA = null;

        private Action m_onFinished = null;
        private ePressurePlateOption m_ppToBeHeldByCloneB = ePressurePlateOption.Neither;
        private bool m_isCloneBHoldingTheirPPWhenRewindStart = false;
        private IReadOnlyList<TimeFrame> m_cloneBPPHeldTimeFramesWhenStartRewind = new List<TimeFrame>();
        private IReadOnlyList<TimeFrame> m_cloneAPPHeldTimeFramesWhenStartRewind = new List<TimeFrame>();
        private float m_farthestTimeWhenStartRewind = 0.0f;

        private bool m_isMonitorCoroutActive = false;

        private bool m_isFinished = false;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerMakeCloneAProgAction, nameof(m_playerMakeCloneAProgAction), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_isCloneBCorrectBA, nameof(m_isCloneBCorrectBA), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedEventIDSO, nameof(m_cloneCreatedEventIDSO), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_condMan = ConditionManager.instance;
            m_skipButton = SkipButton.GetInstanceSafe(this);

            // Set the condition to false (to reset if scene was restarted).
            m_condMan.SetCondition(m_isCloneBCorrectBA, false);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ToggleSubscriptions(false);
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;

            // Grab references from the CloneAProgAction
            m_timeRewinder = m_playerMakeCloneAProgAction.timeRewinder;
            m_inpMapStack = m_playerMakeCloneAProgAction.inpMapStack;
            m_moveSus = m_playerMakeCloneAProgAction.moveSus;
            m_cloneMan = m_playerMakeCloneAProgAction.cloneMan;

            m_exitDoor = m_playerMakeCloneAProgAction.exitDoor;

            IReadOnlyList<TimeClone> t_cloneList = m_cloneMan.GetAllExistingClones();
            #region Asserts
            //CustomDebug.AssertListIsSize(t_cloneList, nameof(t_cloneList), 1, this);
            #endregion Asserts
            m_cloneA = t_cloneList[0];

            SwitchPressurePlateHandler t_ppHandler1 = m_playerMakeCloneAProgAction.ppHandler1;
            SwitchPressurePlateHandler t_ppHandler2 = m_playerMakeCloneAProgAction.ppHandler2;

            // Determine which pp is supposed to be Clone B's (from opposite of Clone A's).
            ePressurePlateOption t_ppHeldByCloneA = m_playerMakeCloneAProgAction.ppHeldByCloneA;
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(t_ppHeldByCloneA != ePressurePlateOption.Neither, $"Expected the pp held by Clone A to not be neither ({ePressurePlateOption.Neither}).", this);
            #endregion Asserts
            // Grab B's
            m_ppToBeHeldByCloneB = t_ppHeldByCloneA == ePressurePlateOption.One ? ePressurePlateOption.Two : ePressurePlateOption.One;
            m_cloneBPPHandler = m_ppToBeHeldByCloneB == ePressurePlateOption.One ? t_ppHandler1 : t_ppHandler2;
            m_cloneBHoldPP = m_cloneBPPHandler.GetComponent<HoldPressurePlate>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_cloneBHoldPP, m_cloneBPPHandler.gameObject, this);
            #endregion Asserts
            // Grab A's
            m_cloneAPPHandler = t_ppHeldByCloneA == ePressurePlateOption.One ? t_ppHandler1 : t_ppHandler2;
            m_cloneAHoldPP = m_cloneAPPHandler.GetComponent<HoldPressurePlate>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_cloneAHoldPP, m_cloneAPPHandler.gameObject, this);
            #endregion Asserts

            // Unfreeze time
            m_timeRewinder.CancelRewind();
            // Let the player move
            m_inpMapStack.SwitchInputMap(m_moveInpMap);
            m_moveSus.CancelSuspensionWithUniqueKey(m_resumeMoveUniqueKey);

            // Subscribe to events.
            ToggleSubscriptions(true);

            // Reset the skip button
            m_skipButton.ResetSkipButton();
        }
        public override bool Advance(ConvoData convoData)
        {
            return m_isFinished;
        }


        private void ToggleSubscriptions(bool cond)
        {
            // When time is paused.
            m_timeRewinder?.onRewindBegin.ToggleSubscription(OnRewindBegin, cond);
            // When a clone is created.
            m_cloneCreatedEventIDSO.ToggleSubscriptionToEvent(OnCloneCreated, cond);
        }
        #region EventDriven
        /// <summary>
        /// Called when player pauses time.
        /// </summary>
        private void OnRewindBegin()
        {
            /// Hold info about the state of things at the time rewind beings that is vital for determining if the branch conditions are true.
            // If Clone B is holding their PP (assume false first).
            m_isCloneBHoldingTheirPPWhenRewindStart = false;
            if (m_cloneBPPHandler.isActive)
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(0, m_cloneBHoldPP.heldWindows, this);
                #endregion Asserts
                GameObject t_holdingObj = m_cloneBHoldPP.heldWindows[^1].holdingObject;
                Collider2D t_holdingCol = t_holdingObj.GetComponent<Collider2D>();
                #region Asserts
                //CustomDebug.AssertComponentOnOtherIsNotNull(t_holdingCol, t_holdingObj, this);
                #endregion Asserts
                Rigidbody2D t_holdingRB = t_holdingCol.attachedRigidbody;
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(t_holdingRB != null, $"that collider ({t_holdingCol}) would have an attached rigidbody.", this);
                #endregion Asserts
                // If it is the player (tag), then it will become clone B.
                if (t_holdingRB.CompareTag(m_playerTag))
                {
                    m_isCloneBHoldingTheirPPWhenRewindStart = true;
                    // Also grab the time frames for the pressure plate to check if there is overlap between them pressure plate later.
                    m_cloneBPPHeldTimeFramesWhenStartRewind = m_cloneBHoldPP.heldWindows.ConvertToTimeFrameList();
                    m_cloneAPPHeldTimeFramesWhenStartRewind = m_cloneAHoldPP.heldWindows.ConvertToTimeFrameList();
                }
            }
            m_farthestTimeWhenStartRewind = m_timeRewinder.farthestTime;
        }
        /// <summary>
        /// Called when a clone is created.
        /// </summary>
        private void OnCloneCreated(ICloneCreatedContext context)
        {
            // Set if clone B is correct.
            m_condMan.SetCondition(m_isCloneBCorrectBA, IsCloneBCorrect());

            // Monitor finish conditions (if the pressure plate that is a clone owns is activated or a clone disappears) (won't start a new monitor if one already exists).
            BeginMonitoringForFinishConditions(context.timeClone);
        }
        #endregion EventDriven

        private bool IsCloneBCorrect()
        {
            // Clone B is not correct if there are not holding down their pressure plate.
            //if (!m_isCloneBHoldingTheirPPWhenRewindStart)
            //{
            //    #region Logs
            //    //CustomDebug.LogForComponent($"{nameof(IsCloneBCorrect)}: Nope. Clone B was not holding down their pressure plate at rewind start.", this, IS_DEBUGGING);
            //    #endregion Logs
            //    return false;
            //}

            IReadOnlyList<TimeFrame> t_cloneAHeldTimes;
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(0, m_playerMakeCloneAProgAction.ppHeldTimeFramesForCloneA, this);
            #endregion Asserts
            if (m_farthestTimeWhenStartRewind > m_playerMakeCloneAProgAction.ppHeldTimeFramesForCloneA[^1].startTime)
            {
                // Current recording goes past the beginning of the last pp held frame.
                // Clone A's held times should be the frames from this timeline because we need to be able to properly restrict the final frame's end time.
                t_cloneAHeldTimes = m_cloneAPPHeldTimeFramesWhenStartRewind;
            }
            else
            {
                // Current recording does not include every frame of the stored frames Clone A will hold in the future. We don't need to worry about properly restricting the final frame's end time because we won't make it there.
                t_cloneAHeldTimes = m_playerMakeCloneAProgAction.ppHeldTimeFramesForCloneA;
            }

            // Clone B is holding down their pressure plate, but if there is no overlap between the time frames, it is incorrect.
            foreach (TimeFrame t_cloneBTimeFrame in m_cloneBPPHeldTimeFramesWhenStartRewind)
            {
                TimeFrame t_correctedCloneBTimeFrame = CorrectTimeFrameToAccountForBlinking(t_cloneBTimeFrame);
                foreach (TimeFrame t_cloneATimeFrame in t_cloneAHeldTimes)
                {
                    TimeFrame t_correctedCloneATimeFrame = CorrectTimeFrameToAccountForBlinking(t_cloneATimeFrame);

                    if (t_correctedCloneBTimeFrame.HasOverlap(t_correctedCloneATimeFrame))
                    {
                        #region Logs
                        //CustomDebug.LogForComponent($"{nameof(IsCloneBCorrect)}: Yes. Overlap found between Clone A's {{ {t_correctedCloneATimeFrame} and Cloen B's PP{t_correctedCloneBTimeFrame}.", this, IS_DEBUGGING);
                        #endregion Logs
                        // Overlap found.
                        return true;
                    }
                }
            }
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(IsCloneBCorrect)}: Nope. No overlaps found.", this, IS_DEBUGGING);
            //CustomDebug.LogForContainerElements("CloneA PPFrameList", t_cloneAHeldTimes, IS_DEBUGGING);
            //CustomDebug.LogForContainerElements("CloneB PPFrameList", m_cloneBPPHeldTimeFramesWhenStartRewind, IS_DEBUGGING);
            #endregion Logs
            return false;
        }
        private TimeFrame CorrectTimeFrameToAccountForBlinking(TimeFrame timeFrame)
        {
            //if (timeFrame.endTime == float.PositiveInfinity)
            //{
            //    return new TimeFrame(timeFrame.startTime, m_farthestTimeWhenStartRewind + m_cloneMan.blinkTimeAm);
            //}
            return timeFrame;
        }

        /// <summary>
        /// Starts a coroutine for repeadedly checking if we should advance the conversation or not. If the monitor corout is already running, does not start a new one.
        /// </summary>
        private void BeginMonitoringForFinishConditions(TimeClone cloneB)
        {
            if (m_isMonitorCoroutActive) { return; }
            StartCoroutine(MonitorFinishConditionsCorout(cloneB));
        }
        /// <summary>
        /// Coroutine that reapeadedly checks if we should advacne the conversation. If we should, then calls <see cref="Finish"/>.
        /// </summary>
        private IEnumerator MonitorFinishConditionsCorout(TimeClone cloneB)
        {
            m_isMonitorCoroutActive = true;

            yield return new WaitUntil(() =>
            {
                // If currently rewinding, ignore all other finish conditions.
                if (m_timeRewinder.hasStarted) { return false; }

                // 1. The door opens
                if (m_exitDoor.isOn)
                {
                    #region Logs
                    //CustomDebug.LogForComponent($"Monitoring done: Door is open.", this, IS_DEBUGGING);
                    #endregion Logs
                    return true;
                }
                // 2. Clone B begins to disappear.
                float t_checkTime = m_cloneMan.curTime - CLONE_DISAPPEAR_BUFFER_TIME;
                t_checkTime = Mathf.Max(t_checkTime, 0.0f);
                List<TimeClone> t_clones = m_cloneMan.GetClonesActiveAtTime(t_checkTime);
                if (!t_clones.Contains(cloneB))
                {
                    #region Logs
                    //CustomDebug.LogForComponent($"Monitoring done: Clone B is disappearing.", this, IS_DEBUGGING);
                    #endregion Logs
                    return true;
                }

                // Default is false.
                return false;
            });

            yield return new WaitForFixedUpdate();
            Finish();

            m_isMonitorCoroutActive = false;
        }
        /// <summary>
        /// Marks the action as finished, unsubscribes from things, prepares for next action (pauses time and restricts player movement) and then invokes the onFinished event.
        /// </summary>
        private void Finish()
        {
            m_isFinished = true;

            ToggleSubscriptions(false);

            // Pause time so that SrAgent can talk
            m_timeRewinder.StartRewind();
            // Restrict player movement.
            m_inpMapStack.PopInputMap(m_moveInpMap);
            m_moveSus.SuspendWithUniqueKey(m_suspendMoveUniqueKey);

            m_onFinished?.Invoke();
        }
    }
}
