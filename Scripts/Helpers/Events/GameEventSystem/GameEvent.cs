using System;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// Default implementation of <see cref="IGameEvent"/> that just
    /// uses a C# event to hold subscriptions.
    /// </summary>
    public sealed class GameEvent : IGameEvent
    {
        private const bool IS_DEBUGGING = false;

        public string eventID => m_eventID;

        private event Action m_internalEvent;

        private readonly string m_eventID = "";


        public GameEvent(string id)
        {
            m_eventID = id;
        }
        public GameEvent(IGameEventIdentifier identifier) : 
            this (identifier.eventID)
        { }


        public void Invoke()
        {
            #region Logs
            //CustomDebug.LogForObject($"Invoking event ({eventID})", this, IS_DEBUGGING);
            #endregion Logs
            m_internalEvent?.Invoke();
        }
        public void ToggleSubscription(Action callback, bool cond)
        {
            #region Logs
            //CustomDebug.LogForObject($"ToggleSubscription ({cond}) for event ({eventID})", this, IS_DEBUGGING);
            #endregion Logs
            if (cond)
            {
                m_internalEvent += callback;
            }
            else
            {
                m_internalEvent -= callback;
            }
        }
    }

    /// <summary>
    /// Default implementation of <see cref="IGameEvent{T}"/> that just
    /// uses a C# event to hold subscriptions.
    /// </summary>
    public class GameEvent<T> : IGameEvent<T>
    {
        private const bool IS_DEBUGGING = false;

        public string eventID => m_eventID;

        private event Action<T> m_internalEvent;

        private readonly string m_eventID = "";


        public GameEvent(string id)
        {
            m_eventID = id;
        }
        public GameEvent(IGameEventIdentifier<T> identifier) :
            this(identifier.eventID) { }


        public void Invoke(T param)
        {
            #region Logs
            //CustomDebug.LogForObject($"Invoking event ({eventID})", this, IS_DEBUGGING);
            #endregion Logs
            m_internalEvent?.Invoke(param);
        }
        public void ToggleSubscription(Action<T> callback, bool cond)
        {
            #region Logs
            //CustomDebug.LogForObject($"ToggleSubscription ({cond}) for event ({eventID})", this, IS_DEBUGGING);
            #endregion Logs
            if (cond)
            {
                m_internalEvent += callback;
            }
            else
            {
                m_internalEvent -= callback;
            }
        }
    }
}
