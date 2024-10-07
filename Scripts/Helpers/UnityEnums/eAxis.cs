using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityEnums
{
    public enum eAxis { x, y, z }

    /// <summary>
    /// Extensions for <see cref="eAxis"/>.
    /// </summary>
    public static class eAxisExtensions
    {
        /// <summary>
        /// Gives the corresponding Vector3 direction vector.
        /// x => Vector3.right;
        /// y => Vector3.up;
        /// z => Vector3.forward;
        /// </summary>
        public static Vector3 GetCorrespondingVector(this eAxis curRotAxis)
        {
            switch (curRotAxis)
            {
                case eAxis.x:
                    return Vector3.right;
                case eAxis.y:
                    return Vector3.up;
                case eAxis.z:
                    return Vector3.forward;
                default:
                    Debug.LogError($"Unspecified {nameof(eAxis)} " +
                        $"of value {curRotAxis}");
                    return Vector3.zero;
            }
        }
        public static float GetValueFromEulerAngles(this eAxis rotAxis,
            Vector3 eulerAngles)
        {
            switch (rotAxis)
            {
                case eAxis.x:
                    return eulerAngles.x;
                case eAxis.y:
                    return eulerAngles.y;
                case eAxis.z:
                    return eulerAngles.z;
                default:
                    Debug.LogError($"Unspecified {nameof(eAxis)} " +
                        $"of value {rotAxis}");
                    return Mathf.NegativeInfinity;
            }
        }

        public static Vector3 ReplaceValueInEulerAngles(this eAxis rotAxis,
            Vector3 eulerAngles, float value)
        {
            switch (rotAxis)
            {
                case eAxis.x:
                    eulerAngles.x = value;
                    break;
                case eAxis.y:
                    eulerAngles.y = value;
                    break;
                case eAxis.z:
                    eulerAngles.z = value;
                    break;
                default:
                    Debug.LogError($"Unspecified {nameof(eAxis)} " +
                        $"of value {rotAxis}");
                    break;
            }

            return eulerAngles;
        }
    }
}
