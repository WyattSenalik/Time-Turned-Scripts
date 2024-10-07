using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.CatchupEvents
{
    /// <summary>
    /// Event that if subscribed to after fired, will invoked the callback instantly.
    /// </summary>
    public class CatchupEvent : IEventPrimer, ICatchupEventReset
    {
        private event Action m_event;
        private int m_amountTimesFired = 0;


        /// <summary>
        /// Subscribes or unsubscribes the given callback to/from the event.
        /// If the event has already been invoked once, then the callback will be called
        /// right away as well as being added to the callbacks list.
        /// True = subscribe. False = unsubscribe.
        /// </summary>
        /// <param name="callback">Callback to be invoked on the event.</param>
        /// <param name="cond">True = subscribe. False = unsubscribe.</param>
        public void ToggleSubscription(Action callback, bool cond)
        {
            // Subscribe
            if (cond)
            {
                // Invoke the callback for every time it has been fired.
                for (int i = 0; i < m_amountTimesFired; ++i)
                {
                    callback?.Invoke();
                }
                m_event += callback;
            }
            // Unsubscribe
            else
            {
                m_event -= callback;
            }
        }
        /// <summary>
        /// Invokes the wrapped event.
        /// </summary>
        public void Invoke()
        {
            ++m_amountTimesFired;
            m_event?.Invoke();
        }
        /// <summary>
        /// Resets the was fired variable so that any future subscriptions
        /// will not be invoked instantly until this event is re-invoked.
        /// </summary>
        public void Reset()
        {
            m_amountTimesFired = 0;
        }
    }

    /// <summary>
    /// <see cref="CatchupEvent"/> with a generic parameter.
    /// </summary>
    public class CatchupEvent<T> : IEventPrimer<T>, ICatchupEventReset
    {
        private event Action<T> m_event;
        private List<T> m_firedParams = new List<T>();


        /// <summary>
        /// Subscribes or unsubscribes the given callback to/from the event.
        /// If the event has already been invoked once, then the callback will be called
        /// right away as well as being added to the callbacks list.
        /// True = subscribe. False = unsubscribe.
        /// </summary>
        /// <param name="callback">Callback to be invoked on the event.</param>
        /// <param name="cond">True = subscribe. False = unsubscribe.</param>
        public void ToggleSubscription(Action<T> callback, bool cond)
        {
            // Subscribe
            if (cond)
            {
                // Invoke the callback for every time it has been fired.
                foreach (T t in m_firedParams)
                {
                    callback?.Invoke(t);
                }
                m_event += callback;
            }
            // Unsubscribe
            else
            {
                m_event -= callback;
            }
        }
        /// <summary>
        /// Invokes the wrapped event.
        /// </summary>
        public void Invoke(T param)
        {
            m_firedParams.Add(param);
            m_event?.Invoke(param);
        }
        /// <summary>
        /// Resets the was fired variable so that any future subscriptions
        /// will not be invoked instantly until this event is re-invoked.
        /// </summary>
        public void Reset()
        {
            m_firedParams.Clear();
        }
    }

    /// <summary>
    /// <see cref="CatchupEvent"/> with a generic parameter.
    /// </summary>
    public class CatchupEvent<T, G> : IEventPrimer<T, G>, ICatchupEventReset
    {
        private event Action<T, G> m_event;
        private List<Tuple<T, G>> m_firedParams = new List<Tuple<T, G>>();


        /// <summary>
        /// Subscribes or unsubscribes the given callback to/from the event.
        /// If the event has already been invoked once, then the callback will be called
        /// right away as well as being added to the callbacks list.
        /// True = subscribe. False = unsubscribe.
        /// </summary>
        /// <param name="callback">Callback to be invoked on the event.</param>
        /// <param name="cond">True = subscribe. False = unsubscribe.</param>
        public void ToggleSubscription(Action<T, G> callback, bool cond)
        {
            // Subscribe
            if (cond)
            {
                // Invoke the callback for every time it has been fired.
                foreach (Tuple<T, G> t in m_firedParams)
                {
                    callback?.Invoke(t.Item1, t.Item2);
                }
                m_event += callback;
            }
            // Unsubscribe
            else
            {
                m_event -= callback;
            }
        }
        /// <summary>
        /// Invokes the wrapped event.
        /// </summary>
        public void Invoke(T param0, G param1)
        {
            m_firedParams.Add(new Tuple<T, G>(param0, param1));
            m_event?.Invoke(param0, param1);
        }
        /// <summary>
        /// Resets the was fired variable so that any future subscriptions
        /// will not be invoked instantly until this event is re-invoked.
        /// </summary>
        public void Reset()
        {
            m_firedParams.Clear();
        }
    }
}
