using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Controls the slider value of the time slider during the transition animation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimeSliderTransitionAnimationController : StopwatchTimelineAnimationTransitionSubAnimationController
    {
        [SerializeField, Required] private TimeSlider m_timeSlider = null;
        [SerializeField, Min(1)] private int m_frameToStartSliderFixToTimelineAnim = 23;
        [SerializeField, Min(1)] private int m_frameToFinishSliderFixToStopwatchAnim = 6;

        private float m_sliderStartVal = 0.0f;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeSlider, nameof(m_timeSlider), this);
            #endregion Asserts
        }


        public override void StartTransitionToTimeline()
        {
            m_timeSlider.enabled = false;
            m_timeSlider.slider.interactable = false;

            base.StartTransitionToTimeline();
        }
        public override void StartTransitionToStopwatch()
        {
            m_timeSlider.enabled = false;
            m_timeSlider.slider.interactable = false;

            m_sliderStartVal = m_timeSlider.slider.value;

            base.StartTransitionToStopwatch();
        }


        protected override void ToTimelineFrameAction(int frame)
        {
            if (frame < m_frameToStartSliderFixToTimelineAnim)
            {
                m_timeSlider.slider.value = 0.0f;
            }
            else
            {
                int t_framesSinceStart = (frame - m_frameToStartSliderFixToTimelineAnim);
                int t_totalAmountFrames = stopwatchTimelineAnimCont.totalAmountFrames;
                int t_framesToExecute = (t_totalAmountFrames - m_frameToStartSliderFixToTimelineAnim);
                float t = ((float)t_framesSinceStart) / t_framesToExecute;
                float t_endVal = GetSliderValueForCurrentTime();
                float t_newVal = Mathf.LerpAngle(0.0f, t_endVal, t);

                m_timeSlider.slider.value = t_newVal;
            }
        }
        protected override void ToStopwatchFrameAction(int frame)
        {
            if (frame >= m_frameToFinishSliderFixToStopwatchAnim)
            {
                m_timeSlider.slider.value = 0.0f;
            }
            else
            {
                float t = ((float)frame) / m_frameToFinishSliderFixToStopwatchAnim;
                float t_newSliderVal = Mathf.Lerp(m_sliderStartVal, 0.0f, t);

                m_timeSlider.slider.value = t_newSliderVal;
            }
        }

        protected override void EndToTimelineTransition()
        {
            m_timeSlider.slider.value = GetSliderValueForCurrentTime();
            m_timeSlider.enabled = true;
            m_timeSlider.slider.interactable = true;
        }
        protected override void EndToStopwatchTransition()
        {
            m_timeSlider.enabled = true;
            m_timeSlider.slider.interactable = true;
            m_timeSlider.slider.value = 0.0f;
        }


        private float GetSliderValueForCurrentTime()
        {
            if (m_timeSlider.timeRewinder == null)
            {
                return 0.0f;
            }
            return m_timeSlider.ConvertFromTimeToSliderValue(m_timeSlider.timeRewinder.curTime);
        }
    }
}
