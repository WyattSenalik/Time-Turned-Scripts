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
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.TwoClone
{
    /// <summary>
    /// Programmed action for the player when after they are prompted to make a clone. Allows the player to make 1 or 2 clones, only advancing when a clone steps on a pressure plate or one of the clones disappears.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerMakeCloneAOrBothClonesProgAction_TwoClone : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;
        public const float CLONE_DISAPPEAR_BUFFER_TIME = 0.5f;

        public MovementSuspender moveSus => m_moveSus;
        public InputMapStack inpMapStack => m_inpMapStack;
        public TimeRewinder timeRewinder => m_timeRewinder;
        public CloneManager cloneMan => m_cloneMan;
        public Door exitDoor => m_exitDoor;
        public SwitchPressurePlateHandler ppHandler1 => m_ppHandler1;
        public SwitchPressurePlateHandler ppHandler2 => m_ppHandler2;
        public CloneCreatedEventIdentifierSO cloneCreatedEventIDSO => m_cloneCreatedEventIDSO;
        public ePressurePlateOption ppHeldByCloneA => m_ppHeldByCloneA;
        public IReadOnlyList<TimeFrame> ppHeldTimeFramesForCloneA => m_ppHeldTimeFramesForCloneA;

        [SerializeField, Required] private MovementSuspender m_moveSus = null;
        [SerializeField, Required] private InputMapStack m_inpMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private CloneManager m_cloneMan = null;
        [SerializeField, Required] private BranchPlayerController m_playerCont = null;

        [SerializeField, Required] private Door m_exitDoor = null;
        [SerializeField, Required] private SwitchPressurePlateHandler m_ppHandler1 = null;
        [SerializeField, Required] private SwitchPressurePlateHandler m_ppHandler2 = null;

        [SerializeField, Required] private BranchConvoAction m_doTwoClonesExistBA = null;
        [SerializeField, Required] private BranchConvoAction m_areBothClonesCorrectBA = null;
        [SerializeField, Required] private BranchConvoAction m_isCloneACorrectBA = null;
        [SerializeField, Required] 
        private CloneCreatedEventIdentifierSO m_cloneCreatedEventIDSO = null;

        [SerializeField] private string m_moveInpMap = "Default";
        [SerializeField] private string m_suspendMoveUniqueKey = "c3014f79a35e47949a27ab1c7d90627d";
        [SerializeField, Tag] private string m_playerTag = "Player";

        private ConditionManager m_condMan = null;
        private SkipButton m_skipButton = null;

        private HoldPressurePlate m_holdPP1 = null;
        private HoldPressurePlate m_holdPP2 = null;

        private Action m_onFinished = null;
        private bool m_isPP1CurHeld = false;
        private bool m_isPP2CurHeld = false;
        private bool m_isPP1HeldWhenStartRewind = false;
        private bool m_isPP2HeldWhenStartRewind = false;
        private GameObject m_gameObjectHoldingPP1WhenStartRewind = null;
        private GameObject m_gameObjectHoldingPP2WhenStartRewind = null;
        private IReadOnlyList<TimeFrame> m_pp1HeldTimeFramesWhenStartRewind = null;
        private IReadOnlyList<TimeFrame> m_pp2HeldTimeFramesWhenStartRewind = null;

        private ePressurePlateOption m_ppHeldByCloneAWhenStartRewind = ePressurePlateOption.Neither;
        private bool m_doesCloneAExist = false;
        private ePressurePlateOption m_ppHeldByCloneA = ePressurePlateOption.Neither;
        private IReadOnlyList<TimeFrame> m_ppHeldTimeFramesForCloneA = null;

        private bool m_areBothClonesCorrectWhenStartRewind = false;
        private ePressurePlateOption m_ppHeldByCloneBWhenStartRewind = ePressurePlateOption.Neither;
        private bool m_doesCloneBExist = false;
        private ePressurePlateOption m_ppHeldByCloneB = ePressurePlateOption.Neither;

        private bool m_isMonitorCoroutActive = false;
        private int? m_cloneAmAtLastCheck = null;

        private bool m_isFinished = false;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_moveSus, nameof(m_moveSus), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inpMapStack, nameof(m_inpMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneMan, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerCont, nameof(m_playerCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_exitDoor, nameof(m_exitDoor), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppHandler1, nameof(m_ppHandler1), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppHandler2, nameof(m_ppHandler2), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_doTwoClonesExistBA, nameof(m_doTwoClonesExistBA), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_areBothClonesCorrectBA, nameof(m_areBothClonesCorrectBA), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_isCloneACorrectBA, nameof(m_isCloneACorrectBA), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedEventIDSO, nameof(m_cloneCreatedEventIDSO), this);
            #endregion Asserts
            m_holdPP1 = m_ppHandler1.GetComponent<HoldPressurePlate>();
            m_holdPP2 = m_ppHandler2.GetComponent<HoldPressurePlate>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_holdPP1, m_ppHandler1.gameObject, this);
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_holdPP2, m_ppHandler2.gameObject, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_condMan = ConditionManager.instance;
            m_skipButton = SkipButton.GetInstanceSafe(this);

            // Set all the conditions are false (to reset if scene was restarted).
            m_condMan.SetCondition(m_doTwoClonesExistBA, false);
            m_condMan.SetCondition(m_areBothClonesCorrectBA, false);
            m_condMan.SetCondition(m_isCloneACorrectBA, false);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ToggleSubscriptions(false);
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;

            // Unfreeze time
            m_timeRewinder.CancelRewind();
            // Let the player move
            m_inpMapStack.SwitchInputMap(m_moveInpMap);
            // Begin time manip to signal player can do stuff now.
            m_playerCont.ForceBeginTimeManipulation(null, true);

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
            // Pressureplates
            m_ppHandler1?.onActivated.ToggleSubscription(OnPressurePlate1Activated, cond);
            m_ppHandler1?.onDeactivated.ToggleSubscription(OnPressurePlate1Deactivated, cond);
            m_ppHandler2?.onActivated.ToggleSubscription(OnPressurePlate2Activated, cond);
            m_ppHandler2?.onDeactivated.ToggleSubscription(OnPressurePlate2Deactivated, cond);

            m_timeRewinder?.onRewindBegin.ToggleSubscription(OnRewindBegin, cond);

            m_cloneCreatedEventIDSO.ToggleSubscriptionToEvent(OnCloneCreated, cond);
        }
        #region EventDriven
        #region PressurePlate Events
        /// <summary>
        /// Called when PP1 is stepped on.
        /// </summary>
        private void OnPressurePlate1Activated()
        {
            m_isPP1CurHeld = true;
        }
        /// <summary>
        /// Called when PP1 is stepped off.
        /// </summary>
        private void OnPressurePlate1Deactivated()
        {
            m_isPP1CurHeld = false;
        }
        /// <summary>
        /// Called when PP2 is stepped on.
        /// </summary>
        private void OnPressurePlate2Activated()
        {
            m_isPP2CurHeld = true;
        }
        /// <summary>
        /// Called when PP2 is stepped off.
        /// </summary>
        private void OnPressurePlate2Deactivated()
        {
            m_isPP2CurHeld = false;
        }
        #endregion PressurePlate Events
        /// <summary>
        /// Called when player pauses time.
        /// </summary>
        private void OnRewindBegin()
        {
            // Hold info about the state of things at the time rewind beings that is vital for determining if the branch conditions are true,
            m_isPP1HeldWhenStartRewind = m_isPP1CurHeld;
            m_isPP2HeldWhenStartRewind = m_isPP2CurHeld;

            if (m_isPP1HeldWhenStartRewind)
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(0, m_holdPP1.heldWindows, this);
                #endregion Asserts
                m_gameObjectHoldingPP1WhenStartRewind = m_holdPP1.heldWindows[^1].holdingObject;
            }
            if (m_isPP2HeldWhenStartRewind)
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(0, m_holdPP2.heldWindows, this);
                #endregion Asserts
                m_gameObjectHoldingPP2WhenStartRewind = m_holdPP2.heldWindows[^1].holdingObject;
            }

            m_ppHeldByCloneAWhenStartRewind = DetermineWhichPPCloneAIsHolding();
            m_ppHeldByCloneBWhenStartRewind = DetermineWhichPPCloneBIsHolding();

            m_pp1HeldTimeFramesWhenStartRewind = m_holdPP1.heldWindows.ConvertToTimeFrameList();
            m_pp2HeldTimeFramesWhenStartRewind = m_holdPP2.heldWindows.ConvertToTimeFrameList();

            // Determine if both clones are correct when rewind starts (because the function AreBothClonesCorrect needs to use data from the pressure plates that will become unavailable when clone is created [aka time rewind and trimmed]). 
            m_areBothClonesCorrectWhenStartRewind = AreBothClonesCorrect();
        }
        /// <summary>
        /// Called when a clone is created.
        /// </summary>
        private void OnCloneCreated(ICloneCreatedContext context)
        {
            // Clone A doesn't exist yet, so it was just made.
            if (!m_doesCloneAExist)
            {
                m_doesCloneAExist = true;
                // Set which pressure plate is being held by Clone A
                m_ppHeldByCloneA = m_ppHeldByCloneAWhenStartRewind;
                // Clone is correct if at least 1 of the pps are held (not neither).
                m_condMan.SetCondition(m_isCloneACorrectBA, m_ppHeldByCloneA != ePressurePlateOption.Neither);

                // Set the pressure plate hold windows for Clone A, for use later in the AreBothClonesCorrect function.
                switch (m_ppHeldByCloneA)
                {
                    case ePressurePlateOption.One:
                    {
                        m_ppHeldTimeFramesForCloneA = new List<TimeFrame>(m_pp1HeldTimeFramesWhenStartRewind);
                        break;
                    }
                    case ePressurePlateOption.Two:
                    {
                        m_ppHeldTimeFramesForCloneA = new List<TimeFrame>(m_pp2HeldTimeFramesWhenStartRewind);
                        break;
                    }
                    case ePressurePlateOption.Neither:
                    {
                        m_ppHeldTimeFramesForCloneA = new List<TimeFrame>();
                        break;
                    }
                    default:
                    {
                        m_ppHeldTimeFramesForCloneA = new List<TimeFrame>();
                        CustomDebug.UnhandledEnum(m_ppHeldByCloneA, this);
                        break;
                    }
                }
            }
            // Clone A already exists, so Clone B was just made.
            else
            {
                m_doesCloneBExist = true;
                // Set which pressure plate is being held by Clone B.
                m_ppHeldByCloneB = m_ppHeldByCloneBWhenStartRewind;

                // Two clones do exist (because we made it here).
                m_condMan.SetCondition(m_doTwoClonesExistBA, true);
                // Both clones are correct only if they both holding down a pressure plate (Clone B cannot be holding same pp as Clone A) at the same time. This was determined on most recent pause (OnRewindBegin) by AreBothClonesCorrect function.
                m_condMan.SetCondition(m_areBothClonesCorrectBA, m_areBothClonesCorrectWhenStartRewind);
            }

            // Monitor finish conditions (if the pressure plate that is a clone owns is activated or a clone disappears) (won't start a new monitor if one already exists).
            BeginMonitoringForFinishConditions();
        }
        #endregion EventDriven

        /// <summary>
        /// Starts a coroutine for repeadedly checking if we should advance the conversation or not. If the monitor corout is already running, does not start a new one.
        /// </summary>
        private void BeginMonitoringForFinishConditions()
        {
            if (m_isMonitorCoroutActive) { return; }
            StartCoroutine(MonitorFinishConditionsCorout());
        }
        /// <summary>
        /// Coroutine that reapeadedly checks if we should advacne the conversation. If we should, then calls <see cref="Finish"/>.
        /// </summary>
        private IEnumerator MonitorFinishConditionsCorout()
        {
            m_isMonitorCoroutActive = true;

            yield return new WaitUntil(() =>
            {
                // If currently rewinding, ignore all other finish conditions.
                if (m_timeRewinder.hasStarted) { return false; }

                if (m_doesCloneAExist)
                {
                    if (m_doesCloneBExist && m_areBothClonesCorrectBA)
                    {
                        // 1. If both clones are correct, then wait for the door to turn on.
                        if (m_exitDoor.isOn)
                        {
                            #region Logs
                            //CustomDebug.LogForComponent($"Monitoring done: Clone B exists and the door is open.", this, IS_DEBUGGING);
                            #endregion Logs
                            return true;
                        }
                        // 1a. If both clones are correct, then don't do any of the other things, and just wait for the door to turn on (handled with the elses).
                    }
                    // 2. Clone A is holding down their pressure plate (or the player is, which is fine too).
                    else if (IsCloneOwnedPressurePlateCurrentlyHeldDown(m_ppHeldByCloneA))
                    {
                        #region Logs
                        //CustomDebug.LogForComponent($"Monitoring done: Clone A's pp is being held down ({m_ppHeldByCloneA}).", this, IS_DEBUGGING);
                        #endregion Logs
                        return true;
                    }
                    else
                    {
                        // 3. A clone begins to disappear
                        // Giving the clones a little wiggle room so that the player can see the clone start to fade away.
                        float t_checkTime = m_cloneMan.curTime - CLONE_DISAPPEAR_BUFFER_TIME;
                        t_checkTime = Mathf.Max(t_checkTime, 0.0f);
                        int t_cloneAmNow = m_cloneMan.GetAmountClonesActiveAtTime(t_checkTime);
                        if (m_cloneAmAtLastCheck.HasValue && t_cloneAmNow < m_cloneAmAtLastCheck.Value)
                        {
                            #region Logs
                            //CustomDebug.LogForComponent($"Monitoring done: Clone amount now ({t_cloneAmNow}) is less than clone amount before ({m_cloneAmAtLastCheck}).", this, IS_DEBUGGING);
                            #endregion Logs
                            return true;
                        }
                        m_cloneAmAtLastCheck = t_cloneAmNow;
                    }
                }

                return false;
            });

            yield return new WaitForFixedUpdate();
            Finish();

            m_isMonitorCoroutActive = false;
        }
        /// <summary>
        /// Checks if the given pressure plate is being held down currently.
        /// </summary>
        private bool IsCloneOwnedPressurePlateCurrentlyHeldDown(ePressurePlateOption cloneOwnedPP)
        {
            switch (cloneOwnedPP)
            {
                case ePressurePlateOption.One: return m_isPP1CurHeld;
                case ePressurePlateOption.Two: return m_isPP2CurHeld;
                case ePressurePlateOption.Neither: return false;
                default:
                {
                    CustomDebug.UnhandledEnum(cloneOwnedPP, this);
                    return false;
                }
            }
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

        /// <summary>
        /// Assumes that this is only called when no clones exist but clone a may be created shortly.
        /// </summary>
        private ePressurePlateOption DetermineWhichPPCloneAIsHolding()
        {
            // Set which pressure plate is being held by clone A
            if (m_isPP1HeldWhenStartRewind)
            {
                return ePressurePlateOption.One;
            }
            else if (m_isPP2HeldWhenStartRewind)
            {
                return ePressurePlateOption.Two;
            }
            else
            {
                return ePressurePlateOption.Neither;
            }
        }
        /// <summary>
        /// Assumes that this is only called when 1 clones exist but clone b may be created shortly.
        /// </summary>
        private ePressurePlateOption DetermineWhichPPCloneBIsHolding()
        {
            switch (m_ppHeldByCloneA)
            {
                case ePressurePlateOption.One:
                {
                    // Check if pp 2 is held by Clone B.
                    if (m_isPP2HeldWhenStartRewind)
                    {
                        // Make sure that its the player holding down the pressure plate and not Clone A.
                        if (IsGameObjectHoldingDownPPThePlayer(m_gameObjectHoldingPP2WhenStartRewind))
                        {
                            return ePressurePlateOption.Two;
                        }
                    }
                    return ePressurePlateOption.Neither;
                }
                case ePressurePlateOption.Two:
                {
                    // Check if pp 1 is held by Clone B.
                    if (m_isPP1HeldWhenStartRewind)
                    {
                        // Make sure that its the player holding down the pressure plate and not Clone A.
                        if (IsGameObjectHoldingDownPPThePlayer(m_gameObjectHoldingPP1WhenStartRewind))
                        {
                            return ePressurePlateOption.One;
                        }
                    }
                    return ePressurePlateOption.Neither;
                }
                case ePressurePlateOption.Neither:
                {
                    // Check if either is held by clone B.
                    if (m_isPP1HeldWhenStartRewind && IsGameObjectHoldingDownPPThePlayer(m_gameObjectHoldingPP1WhenStartRewind))
                    {
                        return ePressurePlateOption.One;
                    }
                    else if (m_isPP2HeldWhenStartRewind && IsGameObjectHoldingDownPPThePlayer(m_gameObjectHoldingPP2WhenStartRewind))
                    {
                        return ePressurePlateOption.Two;
                    }
                    else
                    {
                        return ePressurePlateOption.Neither;
                    }
                }
                default:
                {
                    CustomDebug.UnhandledEnum(m_ppHeldByCloneA, this);
                    return ePressurePlateOption.Neither;
                }
            }
        }
        /// <summary>
        /// Pulls a Collider2D off the given object and checks if it's attachedRigidbody is of the player tag.
        /// </summary>
        private bool IsGameObjectHoldingDownPPThePlayer(GameObject gameObjectHoldingPP)
        {
            Collider2D t_holdingCol = gameObjectHoldingPP.GetComponent<Collider2D>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(t_holdingCol, gameObjectHoldingPP, this);
            #endregion Asserts
            Rigidbody2D t_attRb = t_holdingCol.attachedRigidbody;
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(t_attRb != null, $"Collider {t_holdingCol} to have an attached rb ({t_attRb}).", this);
            #endregion Asserts
            return t_attRb.CompareTag(m_playerTag);
        }
        /// <summary>
        /// Checks if both clones are holding down pressure plates and that there is overlap between when they hold down their respective pressure plates.
        /// 
        /// Assumes that this is only called prior to the 2nd clone actually being created.
        /// </summary>
        private bool AreBothClonesCorrect()
        {
            // If one of the clones isn't even holding a pressure plate, they're wrong.
            // No need to check if they are holding different pressure plates because the function that sets m_ppHeldByCloneB does not allow that to happen.
            if (m_ppHeldByCloneA == ePressurePlateOption.Neither || m_ppHeldByCloneBWhenStartRewind == ePressurePlateOption.Neither)
            {
                #region Logs
                //CustomDebug.LogForComponent($"{nameof(AreBothClonesCorrect)}: Nope. One clone is not on pressure plate. Clone A ({m_ppHeldByCloneA}). Clone B ({m_ppHeldByCloneBWhenStartRewind}).", this, IS_DEBUGGING);
                #endregion Logs
                return false;
            }

            // Determine which stored held times are for the pp that clone b is holding down.
            IReadOnlyList<TimeFrame> t_ppHeldTimeFramesForCloneB;
            switch (m_ppHeldByCloneBWhenStartRewind)
            {
                case ePressurePlateOption.One:
                    t_ppHeldTimeFramesForCloneB = m_pp1HeldTimeFramesWhenStartRewind;
                    break;
                case ePressurePlateOption.Two:
                    t_ppHeldTimeFramesForCloneB = m_pp2HeldTimeFramesWhenStartRewind;
                    break;
                case ePressurePlateOption.Neither:
                    t_ppHeldTimeFramesForCloneB = new List<TimeFrame>();
                    break;
                default:
                    t_ppHeldTimeFramesForCloneB = new List<TimeFrame>();
                    CustomDebug.UnhandledEnum(m_ppHeldByCloneBWhenStartRewind, this);
                    break;
            }

            // Compare each time frame to find overlap (could probably do this more efficiently by making the assumption that the time frames are sorted in chronological order (which they should be) and then incrementing them side by side).
            foreach (TimeFrame t_pp1Frame in m_ppHeldTimeFramesForCloneA)
            {
                // If one of the held windows ends at the current time, the clone will actually continue to stand on it, so take that into account.
                // This heavily assumes that time is paused.
                TimeFrame t_correctedPP1Frame = CorrectTimeFrameToAccountForBlinking(t_pp1Frame);
                foreach (TimeFrame t_pp2Frame in t_ppHeldTimeFramesForCloneB)
                {
                    // Same check with the other time frame.
                    TimeFrame t_correctedPP2Frame = CorrectTimeFrameToAccountForBlinking(t_pp2Frame);

                    if (t_correctedPP1Frame.HasOverlap(t_correctedPP2Frame))
                    {
                        #region Logs
                        //CustomDebug.LogForComponent($"{nameof(AreBothClonesCorrect)}: Yes. Overlap found between PP1 {t_correctedPP1Frame} and PP2 {t_correctedPP2Frame}.", this, IS_DEBUGGING);
                        #endregion Logs
                        // Found an overlap.
                        return true;
                    }
                }
            }
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(AreBothClonesCorrect)}: Nope. No overlaps found.", this, IS_DEBUGGING);
            //CustomDebug.LogForContainerElements("CloneA PPFrameList", m_ppHeldTimeFramesForCloneA, IS_DEBUGGING);
            //CustomDebug.LogForContainerElements("CloneB PPFrameList", t_ppHeldTimeFramesForCloneB, IS_DEBUGGING);
            #endregion Logs
            // None of the held windows overlap.
            return false;
        }
        /// <summary>
        /// Assumes that time is paused. If the time frame's endTime is greater than the farthestTime reached, sets the end time to the farthest time plus the clone blink time.
        /// </summary>
        private TimeFrame CorrectTimeFrameToAccountForBlinking(TimeFrame timeFrame)
        {
            //if (timeFrame.endTime == float.PositiveInfinity)
            //{
            //    return new TimeFrame(timeFrame.startTime, m_timeRewinder.farthestTime + m_cloneMan.blinkTimeAm);
            //}
            return timeFrame;
        }


        public enum ePressurePlateOption { Neither, One, Two }
    }
}