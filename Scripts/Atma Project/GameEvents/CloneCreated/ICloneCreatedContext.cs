// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Context data for when a CloneCreated event has been called.
    /// </summary>
    public interface ICloneCreatedContext 
    {
        /// <summary>
        /// Time the clone was created.
        /// </summary>
        float timeCreated { get; }
        /// <summary>
        /// Reference to the time clone itself.
        /// </summary>
        TimeClone timeClone { get; }
    }
}