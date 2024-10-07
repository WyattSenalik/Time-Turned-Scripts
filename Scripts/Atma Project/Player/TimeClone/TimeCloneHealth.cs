using UnityEngine;

using Atma.Events;
using Helpers.Events;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Handles when a clone dies before they should.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeClone))]
    public sealed class TimeCloneHealth : TimedRecorder, IHealth
    {
        private const bool IS_DEBUGGING = false;
        private const float DEATH_TIME_PADDING = 0.5f;

        public bool isDead => !float.IsNaN(m_earlyDeathTime);
        public float hitAfterDeathTime => m_hitAfterDeathTime;

        private GlobalTimeManager timeMan
        {
            get
            {
                InitializeTimeManIfNotInitialized();
                return m_timeMan;
            }
        }
        private void InitializeTimeManIfNotInitialized()
        {
            if (m_timeMan == null)
            {
                m_timeMan = GlobalTimeManager.instance;
                #region Asserts
                //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
                #endregion Asserts
            }
        }

        [SerializeField] private MixedEvent<float> m_onHitAfterDeath = new MixedEvent<float>();
        [SerializeField] private MixedEvent<float> m_onResumeResetHitAfterDeath = new MixedEvent<float>();

        private GlobalTimeManager m_timeMan = null;

        private TimeClone m_timeClone = null;

        private float m_earlyDeathTime = float.NaN;
        private float m_hitAfterDeathTime = float.PositiveInfinity;
        private bool m_wasTimeManRecording = false;


        protected override void Awake()
        {
            base.Awake();

            m_timeClone = GetComponent<TimeClone>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_timeClone, this);
            #endregion Asserts
        }
        private void Start()
        {
            InitializeTimeManIfNotInitialized();
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            bool t_shouldRecord = timeMan.shouldRecord;

            if (t_shouldRecord && !m_wasTimeManRecording)
            {
                FakeOnRecordingResume();
            }

            m_wasTimeManRecording = t_shouldRecord;
        }
        /// <summary>
        /// Have to use a fake version of OnRecordingResume because time clones don't actually record ever.
        /// </summary>
        private void FakeOnRecordingResume()
        {
            if (curTime < m_hitAfterDeathTime)
            {
                m_hitAfterDeathTime = float.PositiveInfinity;
                m_onResumeResetHitAfterDeath.Invoke(curTime);
            }
            if (curTime < m_earlyDeathTime)
            {
                m_earlyDeathTime = float.NaN;
            }
        }


        public bool TakeDamage(IDamageContext context)
        {
            #region Logs
            if (!timeMan.shouldRecord)
            {
                //CustomDebug.LogErrorForComponent($"Expcted only to take damage while recording (at time {curTime}).", this);
            }
            #endregion Logs
            // When the clone takes damage:
            //  1: Put player into rewind mode
            //  2: Your past self has died
            // Since this needs to talk to both the player object and the UI element, we will invoke an event and let those things subscribe to it.
            TimeCloneInitData t_cloneData = m_timeClone.cloneData;
            float t_endTime = t_cloneData.farthestTime;
            float t_damageTime = context.damageTime;
            // If the clone is hit after it begins to blink, tell the blink to stop blinking (also want to turn off the colldiers).
            // We treat this as a hit after death if the clone is disconnected as well.
            if (t_damageTime >= t_endTime || m_timeClone.isDisconnected)
            {
                m_hitAfterDeathTime = t_damageTime;
                m_onHitAfterDeath.Invoke(t_damageTime);
                // Don't want the bullet to die since the clone is currently blinking and should disappear because of this bullet.
                return false;
            }
            // If the clone dies in the next little bit, don't do anything anyway.
            else if (t_damageTime + DEATH_TIME_PADDING >= t_endTime)
            {
                m_hitAfterDeathTime = t_endTime;
                m_onHitAfterDeath.Invoke(t_damageTime);
                // Clone is going to die in the next few seconds anyway, so don't needlessly pause the game.
                // Don't want the bullet to die since we are just ignoring it hitting (since it's so close to when the clone will die).
                return false;
            }

            // Invoke the clone died event.
            m_earlyDeathTime = t_damageTime;
            CloneManager t_cloneMan = t_cloneData.cloneManager;
            t_cloneMan.InvokeCloneDiedEvent(new CloneDiedContext(context, transform.position, gameObject));

            SteamAchievementManager.instance.GrantAchievement( AchievementId.GRANDFATHER_PARADOX );

            #region Logs
            //CustomDebug.LogForComponent("Clone Died", this, IS_DEBUGGING);
            #endregion Logs

            // Don't want the bullt to disappear because time is about to die.
            return false;
        }
    }
}
