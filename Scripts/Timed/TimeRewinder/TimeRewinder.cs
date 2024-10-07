using System.Collections;
using UnityEngine;

using Helpers;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Implementation of <see cref="ITimeRewinder"/>.
    /// Controls time directly using the <see cref="GlobalTimeManager"/>.
    /// Never have two <see cref="TimeRewinder"/>s active at the same time.
    /// </summary>
    public sealed class TimeRewinder : MonoBehaviour, ITimeRewinder
    {
        private const bool IS_DEBUGGING = false;

        public IEventPrimer onRewindBegin => m_onRewindBegin;
        public IEventPrimer onRewindEnded => m_onRewindEnded;
        public IEventPrimer onBeginReached => m_onBeginReached;
        public IEventPrimer onEndReached => m_onEndReached;
        public IEventPrimer<float> onNavDirChanged => m_onNavDirChanged;
        public float earliestTime { private set; get; } = 0.0f;
        public float farthestTime => m_farthestTimeAllowed;
        public bool hasStarted { get; private set; }
        public float navigationDir => m_timeManipNavDir;
        public float curTime => timeMan.curTime;

        private GlobalTimeManager timeMan
        {
            get
            {
                InitializeTimeManager();
                return m_timeMan;
            }
        }

        [SerializeField] private MixedEvent m_onRewindBegin = new MixedEvent();
        [SerializeField] private MixedEvent m_onRewindEnded = new MixedEvent();
        [SerializeField] private MixedEvent m_onBeginReached = new MixedEvent();
        [SerializeField] private MixedEvent m_onEndReached = new MixedEvent();
        [SerializeField] private MixedEvent<float> m_onNavDirChanged = new MixedEvent<float>();
        [SerializeField, Min(0.1f)] private float m_startTimeSpeed = 1.0f;
        [SerializeField, Min(0.0f)] private float m_timeBeforeAccel = 1.0f;
        [SerializeField, Min(0.0f)] private float m_accelSpeed = 1.0f;

        private GlobalTimeManager m_timeMan = null;

        private bool m_canManipTime = false;
        private float m_timeManipNavDir = 0.0f;
        private float m_timeWhenManipStarted = 0.0f;
        private float m_timeChange = 0.0f;
        private float m_farthestTimeAllowed = 0.0f;
        private int m_shouldNotRecordCheckoutID = int.MinValue;

        private bool m_isManipTimeCoroutActive = false;


        // Foreign Initialization
        private void Start()
        {
            InitializeTimeManager();
        }


        public void StartRewind() => StartRewind(0.0f, null, null);
        public void StartRewind(float startNavDir = 0.0f, float? farthestTime = null, float? earliestTime = null)
        {
            // Don't start again if we've already started.
            if (hasStarted) { return; }
            #region Logs
            //CustomDebug.LogForComponent($"Start Rewind. {nameof(startNavDir)} ({startNavDir}). {nameof(farthestTime)} ({farthestTime}).", this, IS_DEBUGGING);
            #endregion Logs
            hasStarted = true;
            // Don't record when rewinding.
            m_shouldNotRecordCheckoutID = timeMan.RequestShouldNotRecord();

            m_canManipTime = true;
            ChangeNavigationDirection(startNavDir);
            m_timeWhenManipStarted = timeMan.curTime;
            m_timeChange = 0.0f;
            // Set the farthest time to the given time, or
            // the farthest time the time manager has seen (default).
            m_farthestTimeAllowed = farthestTime.HasValue ? farthestTime.Value : timeMan.farthestTime;
            // Only change earliest time if an earliest time has been specified. Otherwise, keep the previous earliest time.
            if (earliestTime.HasValue)
            {
                this.earliestTime = earliestTime.Value;
            }
            StartManipTime();
            m_onRewindBegin.Invoke();
        }
        public void CancelRewind()
        {
            ChangeNavigationDirection(0.0f);

            hasStarted = false;

            m_canManipTime = false;
            m_timeWhenManipStarted = -1.0f;
            m_timeChange = 0.0f;
            m_farthestTimeAllowed = -1.0f;
            // Allow recording again
            timeMan.CancelShouldNotRecordRequest(m_shouldNotRecordCheckoutID);
            m_shouldNotRecordCheckoutID = int.MinValue;
            m_onRewindEnded.Invoke();
        }
        public void ChangeNavigationDirection(float navDir)
        {
            m_timeManipNavDir = navDir;

            m_onNavDirChanged.Invoke(m_timeManipNavDir);
        }
        public void ForceSetTime(float time)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(0.0f <= time && time <= m_farthestTimeAllowed, $"time ({time}) to be between 0.0f and {m_farthestTimeAllowed}.", this);
            #endregion Asserts
            // Update the time change so that the corout will set the time appropriately.
            m_timeChange = time - m_timeWhenManipStarted;
            // Also stop any previous auto rewind or fast forward
            ChangeNavigationDirection(0.0f);
        }
        public void SetEarliestTime(float earliestTime)
        {
            this.earliestTime = earliestTime;
        }
        public void SetEarliestTimeToCurTime() => SetEarliestTime(curTime);


        /// <summary>
        /// Begins a coroutine of <see cref="ManipTimeCorout"/> if one
        /// is not already running. If one is running, no new routine is started.
        /// </summary>
        private void StartManipTime()
        {
            if (m_isManipTimeCoroutActive) { return; }
            CoroutineSingleton.instance.StartCoroutine(ManipTimeCorout());
        }
        /// <summary>
        /// Coroutine that sets the time of the global time manager
        /// each frame.
        /// </summary>
        private IEnumerator ManipTimeCorout()
        {
            m_isManipTimeCoroutActive = true;

            m_timeChange = 0.0f;
            float t_prevTimeManipNavVal = m_timeManipNavDir;
            float t_timeHeld = 0.0f;
            while (m_canManipTime)
            {
                // Determine speed from acceleration
                float t_speed = DetermineRewindSpeed(ref t_prevTimeManipNavVal, ref t_timeHeld);
                m_timeChange += m_timeManipNavDir * t_speed * Time.deltaTime;

                float t_target = m_timeWhenManipStarted + m_timeChange;
                // Clamp the target between earliest and the farthest time.
                // Keep above earliest, also keep change not falling below 0.
                if (t_target < earliestTime)
                {
                    t_target = earliestTime;
                    m_timeChange = earliestTime - m_timeWhenManipStarted;
                    m_timeManipNavDir = 0;

                    m_onBeginReached.Invoke();
                }
                // Keep below farthest, also keep change such that we can
                // reverse direction instantly.
                else if (t_target > m_farthestTimeAllowed)
                {
                    t_target = m_farthestTimeAllowed;
                    m_timeChange = t_target - m_timeWhenManipStarted;
                    m_timeManipNavDir = 0;

                    m_onEndReached.Invoke();
                }
                timeMan.SetTime(t_target);

                #region Logs
                //CustomDebug.LogForComponent($"Trying to set time to {m_timeWhenManipStarted}+{m_timeChange}={t_target}", this, IS_DEBUGGING);
                #endregion Logs
                yield return new WaitForFixedUpdate();
            }

            m_isManipTimeCoroutActive = false;
        }
        private float DetermineRewindSpeed(ref float prevTimeManipNavVal, ref float timeHeld)
        {
            float t_speed;
            if (timeHeld >= m_timeBeforeAccel)
            {
                float t_timeAccelerating = timeHeld - m_timeBeforeAccel;
                // v = v_0 + a * t
                t_speed = m_startTimeSpeed + m_accelSpeed * t_timeAccelerating;
            }
            else
            {
                t_speed = m_startTimeSpeed;
            }
            // Update time if the previous input is the same
            // as the current (holding down).
            if (prevTimeManipNavVal == m_timeManipNavDir)
            {
                timeHeld += Time.deltaTime;
            }
            else
            {
                timeHeld = 0.0f;
            }
            prevTimeManipNavVal = m_timeManipNavDir;

            return t_speed;
        }

        private void InitializeTimeManager()
        {
            if (m_timeMan == null)
            {
                m_timeMan = GlobalTimeManager.instance;
                #region Asserts
                //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
                #endregion Asserts
            }
        }
    }
}
