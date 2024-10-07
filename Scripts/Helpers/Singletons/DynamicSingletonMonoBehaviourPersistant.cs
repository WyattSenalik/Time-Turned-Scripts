using UnityEngine;

using Helpers.Destruction;
// Original Authors - Wyatt Senalik

namespace Helpers.Singletons
{
    /// <summary>
    /// Very similar to <see cref="SingletonMonoBehaviourPersistant{T}"/> but
    /// will dynamically create itself if there is not one in the scene already.
    /// </summary>
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public abstract class DynamicSingletonMonoBehaviourPersistant<T> : MonoBehaviour where T : DynamicSingletonMonoBehaviourPersistant<T>
    {
        /// <summary>
        /// Reference to the current instance of this class.
        /// </summary>
        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject t_singletonObj = new GameObject(typeof(T).Name);
                    m_instance = t_singletonObj.AddComponent<T>();
                    m_instance.OnSingletonCreated();
                }
                return m_instance;
            }
        }
        private static T m_instance = null;


        // Domestic initialization
        protected virtual void Awake()
        {
            // Set the instance to this unless it already exists.
            if (m_instance != null)
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Multiple {GetType().Name} exist in the scene. Destroying the other", this);
                #endregion Logs
                Destroy(gameObject);
                return;
            }

            m_instance = this as T;
            OnSingletonCreated();
        }

        protected virtual void OnSingletonCreated() { }
    }
}
