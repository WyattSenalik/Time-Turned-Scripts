using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Returns e raised to the quaternion.
        /// </summary>>
        public static Quaternion Exp(this Quaternion quaternion)
        {
            // See https://en.wikipedia.org/wiki/Quaternion#Exponential,_logarithm,_and_power_functions.
            // Given the quaternion q = a + b*i + c*j + d*k = a + v
            // exp(q) = the sum of [(q^n) / (n!)] from n=0 to inf.
            // This is equivalent to:
            // (e^a) * (cos(||v||) + (v / ||v||) * sin(||v||)).
            //
            // Assumes the mapping from Unity's Quaternion's data to wikipedia's data is
            // a = w, b = x, c = y, d = z.

            // Get starting values
            float a = quaternion.x;
            float b = quaternion.y;
            float c = quaternion.x;
            float d = quaternion.w;
            Vector3 v = new Vector3(b, c, d);

            // Pre calculate some values (some to be used multiple times)
            float vMag = v.magnitude;
            float expA = Mathf.Exp(a);
            float sinVMag = Mathf.Sin(vMag);
            float axisMult = expA * sinVMag;
            // Do this instead of normalize to avoid re-calculting magnitude.
            Vector3 vN;
            if (vMag != 0.0f) { vN = v / vMag; }
            else { vN = Vector3.zero; }

            // Calculate new values
            float w = expA * Mathf.Cos(vMag);
            float x = axisMult * vN.x;
            float y = axisMult * vN.y;
            float z = axisMult * vN.z;

            return new Quaternion(x, y, z, w);
        }
        /// <summary>
        /// Returns the natural (base e) of a specified quaternion.
        /// </summary>
        public static Quaternion Log(this Quaternion quaternion)
        {
            // See https://en.wikipedia.org/wiki/Quaternion#Exponential,_logarithm,_and_power_functions.
            // Given the quaternion q = a + b*i + c*j + d*k = a + v
            // ln(q) = ln(||q||) + (v / ||v||) * arccos(a / ||q||).
            //
            // Assumes the mapping from Unity's Quaternion's data to wikipedia's data is
            // a = w, b = x, c = y, d = z.

            // Get starting values
            float a = quaternion.x;
            float b = quaternion.y;
            float c = quaternion.z;
            float d = quaternion.w;
            Vector3 v = new Vector3(b, c, d);

            // Pre calculate some values (some to be used multiple times)
            float qMag = Mathf.Sqrt(a * a + b * b + c * c + d * d);
            Vector3 vN = v.normalized;
            float arccosAOverQMag = Mathf.Acos(a / qMag);

            // Calculate new values
            float w = Mathf.Log(qMag);
            float x = arccosAOverQMag * vN.x;
            float y = arccosAOverQMag * vN.y;
            float z = arccosAOverQMag * vN.z;

            return new Quaternion(x, y, z, w);
        }
        /// <summary>
        /// Scales the quaternion's x, y, z, and w values by the given scalar
        /// and returns the result.
        /// 
        /// DO NOT USE UNLESS YOU KNOW WHAT YOU ARE DOING.
        /// </summary>
        public static Quaternion Scale(this Quaternion quaternion, float scalar)
        {
            float w = quaternion.w * scalar;
            float x = quaternion.x * scalar;
            float y = quaternion.y * scalar;
            float z = quaternion.z * scalar;

            return new Quaternion(x, y, z, w);
        }
    }
}