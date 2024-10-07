using UnityEngine;

using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Helpers.Math
{
    /// <summary>
    /// Static class to help with angles.
    /// </summary>
    public static class AngleHelpers
    {
        private const bool IS_DEBUGGING = false;

        /// <summary>
        /// Restricts the given angle to be between -180.0f and 180.0f.
        ///
        /// If given 360.0f, will return 0.0f.
        /// If given -500.0f, will return -140.0f.
        /// </summary>
        public static float RestrictAngle(float angle)
        {
            float temp_original = angle;
            while (angle > 180.0f)
            {
                //CustomDebug.Log($"Angle is greater than 180", IS_DEBUGGING);
                angle -= 360.0f;
            }
            while (angle <= -180.0f)
            {
                //CustomDebug.Log($"Angle is less than -180", IS_DEBUGGING);
                angle += 360.0f;
            }

            //CustomDebug.Log($"Restricted {temp_original} to {angle}", IS_DEBUGGING);

            return angle;
        }
        /// <summary>
        /// Clamps all values in the given vector
        /// to be between -180.0f and 180.0f.
        /// </summary>
        public static Vector3 RestrictAngles(Vector3 angles)
        {
            Vector3 temp_original = angles;

            angles.x = RestrictAngle(angles.x);
            angles.y = RestrictAngle(angles.y);
            angles.z = RestrictAngle(angles.z);

            //CustomDebug.Log($"Restricted {temp_original} to {angles}", IS_DEBUGGING);

            return angles;
        }
        /// <summary>
        /// Restricts the given angle to be between -180.0f and 180.0f.
        ///
        /// If given 360.0f, will return 0.0f.
        /// If given -500.0f, will return -140.0f.
        /// </summary>
        public static int RestrictAngle(int angle)
        {
            while (angle > 180)
            {
                angle -= 360;
            }
            while (angle <= -180)
            {
                angle += 360;
            }
            return angle;
        }
        /// <summary>
        /// Clamps the given angle between the specified min and max.
        /// The value, min, and max will be restricted between -180.0f and 180.0f before clamped.
        /// </summary>
        public static float ClampRestrictAngle(float value, float min, float max)
        {
            value = RestrictAngle(value);
            min = RestrictAngle(min);
            max = RestrictAngle(max);

            return Mathf.Clamp(value, min, max);
        }
        /// <summary>
        /// Clamps one channel of the given vector (chosen by the rotation axis) to be between
        /// the min and maximum.
        ///
        /// Restricts the angles of that channel, the min, and the max
        /// to be between -180.0f before clamping.
        /// </summary>
        /// <param name="angles">Vector containing channel to clamp.</param>
        /// <param name="rotAxis">Which channel to clamp.</param>
        /// <param name="min">Between -180.0f and 180.0f.</param>
        /// <param name="max">Between -180.0f and 180.0f.</param>
        public static Vector3 ClampAnglesAlongRotationAxis(Vector3 angles,
            eAxis rotAxis, float min, float max)
        {
            switch (rotAxis)
            {
                case eAxis.x:
                    angles.x = ClampRestrictAngle(angles.x, min, max);
                    break;
                case eAxis.y:
                    angles.y = ClampRestrictAngle(angles.y, min, max);
                    break;
                case eAxis.z:
                    angles.z = ClampRestrictAngle(angles.z, min, max);
                    break;
                default:
                    Debug.LogError($"Unspecified {nameof(eAxis)} of value {rotAxis}");
                    break;
            }
            return angles;
        }
        /// <summary>
        /// Converts the given angle to its positive equivalent.
        /// 
        /// Example: If given -90.0f, returns 270.0f.
        /// </summary>
        public static float ToPositiveAngle(float angle)
        {
            if (angle > 0.0f) { return angle; }
            while (angle < 0.0f) { angle += 360.0f; }
            return angle;
        }
        /// <summary>
        /// Converts the given angles to their positive equivalent.
        /// 
        /// Example: If given -90.0f, returns 270.0f.
        /// </summary>
        public static Vector3 ToPositiveAngles(Vector3 angles)
        {
            float xAngle = ToPositiveAngle(angles.x);
            float yAngle = ToPositiveAngle(angles.y);
            float zAngle = ToPositiveAngle(angles.z);
            return new Vector3(xAngle, yAngle, zAngle);
        }
        /// <summary>
        /// Subtracts a from b (b - a) accounts for wrapping.
        /// 
        /// Restricts the angle between -180.0f and 180.0f
        /// </summary>
        public static float SubtractAngles(float a, float b)
        {
            float posA = ToPositiveAngle(a);
            float posB = ToPositiveAngle(b) + 360.0f;
            float rawDiff = posB - posA;
            return RestrictAngle(rawDiff);
        }
    }
}
