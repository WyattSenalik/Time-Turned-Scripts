using System;
using UnityEngine;

using Helpers.Math;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Extensions for <see cref="IRunCurves"/>.
    /// </summary>
    public static class IRunCurvesExtensions
    {
        /// <summary>
        /// Approximates how many unity units the acceleration part of the run curve will traverse.
        /// </summary>
        /// <param name="numRectangles">How many rectangles the reimann sum will use to approximate. More rectangles = more accurate.</param>
        public static float ApproximateDistanceAccelerationTravels(this IRunCurves runCurves, int numRectangles)
        {
            AnimationCurve t_accelCurve = runCurves.accelCurve;
            float t_accelTime = runCurves.accelTime;
            float t_topSpeed = runCurves.topSpeed;

            Func<float, float> t_reimannFunc = (float x) =>
            {
                // Curve is in range [0, 1] but x is in range [0, accelTime]. Need to correct.
                float t_curveEval = t_accelCurve.Evaluate(x / t_accelTime);
                return t_curveEval * runCurves.topSpeed;
            };

            return MathHelpers.LeftReimannSum(0.0f, t_accelTime, t_reimannFunc, numRectangles);
        }
        /// <summary>
        /// Approximates how many unity units the deceleration part of the run curve will traverse.
        /// </summary>
        /// <param name="numRectangles">How many rectangles the reimann sum will use to approximate. More rectangles = more accurate.</param>
        public static float ApproximateDistanceDeclerationTravels(this IRunCurves runCurves, int numRectangles)
        {
            AnimationCurve t_decelCurve = runCurves.decelCurve;
            float t_decelTime = runCurves.decelTime;
            float t_topSpeed = runCurves.topSpeed;

            Func<float, float> t_reimannFunc = (float x) =>
            {
                // Curve is in range [0, 1] but x is in range [0, decelTime]. Need to correct.
                float t_curveEval = t_decelCurve.Evaluate(x / t_decelTime);
                return t_curveEval * runCurves.topSpeed;
            };

            return MathHelpers.RightReimannSum(0.0f, t_decelTime, t_reimannFunc, numRectangles);
        }
    }
}