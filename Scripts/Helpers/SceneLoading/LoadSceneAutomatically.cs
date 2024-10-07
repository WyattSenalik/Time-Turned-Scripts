using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers.SceneLoading
{
    /// <summary>
    /// Loads a scene on start.
    /// </summary>
    public class LoadSceneAutomatically : MonoBehaviour
    {
        [SerializeField, Scene] private string m_sceneToLoad = "";


        // Start is called before the first frame update
        private void Start()
        {
            SceneManager.LoadScene(m_sceneToLoad);
        }
    }
}
