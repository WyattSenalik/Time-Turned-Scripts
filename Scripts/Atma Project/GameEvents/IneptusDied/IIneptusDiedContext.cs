using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Context data for when a IneptusDied event has been called.
    /// </summary>
    public interface IIneptusDiedContext
    {
        float deathTime { get; }
        GameObject killerObj { get; }
        IDamageContext damageContext { get; }
        Vector2 positionAtDeath { get; }
        GameObject ineptusObj { get; }
    }
}