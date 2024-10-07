using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// Fades the alpha of an Image over time using a BCA (Better Curve Animation).
    /// </summary>
    public class ChangeImageAlphaBCA : BetterCurveAnimation
    {
        [SerializeField, Required] private Image m_img = null;

        private void Awake()
        {
            //CustomDebug.AssertSerializeFieldIsNotNull(m_img, nameof(m_img), this);
        }

        protected override void TakeCurveAction(float curveValue)
        {
            Color temp_imgColor = m_img.color;
            temp_imgColor.a = curveValue;
            m_img.color = temp_imgColor;
        }
    }
}
