using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Centralized controller for controlling the time manipulation to be used by UI elements and each element's corresponding input.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BranchPlayerController))]
    [RequireComponent(typeof(ITimeRewinder))]
    public sealed class TimeManipController : MonoBehaviour, ITimeManipController
    {
        public IEventPrimer onSkipToBegin => m_onSkipToBegin;
        public ITimeRewinder rewinder => m_timeRewinder;

        [SerializeField, Tag] private string m_timeSliderTag = "Timeline";
        [SerializeField] private MixedEvent m_onSkipToBegin = new MixedEvent();
        [SerializeField, Required] private TimeSlider m_timeSlider = null;
        [SerializeField, Required] private InitialGainChargeAnimationController m_initGainChargeAnimCont = null;

        private BranchPlayerController m_playerCont = null;
        private ITimeRewinder m_timeRewinder = null;


        private void Awake()
        {
            m_playerCont = GetComponent<BranchPlayerController>();
            m_timeRewinder = GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_playerCont, this);
            //CustomDebug.AssertIComponentIsNotNull(m_timeRewinder, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeSlider, nameof(m_timeSlider), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_initGainChargeAnimCont, nameof(m_initGainChargeAnimCont), this);
            #endregion Asserts
        }


        public void BeginTimeManipulation(bool isFirstPause = false)
        {
            #region Error Logs
            if (m_playerCont.timeRewinder.hasStarted)
            {
                //CustomDebug.LogErrorForComponent($"Expected time to be not currently be manipulated before calling {nameof(BeginTimeManipulation)}.", this);
                return;
            }
            #endregion Error Logs
            m_playerCont.ForceBeginTimeManipulation(null, isFirstPause);
        }
        public void CreateTimeClone()
        {
            m_playerCont.CreateTimeCloneAtCurrentTime();
        }

        public void SkipToBeginning()
        {
            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }

            // Already at earliest time
            if (m_timeRewinder.curTime == m_timeRewinder.earliestTime) { return; }

            m_timeRewinder.ForceSetTime(m_timeRewinder.earliestTime);
            m_onSkipToBegin.Invoke();
        }
        public void Rewind()
        {
            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }

            m_playerCont.RewindNavVelocity();
        }
        public void Play()
        {
            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }

            // Need to wait for init anim to be done if its first one.
            if (!m_initGainChargeAnimCont.hasFinished) { return; }

            m_playerCont.TryEndTimeManipulation();
        }
        public void Pause()
        {
            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }

            m_playerCont.PauseNavVelocity();
        }
        public void FastForward()
        {
            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }

            m_playerCont.FastForwardNavVelocity();
        }
        public void SkipToEnd()
        {
            // If player is using mouse to interact w/ time slider, do nothing when they try inputting or pressing buttons.
            if (m_timeSlider.interactState != ITimeSlider.eInteractState.None) { return; }

            m_timeRewinder.ForceSetTime(m_timeRewinder.farthestTime);
        }
    }
}