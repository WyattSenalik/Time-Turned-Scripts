using System;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// An event system that acts as the mediator between subscribers and
    /// event holders.
    /// </summary>
    public interface IEventSystem
    {
        /// <summary>
        /// Creates an event with the given id in the event system and returns it.
        /// </summary>
        IGameEvent CreateEvent(IGameEventIdentifier identifier);
        /// <summary>
        /// Creates an event with the given id in the event system and returns it.
        /// </summary>
        IGameEvent<T> CreateEvent<T>(IGameEventIdentifier<T> identifier);
        /// <summary>
        /// Completely removes an event with the given id from the event system 
        /// whether it is real or fake. All of the subscribed callbacks 
        /// are also lost.
        /// </summary>
        /// <returns>-1 if the event was not in the list. 0 if the event
        /// was a fake event. 1 if the event was a real event.</returns>
        int DeleteEvent(IGameEventIdentifier identifier);
        /// <summary>
        /// Completely removes an event with the given id from the event system 
        /// whether it is real or fake. All of the subscribed callbacks 
        /// are also lost.
        /// </summary>
        /// <returns>-1 if the event was not in the list. 0 if the event
        /// was a fake event. 1 if the event was a real event.</returns>
        int DeleteEvent<T>(IGameEventIdentifier<T> identifier);
        /// <summary>
        /// Subscribes the given callback to the event with the given id.
        /// </summary>
        /// <returns>True if the subscription was to a real event.
        /// False if the subscription was to a fake event.</returns>
        bool SubscribeToEvent(IGameEventIdentifier identifier, Action callback);
        /// <summary>
        /// Subscribes the given callback to the event with the given id.
        /// </summary>
        /// <returns>True if the subscription was to a real event.
        /// False if the subscription was to a fake event.</returns>
        bool SubscribeToEvent<T>(IGameEventIdentifier<T> identifier, Action<T> callback);
        /// <summary>
        /// Unsubscribes the given callback from the event with the given id.
        /// </summary>
        /// <returns>True if was unsubscription was successful 
        /// (to either real or fake). False if there was no event (real or fake) 
        /// to unsubscribe from.</returns>
        bool UnsubscribeFromEvent(IGameEventIdentifier identifier, Action callback);
        /// <summary>
        /// Unsubscribes the given callback from the event with the given id.
        /// </summary>
        /// <returns>True if was unsubscription was successful 
        /// (to either real or fake). False if there was no event (real or fake) 
        /// to unsubscribe from.</returns>
        bool UnsubscribeFromEvent<T>(IGameEventIdentifier<T> identifier, Action<T> callback);
        /// <summary>
        /// If cond is true: calls <see cref="SubscribeToEvent"/>.
        /// If cond is false: calls <see cref="UnsubscribeFromEvent"/>.
        /// </summary>
        /// <returns>If true, returns same as <see cref="SubscribeToEvent"/>.
        /// If false, returns same as <see cref="UnsubscribeFromEvent"/></returns>
        bool ToggleSubscriptionToEvent(bool cond, IGameEventIdentifier identifier, 
            Action callback);
        /// <summary>
        /// If cond is true: calls <see cref="SubscribeToEvent{T}"/>.
        /// If cond is false: calls <see cref="UnsubscribeFromEvent{T}"/>.
        /// </summary>
        /// <returns>If true, returns same as <see cref="SubscribeToEvent{T}"/>.
        /// If false, returns same as <see cref="UnsubscribeFromEvent{T}"/></returns>
        bool ToggleSubscriptionToEvent<T>(bool cond, IGameEventIdentifier<T> identifier,
            Action<T> callback);
    }
}