using System;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class StopwatchTimelineAnimationController : MonoBehaviour
    {
        public Animator animator => m_animator;
        public string isTimeManipActiveBoolAnimParam => m_isTimeManipActiveBoolAnimParam;
        public string skipTriggerAnimParam => m_skipTriggerAnimParam;
        public float transitionAnimLength => m_transitionAnimLength;
        public int fps => m_fps;
        public int totalAmountFrames => m_totalAmountFrames;
        public IEventPrimer onTransitionToStopwatchStarted => m_onTransitionToStopwatchStarted;
        public IEventPrimer onTransitionToTimelineStarted => m_onTransitionToTimelineStarted;
        public IEventPrimer onTransitionAnimFinished => m_onTransitionAnimFinished;
        public StopwatchTimelineAnimationTransitionSubAnimationController[] subControllers { get; private set; }
        public bool isTimeManipActive { get; private set; }
        public UISoundController soundCont
        {
            get
            {
                if (m_soundCont == null)
                {
                    m_soundCont = UISoundController.GetInstanceSafe();
                }
                return m_soundCont;
            }
        }

        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_isTimeManipActiveBoolAnimParam = "IsTimeManipActive";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_skipTriggerAnimParam = "SkipToIdle";
        [SerializeField] private float m_transitionAnimLength = 1.35f;
        [SerializeField, Range(0, 60)] private int m_fps = 20;
        [SerializeField, Min(1)] private int m_totalAmountFrames = 29;
        [SerializeField] private MixedEvent m_onTransitionToStopwatchStarted = new MixedEvent();
        [SerializeField] private MixedEvent m_onTransitionToTimelineStarted = new MixedEvent();
        [SerializeField] private MixedEvent m_onTransitionAnimFinished = new MixedEvent();

        private UISoundController m_soundCont = null;

        private Action m_onTransitionFinishedAction = null;


        private void Awake()
        {
            subControllers = GetComponents<StopwatchTimelineAnimationTransitionSubAnimationController>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            #endregion Asserts
        }
        private void Start()
        {
            if (m_soundCont == null)
            {
                m_soundCont = UISoundController.GetInstanceSafe();
            }
        }


        public void StartTransitionToTimeline(Action onTransitionFinished)
        {
            CancelInvoke();

            StartCoroutine(DoActionOnceUnityTimeIsNotLongerPausedCorout(soundCont.PlayTimeFreezeSound));

            isTimeManipActive = true;
            m_animator.SetBool(m_isTimeManipActiveBoolAnimParam, true);
            foreach (StopwatchTimelineAnimationTransitionSubAnimationController t_subCont in subControllers)
            {
                t_subCont.StartTransitionToTimeline();
            }

            m_onTransitionFinishedAction = onTransitionFinished;
            Invoke(nameof(OnTransitionFinished), m_transitionAnimLength);

            m_onTransitionToTimelineStarted.Invoke();
        }
        public void StartTransitionToStopwatch(Action onTransitionFinished)
        {
            CancelInvoke();

            StartCoroutine(DoActionOnceUnityTimeIsNotLongerPausedCorout(soundCont.PlayTimeResumeSound));

            isTimeManipActive = false;
            m_animator.SetBool(m_isTimeManipActiveBoolAnimParam, false);
            foreach (StopwatchTimelineAnimationTransitionSubAnimationController t_subCont in subControllers)
            {
                t_subCont.StartTransitionToStopwatch();
            }

            m_onTransitionFinishedAction = onTransitionFinished;
            Invoke(nameof(OnTransitionFinished), m_transitionAnimLength);

            m_onTransitionToStopwatchStarted.Invoke();
        }
        public void SkipToEndOfAnimation()
        {
            CancelInvoke();

            soundCont.StopTimeFreezeSound();
            soundCont.StopTimeResumeSound();

            m_animator.SetTrigger(m_skipTriggerAnimParam);

            OnTransitionFinished();
        }


        private void OnTransitionFinished()
        {
            if (m_onTransitionFinishedAction != null)
            {
                m_onTransitionFinishedAction.Invoke();
                m_onTransitionFinishedAction = null;
            }

            m_onTransitionAnimFinished.Invoke();

            if (isTimeManipActive)
            {
                // Timeline
                foreach (StopwatchTimelineAnimationTransitionSubAnimationController t_subCont in subControllers)
                {
                    t_subCont.SkipToTimelineEnd();
                }
            }
            else
            {
                // Stopwatch
                foreach (StopwatchTimelineAnimationTransitionSubAnimationController t_subCont in subControllers)
                {
                    t_subCont.SkipToStopwatchEnd();
                }
            }
        }

        private IEnumerator DoActionOnceUnityTimeIsNotLongerPausedCorout(Action callback)
        {
            yield return new WaitUntil(() => Time.timeScale != 0.0f);
            callback?.Invoke();
        }
    }
}