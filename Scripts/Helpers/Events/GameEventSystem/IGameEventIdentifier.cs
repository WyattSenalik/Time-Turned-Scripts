// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// Unique identifier for a <see cref="IGameEvent"/>.
    /// </summary>
    public interface IGameEventIdentifier
    {
        /// <summary>
        /// Unique identifier for a <see cref="IGameEvent"/>.
        /// </summary>
        string eventID { get; }
    }
    /// <summary>
    /// Unique identifier for a <see cref="IGameEvent{T}"/>.
    /// </summary>
    public interface IGameEventIdentifier<T> : IGameEventIdentifier
    {
    }
}