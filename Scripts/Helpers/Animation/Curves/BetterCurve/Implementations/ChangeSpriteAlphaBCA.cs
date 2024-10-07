using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// Changes the alplha of a sprite renderer.
    /// </summary>
    public class ChangeSpriteAlphaBCA : BetterCurveAnimation
    {
        [SerializeField, Required] private SpriteRenderer m_spriteRend = null;

        private void Awake()
        {
            //CustomDebug.AssertSerializeFieldIsNotNull(m_spriteRend, nameof(m_spriteRend), this);
        }

        protected override void TakeCurveAction(float curveValue)
        {
            // Curve value is new alpha
            Color temp_imgColor = m_spriteRend.color;
            temp_imgColor.a = curveValue;
            m_spriteRend.color = temp_imgColor;
        }
    }
}