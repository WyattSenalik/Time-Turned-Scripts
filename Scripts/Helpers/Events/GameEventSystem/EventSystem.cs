using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.Internal
{
    /// <summary>
    /// Default implementation of <see cref="IEventSystem"/>.
    /// C# Singleton.
    /// Uses 2 dictionaries (one for real events and one for fake events).
    /// </summary>
    public sealed class EventSystem : IEventSystem
    {
        private const bool IS_DEBUGGING = false;

        public static IEventSystem instance
        {
            get
            {
                if (s_instance == null) { s_instance = new EventSystem(); }
                return s_instance;
            }
        }
        private static IEventSystem s_instance = null;

        // References to all the events by their ID.
        // Parameter-less
        private readonly Dictionary<string, IGameEvent> m_eventsHash = null;
        private readonly Dictionary<string, IFakeGameEvent> m_fakeEventsHash = null;
        // With parameters
        private readonly Dictionary<string, object> m_paramEventsHash = null;
        private readonly Dictionary<string, object> m_fakeParamEventsHash = null;

        private readonly IGameEventFactory m_factory = null;


        private EventSystem()
        {
            m_eventsHash = new Dictionary<string, IGameEvent>();
            m_fakeEventsHash = new Dictionary<string, IFakeGameEvent>();

            m_paramEventsHash = new Dictionary<string, object>();
            m_fakeParamEventsHash = new Dictionary<string, object>();

            m_factory = new GameEventFactory();
        }


        #region Parameter-less
        public IGameEvent CreateEvent(IGameEventIdentifier identifier)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(CreateEvent)} for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return CreateEvent(identifier.eventID);
        }
        public int DeleteEvent(IGameEventIdentifier identifier)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(DeleteEvent)} for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return DeleteEvent(identifier.eventID);
        }
        public bool SubscribeToEvent(IGameEventIdentifier identifier,
            Action callback)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(SubscribeToEvent)} for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return SubscribeToEvent(identifier.eventID, callback);
        }
        public bool UnsubscribeFromEvent(IGameEventIdentifier identifier,
            Action callback)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(UnsubscribeFromEvent)} for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return UnsubscribeFromEvent(identifier.eventID, callback);
        }
        public bool ToggleSubscriptionToEvent(bool cond, 
            IGameEventIdentifier identifier, Action callback)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(ToggleSubscriptionToEvent)} ({cond}) for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return ToggleSubscriptionToEvent(cond, identifier.eventID, callback);
        }
        #endregion Parameter-less

        #region W/ Parameter
        public IGameEvent<T> CreateEvent<T>(IGameEventIdentifier<T> identifier)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(CreateEvent)}<{typeof(T).Name}> for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return CreateEvent<T>(identifier.eventID);
        }
        public int DeleteEvent<T>(IGameEventIdentifier<T> identifier)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(DeleteEvent)}<{typeof(T).Name}> for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return DeleteEvent<T>(identifier.eventID);
        }
        public bool SubscribeToEvent<T>(IGameEventIdentifier<T> identifier,
            Action<T> callback)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(SubscribeToEvent)} <{typeof(T).Name}> for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return SubscribeToEvent(identifier.eventID, callback);
        }
        public bool UnsubscribeFromEvent<T>(IGameEventIdentifier<T> identifier,
            Action<T> callback)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(UnsubscribeFromEvent)} <{typeof(T).Name}> for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return UnsubscribeFromEvent(identifier.eventID, callback);
        }
        public bool ToggleSubscriptionToEvent<T>(bool cond,
            IGameEventIdentifier<T> identifier, Action<T> callback)
        {
            #region Logs
            //CustomDebug.LogForObject($"{nameof(ToggleSubscriptionToEvent)} <{typeof(T).Name}> for event {identifier.eventID}", this, IS_DEBUGGING);
            #endregion Logs
            return ToggleSubscriptionToEvent(cond, identifier.eventID, callback);
        }
        #endregion W/ Parameter


        #region Parameter-less (private)
        private IGameEvent CreateEvent(string id)
        {
            // Event to eventually return
            IGameEvent t_gameEvent;

            // If the event does not exist yet, create it
            if (!m_eventsHash.TryGetValue(id, out IGameEvent t_existingEvent))
            {
                t_gameEvent = m_factory.CreateRealEvent(id);
                m_eventsHash.Add(id, t_gameEvent);
            }
            // If the exiting event is null, replace it
            else if (t_existingEvent == null)
            {
                t_gameEvent = m_factory.CreateRealEvent(id);
                m_eventsHash[id] = t_gameEvent;
            }
            // If the existing event is a fake event, create a
            // real event from the fake event.
            else if (m_fakeEventsHash.TryGetValue(id, out IFakeGameEvent t_fakeEvent))
            {
                t_gameEvent = t_fakeEvent.CreateReal();
                m_eventsHash.Add(id, t_gameEvent);
                // Transfer the callbacks
                t_fakeEvent.TransferCallbacksToRealEvent(t_gameEvent);
                // Also get rid of the fake event
                t_fakeEvent.Clear();
                m_fakeEventsHash.Remove(id);
            }
            // If the existing event is not null or fake, its a real event.
            // That is not allowed, so throw an error.
            else
            {
                t_gameEvent = null;
                //CustomDebug.LogError($"Event with ID {id} already exists.");
            }

            return t_gameEvent;
        }
        private int DeleteEvent(string id)
        {
            if (m_eventsHash.Remove(id))
            {
                return 1;
            }
            else if (m_fakeEventsHash.Remove(id))
            {
                return 0;
            }
            else
            {
                // Wasn't in either
                return -1;
            }
        }
        private bool SubscribeToEvent(string id, Action callback)
        {
            // Event exists as a real event
            if (m_eventsHash.TryGetValue(id, out IGameEvent t_existingEvent))
            {
                t_existingEvent.ToggleSubscription(callback, true);
                return true;
            }
            // Event exists as a fake event
            if (m_fakeEventsHash.TryGetValue(id, out IFakeGameEvent t_existingFake))
            {
                t_existingFake.ToggleSubscription(callback, true);
                return false;
            }
            // Event does not yet exist at all, create a fake event to hold
            // the callbacks until the real event is created.
            else
            {
                // Create fake event and add it to the events hash.
                IFakeGameEvent t_newFake = m_factory.CreateFakeEvent(id);
                m_fakeEventsHash.Add(id, t_newFake);

                t_newFake.ToggleSubscription(callback, true);
                return false;
            }
        }
        private bool SubscribeToEvent<T>(string id, Action callback)
        {
            // Event exists as a real event
            if (m_eventsHash.TryGetValue(id, out IGameEvent t_existingEvent))
            {
                t_existingEvent.ToggleSubscription(callback, true);
                return true;
            }
            // Event exists as a fake event
            if (m_fakeEventsHash.TryGetValue(id, out IFakeGameEvent t_existingFake))
            {
                t_existingFake.ToggleSubscription(callback, true);
                return false;
            }
            // Event does not yet exist at all, create a fake event to hold
            // the callbacks until the real event is created.
            else
            {
                // Create fake event and add it to the events hash.
                IFakeGameEvent t_newFake = m_factory.CreateFakeEvent(id);
                m_fakeEventsHash.Add(id, t_newFake);

                t_newFake.ToggleSubscription(callback, true);
                return false;
            }
        }
        private bool UnsubscribeFromEvent(string id, Action callback)
        {
            // Event exists as a real event
            if (m_eventsHash.TryGetValue(id, out IGameEvent t_existingEvent))
            {
                t_existingEvent.ToggleSubscription(callback, false);
                return true;
            }
            // Event exists as a fake event
            if (m_fakeEventsHash.TryGetValue(id, out IFakeGameEvent t_existingFake))
            {
                t_existingFake.ToggleSubscription(callback, false);
                return true;
            }
            // Event does not yet exist at all, so can't unsubscribe.
            else
            {
                return false;
            }
        }
        private bool ToggleSubscriptionToEvent(bool cond, string id, Action callback)
        {
            if (cond) { return SubscribeToEvent(id, callback); }
            else { return UnsubscribeFromEvent(id, callback); }
        }
        #endregion Parameter-less (private)


        #region W/ Parameter (private)
        private IGameEvent<T> CreateEvent<T>(string id)
        {
            IGameEvent<T> t_gameEvent;

            // Try and find if there is already a fake event for this event.
            int t_tryResult = TryGetFakeEvent(id, out IFakeGameEvent<T> t_fakeEvent);
            switch (t_tryResult)
            {
                // Found existing fake event.
                // Create a real event from the fake.
                case 1:
                    #region Asserts
                    //CustomDebug.AssertIsTrue(!m_paramEventsHash.ContainsKey(id), $"id ({id}) not to have an entry in both the fake and real events.", GetType().Name);
                    #endregion Asserts
                    t_gameEvent = t_fakeEvent.CreateReal();
                    m_paramEventsHash.Add(id, t_gameEvent);
                    // Transfer the callbacks
                    t_fakeEvent.TransferCallbacksToRealEvent(t_gameEvent);
                    // Also get rid of the fake event
                    t_fakeEvent.Clear();
                    m_fakeParamEventsHash.Remove(id);
                    return t_gameEvent;
                // Fake event was not in the dictionary, so try to just
                // make it in the real events dictionary.
                case -1:
                    // If the event does not exist yet, create it
                    if (!m_paramEventsHash.TryGetValue(id, 
                        out object t_existingEventObj))
                    {
                        t_gameEvent = m_factory.CreateRealEvent<T>(id);
                        m_paramEventsHash.Add(id, t_gameEvent);
                        return t_gameEvent;
                    }
                    // If the event already exists and is not null
                    else
                    {
                        #region Asserts
                        //CustomDebug.AssertIsTrue(t_existingEventObj != null, $"the game event with id ({id}) to not be null", GetType().Name);
                        #endregion Asserts
                        //CustomDebug.LogError($"Event with ID {id} already exists.");
                        return null;
                    }
                // Fake event could not cast correctly. An error will be
                // thrown in the try function, so just do nothing.
                case -2:
                    return null;
                default:
                    CustomDebug.UnhandledEnum(t_tryResult, nameof(EventSystem));
                    return null;
            }
        }
        private int DeleteEvent<T>(string id)
        {
            if (m_paramEventsHash.Remove(id))
            {
                return 1;
            }
            else if (m_fakeParamEventsHash.Remove(id))
            {
                return 0;
            }
            else
            {
                // Wasn't in either
                return -1;
            }
        }
        private bool SubscribeToEvent<T>(string id, Action<T> callback)
            => ToggleSubscriptionToEvent(true, id, callback);
        private bool UnsubscribeFromEvent<T>(string id, Action<T> callback)
            => ToggleSubscriptionToEvent(false, id, callback);
        private bool ToggleSubscriptionToEvent<T>(bool cond, string id,
            Action<T> callback)
        {
            int t_tryValue = TryGetGameEvent(id, out IGameEvent<T> t_existingEvent);

            switch (t_tryValue)
            {
                // Succeeded to find and cast.
                case 1:
                    #region Asserts
                    //CustomDebug.AssertIsTrue(t_existingEvent != null, "the game event with id ({id}) to not be null", GetType().Name);
                    #endregion Asserts
                    t_existingEvent.ToggleSubscription(callback, cond);
                    return true;
                // Did not find, check for fake event.
                case -1:
                    ToggleSubscriptionToFakeEvent(cond, id, callback);
                    return false;
                // Found it, but failed cast. TryGetGameEvent will have thrown an
                // error already.
                case -2:
                    return false;
                default:
                    CustomDebug.UnhandledEnum(t_tryValue, GetType().Name);
                    return false;
            }
        }
        private void ToggleSubscriptionToFakeEvent<T>(bool cond, string id,
            Action<T> callback)
        {
            int t_fakeTryValue = TryGetFakeEvent(id, out IFakeGameEvent<T>
                t_existingFakeEvent);

            IFakeGameEvent<T> t_newFake;
            switch (t_fakeTryValue)
            {
                // Succeeded in finding and casting.
                case 1:
                    #region Asserts
                    //CustomDebug.AssertIsTrue(t_existingFakeEvent != null, $"the fake event with id ({id}) to not be null", GetType().Name);
                    #endregion Asserts
                    t_existingFakeEvent.ToggleSubscription(callback, cond);
                    break;
                // Failed to find it. So create new fake event.
                case -1:
                    t_newFake = m_factory.CreateFakeEvent<T>(id);
                    m_fakeParamEventsHash.Add(id, t_newFake);

                    t_newFake.ToggleSubscription(callback, cond);
                    break;
                // Found it, but failed the cast. TryGetFakeEvent will have already
                // thrown an error.
                case -2:
                    break;
                default:
                    CustomDebug.UnhandledEnum(t_fakeTryValue, GetType().Name);
                    break;
            }
        }

        /// <summary>
        /// Trys to find a fake event with the given id in the dictionary and
        /// also trys to cast it to a <see cref="IFakeGameEvent{T}"/>.
        /// </summary>
        /// <returns>1 if value was in the dictionary and was casted.
        /// -2 if value was in the dictionary, but failed to cast.
        /// -1 if value was not in the dictionary.</returns>
        private int TryGetFakeEvent<T>(string id, out IFakeGameEvent<T> fakeEvent)
        {
            if (m_fakeParamEventsHash.TryGetValue(id, out object t_fakeEventObj))
            {
                // Cast to a fake event (succeeded).
                if (t_fakeEventObj is IFakeGameEvent<T> t_fakeEventCasted)
                {
                    fakeEvent = t_fakeEventCasted;
                    return 1;
                }
                // Failed the cast. (Shouldn't happen hopefully).
                // If it does happen. Two events with different parameters must
                // share the same name.
                else
                {
                    #region Logs
                    //CustomDebug.LogError($"the fake event with id ({id}) to be able to be casted to {nameof(IFakeGameEvent)}<{typeof(T).Name}>. This may be because another event shares the same id. Check that all your event names are unique.");
                    #endregion Logs
                    fakeEvent = null;
                    return -2;
                }
            }
            // Fake event simply ins't in hash. Totally okay.
            else
            {
                fakeEvent = null;
                return -1;
            }
        }
        /// <summary>
        /// Trys to find a real event with the given id in the dictionary and
        /// also trys to cast it to a <see cref="IGameEvent{T}"/>.
        /// </summary>
        /// <returns>1 if value was in the dictionary and was casted.
        /// -2 if value was in the dictionary, but failed to cast.
        /// -1 if value was not in the dictionary.</returns>
        private int TryGetGameEvent<T>(string id, out IGameEvent<T> gameEvent)
        {
            if (m_paramEventsHash.TryGetValue(id, out object t_eventObj))
            {
                // Cast to a real event (succeeded).
                if (t_eventObj is IGameEvent<T> t_gameEventCasted)
                {
                    gameEvent = t_gameEventCasted;
                    return 1;
                }
                // Failed the cast. (Shouldn't happen hopefully).
                // If it does happen. Two events with different parameters must
                // share the same name.
                else
                {
                    #region Logs
                    //CustomDebug.LogError($"the game event with id ({id}) to be able to be casted to {nameof(IGameEvent)}<{typeof(T).Name}>. This may be because another event shares the same id. Check that all your event names are unique.");
                    #endregion Logs
                    gameEvent = null;
                    return -2;
                }
            }
            // Fake event simply isn't in hash. Totally okay.
            else
            {
                gameEvent = null;
                return -1;
            }
        }
        #endregion W/ Parameter (private)
    }
}