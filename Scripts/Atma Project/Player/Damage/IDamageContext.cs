// Original Authors - Wyatt Senalik

using UnityEngine;

namespace Atma
{
    /// <summary>
    /// Pass context when damage is taken.
    /// </summary>
    public interface IDamageContext
    {
        float damageTime { get; }
        GameObject damageSourceObj { get; }
    }
}