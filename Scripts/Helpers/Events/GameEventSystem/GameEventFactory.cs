// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.Internal
{
    /// <summary>
    /// Default implementation of <see cref="IGameEventFactory"/>.
    /// Returns <see cref="GameEvent"/> for <see cref="CreateRealEvent"/>
    /// and <see cref="FakeEvent"/> for <see cref="CreateFakeEvent"/>.
    /// </summary>
    public sealed class GameEventFactory : IGameEventFactory
    {
        public IEventSystem GetEventSystem()
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, this);
            #endregion Asserts
            return t_eventSystem;
        }

        #region Parameter-less
        public IGameEvent CreateRealEvent(string id)
        {
            return new GameEvent(id);
        }
        public IGameEvent CreateRealEvent(IGameEventIdentifier identifier)
        {
            return CreateRealEvent(identifier.eventID);
        }
        public IFakeGameEvent CreateFakeEvent(string id)
        {
            return new FakeEvent(id);
        }
        public IFakeGameEvent CreateFakeEvent(IGameEventIdentifier identifier)
        {
            return CreateFakeEvent(identifier.eventID);
        }
        #endregion Parameter-less

        #region W/ Parameter
        public IGameEvent<T> CreateRealEvent<T>(string eventID)
        {
            return new GameEvent<T>(eventID);
        }
        public IGameEvent<T> CreateRealEvent<T>(IGameEventIdentifier<T> eventID)
        {
            return CreateRealEvent<T>(eventID.eventID);
        }
        public IFakeGameEvent<T> CreateFakeEvent<T>(string eventID)
        {
            return new FakeEvent<T>(eventID);
        }
        public IFakeGameEvent<T> CreateFakeEvent<T>(IGameEventIdentifier<T> eventID)
        {
            return CreateFakeEvent<T>(eventID.eventID);
        }
        #endregion W/ Parameter
    }
}