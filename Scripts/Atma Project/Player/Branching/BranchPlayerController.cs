using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Extension;

using NaughtyAttributes;

using Atma.Events;
using Atma.UI;
using Timed;
using Helpers.Events;
using Helpers.Events.GameEventSystem;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Player controller for rewinding time and creating time clones.
    /// Assumes it is attached to the player.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CloneManager))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(InputMapStack))]
    [RequireComponent(typeof(ITimeRewinder))]
    public sealed class BranchPlayerController : MonoBehaviour
    {
        public const float RESUME_BUFFER_TIME = 0.15f;
        private const bool IS_DEBUGGING = false;

        public GameObject playerInstance => gameObject;
        public CloneManager cloneManager => m_cloneManager;
        public ITimeRewinder timeRewinder => m_timeRewinder;
        public float timeUntilIncreaseNavSpeed => m_timeUntilIncreaseNavSpeed;
        public float maxRewindSpeed => m_maxRewindSpeed;
        public int timeManipNavVal => m_timeManipNavVal;
        public bool isFirstPause { get; private set; } = false;
        public bool isTransitionToStopwatchPlaying { get; private set; } = false;
        public bool isTransitionToTimelinePlaying { get; private set; } = false;
        public bool wasTransitionPlayedWhenManipulatingTime { get; private set; } = false;

        public IEventPrimer onBeginTimeManip => m_onBeginTimeManip;
        public IEventPrimer onEndTimeManip => m_onEndTimeManip;
        public IEventPrimer<FailResumeContext> onFailedToEndTimeManip => m_onFailedToEndTimeManip;
        public IEventPrimer onBeginRewinding => m_onBeginRewinding;
        public IEventPrimer<FailResumeContext> onFailedToCreateClone => m_onFailedToCreateClone;
        public IEventPrimer onTransitionToTimelineStarted => m_onTransitionToTimelineStarted;
        public IEventPrimer onRewindNavVelocity => m_onRewindNavVelocity;
        public IEventPrimer onFastForwardNavVelocity => m_onFastForwardNavVelocity;

        [SerializeField, Required] private StopwatchTimelineAnimationController m_stopwatchTimelineAnimCont = null;
        [SerializeField, Required] private TimeFreezeEffectActivator m_timeFreezeEffectActivator = null;

        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneCreatedEventIdentifierSO m_cloneCreatedEventID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private PlayerTimeManipEndEventIdentifierSO m_playerTimeManipEndedID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneDiedEventIdentifierSO m_cloneDiedEventID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private IneptusDiedEventIdentifierSO m_inptusDiedEventID = null;

        [SerializeField] private string m_timeManipInputMap = "TimeManipulation";
        [SerializeField] private string m_transitioningInputMap = "Transitioning";
        [SerializeField] private string m_restrictedInputMap = "Restricted";
        [SerializeField, Min(0.01f)] private float m_timeUntilIncreaseNavSpeed = 1.0f;
        [SerializeField, Min(1)] private int m_maxRewindSpeed = 4;
        [SerializeField] private MixedEvent m_onBeginTimeManip = new MixedEvent();
        [SerializeField] private MixedEvent m_onEndTimeManip = new MixedEvent();
        [SerializeField] private MixedEvent<FailResumeContext> m_onFailedToEndTimeManip = new MixedEvent<FailResumeContext>();
        [SerializeField] private MixedEvent<FailResumeContext> m_onFailedToCreateClone = new MixedEvent<FailResumeContext>();
        [SerializeField] private MixedEvent m_onBeginRewinding = new MixedEvent();
        [SerializeField] private MixedEvent m_onTransitionToTimelineStarted = new MixedEvent();
        [SerializeField] private MixedEvent m_onRewindNavVelocity = new MixedEvent();
        [SerializeField] private MixedEvent m_onFastForwardNavVelocity = new MixedEvent();

        [SerializeField, Required, BoxGroup("Sound")] private UIntReference m_playTimeCloneCreateLoopEventID = null;
        [SerializeField, Required, BoxGroup("Sound")] private UIntReference m_stopTimeCloneCreateLoopEventID = null;
        [SerializeField, Required, BoxGroup("Sound")] private UIntReference m_playTimeCloneCreateInEventID = null;
        [SerializeField, Min(0.0f), BoxGroup("Sound")] private float m_timeBeforeEndToEndLoop = 1.248f;


        // Why is this in here and not CloneManager? Because need to trim player data
        // after waiting a frame for the clone to set itself up.
        private IGameEvent<ICloneCreatedContext> m_cloneCreatedEvent = null;
        private IGameEvent<IPlayerTimeManipEndContext> m_playerTimeManipEndedEvent = null;

        private GlobalTimeManager m_globalTimeMan = null;
        private LevelOptions m_levelOptions = null;
        private UISoundController m_uiSoundCont = null;

        private CloneManager m_cloneManager = null;
        private PlayerStateManager m_playerStateMan = null;
        private ITimeRewinder m_timeRewinder = null;
        private InputMapStack m_inpMapStack = null;
        private ITimedObject m_playerTimedObject = null;

        private SubManager<ICloneDiedContext> m_cloneDiedSubMan = null;
        private SubManager<IIneptusDiedContext> m_ineptusDiedSubMan = null;

        private int m_timeManipNavVal = 0;
        private ICloneDiedContext m_recentCloneDeathContext = null;

        private IIneptusDiedContext m_recentIneptusDeathContext = null;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopwatchTimelineAnimCont, nameof(m_stopwatchTimelineAnimCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeFreezeEffectActivator, nameof(m_timeFreezeEffectActivator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedEventID, nameof(m_cloneCreatedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerTimeManipEndedID, nameof(m_playerTimeManipEndedID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDiedEventID, nameof(m_cloneDiedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playTimeCloneCreateLoopEventID, nameof(m_playTimeCloneCreateLoopEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopTimeCloneCreateLoopEventID, nameof(m_stopTimeCloneCreateLoopEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playTimeCloneCreateInEventID, nameof(m_playTimeCloneCreateInEventID), this);
            #endregion Asserts
            m_cloneManager = GetComponent<CloneManager>();
            m_playerStateMan = GetComponent<PlayerStateManager>();
            m_timeRewinder = GetComponent<ITimeRewinder>();
            m_inpMapStack = GetComponent<InputMapStack>();
            m_playerTimedObject = GetComponent<ITimedObject>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_cloneManager, this);
            //CustomDebug.AssertComponentIsNotNull(m_playerStateMan, this);
            //CustomDebug.AssertIComponentIsNotNull(m_timeRewinder, this);
            //CustomDebug.AssertComponentIsNotNull(m_inpMapStack, this);
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_playerTimedObject, gameObject, this);
            #endregion Asserts

            m_playerTimeManipEndedEvent = m_playerTimeManipEndedID.CreateEvent();
            m_cloneCreatedEvent = m_cloneCreatedEventID.CreateEvent();
        }
        private void Start()
        {
            m_globalTimeMan = GlobalTimeManager.instance;
            m_levelOptions = LevelOptions.GetInstanceSafe(this);
            m_uiSoundCont = UISoundController.GetInstanceSafe(this);

            m_cloneDiedSubMan = new SubManager<ICloneDiedContext>(m_cloneDiedEventID, OnCloneDied);
            m_cloneDiedSubMan.Subscribe();

            m_ineptusDiedSubMan = new SubManager<IIneptusDiedContext>(m_inptusDiedEventID, OnIneptusDied);
            m_ineptusDiedSubMan.Subscribe();

            m_timeRewinder.onBeginReached.ToggleSubscription(OnTimeRewinderBeginReached, true);
            m_timeRewinder.onEndReached.ToggleSubscription(OnTimeRewinderEndReached, true);
        }
        private void OnDestroy()
        {
            m_playerTimeManipEndedEvent.DeleteEvent();
            m_cloneCreatedEvent.DeleteEvent();

            m_cloneDiedSubMan.Unsubscribe();

            m_ineptusDiedSubMan.Unsubscribe();

            m_timeRewinder.onBeginReached.ToggleSubscription(OnTimeRewinderBeginReached, false);
            m_timeRewinder.onEndReached.ToggleSubscription(OnTimeRewinderEndReached, false);
        }


        public void ForceBeginTimeManipulation(float? farthestTime = null, bool isFirstPause = false)
        {
            // Make sure not already in time manip
            if (m_timeRewinder.hasStarted) { return; }

            this.isFirstPause = isFirstPause;
            BeginTransitionToBeginTimeManipulation(farthestTime);
        }
        public bool TryEndTimeManipulation()
        {
            // If can't resume now, 
            if (!CanResumeNow(out FailResumeContext t_failContext))
            {
                #region Logs
                //CustomDebug.LogForComponent($"Failed to resume. Reason ({t_failContext.reason}).", this, IS_DEBUGGING);
                #endregion Logs
                m_onFailedToEndTimeManip.Invoke(t_failContext);
                return false;
            }

            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_timeRewinder.hasStarted, $"time to be being manipulated before {nameof(TryEndTimeManipulation)}.", this);
            #endregion Asserts
            // Trim the future if the player hasn't gone back to the farthest time.
            if (m_playerTimedObject.curTime < m_playerTimedObject.farthestTime)
            {
                TrimFuture();
            }
            // Then end time manipulation.
            BeginTransitionToEndTimeManipulation();

            #region Logs
            //CustomDebug.LogForComponent($"Resume succeeded.", this, IS_DEBUGGING);
            #endregion Logs
            return true;
        }
        public void PauseNavVelocity()
        {
            m_timeManipNavVal = 0;
            m_timeRewinder.ChangeNavigationDirection(m_timeManipNavVal);

            // Get rid of increase speed calls
            CancelInvoke();

            // Stop playing rewind sound
            m_uiSoundCont.StopRewindSound();
        }
        public void RewindNavVelocity()
        {
            if (m_timeManipNavVal >= 0.0f)
            {
                m_timeManipNavVal = -1;
                // Auto increase speed after a wait time
                string t_funcName = nameof(RewindNavVelocity);
                CancelInvoke(t_funcName);
                InvokeRepeating(t_funcName, m_timeUntilIncreaseNavSpeed, m_timeUntilIncreaseNavSpeed);

                // Start playing rewind sound
                m_uiSoundCont.PlayRewindSound();

                m_onBeginRewinding.Invoke();
            }
            else
            {
                m_timeManipNavVal -= 1;
                m_timeManipNavVal = Mathf.Max(m_timeManipNavVal, -m_maxRewindSpeed);
            }
            m_timeRewinder.ChangeNavigationDirection(m_timeManipNavVal);

            m_onRewindNavVelocity.Invoke();
        }
        public void FastForwardNavVelocity()
        {
            if (m_timeManipNavVal <= 0.0f)
            {
                m_timeManipNavVal = 1;
                // Auto increase speed after a wait time
                string t_funcName = nameof(FastForwardNavVelocity);
                CancelInvoke(t_funcName);
                InvokeRepeating(t_funcName, m_timeUntilIncreaseNavSpeed, m_timeUntilIncreaseNavSpeed);

                // Start playing rewind sound
                m_uiSoundCont.PlayRewindSound();
            }
            else
            {
                m_timeManipNavVal += 1;
                m_timeManipNavVal = Mathf.Min(m_timeManipNavVal, m_maxRewindSpeed);
            }
            m_timeRewinder.ChangeNavigationDirection(m_timeManipNavVal);

            m_onFastForwardNavVelocity.Invoke();
        }
        public void CreateTimeCloneAtCurrentTime()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_timeRewinder.hasStarted, $"time to be being manipulated before {nameof(CreateTimeCloneAtCurrentTime)}.", this);
            #endregion Asserts

            if (Mathf.Abs(m_playerTimedObject.curTime - m_timeRewinder.earliestTime) < 0.1f) { return; }

            float t_cloneStartTime = 0.0f;
            float t_cloneFarTime = m_playerTimedObject.farthestTime;
            // If we've reached max clone capacity, then don't create another time clone.
            int t_activeCloneAmount = m_cloneManager.GetAmountClonesInTimeFrame(t_cloneStartTime, t_cloneFarTime);
            if (t_activeCloneAmount >= m_levelOptions.maxCloneCharges)
            {
                m_onFailedToCreateClone.Invoke(new FailResumeContext(FailResumeContext.eFailResumeReason.OutOfCloneCharges));
                return;
            }

            // Get rid of increase speed calls
            CancelInvoke();

            // Fully rewind, and then spawn clone.
            PlayFullRewindAnimation(() =>
            {
                // Spawn the clone.
                TimeClone t_spawnedClone = m_cloneManager.SpawnClone(t_cloneStartTime, t_cloneFarTime);
                // Wait until the end of the frame before trimming the player's data
                // because we need to let the clone set itself up using that data first.
                StartCoroutine(TrimPlayerRecordingAfterCloneCreatedCorout(t_spawnedClone));
                // Resume normal play.
                BeginTransitionToEndTimeManipulation();
            });
        }
        public void EndTransitionEarly()
        {
            isTransitionToTimelinePlaying = false;
            isTransitionToStopwatchPlaying = false;

            m_stopwatchTimelineAnimCont.SkipToEndOfAnimation();
            m_timeFreezeEffectActivator.ManuallyEndEffect();
        }


        private bool CanResumeNow() => CanResumeNow(out _);
        private bool CanResumeNow(out FailResumeContext failContext)
        {
            float t_curTime = m_playerTimedObject.curTime;
            // End of default state and start of dead state overlap, so be end exclusive
            float t_deathCheckTime;
            if (t_curTime <= m_timeRewinder.earliestTime)
            {
                //CustomDebug.Log($"CurTime ({t_curTime}) is before earliest time ({m_timeRewinder.earliestTime})", IS_DEBUGGING);
                t_deathCheckTime = t_curTime;
            }
            else
            {
                t_deathCheckTime = t_curTime + RESUME_BUFFER_TIME;
                // DO NOT DO A Mathf.Min because it breaks it. It handles if time is greater to mean its dead, so its okay.
                //CustomDebug.Log($"CurTime ({t_curTime}) is after earliest time ({m_timeRewinder.earliestTime}). OG CheckTime {t_curTime + RESUME_BUFFER_TIME}.", IS_DEBUGGING);
            }
            WindowData<ePlayerState> t_resumeStateWindow = m_playerStateMan.windowCollection.GetWindow(t_deathCheckTime, eTimeFrameContainsOption.EndExclusive);
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(t_resumeStateWindow != null, $"resume state window at {t_curTime} to not be null", this);
            #endregion Asserts

            // Can't resume now if the player has started dying.
            if (t_resumeStateWindow.data == ePlayerState.Dead)
            {
                failContext = new FailResumeContext(FailResumeContext.eFailResumeReason.Dead);
                return false;
            }
            // Can't resume if a clone is about to die.
            if (m_recentCloneDeathContext != null &&
                m_recentCloneDeathContext.deathTime <= t_curTime)
            {
                failContext = new FailResumeContext(FailResumeContext.eFailResumeReason.CloneDead);
                return false;
            }
            // Can't resume if out of time.
            if (!m_levelOptions.noTimeLimit && t_curTime >= m_levelOptions.time)
            {
                failContext = new FailResumeContext(FailResumeContext.eFailResumeReason.OutOfTime);
                return false;
            }
            // Can't resume if ineptus died.
            if (m_recentIneptusDeathContext != null && m_recentIneptusDeathContext.deathTime <= t_curTime)
            {
                failContext = new FailResumeContext(FailResumeContext.eFailResumeReason.IneptusDead);
                return false;
            }

            #region Logs
            //CustomDebug.LogForComponent($"Resuming with state {t_resumeStateWindow.data} in window ({t_resumeStateWindow.window})", this, IS_DEBUGGING);
            #endregion Logs
            failContext = null;
            return true;
        }

        public void ForceBeginTimeManipulationWithoutTransitioningToTimeline()
        {
            // Make sure not already in time manip
            if (m_timeRewinder.hasStarted) { return; }
            wasTransitionPlayedWhenManipulatingTime = false;
            // Start rewinding
            m_timeRewinder.StartRewind();

            m_timeFreezeEffectActivator.StartEffect();
            m_onBeginTimeManip.Invoke();
        }
        public bool TryEndTimeManipulationWithoutTransitioningToStopwatch()
        {
            wasTransitionPlayedWhenManipulatingTime = false;
            // If can't resume now, 
            if (!CanResumeNow(out FailResumeContext t_failContext))
            {
                #region Logs
                //CustomDebug.LogForComponent($"Failed to resume. Reason ({t_failContext.reason}).", this, IS_DEBUGGING);
                #endregion Logs
                //m_onFailedToEndTimeManip.Invoke(t_failContext);
                return false;
            }

            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_timeRewinder.hasStarted, $"time to be being manipulated before {nameof(TryEndTimeManipulation)}.", this);
            #endregion Asserts
            // Trim the future if the player hasn't gone back to the farthest time.
            if (m_playerTimedObject.curTime < m_playerTimedObject.farthestTime)
            {
                TrimFuture();
            }

            // Then end time manipulation.
            EndTimeManipulation(false);
            m_timeFreezeEffectActivator.ManuallyEndEffect();
            // Reset the nav value.
            PauseNavVelocity();
            // Get rid of increase speed calls
            CancelInvoke();

            #region Logs
            //CustomDebug.LogForComponent($"Resume succeeded.", this, IS_DEBUGGING);
            #endregion Logs
            return true;
        }


        private void BeginTransitionToBeginTimeManipulation(float? explicitFarthestTime = null)
        {
            isTransitionToTimelinePlaying = true;
            // Swap input map
            m_inpMapStack.SwitchInputMap(m_transitioningInputMap);

            // Farthest time is optionally given to be some specific value, otherwise (in most cases, its just the player's farthest time.
            float t_farthestTime = m_playerTimedObject.farthestTime;
            if (explicitFarthestTime.HasValue)
            {
                //t_farthestTime = Mathf.Min(t_farthestTime, explicitFarthestTime.Value);
                t_farthestTime = explicitFarthestTime.Value;
            }
            // Start rewinding
            m_timeRewinder.StartRewind(m_timeManipNavVal, t_farthestTime);

            // Start the animation for becoming the timeline
            wasTransitionPlayedWhenManipulatingTime = true;
            m_stopwatchTimelineAnimCont.StartTransitionToTimeline(BeginTimeManipulation);

            m_timeFreezeEffectActivator.StartEffect();

            m_onTransitionToTimelineStarted.Invoke();
        }
        private void BeginTimeManipulation()
        {
            isTransitionToTimelinePlaying = false;

            // Remove the transitioning input map
            m_inpMapStack.PopInputMap(m_transitioningInputMap);
            // Swap input map
            m_inpMapStack.SwitchInputMap(m_timeManipInputMap);

            m_onBeginTimeManip.Invoke();
        }
        private void BeginTransitionToEndTimeManipulation()
        {
            isTransitionToStopwatchPlaying = true;

            // Remove the time manip input map
            m_inpMapStack.PopInputMap(m_timeManipInputMap);
            // Change to the transitioning input map
            m_inpMapStack.SwitchInputMap(m_transitioningInputMap);

            // Start the animation for becoming the stopwatch, at the end of the animation, end time manipulation.
            wasTransitionPlayedWhenManipulatingTime = true;
            m_stopwatchTimelineAnimCont.StartTransitionToStopwatch(EndTimeManipulation);

            m_timeFreezeEffectActivator.ManuallyEndEffect();
            // Reset the nav value.
            PauseNavVelocity();
            // Get rid of increase speed calls
            CancelInvoke();
        }
        private void EndTimeManipulation() => EndTimeManipulation(true);
        private void EndTimeManipulation(bool switchInputMap)
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(EndTimeManipulation), this, IS_DEBUGGING);
            #endregion Logs
            isTransitionToStopwatchPlaying = false;

            // In case the was paused because a clone died, reset the death context.
            ResetCloneDeathContext();
            ResetIneptusDeathContext();

            // Cancel time manip.
            m_timeRewinder.CancelRewind();
            if (switchInputMap)
            {
                // Swap input map back
                m_inpMapStack.PopInputMap(m_transitioningInputMap);
            }
            // Invoke the end time manip events
            m_onEndTimeManip.Invoke();
            m_playerTimeManipEndedEvent.Invoke(new PlayerTimeManipEndContext(m_globalTimeMan.curTime));
        }
        private void TrimFuture()
        {
            float t_curTime = m_playerTimedObject.curTime;
            // Destroy clones that were created after the current time
            cloneManager.DestroyClonesAfterTime(t_curTime);
            // Trim only the player instance.
            m_playerTimedObject.TrimDataAfter(t_curTime);

            // We used to trim everything with
            //      m_globalTimeMan.TrimDataFromAll
            // but this involved problems. Now everything ONLY holds data for rewinding
            // purposes and ALL TimedObjects are marked as AlwaysRecordWhenShould,
            // which will automatically trim data if the current time exists
            // before that data.
        }
        private IEnumerator TrimPlayerRecordingAfterCloneCreatedCorout(TimeClone spawnedClone)
        {
            // Wait until the end of the frame before trimming the player's data
            // because we need to let the clone set itself up using that data first.
            yield return new WaitForEndOfFrame();

            m_playerTimedObject.TrimDataAfter(m_playerTimedObject.curTime);
            // Invoke the clone created event as well.
            m_cloneCreatedEvent.Invoke(new CloneCreatedContext(m_playerTimedObject.curTime, spawnedClone));
        }


        /// <summary>
        /// Causes time to fully rewind w/ an animation and then invokes the given callback.
        /// </summary>
        private void PlayFullRewindAnimation(Action onAnimFin)
        {
            // Change the input map so the player can't input anything.
            m_inpMapStack.SwitchInputMap(m_restrictedInputMap);
            // Begin the full rewind.
            RewindNavVelocity();
            //m_timeRewinder.ChangeNavigationDirection(-m_cloneRewindSpeed);

            StartCoroutine(FullRewindingCoroutine(onAnimFin));
        }
        private IEnumerator FullRewindingCoroutine(Action onAnimFin)
        {
            // Calculate how long it will take for the rewind to finish so we can play the end 
            float t_realTimeItWillTake = 0.0f;
            float t_fakeCurTime = m_timeRewinder.curTime;

            int t_amountIncreases = 1;
            float t_remainingTime = t_fakeCurTime - m_timeRewinder.earliestTime;
            while (t_remainingTime > t_amountIncreases * m_timeUntilIncreaseNavSpeed && t_amountIncreases < m_maxRewindSpeed)
            {
                t_remainingTime -= t_amountIncreases * m_timeUntilIncreaseNavSpeed;
                ++t_amountIncreases;
                t_realTimeItWillTake += 1.0f;
            }
            t_realTimeItWillTake += t_remainingTime / t_amountIncreases;
            float t_timeToWaitUntilStoppingLoop = t_realTimeItWillTake - m_timeBeforeEndToEndLoop;
            t_timeToWaitUntilStoppingLoop = Mathf.Max(0.0f, t_timeToWaitUntilStoppingLoop);
            #region Logs
            //CustomDebug.LogForComponent($"TimeToWaitUntilStopLoop=({t_timeToWaitUntilStoppingLoop}). curTime=({t_fakeCurTime}). earliestTime=({m_timeRewinder.earliestTime}). AmountIncreases=({t_amountIncreases})", this, IS_DEBUGGING);
            #endregion Logs

            if (t_timeToWaitUntilStoppingLoop <= 0.0f)
            {
                AkSoundEngine.PostEvent(m_playTimeCloneCreateInEventID.value, gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent(m_playTimeCloneCreateLoopEventID.value, gameObject);
                yield return new WaitForSeconds(t_timeToWaitUntilStoppingLoop);
                AkSoundEngine.PostEvent(m_stopTimeCloneCreateLoopEventID.value, gameObject);
                AkSoundEngine.PostEvent(m_playTimeCloneCreateInEventID.value, gameObject);
            }


            // Wait until the fully rewound to earliest time.
            yield return new WaitUntil(() => m_playerTimedObject.curTime <= m_timeRewinder.earliestTime);

            // Undo what we did before.
            m_timeRewinder.ChangeNavigationDirection(0);
            m_inpMapStack.PopInputMap(m_restrictedInputMap);

            // Invoke finished callback
            onAnimFin?.Invoke();
        }

        #region GameEventDriven
        private void OnCloneDied(ICloneDiedContext context)
        {
            #region Logs
            //CustomDebug.LogForComponent("Player received that clone died", this, IS_DEBUGGING);
            #endregion Logs
            m_recentCloneDeathContext = context;
            // When a clone dies, we want to pause time.
            ForceBeginTimeManipulation();
        }
        private void OnIneptusDied(IIneptusDiedContext context)
        {
            m_recentIneptusDeathContext = context;
            // Pause when ineptus dies
            ForceBeginTimeManipulation();
        }
        #endregion GameEventDriven

        #region Time Rewinder Event Driven
        private void OnTimeRewinderBeginReached()
        {
            // When the beginning is reached, reset the nav velocity
            PauseNavVelocity();
        }
        private void OnTimeRewinderEndReached()
        {
            // When the end is reached, reset the nav velocity
            PauseNavVelocity();
        }
        #endregion Time Rewinder Event Driven

        /// <summary>
        /// Sets clone death context to null to get rid of any context it may be holding.
        /// Needs to be reset like this for when time is resumed so that once a clone dies
        /// once, the game doesn't always think a clone is dead.
        /// </summary>
        private void ResetCloneDeathContext()
        {
            m_recentCloneDeathContext = null;
        }
        private void ResetIneptusDeathContext()
        {
            m_recentIneptusDeathContext = null;
        }
    }

    public class FailResumeContext
    {
        public eFailResumeReason reason { get; private set; }


        public FailResumeContext(eFailResumeReason reason)
        {
            this.reason = reason;
        }


        public enum eFailResumeReason { Dead, CloneDead, OutOfTime, OutOfCloneCharges, IneptusDead }
    }
}