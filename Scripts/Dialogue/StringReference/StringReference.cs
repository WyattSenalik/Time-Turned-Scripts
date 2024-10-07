using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Base class for dynamic and constant string referencing.
    /// </summary>
    public abstract class StringReference : ScriptableObject
    {
        public abstract string refString { get; }
    }
}