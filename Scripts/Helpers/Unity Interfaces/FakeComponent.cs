using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Wrapper that gives a <see cref="Component"/> the ability to utilize
    /// <see cref="IComponent"/> by newing this class up.
    /// </summary>
    public class FakeComponent : FakeObject, IComponent
    {
        public GameObject gameObject => m_component.gameObject;
        public string tag
        {
            get => m_component.tag;
            set => m_component.tag = value;
        }
        public Transform transform => m_component.transform;


        // Reference to the actual Component
        private Component m_component = null;


        public FakeComponent(Component component) : base(component)
        {
            m_component = component;
        }


        public void BroadcastMessage(string methodName, object parameter = null,
            SendMessageOptions options = SendMessageOptions.RequireReceiver)
            => m_component.BroadcastMessage(methodName, parameter, options);
        public void BroadcastMessage(string methodName, SendMessageOptions options)
            => m_component.BroadcastMessage(methodName, options);
        public bool CompareTag(string tag) => m_component.CompareTag(tag);
        public Component GetComponent(Type type) => m_component.GetComponent(type);
        public T GetComponent<T>() => m_component.GetComponent<T>();
        public Component GetComponent(string type)
            => m_component.GetComponent(type);
        public Component GetComponentInChildren(Type t)
            => m_component.GetComponentInChildren(t);
        public T GetComponentInChildren<T>()
            => m_component.GetComponentInChildren<T>();
        public Component GetComponentInParent(Type t)
            => m_component.GetComponentInParent(t);
        public T GetComponentInParent<T>() => m_component.GetComponentInParent<T>();
        public Component[] GetComponents(Type type)
            => m_component.GetComponents(type);
        public T[] GetComponents<T>() => m_component.GetComponents<T>();
        public Component[] GetComponentsInChildren(Type t,
            bool includeInactive = false)
            => m_component.GetComponentsInChildren(t, includeInactive);
        public T[] GetComponentsInChildren<T>(bool includeInactive)
            => m_component.GetComponentsInChildren<T>(includeInactive);
        public T[] GetComponentsInChildren<T>()
            => m_component.GetComponentsInChildren<T>();
        public Component[] GetComponentsInParent(Type t,
            bool includeInactive = false)
            => m_component.GetComponentsInParent(t, includeInactive);
        public T[] GetComponentsInParent<T>(bool includeInactive)
            => m_component.GetComponentsInParent<T>(includeInactive);
        public T[] GetComponentsInParent<T>()
            => m_component.GetComponentsInParent<T>();
        public void SendMessage(string methodName)
            => m_component.SendMessage(methodName);
        public void SendMessage(string methodName, object value)
            => m_component.SendMessage(methodName, value);
        public void SendMessage(string methodName, object value,
            SendMessageOptions options)
            => m_component.SendMessage(methodName, value, options);
        public void SendMessage(string methodName, SendMessageOptions options)
            => m_component.SendMessage(methodName, options);
        public void SendMessageUpwards(string methodName,
            SendMessageOptions options)
            => m_component.SendMessageUpwards(methodName, options);
        public void SendMessageUpwards(string methodName, object value = null,
            SendMessageOptions options = SendMessageOptions.RequireReceiver)
            => m_component.SendMessageUpwards(methodName, value, options);
        public bool TryGetComponent(Type type, out Component component)
            => m_component.TryGetComponent(type, out component);
        public bool TryGetComponent<T>(out T component)
            => m_component.TryGetComponent(out component);
    }

    public static class ComponentExtensionsForFakeComponent
    {
        /// <summary>
        /// Finds the <see cref="IComponent"/> driving this <see cref="Component"/>
        /// and returns it. If no <see cref="IComponent"/> is driving this
        /// <see cref="Component"/>, then news up a <see cref="FakeComponent"/> and
        /// returns that.
        /// </summary>
        public static IComponent ToIComponent(this Component component)
        {
            // Check if the given component is being driven by an IComponent
            // by finding an IComponent with the same InstanceID.
            IComponent[] compArr = component.GetComponents<IComponent>();
            foreach (IComponent comp in compArr)
            {
                if (comp.GetInstanceID() == component.GetInstanceID())
                {
                    // Found the right component
                    return comp;
                }
            }

            // Otherwise, just return a fake version of it.
            return new FakeComponent(component);
        }
    }
}
