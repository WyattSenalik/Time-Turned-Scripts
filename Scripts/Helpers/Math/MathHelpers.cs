using Helpers.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace Helpers.Math
{
    /// <summary>
    /// Collection of math helpers functions.
    /// </summary>
    public static class MathHelpers
    {
        public const float SQRT_TWO_OVER_TWO = 0.70710678118f;

        /// <summary>
        /// Finds the minimum value from the given values,
        /// ignoring the sign. 
        /// </summary>
        public static float MinIgnoreSign(params float[] values)
        {
            if (values.Length == 0)
            {
                Debug.LogError("Array with length 0 passed into function.");
                return 0f;
            }

            int temp_minValueIndex = 0;
            float temp_minValue = Mathf.Abs(values[temp_minValueIndex]);
            for (int i = 1; i < values.Length; i++)
            {
                float temp_curVal = values[i];
                float temp_curAbsVal = Mathf.Abs(temp_curVal);
                if (temp_curAbsVal < temp_minValue)
                {
                    temp_minValueIndex = i;
                    temp_minValue = temp_curVal;
                }
            }

            return values[temp_minValueIndex];
        }
        /// <summary>
        /// Faster equivalent of number % 2 == 0.
        /// Uses bitwise operators instead of modulus operator.
        /// </summary>
        public static bool IsEven(int number)
        {
            // By or-ing the last bit of the number, we either:
            // 1. Change the number (last bit was not 1) -> means its even.
            // 2. Number stays same (last bit was 1) -> means its odd.
            int t_numAndOne = (number | 1);
            return number != t_numAndOne;
        }
        /// <summary>
        /// Faster equivalent of number % 2 != 0.
        /// Uses bitwise operators instead of modulus operator.
        /// </summary>
        public static bool IsOdd(int number) => !IsEven(number);
        /// <summary>
        /// Faster equivalent of number / 2.
        /// Uses bitwise shift right instead of division.
        /// </summary>
        public static int FastDivideBy2(int number) => number >> 1;
        /// <summary>
        /// Faster equivalent of number / 2.
        /// Uses bitwise shift right instead of division.
        /// </summary>
        public static Vector2Int FastDivideBy2(Vector2Int vector) => new Vector2Int(vector.x >> 1, vector.y >> 1);
        /// <summary>
        /// Faster equivalent of number * 2.
        /// Uses bitwise shift right instead of division.
        /// </summary>
        public static int FastMultiplyBy2(int number) => number << 1;

        /// <summary>
        /// Converts <paramref name="value"/> to be between 0 and 1 where <paramref name="min"/> is the lower bound and <paramref name="max"/> is the upper bound. 
        /// If <paramref name="value"/> = <paramref name="min"/>, returns 0. 
        /// If <paramref name="value"/> = <paramref name="max"/>, returns 1.
        /// If <paramref name="value"/> is halfway between <paramref name="min"/> and <paramref name="max"/>, return 0.5.
        /// 
        /// If <paramref name="value"/> is outside the bounds set by <paramref name="min"/> and <paramref name="max"/>, will clamp  <paramref name="value"/> to be in bounds.
        /// Assumes <paramref name="min"/> is less than <paramref name="max"/>.
        /// </summary>
        public static float Normalize(float value, float min, float max)
        {
            float t = NormalizeUnclamped(value, min, max);
            return Mathf.Clamp01(t);
        }
        /// <summary>
        /// Same as <see cref="Normalize"/> but <paramref name="value"/> may be be outside the bounds set by <paramref name="min"/> and <paramref name="max"/>.
        /// 
        /// Assumes <paramref name="min"/> is less than <paramref name="max"/>.
        /// </summary>
        public static float NormalizeUnclamped(float value, float min, float max)
        {
            // Interpolation percent. Commonly referred to as t.
            // For interpolation, this value should be in the range [0, 1].
            // For extrapolation, this value should be within the ranges
            // (-infinity, 0) U (1, infinity).
            float t_denom = max - min;
            float t = 0;
            if (t_denom != 0)
            {
                t = (value - min) / t_denom;
            }

            return t;
        }
        /// <summary>
        /// Converts the value in a base where min0 and max0 are bounds into a corresponding value between min1 and max1 such that the percentage between the min and max is the same.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min0"></param>
        /// <param name="max0"></param>
        /// <param name="min1"></param>
        /// <param name="max1"></param>
        /// <returns></returns>
        public static float ChangeBase(float value, float min0, float max0, float min1, float max1)
        {
            float t = NormalizeUnclamped(value, min0, max0);
            return Mathf.LerpUnclamped(min1, max1, t);
        }
        /// <summary>
        /// Rounds the value to the nearest (a * increment) where a is a whole number.
        /// </summary>
        public static float RoundDownToNearestMultiple(float value, float increment)
        {
            return Mathf.Ceil(value / increment) * increment;
        }
        /// <summary>
        /// Returns where the point should be if it were scaled from the position with the given relative scale factor.
        /// </summary>
        /// <param name="point">Point to be scaled.</param>
        /// <param name="position">Pivot that is being scaled about.</param>
        /// <param name="relativeScaleFactor">How much the point is to be scaled.</param>
        /// <returns>New position the point should be located at.</returns>
        public static Vector3 ScalePointAboutPosition(Vector3 point, Vector3 position, float relativeScaleFactor)
        {
            Vector3 t_pointStartPos = point;
            Vector3 t_diff = t_pointStartPos - position; // diff from object pivot to desired pivot/origin
            // calc final position post-scale
            return position + t_diff * relativeScaleFactor;
        }
        /// <summary>
        /// Approximates the integral of a given function (1 float input, 1 float output) across the domain [xMin, xMax] using a left reimann sum.
        /// </summary>
        /// <param name="xMin">Lower domain value.</param>
        /// <param name="xMax">Upper domain value.</param>
        /// <param name="function">Function on which reimann sum is being calculated.</param>
        /// <param name="numRectangles">Number of rectangles the reimann sum will use.</param>
        public static float LeftReimannSum(float xMin, float xMax, Func<float, float> function, int numRectangles)
        {
            #region Asserts
            //CustomDebug.AssertIsTrue(numRectangles > 0, $"numRectangles ({numRectangles}) to be greater than 0", nameof(MathHelpers.LeftReimannSum));
            #endregion Asserts
            numRectangles = Mathf.Max(1, numRectangles);

            float t_deltaX = (xMax - xMin) / numRectangles;
            float t_sum = 0.0f;
            for (int i = 0; i < numRectangles; ++i)
            {
                float t_curX = xMin + t_deltaX * i;
                t_sum += function.Invoke(t_curX);
            }
            return t_sum * t_deltaX;
        }
        /// <summary>
        /// Approximates the integral of a given function (1 float input, 1 float output) in the domain [xMin, xMax] using a right reimann sum.
        /// </summary>
        /// <param name="xMin">Lower domain value.</param>
        /// <param name="xMax">Upper domain value.</param>
        /// <param name="function">Function on which reimann sum is being calculated.</param>
        /// <param name="numRectangles">Number of rectangles the reimann sum will use.</param>
        public static float RightReimannSum(float xMin, float xMax, Func<float, float> function, int numRectangles)
        {
            #region Asserts
            //CustomDebug.AssertIsTrue(numRectangles > 0, $"numRectangles ({numRectangles}) to be greater than 0", nameof(MathHelpers.RightReimannSum));
            #endregion Asserts
            numRectangles = Mathf.Max(1, numRectangles);

            float t_deltaX = (xMax - xMin) / numRectangles;
            float t_sum = 0.0f;
            for (int i = 0; i < numRectangles; ++i)
            {
                float t_curX = xMin + t_deltaX * i;
                t_sum += function.Invoke(t_curX);
            }
            return t_sum * t_deltaX;
        }


        /// <summary>
        /// <see cref="Mathf.LerpAngle(float, float, float)"/> but unclamped.
        /// </summary>
        public static float LerpAngleUnclamped(float a, float b, float t)
        {
            float t_diff = AngleHelpers.RestrictAngle(b - a);

            float t_delta = Mathf.Repeat(t_diff, 360.0f);
            if (t_delta > 180.0f)
            {
                t_delta -= 360.0f;
            }

            float t_angle = Mathf.LerpUnclamped(a, a + t_delta, t);
            return AngleHelpers.RestrictAngle(t_angle);
        }


        /// <summary>
        /// Creates a list that contains all unique values in the array once and only once.
        /// </summary>
        public static List<int> CreateListOfUniqueValues(params int[] values)
        {
            if (values.Length == 0) { return new List<int>(); }

            List<int> t_rtnList = new List<int>(values.Length) { values[0] };
            for (int i = 1; i < values.Length; ++i)
            {
                int t_curVal = values[i];
                if (!t_rtnList.Contains(t_curVal))
                {
                    t_rtnList.Add(t_curVal);
                }
            }
            return t_rtnList;
        }
        public static List<int> CreateListOfUniqueValues(IReadOnlyList<int> values)
        {
            if (values.Count == 0) { return new List<int>(); }

            List<int> t_rtnList = new List<int>(values.Count) { values[0] };
            for (int i = 1; i < values.Count; ++i)
            {
                int t_curVal = values[i];
                if (!t_rtnList.Contains(t_curVal))
                {
                    t_rtnList.Add(t_curVal);
                }
            }
            return t_rtnList;
        }
    }
}
