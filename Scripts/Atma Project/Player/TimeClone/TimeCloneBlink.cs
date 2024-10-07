using UnityEngine;

using Timed;
using Helpers.Math;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeClone))]
    [RequireComponent(typeof(TimeCloneHealth))]
    public sealed class TimeCloneBlink : TimedRecorder
    {
        private const bool IS_DEBUGGING = false;
        private const float ONE_THIRD = 1.0f / 3.0f;
        private const float TWO_THIRDS = 2.0f / 3.0f;

        public float blinkTimeStart => Mathf.Min(m_timeClone.DetermineEarliestDisappearanceTime(), m_data.farthestTime);
        public float blinkTimeEnd => blinkTimeStart + m_data.blinkTime;
        public bool isBlinking => curTime >= blinkTimeStart && curTime <= blinkTimeEnd;

        [SerializeField, Range(0.0f, 1.0f)] private float m_normalAlpha = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_blinkAlpha = 0.0f;
        [SerializeField] private float m_targetBlinksPerSecond = 0.5f;
        [SerializeField] private SpriteRenderer[] m_sprRenderers = new SpriteRenderer[0];

        private GlobalTimeManager m_timeMan = null;

        private TimeClone m_timeClone = null;
        private TimeCloneHealth m_timeCloneHealth = null;

        private TimeCloneInitData m_data = null;
        private Color m_defaultCloneColor = Color.white;

        private float m_manualBlinkStartUnityTime = float.PositiveInfinity;
        private float m_manualBlinkEndUnityTime = float.PositiveInfinity;


        protected override void Awake()
        {
            base.Awake();

            m_timeClone = GetComponent<TimeClone>();
            m_timeCloneHealth = GetComponent<TimeCloneHealth>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_timeClone, this);
            //CustomDebug.AssertComponentIsNotNull(m_timeCloneHealth, this);
            #endregion Asserts

            m_timeClone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
            #endregion Asserts

            if (m_sprRenderers.Length > 0)
            {
                m_defaultCloneColor = m_sprRenderers[0].color;
            }
        }
        private void OnDestroy()
        {
            if (m_timeClone != null)
            {
                m_timeClone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }
        private void Initialize()
        {
            m_data = m_timeClone.cloneData;
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Wait until initialized
            if (m_data == null) { return; }

            if (float.IsPositiveInfinity(m_manualBlinkStartUnityTime))
            {
                // When to begin blinking, by default its the data's furthest time, but if the time clone has an early disappearance
                float t_blinkTimeStart = blinkTimeStart;

                // If the clone has been hit after it blinked, set alpha to 0 (we do this instead of disabling the SpriteRenderer because SpriteRenderer.enabled is being controlled by the TimedSpriteRenderer, but SpriteRenderer.color is not).
                if (curTime >= m_timeCloneHealth.hitAfterDeathTime)
                {
                    SetSpriteRendererAlpha(0.0f);
                }
                else if (curTime <= t_blinkTimeStart)
                {
                    // Is before we are supposed to blink
                    SetSpriteRendererAlpha(m_normalAlpha);
                    #region Logs
                    //CustomDebug.LogForComponent($"NORMAL. curTime ({curTime}). farthestTime ({m_data.farthestTime}). blinkTime ({m_data.blinkTime}). Alpha Value ({m_normalAlpha}).", this, IS_DEBUGGING);
                    #endregion Logs

                }
                else if (curTime < blinkTimeEnd)
                {
                    float t_correctedSecondsPerBlink;
                    if (float.IsPositiveInfinity(blinkTimeEnd))
                    {
                        t_correctedSecondsPerBlink = 1.0f / m_targetBlinksPerSecond;
                    }
                    else
                    {
                        // Make the final blink end when time ends.
                        float t_blinkCount = m_data.blinkTime * m_targetBlinksPerSecond;
                        int t_correctedBlinkCount = Mathf.RoundToInt(t_blinkCount);
                        t_correctedSecondsPerBlink = m_data.blinkTime / t_correctedBlinkCount;
                    }

                    // Do the blinking animation
                    UpdateAnimation(t_blinkTimeStart, curTime, t_correctedSecondsPerBlink);
                }
                else if (curTime == blinkTimeEnd)
                {
                    SetSpriteRendererAlpha(m_blinkAlpha);
                }
                else
                {
                    #region Logs
                    //CustomDebug.LogForComponent($"Edge case. curTime ({curTime}). farthestTime ({m_data.farthestTime}). blinkTime ({m_data.blinkTime}).", this, IS_DEBUGGING);
                    #endregion Logs
                }
            }
            else
            {
                if (Time.time < m_manualBlinkEndUnityTime)
                {
                    // Playing the animation manually.

                    // Make the final blink end when time ends.
                    float t_timeToTake = m_manualBlinkEndUnityTime - m_manualBlinkStartUnityTime;
                    float t_blinkCount = t_timeToTake * m_targetBlinksPerSecond;
                    int t_correctedBlinkCount = Mathf.RoundToInt(t_blinkCount);
                    float t_correctedSecondsPerBlink = t_timeToTake / t_correctedBlinkCount;
                    
                    UpdateAnimation(m_manualBlinkStartUnityTime, Time.time, t_correctedSecondsPerBlink * 2);
                }
                else
                {
                    // After animation should end.
                    SetSpriteRendererAlpha(0.0f);
                }
            }
        }


        public void PlayBlinkAnimationInUnityTime(float timeToTake)
        {
            m_manualBlinkStartUnityTime = Time.time;
            m_manualBlinkEndUnityTime = Time.time + timeToTake;
        }


        private void UpdateAnimation(float blinkStartTime, float blinkCurTime, float secondsPerBlink)
        {
            float t_elapsedTimeWrapped = (blinkCurTime - blinkStartTime) % secondsPerBlink;
            float t_alphaLerpT = MathHelpers.Normalize(t_elapsedTimeWrapped, 0.0f, secondsPerBlink);

            float t_alpha;
            if (t_alphaLerpT < 0.5f)
            {
                t_alpha = Mathf.Lerp(m_normalAlpha, m_blinkAlpha, t_alphaLerpT * 2);
            }
            else
            {
                t_alpha = Mathf.Lerp(m_blinkAlpha, m_normalAlpha, (t_alphaLerpT - 0.5f) * 2);
            }
            SetSpriteRendererAlpha(t_alpha);
            #region Logs
            //CustomDebug.LogForComponent($"BLINKING. curTime ({curTime}). farthestTime ({m_data.farthestTime}). blinkTime ({m_data.blinkTime}). Alpha Value ({t_alpha}). LerpValue ({t_alphaLerpT})", this, IS_DEBUGGING);
            #endregion Logs
        }
        private void SetSpriteRendererAlpha(float alpha)
        {
            Color t_col = m_defaultCloneColor;
            t_col.a = alpha;
            SetSpriteRendererColor(t_col);
        }
        private void SetSpriteRendererColor(Color color)
        {
            foreach (SpriteRenderer t_singleRenderer in m_sprRenderers)
            {
                t_singleRenderer.color = color;
            }
        }
    }
}
