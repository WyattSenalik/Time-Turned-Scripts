using System;
using UnityEngine;
using UnityEngine.Events;
// Original Authors - Wyatt Senalik

namespace Helpers.Events
{
    /// <summary>
    /// Event that uses a <see cref="UnityEvent"/> (serialization only)
    /// and a C# event (Subscribe and Unsubscribe only).
    /// 
    /// All serialized callbacks are called using the unity event.
    /// All callbacks added during runtime are called using the C# event.
    /// </summary>
    [Serializable]
    public class MixedEvent : IEventPrimer
    {
        [SerializeField] private UnityEvent m_event = new UnityEvent();
        private event Action m_csharpEvent;


        public void Subscribe(Action callback)
        {
            m_csharpEvent += callback;
        }
        public void Unsubscribe(Action callback)
        {
            m_csharpEvent -= callback;
        }
        public void ToggleSubscription(Action callback, bool cond)
        {
            if (cond)
            { Subscribe(callback); }
            else
            { Unsubscribe(callback); }
        }
        public void Invoke()
        {
            m_event.Invoke();
            m_csharpEvent?.Invoke();
        }
    }

    /// <summary>
    /// <see cref="MixedEvent"/> with a generic parameter.
    /// </summary>
    [Serializable]
    public class MixedEvent<T> : IEventPrimer<T>
    {
        [SerializeField] private UnityEvent<T> m_event = new UnityEvent<T>();
        private event Action<T> m_csharpEvent;


        public void Subscribe(Action<T> callback)
        {
            m_csharpEvent += callback;
        }
        public void Unsubscribe(Action<T> callback)
        {
            m_csharpEvent -= callback;
        }
        public void ToggleSubscription(Action<T> callback, bool cond)
        {
            if (cond)
            { Subscribe(callback); }
            else
            { Unsubscribe(callback); }
        }
        public void Invoke(T param)
        {
            m_event.Invoke(param);
            m_csharpEvent?.Invoke(param);
        }
    }
}
