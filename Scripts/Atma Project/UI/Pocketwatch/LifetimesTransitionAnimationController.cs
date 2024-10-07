using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;

using Atma.Events;
using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class LifetimesTransitionAnimationController : StopwatchTimelineAnimationTransitionSubAnimationController
    {
        [SerializeField, Required] private LifetimeManager m_stopwatchLifetimeMan = null;
        [SerializeField, Required] private LifetimeManager m_timelineLifetimeMan = null;
        [SerializeField, Required] private CloneCreatedEventIdentifierSO m_cloneCreatedEventIDSO = null;
        [SerializeField, Min(1)] private int m_frameToFinishStopwatchFadeOutToTimelineAnim = 8;
        [SerializeField, Min(1)] private int m_frameToStartTimelineFadeInToTimelineAnim = 25;
        [SerializeField, Min(1)] private int m_frameToFinishTimelineFadeOutToStopwatchAnim = 4;
        [SerializeField, Min(1)] private int m_frameToStartStopwatchFadeInToStopwatchAnim = 21;

        private Image[] m_stopwatchLifetimesImgs = new Image[0];
        private Image[] m_timelineLifetimesImgs = new Image[0];

        private float m_stopwatchLifetimesStartAlpha = 0.0f;

        private SubManager<ICloneCreatedContext> m_cloneCreatedSubMan = null;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedEventIDSO, nameof(m_cloneCreatedEventIDSO), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_cloneCreatedSubMan = new SubManager<ICloneCreatedContext>(m_cloneCreatedEventIDSO, OnCloneCreated);
            m_cloneCreatedSubMan.Subscribe();
        }
        private void OnDestroy()
        {
            m_cloneCreatedSubMan.Unsubscribe();
        }


        public override void StartTransitionToTimeline()
        {
            m_stopwatchLifetimeMan.enabled = false;
            m_timelineLifetimeMan.enabled = false;

            m_stopwatchLifetimesImgs = m_stopwatchLifetimeMan.GetComponentsInChildren<Image>();
            m_timelineLifetimesImgs = m_timelineLifetimeMan.GetComponentsInChildren<Image>();

            if (m_stopwatchLifetimesImgs.Length > 0)
            {
                m_stopwatchLifetimesStartAlpha = m_stopwatchLifetimesImgs[0].color.a;
            }

            base.StartTransitionToTimeline();
        }
        public override void StartTransitionToStopwatch()
        {
            m_stopwatchLifetimeMan.enabled = false;
            m_timelineLifetimeMan.enabled = false;

            m_stopwatchLifetimesImgs = m_stopwatchLifetimeMan.GetComponentsInChildren<Image>(true);
            m_timelineLifetimesImgs = m_timelineLifetimeMan.GetComponentsInChildren<Image>(true);

            base.StartTransitionToStopwatch();
        }


        protected override void ToTimelineFrameAction(int frame)
        {
            // Fade out stopwatch
            if (frame < m_frameToFinishStopwatchFadeOutToTimelineAnim)
            {
                float t = ((float)frame) / m_frameToFinishStopwatchFadeOutToTimelineAnim;

                float t_newAlpha = Mathf.Lerp(m_stopwatchLifetimesStartAlpha, 0.0f, t);
                foreach (Image t_img in m_stopwatchLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = t_newAlpha;
                    t_img.color = t_col;
                }
            }
            else
            {
                foreach (Image t_img in m_stopwatchLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = 0.0f;
                    t_img.color = t_col;
                }
            }

            // Fade in timeline
            if (frame < m_frameToStartTimelineFadeInToTimelineAnim)
            {
                foreach (Image t_img in m_timelineLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = 0.0f;
                    t_img.color = t_col;
                }
            }
            else
            {
                int t_framesSinceStart = (frame - m_frameToStartTimelineFadeInToTimelineAnim);
                int t_totalAmountFrames = stopwatchTimelineAnimCont.totalAmountFrames;
                int t_framesToExecute = (t_totalAmountFrames - m_frameToStartTimelineFadeInToTimelineAnim);
                float t = ((float)t_framesSinceStart) / t_framesToExecute;
                float t_newAlpha = Mathf.LerpAngle(0.0f, 1.0f, t);

                foreach (Image t_img in m_timelineLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = t_newAlpha;
                    t_img.color = t_col;
                }
            }
        }
        protected override void ToStopwatchFrameAction(int frame)
        {
            // Fade out timeline
            if (frame < m_frameToFinishTimelineFadeOutToStopwatchAnim)
            {
                float t = ((float)frame) / m_frameToFinishTimelineFadeOutToStopwatchAnim;

                float t_newAlpha = Mathf.Lerp(1.0f, 0.0f, t);
                foreach (Image t_img in m_timelineLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = t_newAlpha;
                    t_img.color = t_col;
                }
            }
            else
            {
                foreach (Image t_img in m_timelineLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = 0.0f;
                    t_img.color = t_col;
                }
            }

            // Fade in stopwatch
            if (frame < m_frameToStartStopwatchFadeInToStopwatchAnim)
            {
                foreach (Image t_img in m_stopwatchLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = 0.0f;
                    t_img.color = t_col;
                }
            }
            else
            {
                int t_framesSinceStart = (frame - m_frameToStartStopwatchFadeInToStopwatchAnim);
                int t_totalAmountFrames = stopwatchTimelineAnimCont.totalAmountFrames;
                int t_framesToExecute = (t_totalAmountFrames - m_frameToStartStopwatchFadeInToStopwatchAnim);
                float t = ((float)t_framesSinceStart) / t_framesToExecute;
                float t_newAlpha = Mathf.LerpAngle(0.0f, m_stopwatchLifetimesStartAlpha, t);

                foreach (Image t_img in m_stopwatchLifetimesImgs)
                {
                    if (t_img == null) { continue; }
                    Color t_col = t_img.color;
                    t_col.a = t_newAlpha;
                    t_img.color = t_col;
                }
            }

        }

        protected override void EndToTimelineTransition()
        {
            m_stopwatchLifetimeMan.enabled = true;
            m_timelineLifetimeMan.enabled = true;

            foreach (Image t_img in m_stopwatchLifetimesImgs)
            {
                if (t_img == null) { continue; }
                Color t_col = t_img.color;
                t_col.a = 0.0f;
                t_img.color = t_col;
            }
            foreach (Image t_img in m_timelineLifetimesImgs)
            {
                if (t_img == null) { continue; }
                Color t_col = t_img.color;
                t_col.a = 1.0f;
                t_img.color = t_col;
            }
        }
        protected override void EndToStopwatchTransition()
        {
            m_stopwatchLifetimeMan.enabled = true;
            m_timelineLifetimeMan.enabled = true;

            foreach (Image t_img in m_stopwatchLifetimesImgs)
            {
                if (t_img == null) { continue; }
                Color t_col = t_img.color;
                t_col.a = m_stopwatchLifetimesStartAlpha;
                t_img.color = t_col;
            }
            foreach (Image t_img in m_timelineLifetimesImgs)
            {
                if (t_img == null) { continue; }
                Color t_col = t_img.color;
                t_col.a = 0.0f;
                t_img.color = t_col;
            }
        }


        private void OnCloneCreated(ICloneCreatedContext context)
        {
            // When a clone is created, grab the images the frame after its created so that they have time to be created. Also need to redisable
            StartCoroutine(GrabImageReferencesAfterAFrame());
        }
        private IEnumerator GrabImageReferencesAfterAFrame()
        {
            yield return null;

            m_stopwatchLifetimesImgs = m_stopwatchLifetimeMan.GetComponentsInChildren<Image>(true);
            m_timelineLifetimesImgs = m_timelineLifetimeMan.GetComponentsInChildren<Image>(true);

            if (m_stopwatchLifetimesImgs.Length > 0)
            {
                m_stopwatchLifetimesStartAlpha = m_stopwatchLifetimesImgs[0].color.a;
            }
        }
    }
}