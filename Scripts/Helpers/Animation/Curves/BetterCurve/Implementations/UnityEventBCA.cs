using UnityEngine;
using UnityEngine.Events;

namespace Helpers.Animation.BetterCurve
{
    public class UnityEventBCA : BetterCurveAnimation
    {
        [SerializeField] private UnityEvent<float> m_curveActions =
            new UnityEvent<float>();

        protected override void TakeCurveAction(float curveValue)
        {
            m_curveActions.Invoke(curveValue);
        }
    }
}
