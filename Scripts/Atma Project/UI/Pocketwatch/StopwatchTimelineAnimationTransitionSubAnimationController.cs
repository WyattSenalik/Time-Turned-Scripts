using System;
using System.Collections;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [RequireComponent(typeof(StopwatchTimelineAnimationController))]
    public abstract class StopwatchTimelineAnimationTransitionSubAnimationController : MonoBehaviour
    {
        public StopwatchTimelineAnimationController stopwatchTimelineAnimCont {  get; private set; }

        private bool m_isToTimelineAnimCoroutActive = false;
        private Coroutine m_toTimelineAnimCorout = null;

        private bool m_isToStopwatchAnimCoroutActive = false;
        private Coroutine m_toStopwatchAnimCorout = null;


        protected virtual void Awake()
        {
            stopwatchTimelineAnimCont = GetComponent<StopwatchTimelineAnimationController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(stopwatchTimelineAnimCont, this);
            #endregion Asserts
        }


        public virtual void StartTransitionToTimeline()
        {
            if (m_isToTimelineAnimCoroutActive)
            {
                StopCoroutine(m_toTimelineAnimCorout);
                m_toTimelineAnimCorout = null;
                m_isToTimelineAnimCoroutActive = false;
            }
            m_toTimelineAnimCorout = StartCoroutine(FPSAwareCoroutine(stopwatchTimelineAnimCont.transitionAnimLength, ToTimelineFrameAction, EndToTimelineTransition));

            m_isToTimelineAnimCoroutActive = true;
        }
        public virtual void StartTransitionToStopwatch()
        {
            if (m_isToStopwatchAnimCoroutActive)
            {
                StopCoroutine(m_toStopwatchAnimCorout);
                m_toStopwatchAnimCorout = null;
                m_isToStopwatchAnimCoroutActive = false;
            }
            m_toStopwatchAnimCorout = StartCoroutine(FPSAwareCoroutine(stopwatchTimelineAnimCont.transitionAnimLength, ToStopwatchFrameAction, EndToStopwatchTransition));

            m_isToStopwatchAnimCoroutActive = true;
        }
        public void SkipToTimelineEnd()
        {
            if (m_isToTimelineAnimCoroutActive)
            {
                StopCoroutine(m_toTimelineAnimCorout);
                m_toTimelineAnimCorout = null;
                m_isToTimelineAnimCoroutActive = false;
            }

            EndToTimelineTransition();
        }
        public void SkipToStopwatchEnd()
        {
            if (m_isToStopwatchAnimCoroutActive)
            {
                StopCoroutine(m_toStopwatchAnimCorout);
                m_toStopwatchAnimCorout = null;
                m_isToStopwatchAnimCoroutActive = false;
            }

            EndToStopwatchTransition();
        }

        protected virtual void EndToTimelineTransition()
        {
            m_isToTimelineAnimCoroutActive = false;
        }
        protected virtual void EndToStopwatchTransition()
        {
            m_isToStopwatchAnimCoroutActive = false;
        }
        protected abstract void ToTimelineFrameAction(int frame);
        protected abstract void ToStopwatchFrameAction(int frame);


        private IEnumerator FPSAwareCoroutine(float duration, Action<int> frameAction, Action endAction)
        {
            float t_beginTimeStamp = Time.time;
            float t_elapsedTime = 0.0f;
            float t_pauseTimeBetweenFrames = 1.0f / stopwatchTimelineAnimCont.fps;
            int t_iterations = 0;

            while (t_elapsedTime < duration)
            {
                frameAction.Invoke(t_iterations + 1);

                float t_expectedElapsedTime = t_iterations * t_pauseTimeBetweenFrames;
                float t_elapsedTimeDiff = t_expectedElapsedTime - t_elapsedTime;
                float t_correctedPauseTime = t_pauseTimeBetweenFrames + t_elapsedTimeDiff;
                yield return new WaitForSeconds(t_correctedPauseTime);
                t_elapsedTime = Time.time - t_beginTimeStamp;
                ++t_iterations;
            }

            endAction.Invoke();
        }
    }
}