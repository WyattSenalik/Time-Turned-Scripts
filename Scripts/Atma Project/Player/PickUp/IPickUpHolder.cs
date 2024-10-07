using Helpers.UnityInterfaces;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Holds pick ups.
    /// </summary>
    public interface IPickUpHolder : IMonoBehaviour
    {
        /// <summary>Center of the hold circle.</summary>
        public Vector3 holdPosCenterWorld { get; }
        /// <summary>Size the pickup should be at while its being held.</summary>
        public float pickupSize { get; }
    }
}