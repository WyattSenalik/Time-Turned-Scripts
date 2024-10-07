using UnityEngine;
// Original Author - Wyatt Senalik

namespace Helpers.Destruction
{
    /// <summary>
    /// Simple script to make an object persist through scenes.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField] private bool m_allowUnparenting = true;

        private void Start()
        {
            if (transform.parent != null)
            {
                if (m_allowUnparenting)
                {
                    transform.parent = null;
                }
                else
                {
                    //CustomDebug.LogWarningForComponent($"Could not make Don't Destroy On Load because it has a parent and {nameof(m_allowUnparenting)} is false.", this);
                }
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
