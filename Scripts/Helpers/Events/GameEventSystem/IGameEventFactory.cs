// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.Internal
{
    /// <summary>
    /// Factory for creating <see cref="IGameEvent"/>s and 
    /// <see cref="IFakeGameEvent"/>s.
    /// </summary>
    public interface IGameEventFactory
    {
        /// <summary>
        /// Returns an implemenation of <see cref="IEventSystem"/>.
        /// </summary>
        IEventSystem GetEventSystem();
        /// <summary>
        /// News up an implemenation of <see cref="IGameEvent"/>.
        /// </summary>
        IGameEvent CreateRealEvent(string eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IGameEvent"/>.
        /// </summary>
        IGameEvent CreateRealEvent(IGameEventIdentifier eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IGameEvent{T}"/>.
        /// </summary>
        IGameEvent<T> CreateRealEvent<T>(string eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IGameEvent{T}"/>.
        /// </summary>
        IGameEvent<T> CreateRealEvent<T>(IGameEventIdentifier<T> eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IFakeGameEvent"/>.
        /// </summary>
        IFakeGameEvent CreateFakeEvent(string eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IFakeGameEvent"/>.
        /// </summary>
        IFakeGameEvent CreateFakeEvent(IGameEventIdentifier eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IFakeGameEvent{T}"/>.
        /// </summary>
        IFakeGameEvent<T> CreateFakeEvent<T>(string eventID);
        /// <summary>
        /// News up an implemenation of <see cref="IFakeGameEvent{T}"/>.
        /// </summary>
        IFakeGameEvent<T> CreateFakeEvent<T>(IGameEventIdentifier<T> eventID);
    }
}