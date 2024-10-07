using System;

using Helpers.Events.GameEventSystem.Internal;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// Extension methods for <see cref="IGameEventIdentifier"/> so that a user
    /// does not need to directly access <see cref="IEventSystem"/>.
    /// </summary>
    public static class IGameEventIdentifierExtensions
    {
        #region Parameter-less
        /// <summary>
        /// Finds the event with this <see cref="IGameEventIdentifier"/> in the
        /// <see cref="IEventSystem"/> and creates an event for it.
        /// </summary>
        public static IGameEvent CreateEvent(this IGameEventIdentifier identifier)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.CreateEvent(identifier);
        }
        /// <summary>
        /// Finds the event with this <see cref="IGameEventIdentifier"/> in the
        /// <see cref="IEventSystem"/> and subscribes the callback to it.
        /// </summary>
        public static bool SubscribeToEvent(this IGameEventIdentifier identifier,
            Action callback)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.SubscribeToEvent(identifier, callback);
        }
        /// <summary>
        /// Finds the event with this <see cref="IGameEventIdentifier"/> in the
        /// <see cref="IEventSystem"/> and unsubcribes the callback from it.
        /// </summary>
        public static bool UnsubscribeFromEvent(this IGameEventIdentifier identifier,
            Action callback)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.UnsubscribeFromEvent(identifier, callback);
        }
        /// <summary>
        /// If subOrUnsub is true: calls <see cref="SubscribeToEvent"/>.
        /// If subOrUnsub is false: calls <see cref="UnsubscribeFromEvent"/>.
        /// </summary>
        public static bool ToggleSubscriptionToEvent(
            this IGameEventIdentifier identifier, Action callback, bool subOrUnsub)
        {
            // Sub
            if (subOrUnsub)
            {
                return SubscribeToEvent(identifier, callback);
            }
            // Unsub
            else
            {
                return UnsubscribeFromEvent(identifier, callback);
            }
        }
        #endregion Parameter-less

        #region W/ Parameter
        /// <summary>
        /// Finds the event with this <see cref="IGameEventIdentifier{T}"/> in the
        /// <see cref="IEventSystem"/> and creates an event for it.
        /// </summary>
        public static IGameEvent<T> CreateEvent<T>(this IGameEventIdentifier<T> identifier)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.CreateEvent(identifier);
        }
        /// <summary>
        /// Finds the event with this <see cref="IGameEventIdentifier{T}"/> in the
        /// <see cref="IEventSystem"/> and subscribes the callback to it.
        /// </summary>
        public static bool SubscribeToEvent<T>(this IGameEventIdentifier<T> identifier,
            Action<T> callback)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.SubscribeToEvent(identifier, callback);
        }
        /// <summary>
        /// Finds the event with this <see cref="IGameEventIdentifier{T}"/> in the
        /// <see cref="IEventSystem"/> and unsubcribes the callback from it.
        /// </summary>
        public static bool UnsubscribeFromEvent<T>(this IGameEventIdentifier<T> identifier,
            Action<T> callback)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.UnsubscribeFromEvent(identifier, callback);
        }
        /// <summary>
        /// If subOrUnsub is true: calls <see cref="SubscribeToEvent{T}"/>.
        /// If subOrUnsub is false: calls <see cref="UnsubscribeFromEvent{T}"/>.
        /// </summary>
        public static bool ToggleSubscriptionToEvent<T>(
            this IGameEventIdentifier<T> identifier, Action<T> callback,
            bool subOrUnsub)
        {
            // Sub
            if (subOrUnsub)
            {
                return SubscribeToEvent(identifier, callback);
            }
            // Unsub
            else
            {
                return UnsubscribeFromEvent(identifier, callback);
            }
        }
        #endregion W/ Parameter
    }
}