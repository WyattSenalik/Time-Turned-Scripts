using UnityEngine;
// Original Authors - Wyatt Senalik (Created referencing BetterCurve)

namespace Helpers.Animation.BetterCurve
{
    public interface IColorCurve : ICurve
    {
        Color minValue { get; }
        Color maxValue { get; }


        Color Evaluate(float time);
    }
}