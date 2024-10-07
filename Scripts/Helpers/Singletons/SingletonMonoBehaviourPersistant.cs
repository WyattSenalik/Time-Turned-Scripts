using UnityEngine;

using Helpers.Destruction;
// Original Authors - Wyatt Senalik

namespace Helpers.Singletons
{
    /// <summary>
    /// Base singleton class for a MonoBehaviour in the scene.
    /// Is not destroyed on scene transition (DontDestroyOnLoad).
    /// Will destroy any additional instances of itself that appear.
    /// </summary>
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class SingletonMonoBehaviourPersistant<T> : SingletonMonoBehaviour<T>
        where T : SingletonMonoBehaviourPersistant<T>
    {
    }
}