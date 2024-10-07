using UnityEngine;

using Timed;
// Original Authors - Eslis Vang
// Tweaked by Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Rotates the pocketwatch's clockhand.
    /// </summary>
    public class ClockHand : MonoBehaviour
    {
        public LevelOptions levelOptions
        {
            get
            {
                if (!m_isInitialized)
                {
                    InitializeSingletonReferences();
                }
                return m_levelOptions;
            }
        }
        public GlobalTimeManager timeManager
        {
            get
            {
                if (!m_isInitialized)
                {
                    InitializeSingletonReferences();
                }
                return m_timeManager;
            }
        }

        [SerializeField, Range(-180.0f, 180.0f)] private float m_startAngle = 90.0f;
        [SerializeField] private bool m_isSecondHand = false;
        [SerializeField, Min(0.0f)] private float m_noTimeLimitSpinSpeed = 0.5f;

        private bool m_isInitialized = false;
        private GlobalTimeManager m_timeManager = null;
        private LevelOptions m_levelOptions = null;


        private void Start()
        {
            InitializeSingletonReferences();
        }
        private void Update()
        {
            float t_zEulerAngle = DetermineCurrentAngle();
            transform.localEulerAngles = new Vector3(0f, 0f, t_zEulerAngle);
        }


        public float DetermineCurrentAngle()
        {
            float t_endAngle = m_startAngle - 360.0f;
            if (levelOptions.noTimeLimit)
            {
                // If no time limit, just make the stopwatch turn once per second.
                float t = (timeManager.curTime * m_noTimeLimitSpinSpeed) % 1.0f;
                return Mathf.Lerp(m_startAngle, t_endAngle, t);
            }
            else if (timeManager.curTime >= levelOptions.time)
            {
                // Time is up.
                return t_endAngle;
            }
            else
            {
                float t;
                if (m_isSecondHand)
                {
                    // The second hand makes 1 revolution per second.
                    t = (1.0f - (levelOptions.time - timeManager.curTime) % 1.0f);
                }
                else
                {
                    // The other hand makes 1 revolution per level.
                    t = timeManager.curTime / levelOptions.time;
                }
                return Mathf.Lerp(m_startAngle, t_endAngle, t);
            }
        }


        private void InitializeSingletonReferences()
        {
            if (m_isInitialized) { return; }
            m_timeManager = GlobalTimeManager.instance;
            m_levelOptions = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeManager, this);
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_levelOptions, this);
            #endregion
            m_isInitialized = true;
        }
    }
}
