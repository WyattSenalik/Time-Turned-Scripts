using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace Helpers.Singletons
{
    /// <summary>
    /// Base singleton class for a MonoBehaviour in the scene.
    /// Is destroyed on scene transition.
    /// Will destroy any additional instances of itself that appear.
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T :
        SingletonMonoBehaviour<T>
    {
        /// <summary>
        /// Reference to the current instance of this class.
        /// </summary>
        public static T instance => m_instance;
        private static T m_instance = null;


        // Domestic initialization
        protected virtual void Awake()
        {
            // Set the instance to this unless it already exists.
            if (m_instance != null)
            {
                Debug.Log($"Multiple {GetType().Name} exist in the scene. " +
                    $"Destroying the other");
                Destroy(gameObject);
                return;
            }

            m_instance = this as T;
        }

        public static T GetInstanceSafe(Component queryComp = null)
        {
            #region Asserts
            CustomDebug.RunDebugFunction(() =>
            {
                string t_querierName = queryComp == null ? nameof(GetInstanceSafe) : $"{queryComp.name}'s {queryComp.GetType().Name}";
                if (m_instance == null)
                {
                    //CustomDebug.LogError($"{t_querierName} expected singleton ({typeof(T)}) to exist");
                }
            });
            #endregion Asserts
            return m_instance;
        }
    }
}