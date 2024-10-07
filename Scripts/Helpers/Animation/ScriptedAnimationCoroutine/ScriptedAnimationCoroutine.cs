using System;
using System.Collections;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Frequently when making ScriptedAnimations, a single large coroutine is used in conjunction with multiple BetterCurves. Instead of having to rewrite the coroutine logic every time, here is the boiler plate for that coroutine logic and now all that is needed is to write the specifics for applying the animation and incrementing any and all lerp t variables.
    /// </summary>
    public abstract class ScriptedAnimationCoroutine : ScriptedAnimationMonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public override event Action onEnd;

        public float speed
        {
            get => m_speed;
            set
            {
                m_speed = value;
                if (m_speed < 0.0f)
                {
                    m_speed = 0.0f;
                }
            }
        }

        private Coroutine m_animCorout = null;
        private bool m_isAnimCoroutActive = false;

        private float m_speed = 1.0f;


        public override void Play(bool shouldInterrupt = false)
        {
            #region Logs
            //CustomDebug.LogForComponent($"Play scripted animation ({name}).", this, IS_DEBUGGING);
            #endregion Logs
            if (m_isAnimCoroutActive)
            {
                if (shouldInterrupt)
                {
                    StopCoroutine(m_animCorout);
                    m_isAnimCoroutActive = false;
                }
                else
                {
                    // Stop! In the name of the law!
                    // Not allowed it interrupt currently active animation.
                    return;
                }
            }
            StartAnimation();
            m_animCorout = StartCoroutine(AnimationCoroutine());
        }
        public override void Stop()
        {
            if (m_isAnimCoroutActive)
            {
                StopCoroutine(m_animCorout);
                m_isAnimCoroutActive = false;
            }
        }


        protected abstract void StartAnimation();
        protected abstract float GetEndTime();
        protected abstract void UpdateAnimation(float timeElapsed);
        protected abstract void UpdateAnimationFinal();


        private IEnumerator AnimationCoroutine()
        {
            m_isAnimCoroutActive = true;

            float t_timeElapsed = 0.0f;
            while (t_timeElapsed < GetEndTime())
            {
                UpdateAnimation(t_timeElapsed);

                yield return null;

                t_timeElapsed += Time.deltaTime * speed;
            }
            UpdateAnimationFinal();

            m_isAnimCoroutActive = false;
            onEnd?.Invoke();
        }
    }
}