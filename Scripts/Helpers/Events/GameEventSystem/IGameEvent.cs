// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// Abstracted event that can be subscribed to using its
    /// <see cref="IGameEventIdentifier"/> and invoked by its single owner.
    /// </summary>
    public interface IGameEvent : IReadOnlyGameEvent
    {
        /// <summary>
        /// Calls each callback that was subscribed to this event.
        /// </summary>
        void Invoke();
    }
    /// <summary>
    /// Like <see cref="IGameEvent"/> but with a generic parameter.
    /// </summary>
    public interface IGameEvent<T> : IReadOnlyGameEvent<T>
    {
        /// <summary>
        /// Calls each callback that was subscribed to this event.
        /// </summary>
        void Invoke(T param);
    }
}
