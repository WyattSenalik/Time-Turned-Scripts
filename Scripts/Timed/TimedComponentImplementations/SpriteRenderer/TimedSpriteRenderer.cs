using Helpers.Extensions;
using NaughtyAttributes;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// Records moments of when the SpriteRenderer changes Sprites.
    /// </summary>
    public sealed class TimedSpriteRenderer : TimedRecorder
    {
        private const bool IS_DEBUGGING = false;

        public TimedBool timedRendEnabled => m_timedRendEnabled;
        public TimedSprite timedSpr => m_timedSpr;
        public TimedBool timedFlipX => m_timedFlipX;
        public TimedInt timedSortingOrder => m_timedSortingOrder;
        public new SpriteRenderer renderer => m_renderer;

        [SerializeField] private bool m_useSerializedRenderer = false;
        [SerializeField, Required, ShowIf(nameof(m_useSerializedRenderer))] private SpriteRenderer m_serializedRenderer = null;

        private SpriteRenderer m_renderer = null;

        private TimedBool m_timedRendEnabled = null;
        private TimedSprite m_timedSpr = null;
        private TimedBool m_timedFlipX = null;
        private TimedInt m_timedSortingOrder = null;
        private bool m_isInitialized = false;


        protected override void Awake()
        {
            base.Awake();

            if (m_useSerializedRenderer)
            {
                m_renderer = m_serializedRenderer;
            }
            else
            {
                m_renderer = this.GetComponentSafe<SpriteRenderer>(this);
            }
        }
        private void Start()
        {
            m_timedRendEnabled = new TimedBool(m_renderer.enabled);
            m_timedSpr = new TimedSprite(m_renderer.sprite);
            m_timedFlipX = new TimedBool(m_renderer.flipX);
            m_timedSortingOrder = new TimedInt(m_renderer.sortingOrder, eInterpolationOption.Step);

            m_isInitialized = true;
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);
            if (!enabled) { return; }
            if (!m_isInitialized) { return; }

            if (isRecording)
            {
                // Update timed cur data
                m_timedRendEnabled.curData = m_renderer.enabled;
                m_timedSpr.curData = new SpriteData(m_renderer.sprite);
                m_timedFlipX.curData = m_renderer.flipX;
                m_timedSortingOrder.curData = m_renderer.sortingOrder;
            }
            else
            {
                #region Guard Checks
                if (m_renderer == null)
                {
                    //CustomDebug.LogErrorForComponent($"Renderer is null", this);
                    return;
                }
                if (m_timedRendEnabled == null)
                {
                    //CustomDebug.LogErrorForComponent($"TimedRendEnabled is null", this);
                    return;
                }
                #endregion Guard Checks

                // Set the renderer
                m_renderer.enabled = m_timedRendEnabled.curData;
                m_renderer.sprite = m_timedSpr.curData.sprite;
                m_renderer.flipX = m_timedFlipX.curData;
                m_renderer.sortingOrder = m_timedSortingOrder.curData;
            }
        }
    }
}