using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public class FixLeapingColliderScales : MonoBehaviour
    {
        private void LateUpdate()
        {
            Vector3 t_fixedLocalScale = transform.localScale;
            Vector3 t_correctedLossyScale = transform.lossyScale;
            t_correctedLossyScale.x = t_correctedLossyScale.x / t_fixedLocalScale.x;
            t_correctedLossyScale.y = t_correctedLossyScale.y / t_fixedLocalScale.y;

            t_fixedLocalScale.x = 1 / t_correctedLossyScale.x;
            t_fixedLocalScale.y = 1 / t_correctedLossyScale.y;

            if (float.IsInfinity(t_fixedLocalScale.x) || float.IsNaN(t_fixedLocalScale.x))
            {
                t_fixedLocalScale.x = 1.0f;
            }
            if (float.IsInfinity(t_fixedLocalScale.y) || float.IsNaN(t_fixedLocalScale.y))
            {
                t_fixedLocalScale.y = 1.0f;
            }
            transform.localScale = t_fixedLocalScale;
        }
    }
}