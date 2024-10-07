using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Animation;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SwitchPressurePlateHandler))]
    public sealed class PressurePlateAnimator : TimedRecorder
    {
        [SerializeField, Required] private SpriteRenderer m_bodySprRend = null;
        [SerializeField, Required] private SpriteRenderer m_onSymbolSprRend = null;
        [SerializeField, Required] private SpriteRenderer m_offSymbolSprRend = null;

        [SerializeField] private ManualSpriteAnimation m_onAnim = new ManualSpriteAnimation();
        [SerializeField] private ManualSpriteAnimation m_offAnim = new ManualSpriteAnimation();

        private SwitchPressurePlateHandler m_ppHandler = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_bodySprRend, nameof(m_bodySprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_onSymbolSprRend, nameof(m_onSymbolSprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_offSymbolSprRend, nameof(m_offSymbolSprRend), this);
            #endregion Asserts
            m_ppHandler = this.GetComponentSafe<SwitchPressurePlateHandler>(this);
        }
        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            m_onSymbolSprRend.enabled = m_ppHandler.isActive;
            m_offSymbolSprRend.enabled = !m_ppHandler.isActive;

            Sprite t_spr;
            if (m_ppHandler.isActive)
            {
                ManualSpriteAnimation.Frame t_frame = m_onAnim.GetFrameForTime(0.0f, time);
                t_spr = t_frame.sprite;
            }
            else
            {
                ManualSpriteAnimation.Frame t_frame = m_offAnim.GetFrameForTime(0.0f, time);
                t_spr = t_frame.sprite;
            }
            m_bodySprRend.sprite = t_spr;
        }
    }
}