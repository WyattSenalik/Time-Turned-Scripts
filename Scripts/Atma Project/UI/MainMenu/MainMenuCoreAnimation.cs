// Original Authors - Wyatt Senalik
using UnityEngine;
using UnityEngine.UI;

using Timed;
using Helpers.Animation.BetterCurve;

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class MainMenuCoreAnimation : MonoBehaviour
    {
        public float animLength => m_frames.Length / m_framesPerSecond;
        public int curFrame => m_curFrame;
        public Sprite[] frames => m_frames;
        public int frameCount => frames.Length;

        [SerializeField] private Sprite[] m_frames = new Sprite[50];
        [SerializeField, Min(1)] private int m_framesPerSecond = 8;
        [SerializeField] private bool m_useUnscaledTime = false;
        [SerializeField] private BetterCurve m_nonNormalAnimSpeedCurve = new BetterCurve();

        private Image m_image = null;
        private float m_inverseFPS = float.NaN;
        private float m_nextFrameShowTime = float.NaN;
        private int m_curFrame = 0;
        private bool m_isPlayingNormally = true;

        private float m_nextNonNormalFrameShowTime = float.PositiveInfinity;
        private int m_beginFrame = -1;
        private int m_targetFrame = -1;
        private bool m_shouldMoveTowardsTargetFrameForward = false;

        private float m_startTime = float.NegativeInfinity;
        private bool m_isFirstUpdate = true;

        private void Awake()
        {
            m_image = GetComponent<Image>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_image, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_curFrame = 0;
            m_image.sprite = m_frames[m_curFrame];
            m_inverseFPS = 1.0f / m_framesPerSecond;
        }
        private void Update()
        {
            if (m_isFirstUpdate)
            {
                m_startTime = m_useUnscaledTime ? Time.unscaledTime : Time.time;
                m_nextFrameShowTime = GetCurrentTime() + m_inverseFPS;
                m_isFirstUpdate = false;
            }

            if (m_isPlayingNormally)
            {
                UpdateNormalAnimation();
            }
            else
            {
                UpdateNonNormalAnimation();
            }
        }


        public void StopNormalAnimation()
        {
            m_isPlayingNormally = false;
        }
        public void AnimateTowardsFrame(int frame, bool shouldAnimateForwards)
        {
            // already on the target frame.
            if (frame == curFrame)
            {
                m_nextNonNormalFrameShowTime = float.PositiveInfinity;
                return;
            }

            // If we are not already animating towards a frame, this is infinity.
            if (float.IsInfinity(m_nextNonNormalFrameShowTime))
            {
                m_beginFrame = curFrame;
            }
            m_targetFrame = frame;
            m_shouldMoveTowardsTargetFrameForward = shouldAnimateForwards;
            m_nextNonNormalFrameShowTime = GetCurrentTime() + 1.0f / DetermineNonNormalAnimSpeed();
        }


        private void UpdateNormalAnimation()
        {
            // Animating normally.
            if (GetCurrentTime() >= m_nextFrameShowTime)
            {
                m_curFrame = (m_curFrame + 1) % m_frames.Length;
                m_image.sprite = m_frames[m_curFrame];
                m_nextFrameShowTime += m_inverseFPS;
            }
        }
        private void UpdateNonNormalAnimation()
        {
            // Animating in a weird way where we animate towards a specific frame then stop once reaching that frame.
            if (GetCurrentTime() >= m_nextNonNormalFrameShowTime)
            {
                if (m_shouldMoveTowardsTargetFrameForward)
                {
                    m_curFrame = (m_curFrame + 1) % m_frames.Length;
                }
                else
                {
                    m_curFrame = (m_curFrame - 1);
                    if (m_curFrame < 0)
                    {
                        m_curFrame = m_frames.Length - 1;
                    }
                }
                m_image.sprite = m_frames[m_curFrame];
                m_nextNonNormalFrameShowTime += 1.0f / DetermineNonNormalAnimSpeed();

                // We've reach our target, no longer need to move
                if (m_curFrame == m_targetFrame)
                {
                    m_nextNonNormalFrameShowTime = float.PositiveInfinity;
                }
            }
        }
        private float DetermineNonNormalAnimSpeed()
        {
            float t_adjustedBeginFrame = m_beginFrame;
            float t_adjustedTargetFrame = m_targetFrame;
            if (m_shouldMoveTowardsTargetFrameForward)
            {
                if (m_beginFrame > m_targetFrame)
                {
                    t_adjustedTargetFrame = m_targetFrame + frameCount;
                }
            }
            else
            {
                if (m_beginFrame < m_targetFrame)
                {
                    t_adjustedBeginFrame = m_beginFrame + frameCount;
                }
            }

            float t_curFrameDiff = Mathf.Abs(curFrame - t_adjustedBeginFrame);
            float t_targetFrameDiff = Mathf.Abs(t_adjustedTargetFrame - t_adjustedBeginFrame);
            if (t_targetFrameDiff == 0.0f)
            {
                t_targetFrameDiff = frameCount;
            }
            return m_nonNormalAnimSpeedCurve.EvaluateClamped(t_curFrameDiff / t_targetFrameDiff);
        }
        private float GetCurrentTime()
        {
            return (m_useUnscaledTime ? Time.unscaledTime : Time.time) - m_startTime;
        }
    }
}