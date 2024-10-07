using System;

using Helpers.Events.GameEventSystem.Internal;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// Helper function for subscribing and unsubscribing callbacks to a
    /// <see cref="IGameEvent"/>.
    /// </summary>
    public sealed class SubManager
    {
        public bool isSubbed { get; private set; }

        private readonly IGameEventFactory m_factory = null;
        private readonly IEventSystem m_eventSystem = null;

        private readonly IGameEventIdentifier m_eventID = null;
        private readonly Action[] m_callbacks = new Action[0];


        public SubManager(IGameEventIdentifier identifier, 
            params Action[] callbacks)
        {
            m_factory = new GameEventFactory();
            m_eventSystem = m_factory.GetEventSystem();

            m_eventID = identifier;
            m_callbacks = callbacks;
            isSubbed = false;
        }

        /// <summary>
        /// Subscribes callbacks to the event.
        /// If already subscribed, does nothing.
        /// </summary>
        public void Subscribe()
        {
            // Already subscribed, don't double sub.
            if (isSubbed) { return; }
            isSubbed = true;

            foreach (Action t_callback in m_callbacks)
            {
                m_eventSystem.SubscribeToEvent(m_eventID, t_callback);
            }
        }
        /// <summary>
        /// Unsubscribes callbacks from the event.
        /// If not yet subscribed, does nothing.
        /// </summary>
        public void Unsubscribe()
        {
            // Already unsubscribed, don't need to unsub again.
            if (!isSubbed) { return; }
            isSubbed = false;

            foreach (Action t_callback in m_callbacks)
            {
                m_eventSystem.UnsubscribeFromEvent(m_eventID, t_callback);
            }
        }
        /// <summary>
        /// If given true, calls <see cref="Subscribe"/>.
        /// If given false, calls <see cref="Unsubscribe"/>.
        /// </summary>
        public void ToggleSubscription(bool cond)
        {
            if (cond) { Subscribe(); }
            else { Unsubscribe(); }
        }
    }

    /// <summary>
    /// Helper function for subscribing and unsubscribing callbacks to a
    /// <see cref="IGameEvent{T}"/>.
    /// </summary>
    public class SubManager<T>
    {
        private const bool IS_DEBUGGING = false;

        public bool isSubbed { get; private set; }

        private readonly IGameEventFactory m_factory = null;
        private readonly IEventSystem m_eventSystem = null;

        private readonly IGameEventIdentifier<T> m_eventID = null;
        private readonly Action<T>[] m_callbacks = new Action<T>[0];


        public SubManager(IGameEventIdentifier<T> identifier,
            params Action<T>[] callbacks)
        {
            m_factory = new GameEventFactory();
            m_eventSystem = m_factory.GetEventSystem();

            m_eventID = identifier;
            m_callbacks = callbacks;
            isSubbed = false;
        }

        /// <summary>
        /// Subscribes callbacks to the event.
        /// If already subscribed, does nothing.
        /// </summary>
        public void Subscribe()
        {
            //CustomDebug.LogForObject(nameof(Subscribe), this, IS_DEBUGGING);
            // Already subscribed, don't double sub.
            if (isSubbed) { return; }
            isSubbed = true;

            foreach (Action<T> t_callback in m_callbacks)
            {
                m_eventSystem.SubscribeToEvent(m_eventID, t_callback);
            }
        }
        /// <summary>
        /// Unsubscribes callbacks from the event.
        /// If not yet subscribed, does nothing.
        /// </summary>
        public void Unsubscribe()
        {
            //CustomDebug.LogForObject(nameof(Unsubscribe), this, IS_DEBUGGING);
            // Already unsubscribed, don't need to unsub again.
            if (!isSubbed) { return; }
            isSubbed = false;

            foreach (Action<T> t_callback in m_callbacks)
            {
                m_eventSystem.UnsubscribeFromEvent(m_eventID, t_callback);
            }
        }
        /// <summary>
        /// If given true, calls <see cref="Subscribe"/>.
        /// If given false, calls <see cref="Unsubscribe"/>.
        /// </summary>
        public void ToggleSubscription(bool cond)
        {
            if (cond) { Subscribe(); }
            else { Unsubscribe(); }
        }
    }
}
