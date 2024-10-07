using UnityEngine;

using NaughtyAttributes;

using Helpers.Physics.Custom2DInt;
// Original Authors - Wyatt Senalik

namespace Timed.Animation.BetterCurve
{
    /// <summary>
    /// Sets the scale to the value in the better curve animation.
    /// Knows about time.
    /// </summary>
    public sealed class ScaleTimedBCA : TimedBetterCurveAnimation
    {
        [SerializeField] private bool m_useIntTrans = false;
        [SerializeField, Required, ShowIf(nameof(useNormalTrans))] private Transform m_affectedTrans = null;
        [SerializeField, Required, ShowIf(nameof(m_useIntTrans))] private Int2DTransform m_affectedIntTrans = null;

        protected override void TakeCurveAction(float curveValue)
        {
            Vector3 t_size = new Vector3(curveValue, curveValue, curveValue);
            if (m_useIntTrans)
            {
                m_affectedIntTrans.localSizeFloat = t_size;
            }
            else
            {
                m_affectedTrans.localScale = t_size;
            }
        }


        private bool useNormalTrans => !m_useIntTrans;
    }
}