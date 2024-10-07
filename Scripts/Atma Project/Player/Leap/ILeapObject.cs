using UnityEngine;

using Helpers.Events;
using Helpers.UnityEnums;
using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// For <see cref="PlayerLeap"/> to talk with the object they are about
    /// to leap over so that previews and prompts can be displayed.
    /// </summary>
    public interface ILeapObject : IMonoBehaviour
    {
        /// <summary>
        /// If the leap object is available to be used as a leaping point.
        /// </summary>
        bool availableToUse { get; set; }
        /// <summary>
        /// Event called at the end of <see cref="OnLeptFrom"/>.
        /// </summary>
        IEventPrimer onLeptFrom { get; }
        /// <summary>
        /// Center position of the leap object.
        /// </summary>
        Vector2 leapObjectPos { get; }

        /// <summary>
        /// Called by <see cref="PlayerLeap"/> when the object becomes their 
        /// current leap object.
        /// </summary>
        /// <param name="desiredLeapDir">The direction that the player
        /// wants to leap over the object. Normalized expected.</param>
        void OnLeapHighlight(eEightDirection desiredLeapDir);
        /// <summary>
        /// Called by <see cref="PlayerLeap"/> when the object is no longer their 
        /// current leap object.
        /// </summary>
        void OnLeapHighlightEnd();
        /// <summary>
        /// Called by <see cref="PlayerLeap"/> when they use this object to leap from.
        /// </summary>
        void OnLeptFrom();
    }
}