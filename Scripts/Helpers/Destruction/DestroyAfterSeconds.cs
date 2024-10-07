using UnityEngine;
// Original Author - Wyatt Senalik

namespace Helpers.Destruction
{
    /// <summary>
    /// Destroys this gameObject after a specified amount of seconds.
    /// </summary>
    public class DestroyAfterSeconds : MonoBehaviour
    {
        // After how many seconds do we destroy this object
        [SerializeField] private float m_secondsToLive = 10.0f;


        // Domestic Initialization
        private void Awake()
        {
            Destroy(gameObject, m_secondsToLive);
        }
    }
}
