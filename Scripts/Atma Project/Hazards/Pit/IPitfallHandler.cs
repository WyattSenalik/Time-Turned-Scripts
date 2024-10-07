using Helpers.UnityInterfaces;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Interface for <see cref="Pitfall"/> to look for on objects that should be
    /// affected by a pitfall.
    /// </summary>
    public interface IPitfallHandler : IMonoBehaviour
    {
        /// <summary>
        /// Called when a handler has fully entered the pit.
        /// </summary>
        void OnEnclosedInPitStart();
        /// <summary>
        /// Called when a handler is not longer fully in the pit.
        /// </summary>
        void OnEnclosedInPitEnd();
        /// <summary>
        /// Called when pitfall behaviour should be executed.
        /// </summary>
        void Fall(GameObject pitVisualsParentObj);
        /// <summary>
        /// Called if over pitfall still after fall (if survived intial fall).
        /// </summary>
        void FallStay(GameObject pitVisualsParentObj);
    }
}