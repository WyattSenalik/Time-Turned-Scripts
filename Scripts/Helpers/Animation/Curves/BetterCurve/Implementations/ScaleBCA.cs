using UnityEngine;

using NaughtyAttributes;

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// Sets the scale to the value in the better curve animation.
    /// </summary>
    public class ScaleBCA : BetterCurveAnimation
    {
        [SerializeField, Required] private Transform m_affectedTrans = null;

        protected override void TakeCurveAction(float curveValue)
        {
            m_affectedTrans.localScale = new Vector3(curveValue, curveValue, curveValue);
        }
    }
}
