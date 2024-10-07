using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TimedSpriteRendererColor : TimedRecorder
    {
        private SpriteRenderer m_sprRend = null;

        private TimedFloat m_redValue = null;
        private TimedFloat m_greenValue = null;
        private TimedFloat m_blueValue = null;
        private TimedFloat m_alphaValue = null;

        protected override void Awake()
        {
            base.Awake();
            m_sprRend = this.GetComponentSafe<SpriteRenderer>();
        }
        private void Start()
        {
            Color t_color = m_sprRend.color;
            m_redValue = new TimedFloat(t_color.r);
            m_greenValue = new TimedFloat(t_color.g);
            m_blueValue = new TimedFloat(t_color.b);
            m_alphaValue = new TimedFloat(t_color.a);
        }
        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (m_redValue == null || m_greenValue == null || m_blueValue == null || m_alphaValue == null) { return; }

            if (isRecording)
            {
                Color t_color = m_sprRend.color;
                m_redValue.curData = t_color.r;
                m_greenValue.curData = t_color.g;
                m_blueValue.curData = t_color.b;
                m_alphaValue.curData = t_color.a;
            }
            else
            {
                Color t_color;
                t_color.r = m_redValue.curData;
                t_color.g = m_greenValue.curData;
                t_color.b = m_blueValue.curData;
                t_color.a = m_alphaValue.curData;
                m_sprRend.color = t_color;
            }
        }
    }
}