using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// Smoothly translates position using a BetterCurveAnimation.
    /// </summary>
    public class TranslateBCA : BetterCurveAnimation
    {

        [SerializeField] private bool m_changesX = false;
        [SerializeField] private bool m_changesY = false;
        [SerializeField] private bool m_changesZ = false;
        [SerializeField] private Transform[] m_affectedTransforms = new Transform[1];

        protected override void TakeCurveAction(float curveValue)
        {

            foreach (Transform trans in m_affectedTransforms)
            {
                Vector3 temp_pos = trans.position;
                if (m_changesX) { temp_pos.x += curveValue; }
                if (m_changesY) { temp_pos.y += curveValue; }
                if (m_changesZ) { temp_pos.z += curveValue; }
                trans.position = temp_pos;
            }
        }
    }
}
