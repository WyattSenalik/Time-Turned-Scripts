using Helpers.Events.GameEventSystem.Internal;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// Extensions for <see cref="IGameEvent"/> so that the user does not have to
    /// directly access the <see cref="IEventSystem"/>.
    /// </summary>
    public static class IGameEventExtensions
    {
        /// <summary>
        /// Removes the <see cref="IGameEvent"/> from the event system so that
        /// nothing else can be subscribed to it. Anything that was subscribed to
        /// this event loses its subscription.
        /// </summary>
        /// <returns>-1 if the event was not in the list. 0 if the event
        /// was a fake event. 1 if the event was a real event.</returns>
        public static int DeleteEvent(this IGameEvent gameEvent)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.DeleteEvent(gameEvent);
        }
        /// <summary>
        /// Removes the <see cref="IGameEvent{T}"/> from the event system so that
        /// nothing else can be subscribed to it. Anything that was subscribed to
        /// this event loses its subscription.
        /// </summary>
        /// <returns>-1 if the event was not in the list. 0 if the event
        /// was a fake event. 1 if the event was a real event.</returns>
        public static int DeleteEvent<T>(this IGameEvent<T> gameEvent)
        {
            IEventSystem t_eventSystem = EventSystem.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_eventSystem, nameof(IGameEventIdentifierExtensions));
            #endregion Asserts
            return t_eventSystem.DeleteEvent(gameEvent);
        }
    }
}