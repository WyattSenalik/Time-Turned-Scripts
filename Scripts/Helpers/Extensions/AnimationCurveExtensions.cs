using Helpers.Math;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    /// <summary>
    /// Extensions for Unity's <see cref="AnimationCurve"/>.
    /// </summary>
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Calculates a left reimann sum of the current curve.
        /// </summary>
        /// <param name="curve">Curve the reimann sum will be calculated on.</param>
        /// <param name="numRectangles">How many rectangles to use for the reimann sum.</param>
        public static float LeftReimannSum(this AnimationCurve curve, int numRectangles)
        {
            #region Asserts
            //CustomDebug.AssertIsTrue(curve.keys.Length > 0, $"the curve ({curve}) to have at least 1 key", nameof(AnimationCurveExtensions.LeftReimannSum));
            #endregion Asserts
            float t_earliestTime = curve.keys[0].time;
            float t_furthestTime = curve.keys[^1].time;
            return MathHelpers.LeftReimannSum(t_earliestTime, t_furthestTime, curve.Evaluate, numRectangles);
        }
        /// <summary>
        /// Calculates an right reimann sum of the current curve.
        /// </summary>
        /// <param name="curve">Curve the reimann sum will be calculated on.</param>
        /// <param name="numRectangles">How many rectangles to use for the reimann sum.</param>
        public static float RightReimannSum(this AnimationCurve curve, int numRectangles)
        {
            #region Asserts
            //CustomDebug.AssertIsTrue(curve.keys.Length > 0, $"the curve ({curve}) to have at least 1 key", nameof(AnimationCurveExtensions.RightReimannSum));
            #endregion Asserts
            float t_earliestTime = curve.keys[0].time;
            float t_furthestTime = curve.keys[^1].time;
            return MathHelpers.RightReimannSum(t_earliestTime, t_furthestTime, curve.Evaluate, numRectangles);
        }
    }
}