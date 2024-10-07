using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Controls the rotation of the clock hand and center part of the stopwatch during the transition animation.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ClockHandTransitionAnimationController : StopwatchTimelineAnimationTransitionSubAnimationController
    {
        [SerializeField, Required] private ClockHand m_handClockHand = null;
        [SerializeField, Required] private ClockHand m_centerClockHand = null;
        [SerializeField, Min(1)] private int m_frameToFinishRotationFixToTimelineAnim = 5;
        [SerializeField, Min(1)] private int m_frameToStartRotationFixToStopwatchAnim = 24;

        private float m_startingRot = 0.0f;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_handClockHand, nameof(m_handClockHand), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_centerClockHand, nameof(m_centerClockHand), this);
            #endregion Asserts
        }


        public override void StartTransitionToTimeline()
        {
            m_startingRot = m_handClockHand.transform.eulerAngles.z;

            m_handClockHand.enabled = false;
            m_centerClockHand.enabled = false;

            base.StartTransitionToTimeline();
        }
        public override void StartTransitionToStopwatch()
        {
            m_handClockHand.enabled = false;
            m_centerClockHand.enabled = false;

            base.StartTransitionToStopwatch();
        }


        protected override void ToTimelineFrameAction(int frame)
        {
            if (frame >= m_frameToFinishRotationFixToTimelineAnim)
            {
                m_handClockHand.transform.rotation = Quaternion.identity;
                m_centerClockHand.transform.rotation = Quaternion.identity;
            }
            else
            {
                float t = ((float)frame) / m_frameToFinishRotationFixToTimelineAnim;
                float t_newRot = Mathf.LerpAngle(m_startingRot, 0.0f, t);

                m_handClockHand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, t_newRot);
                m_centerClockHand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, t_newRot);
            }
        }
        protected override void ToStopwatchFrameAction(int frame)
        {
            if (frame < m_frameToStartRotationFixToStopwatchAnim)
            {
                m_handClockHand.transform.rotation = Quaternion.identity;
                m_centerClockHand.transform.rotation = Quaternion.identity;
            }
            else
            {
                int t_framesSinceStart = (frame - m_frameToStartRotationFixToStopwatchAnim);
                int t_totalAmountFrames = stopwatchTimelineAnimCont.totalAmountFrames;
                int t_framesToExecute = (t_totalAmountFrames - m_frameToStartRotationFixToStopwatchAnim);
                float t = ((float)t_framesSinceStart) / t_framesToExecute;
                float t_endRot = m_handClockHand.DetermineCurrentAngle();
                float t_newRot = Mathf.LerpAngle(0.0f, t_endRot, t);

                m_handClockHand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, t_newRot);
                m_centerClockHand.transform.rotation = Quaternion.Euler(0.0f, 0.0f, t_newRot);
            }
        }

        protected override void EndToTimelineTransition()
        {
            base.EndToTimelineTransition();

            m_handClockHand.enabled = true;
            m_centerClockHand.enabled = true;
        }
        protected override void EndToStopwatchTransition()
        {
            base.EndToStopwatchTransition();

            m_handClockHand.enabled = true;
            m_centerClockHand.enabled = true;
        }
    }
}