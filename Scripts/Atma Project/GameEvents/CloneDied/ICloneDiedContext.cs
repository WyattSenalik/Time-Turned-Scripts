using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Context data for when a CloneDied event has been called.
    /// </summary>
    public interface ICloneDiedContext
    {
        float deathTime { get; }
        GameObject killerObj { get; }
        IDamageContext damageContext { get; }
        Vector2 positionAtDeath { get; }
        GameObject cloneObj { get; }
    }
}
