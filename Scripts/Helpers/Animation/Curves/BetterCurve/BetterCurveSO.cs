using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// ScriptableObject for <see cref="BetterCurve"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new BettterCurve", menuName = "ScriptableObjects/Curves/BetterCurve")]
    public class BetterCurveSO : ScriptableObject, IBetterCurve
    {
        public float minValue => m_curve.minValue;
        public float maxValue => m_curve.maxValue;
        public BetterCurve.eTimeChoice timing => m_curve.timing;
        public float timeScale => m_curve.timeScale;
        public float timeDuration => m_curve.timeDuration;

        [SerializeField] private BetterCurve m_curve = new BetterCurve();


        public float Evaluate(float time) => m_curve.Evaluate(time);
        public float EvaluateClamped(float time) => m_curve.EvaluateClamped(time);
        public float GetEndTime() => m_curve.GetEndTime();
    }
}