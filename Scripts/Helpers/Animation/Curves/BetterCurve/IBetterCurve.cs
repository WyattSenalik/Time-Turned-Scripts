// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    public interface IBetterCurve : ICurve
    {
        float minValue { get; }
        float maxValue { get; }


        float Evaluate(float time);
    }
}