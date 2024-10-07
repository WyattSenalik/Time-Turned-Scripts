using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik (created referencing BetterCurveAnimation)

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// Animation that uses a <see cref="ColorCurveSO"/>.
    /// </summary>
    public abstract class ColorCurveAnimation : ScriptedAnimationMonoBehaviour
    {
        public override event Action onEnd;

        public IColorCurve curve => m_curve;

        [SerializeField, Required, Expandable] private ColorCurveSO m_curve = null;
        [SerializeField] private bool m_playOnStart = false;
        [SerializeField] private bool m_repeat = false;

        private bool m_isPlaying = false;
        private float m_playBeginTime = 0.0f;


        protected virtual void Start()
        {
            if (m_playOnStart)
            { Play(); }
        }
        protected virtual void Update()
        {
            if (m_isPlaying)
            {
                float t_curGlobalTime = Time.time;
                float t_curEvalTime;
                // If the current time is after when the animation should end
                // then we need to either repeat the animation of stop playing it.
                if (t_curGlobalTime > m_playBeginTime + m_curve.timeDuration)
                {
                    if (m_repeat)
                    {
                        Restart();
                        t_curEvalTime = t_curGlobalTime - m_playBeginTime;
                    }
                    else
                    {
                        StopPlaying();
                        // Update the curve to its end.
                        t_curEvalTime = m_curve.GetEndTime();
                    }
                }
                else
                {
                    t_curEvalTime = t_curGlobalTime - m_playBeginTime;
                }
                Color t_curveVal = m_curve.Evaluate(t_curEvalTime);
                TakeCurveAction(t_curveVal);
            }
        }


        public override void Play(bool shouldInterrupt = false)
        {
            // Already playing.
            if (m_isPlaying)
            {
                if (shouldInterrupt)
                {
                    Restart();
                }
            }
            // Not playing yet.
            else
            {
                StartPlaying();
            }
        }
        public override void Stop()
        {
            StopPlaying();
        }

        protected abstract void TakeCurveAction(Color curveValue);


        private void StartPlaying()
        {
            m_playBeginTime = Time.time;
            m_isPlaying = true;
        }
        private void StopPlaying()
        {
            m_isPlaying = false;
            onEnd?.Invoke();
        }
        private void Restart()
        {
            m_playBeginTime = Time.time;
        }
    }
}
