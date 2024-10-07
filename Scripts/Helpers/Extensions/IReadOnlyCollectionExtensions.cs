using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class IReadOnlyCollectionExtensions
    {
        /// <summary>
        /// Wraps the given index to be within the collection's bounds.
        /// For example: If <paramref name="collection"/> had 10 elements
        /// (indices from 0 to 9) and <paramref name="rawIndex"/> was -1,
        /// this function would return 9. Similary, if <paramref name="rawIndex"/>
        /// was 10, the function would return 0.
        /// </summary>
        /// <param name="collection">Current collection to wrap the index
        /// within. CANNOT be empty.</param>
        /// <param name="rawIndex">Index to be wrapped.</param>
        /// <returns>Wrapped index that is in the bounds.</returns>
        public static int WrapIndex<T>(this IReadOnlyCollection<T> collection,
            int rawIndex)
        {
            #region Asserts
            Assert.IsTrue(collection.Count != 0, $"Cannot wrap an index for a " +
                $"collection with no elements");
            #endregion Asserts

            // Make a too big index wrap back around right-ward to the beginning.
            while (rawIndex >= collection.Count)
            {
                rawIndex -= collection.Count;
            }
            // Make a negative index wrap around left-ward to the end.
            while (rawIndex < 0)
            {
                rawIndex += collection.Count;
            }
            return rawIndex;
        }
        /// <summary>
        /// Simple function for converting contents of IReadOnlyCollection &lt;<see cref="GameObject"/>&gt;
        /// </summary>
        /// <param name="collection">The IReadOnlyCollection of GameObjects.</param>
        /// <returns>String representation of the elements in the IReadOnlyCollection</returns>
        public static string ElementsAsString(this IReadOnlyCollection<GameObject> collection)
        {
            string temp_collectionString = "";
            foreach (GameObject go in collection)
            {
                temp_collectionString += go.name + ", ";
            }
            return temp_collectionString;
        }
    }
}
