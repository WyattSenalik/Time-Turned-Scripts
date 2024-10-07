using Helpers.Math;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    public static class IBetterCurveExtensions
    {
        public static float ApproximateIntegralUsingRiemannSum(this IBetterCurve curve, int numRectangles)
        {
            return MathHelpers.LeftReimannSum(0.0f, curve.GetEndTime(), curve.Evaluate, numRectangles);
        }
    }
}