using UnityEngine;

using NaughtyAttributes;

using Helpers.SceneLoading;
// Original Authors - Wyatt Senalik

namespace Atma.Testing
{
    [DisallowMultipleComponent]
    public sealed class ReplayGameButton : MonoBehaviour
    {
        [SerializeField, Scene] private string m_sceneToLoad = "0_CallToAction_SCENE";
        private SceneLoader m_sceneLoader = null;


        private void Start()
        {
            m_sceneLoader = SceneLoader.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_sceneLoader, this);
            #endregion Asserts
        }


        public void RestartGame()
        {
            m_sceneLoader.LoadScene(m_sceneToLoad);
        }
    }
}