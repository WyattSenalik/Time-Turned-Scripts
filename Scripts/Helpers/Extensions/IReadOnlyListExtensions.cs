using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    /// <summary>
    /// Extensions for IReadOnlyList.
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        /// <summary>
        /// Converts the IReadOnlyList to a List. This new List
        /// is a shallow copy of the data in the IReadOnlyList.
        /// Uses 2*n memory where n is the size of the IReadOnlyList.
        /// </summary>
        public static List<T> ToList<T>(this IReadOnlyList<T> readOnlyList)
        {
            return new List<T>(readOnlyList);
        }
        /// <summary>
        /// Converts the IReadOnlyList to an array. This new array
        /// is a shallow copy of the data in the IReadOnlyList.
        /// Uses 3*n memory  where n is the size of the IReadOnlyList.
        /// </summary>
        public static T[] ToArray<T>(this IReadOnlyList<T> readOnlyList)
        {
            return readOnlyList.ToList().ToArray();
        }
    }
}
