using System;
// Original Authors - Wyatt Senalik

namespace Helpers.Events
{
    /// <summary>
    /// Interface for foreign classes to use to subscribe and
    /// unsubscribe to and from custom events without giving
    /// access to invoke the events.
    /// </summary>
    public interface IEventPrimer
    {
        /// <summary>
        /// Subscribes or unsubscribes the given callback to/from the event.
        /// True = subscribe. False = unsubscribe.
        /// </summary>
        /// <param name="callback">Callback to be invoked on the event.</param>
        /// <param name="cond">True = subscribe. False = unsubscribe.</param>
        public void ToggleSubscription(Action callback, bool cond);
    }


    /// <summary>
    /// <see cref="IEventPrimer"/> with a generic parameter.
    /// </summary>
    public interface IEventPrimer<T>
    {
        /// <summary>
        /// Subscribes or unsubscribes the given callback to/from the event.
        /// True = subscribe. False = unsubscribe.
        /// </summary>
        /// <param name="callback">Callback to be invoked on the event.</param>
        /// <param name="cond">True = subscribe. False = unsubscribe.</param>
        public void ToggleSubscription(Action<T> callback, bool cond);
    }

    /// <summary>
    /// <see cref="IEventPrimer"/> with 2 generic parameters.
    /// </summary>
    public interface IEventPrimer<T, G>
    {
        /// <summary>
        /// Subscribes or unsubscribes the given callback to/from the event.
        /// True = subscribe. False = unsubscribe.
        /// </summary>
        /// <param name="callback">Callback to be invoked on the event.</param>
        /// <param name="cond">True = subscribe. False = unsubscribe.</param>
        public void ToggleSubscription(Action<T, G> callback, bool cond);
    }
}
