using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.Internal
{
    /// <summary>
    /// Default implementation of <see cref="IFakeGameEvent"/>.
    /// Stores the callbacks in a list.
    /// </summary>
    public sealed class FakeEvent : IFakeGameEvent
    {
        public string eventID => m_eventID;

        private readonly IGameEventFactory m_factory = null;
        // Callbacks for the real event when its created
        private readonly ICollection<Action> m_callbacks = null;
        private readonly string m_eventID = "";


        public FakeEvent(string id)
        {
            m_factory = new GameEventFactory();
            m_callbacks = new List<Action>();
            m_eventID = id;
        }
        public FakeEvent(IGameEventIdentifier identifier) : this(identifier.eventID)
        { }


        public IGameEvent CreateReal()
        {
            return m_factory.CreateRealEvent(eventID);
        }
        public void Clear()
        {
            m_callbacks.Clear();
        }
        public void ToggleSubscription(Action callback, bool cond)
        {
            if (cond)
            {
                m_callbacks.Add(callback);
            }
            else
            {
                m_callbacks.Remove(callback);
            }
        }
        public void TransferCallbacksToRealEvent(IGameEvent realEventToTransferTo)
        {
            foreach (Action t_action in m_callbacks)
            {
                realEventToTransferTo.SubscribeToEvent(t_action);
            }
        }
    }

    /// <summary>
    /// Default implementation of <see cref="IFakeGameEvent{T}"/>.
    /// Stores the callbacks in a list.
    /// </summary>
    public sealed class FakeEvent<T> : IFakeGameEvent<T>
    {
        public string eventID => m_eventID;

        private readonly IGameEventFactory m_factory = null;
        // Callbacks for the real event when its created
        private readonly ICollection<Action<T>> m_callbacks = null;
        private readonly string m_eventID = "";


        public FakeEvent(string id)
        {
            m_factory = new GameEventFactory();
            m_callbacks = new List<Action<T>>();
            m_eventID = id;
        }
        public FakeEvent(IGameEventIdentifier identifier) : this(identifier.eventID)
        { }


        public IGameEvent<T> CreateReal()
        {
            IGameEvent<T> t_realEvent = m_factory.CreateRealEvent<T>(eventID);
            return t_realEvent;
        }
        public void Clear()
        {
            m_callbacks.Clear();
        }
        public void ToggleSubscription(Action<T> callback, bool cond)
        {
            if (cond)
            {
                m_callbacks.Add(callback);
            }
            else
            {
                m_callbacks.Remove(callback);
            }
        }
        public void TransferCallbacksToRealEvent(IGameEvent<T> realEventToTransferTo)
        {
            foreach (Action<T> t_action in m_callbacks)
            {
                realEventToTransferTo.SubscribeToEvent(t_action);
            }
        }
    }
}