using UnityEngine;

using Timed;
using Timed.TimedComponentImplementations;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(TimeClone))]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TimeCloneSpriteRenderer : TimedRecorder
    {
        private TimeClone m_clone = null;
        private SpriteRenderer m_sprRend = null;

        private TimedBool m_timedRendEnabled = null;
        private TimedSprite m_timedSpr = null;
        private TimedBool m_timedFlipX = null;
        private TimedInt m_timedSortingOrder = null;


        protected override void Awake()
        {
            base.Awake();

            m_clone = this.GetComponentSafe<TimeClone>(this);
            m_sprRend = this.GetComponentSafe<SpriteRenderer>(this);
            m_clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (m_clone != null)
            {
                m_clone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }
        private void Initialize()
        {
            TimeCloneInitData t_cloneData = m_clone.cloneData;

            // Get SpriteRenderer and TimedSpriteRenderer to copy from.
            GameObject t_playerObj = t_cloneData.originalPlayerObj;
            TimedSpriteRenderer t_timedSprRend = t_playerObj.GetComponentSafe<TimedSpriteRenderer>(this);

            m_timedRendEnabled = new TimedBool(t_timedSprRend.timedRendEnabled.scrapbook, true);
            m_timedSpr = new TimedSprite(t_timedSprRend.timedSpr.scrapbook, true);
            m_timedFlipX = new TimedBool(t_timedSprRend.timedFlipX.scrapbook, true);
            m_timedSortingOrder = new TimedInt(t_timedSprRend.timedSortingOrder.scrapbook, true);

            // Apply the data right away
            ApplyCurrentData();
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Early disappearance
            if (m_clone.HasEarlyDisappearanceBeforeOrAtTime(time)) { return; }

            ApplyCurrentData();
        }

        private void ApplyCurrentData()
        {
            m_sprRend.enabled = m_timedRendEnabled.curData;
            m_sprRend.sprite = m_timedSpr.curData.sprite;
            m_sprRend.flipX = m_timedFlipX.curData;
            m_sprRend.sortingOrder = m_timedSortingOrder.curData;
        }
    }
}
