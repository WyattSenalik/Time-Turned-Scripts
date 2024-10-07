using UnityEngine;

using Timed;
using NaughtyAttributes;
using static Atma.PushableBoxCollider;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class BoxPushingDetector : WindowRecorder<PushBox>
    {
        public bool isABoxBeingPushed => boxBeingPushed != null;
        public PushBox boxBeingPushed { get; private set; }

        [SerializeField, Required] private PushableBoxCollider m_pusherCollider = null;

        private bool m_isCollisionDirty = false;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pusherCollider, nameof(m_pusherCollider), this);
            #endregion Asserts
        }
        private void Start()
        {
            StartNewWindow(null);
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            if (!isRecording)
            {
                m_isCollisionDirty = false;
                return;
            }

            if (HasCurrentWindow())
            {
                if (!m_isCollisionDirty)
                {
                    // Collision ended, so end current window
                    UpdateWindow(null);
                }
            }

            m_isCollisionDirty = false;
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_pusherCollider != null)
            {
                m_pusherCollider.onCollision.ToggleSubscription(HandleCollision, cond);
            }
        }
        private void HandleCollision(CollisionInfo colInfo)
        {
            // Ignore if not recording.
            if (!isRecording) { return; }

            // Not a box, boxes are neither immobile or only pushers.
            if (colInfo.otherCollider.isImmobile || colInfo.otherCollider.isPusherOnly) { return; }
            // Doesn't have box script.
            if (!colInfo.otherCollider.rootTransform.TryGetComponent(out PushBox t_pushBox)) { return; }

            if (boxBeingPushed != t_pushBox)
            {
                UpdateWindow(t_pushBox);
            }

            m_isCollisionDirty = true;
        }

        protected override void SetToTimeRewindingDuringWindow(WindowData<PushBox> windowData)
        {
            base.SetToTimeRewindingDuringWindow(windowData);

            boxBeingPushed = windowData.data;
        }
        protected override void OnRecordingResumeDuringWindow(WindowData<PushBox> windowResumedDuring)
        {
            base.OnRecordingResumeDuringWindow(windowResumedDuring);

            boxBeingPushed = windowResumedDuring.data;
        }


        private void UpdateWindow(PushBox newBox)
        {
            if (HasCurrentWindow())
            {
                EndCurrentWindow();
            }
            StartNewWindow(newBox);
            boxBeingPushed = newBox;
        }
    }
}