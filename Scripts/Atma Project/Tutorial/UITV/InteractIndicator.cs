using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Helpers.Singletons;
using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(TimedObject))]
    public sealed class InteractIndicator : SingletonMonoBehaviour<InteractIndicator>, ITimedRecorder
    {
        public ITimedObject timedObject { get; private set; } = null;
        public bool isRecording => timedObject.isRecording;

        [SerializeField] private GameObject m_visuals = null;

        protected override void Awake()
        {
            base.Awake();

            timedObject = this.GetIComponentSafe<ITimedObject>();
        }

        public void Show()
        {
            if (!isRecording) { return; }
            m_visuals.SetActive(true);
        }
        public void Hide()
        {
            m_visuals.SetActive(false);
        }
        public void MoveTo(Vector2 pos, bool show) => MoveTo(pos, Vector2.zero, show);
        public void MoveTo(Vector2 pos, Vector2? offset = null, bool show = true)
        {
            if (!offset.HasValue)
            {
                offset = Vector2.zero;
            }
            transform.position = pos + offset.Value;

            if (show)
            {
                Show();
            }
        }

        public void SetToTime(float time) { }
        public void OnRecordingStop(float time)
        {
            Hide();
        }
        public void OnRecordingResume(float time) { }
        public void TrimDataAfter(float time) { }
    }
}