using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// If the given bounds is completely inside of this bounds.
        /// </summary>
        public static bool Contains(this Bounds bounds, Bounds other)
        {
            return bounds.Contains(other.min) && bounds.Contains(other.max);
        }
    }
}