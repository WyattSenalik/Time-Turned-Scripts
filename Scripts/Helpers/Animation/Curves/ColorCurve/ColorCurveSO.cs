using UnityEngine;

using static Helpers.Animation.BetterCurve.BetterCurve;
// Original Authors - Wyatt Senalik (Created referencing BetterCurveSO)

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// ScriptableObject for <see cref="ColorCurve"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new ColorCurve", menuName = "ScriptableObjects/Curves/ColorCurve")]
    public sealed class ColorCurveSO : ScriptableObject, IColorCurve
    {
        public Color minValue => m_curve.minValue;
        public Color maxValue => m_curve.maxValue;
        public eTimeChoice timing => m_curve.timing;
        public float timeScale => m_curve.timeScale;
        public float timeDuration => m_curve.timeDuration;

        [SerializeField] private ColorCurve m_curve = new ColorCurve();


        public Color Evaluate(float time) => m_curve.Evaluate(time);
        public float GetEndTime() => m_curve.GetEndTime();
    }
}
