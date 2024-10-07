using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Interface for <see cref="Component"/> if a new Component script is created
    /// with its own interface and you want to be able to do Component things from that
    /// interface.
    /// </summary>
    public interface IComponent : IObject
    {
        /// <summary>
        /// The game object this component is attached to.
        /// A component is always attached to a game object.
        /// </summary>
        public GameObject gameObject { get; }
        /// <summary>
        /// The tag of this game object.
        ///
        /// A tag can be used to identify a game object. Tags must be declared in the
        /// Tags and Layers manager before using them.
        ///
        /// Note: You should not set a tag from the Awake() or OnValidate() method.
        /// This is because the order in which components become awake is not
        /// deterministic, and therefore can result in unexpected behaviour such as
        /// the tag being overwritten when it is awoken. If you try this, Unity will
        /// generate a warning reading "SendMessage cannot be called during Awake,
        /// CheckConsistency, or OnValidate".
        /// </summary>
        public string tag { get; set; }
        /// <summary>
        /// The Transform attached to this GameObject.
        /// </summary>
        public Transform transform { get; }

        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this game
        /// object or any of its children.
        /// 
        /// The receiving method can choose to ignore parameter by having zero
        /// arguments. if options is set to SendMessageOptions.RequireReceiver an
        /// error is printed when the message is not picked up by any component.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="parameter">Optional parameter to pass to the method
        /// (can be any value).</param>
        /// <param name="options">Should an error be raised if the method does not
        /// exist for a given target object?</param>
        public void BroadcastMessage(string methodName, object parameter = null,
            SendMessageOptions options = SendMessageOptions.RequireReceiver);
        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this game
        /// object or any of its children.
        /// 
        /// The receiving method can choose to ignore parameter by having zero
        /// arguments. if options is set to SendMessageOptions.RequireReceiver an
        /// error is printed when the message is not picked up by any component.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="options">Should an error be raised if the method does not
        /// exist for a given target object?</param>
        public void BroadcastMessage(string methodName, SendMessageOptions options);
        /// <summary>
        /// Is this game object tagged with <paramref name="tag"/> ?
        /// </summary>
        /// <param name="tag">The tag to compare.</param>
        public bool CompareTag(string tag);
        /// <summary>
        /// Returns the component of Type <paramref name="type"/> if the GameObject
        /// has one attached, null if it doesn't. Will also return disabled
        /// components.
        /// 
        /// Component.GetComponent will return the first component that is found and
        /// the order is undefined.If you expect there to be more than one component
        /// of the same type, use Component.GetComponents instead, and cycle through
        /// the returned components testing for some unique property.
        ///
        /// To get a component on a different GameObject, use GameObject.Find to get
        /// a reference to the other GameObject, and then use GameObject.GetComponent
        /// on the other GameObject.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        public Component GetComponent(Type type);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        public T GetComponent<T>();
        /// <summary>
        /// Returns the component with name <paramref name="type"/> if the GameObject
        /// has one attached, null if it doesn't.
        ///
        /// It is better to use GetComponent with a Type instead of a string for
        /// performance reasons.Sometimes you might not be able to get to the type
        /// however, for example when trying to access a C# script from Javascript.
        /// In that case you can simply access the component by name instead of type.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        public Component GetComponent(string type);
        /// <summary>
        /// Returns the component of Type type in the GameObject or any of its
        /// children using depth first search.
        ///
        /// A component is returned only if it is found on an active GameObject.
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <returns>A component of the matching type, if found.</returns>
        public Component GetComponentInChildren(Type t);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        /// <returns>A component of the matching type, if found.</returns>
        public T GetComponentInChildren<T>();
        /// <summary>
        /// Returns the component of Type type in the GameObject or any of its parents.
        /// 
        /// Recurses upwards until it finds a valid component.Returns null if no
        /// component found. Only components on active GameObjects are returned.
        /// </summary>
        /// <param name="t">The type of Component to retrieve.</param>
        /// <returns>A component of the matching type, if found.</returns>
        public Component GetComponentInParent(Type t);
        /// <summary>
        /// Generic version of this method.
        ///
        /// Recurses upwards till it finds a valid component. Returns null if no
        /// component found.Only component on active Game Objects are returned.
        /// </summary>
        /// <returns>A component of the matching type, if found.</returns>
        public T GetComponentInParent<T>();
        /// <summary>
        /// Returns all components of Type type in the GameObject.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        public Component[] GetComponents(Type type);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        public T[] GetComponents<T>();
        /// <summary>
        /// Returns all components of Type type in the GameObject or any of its
        /// children using depth first search. Works recursively.
        /// </summary>
        /// <param name="includeInactive">Should Components on inactive GameObjects
        /// be included in the found set? includeInactive decides which children of
        /// the GameObject will be searched. The GameObject that you call
        /// GetComponentsInChildren on is always searched regardless.
        /// Default is false.</param>
        public Component[] GetComponentsInChildren(Type t,
            bool includeInactive = false);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        /// <param name="includeInactive">Should Components on inactive GameObjects
        /// be included in the found set? includeInactive decides which children of
        /// the GameObject will be searched. The GameObject that you call
        /// GetComponentsInChildren on is always searched regardless.</param>
        /// <returns>A list of all found components matching the specified
        /// type.</returns>
        public T[] GetComponentsInChildren<T>(bool includeInactive);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        /// <returns>A list of all found components matching the specified
        /// type.</returns>
        public T[] GetComponentsInChildren<T>();
        /// <summary>
        /// Returns all components of Type type in the GameObject or any of its parents.
        /// </summary>
        /// <param name="includeInactive">Should inactive Components be included in
        /// the found set?</param>
        public Component[] GetComponentsInParent(Type t, bool includeInactive = false);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        /// <param name="includeInactive">Should inactive Components be included in
        /// the found set?</param>
        public T[] GetComponentsInParent<T>(bool includeInactive);
        /// <summary>
        /// Generic version of this method.
        /// </summary>
        T[] GetComponentsInParent<T>();
        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this
        /// game object.
        ///
        /// The receiving method can choose to ignore the argument by having zero
        /// arguments.
        /// If options is set to SendMessageOptions. RequireReceiver an error is printed
        /// when the message is not picked up by any component.
        ///
        /// Note that messages will not be sent to inactive objects (ie, those that have
        /// been deactivated in the editor or with the GameObject.SetActive function).
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        public void SendMessage(string methodName);
        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this
        /// game object.
        ///
        /// The receiving method can choose to ignore the argument by having zero
        /// arguments.
        /// If options is set to SendMessageOptions. RequireReceiver an error is printed
        /// when the message is not picked up by any component.
        ///
        /// Note that messages will not be sent to inactive objects (ie, those that have
        /// been deactivated in the editor or with the GameObject.SetActive function).
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="value">Optional parameter for the method.</param>
        public void SendMessage(string methodName, object value);
        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this
        /// game object.
        ///
        /// The receiving method can choose to ignore the argument by having zero
        /// arguments.
        /// If options is set to SendMessageOptions. RequireReceiver an error is printed
        /// when the message is not picked up by any component.
        ///
        /// Note that messages will not be sent to inactive objects (ie, those that have
        /// been deactivated in the editor or with the GameObject.SetActive function).
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="value">Optional parameter for the method.</param>
        /// <param name="options">Should an error be raised if the target object doesn't
        /// implement the method for the message?</param>
        public void SendMessage(string methodName, object value,
            SendMessageOptions options);
        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this
        /// game object.
        ///
        /// The receiving method can choose to ignore the argument by having zero
        /// arguments.
        /// If options is set to SendMessageOptions. RequireReceiver an error is printed
        /// when the message is not picked up by any component.
        ///
        /// Note that messages will not be sent to inactive objects (ie, those that have
        /// been deactivated in the editor or with the GameObject.SetActive function).
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="options">Should an error be raised if the target object doesn't
        /// implement the method for the message?</param>
        public void SendMessage(string methodName, SendMessageOptions options);
        /// <summary>
        /// Calls the method named methodName on every MonoBehaviour in this game object
        /// and on every ancestor of the behaviour.
        ///
        /// The receiving method can choose to ignore the argument by having zero
        /// arguments. If the options parameter is set to SendMessageOptions.
        /// RequireReceiver an error is printed when the message is not picked up by any
        /// component.
        ///
        /// Note that messages will not be sent to inactive objects (ie, those that have
        /// been deactivated in the editor or with the GameObject.SetActive function).
        /// </summary>
        /// <param name="methodName">Name of method to call.</param>
        /// <param name="options">Should an error be raised if the method does not exist
        /// on the target object?</param>
        public void SendMessageUpwards(string methodName, SendMessageOptions options);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName">Name of method to call.</param>
        /// <param name="value">Optional parameter value for the method.</param>
        /// <param name="options">Should an error be raised if the method does not exist
        /// on the target object?</param>
        public void SendMessageUpwards(string methodName, object value = null,
            SendMessageOptions options = SendMessageOptions.RequireReceiver);
        /// <summary>
        /// Gets the component of the specified type, if it exists.
        /// 
        /// TryGetComponent will attempt to retrieve the component of the given type.
        /// The notable difference compared to GameObject.GetComponent is that this
        /// method does not allocate in the Editor when the requested component does not
        /// exist.
        /// </summary>
        /// <param name="type">The type of the component to retrieve.</param>
        /// <param name="component">The output argument that will contain the
        /// component or null.</param>
        /// <returns>Returns true if the component is found, false otherwise.</returns>
        public bool TryGetComponent(Type type, out Component component);
        /// <summary>
        /// Gets the component of the specified type, if it exists.
        /// 
        /// TryGetComponent will attempt to retrieve the component of the given type.
        /// The notable difference compared to GameObject.GetComponent is that this
        /// method does not allocate in the Editor when the requested component does not
        /// exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The output argument that will contain the
        /// component or null.</param>
        /// <returns>Returns true if the component is found, false otherwise.</returns>
        public bool TryGetComponent<T>(out T component);
    }
}
