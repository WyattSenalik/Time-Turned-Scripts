// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// <see cref="IGameEvent"/> that cannot be invoked, only subscribed to.
    /// </summary>
    public interface IReadOnlyGameEvent : IEventPrimer, IGameEventIdentifier
    {
    }

    /// <summary>
    /// Like <see cref="IReadOnlyGameEvent"/> but with a generic parameter.
    /// </summary>
    public interface IReadOnlyGameEvent<T> : IEventPrimer<T>, IGameEventIdentifier<T>
    {
    }
}
