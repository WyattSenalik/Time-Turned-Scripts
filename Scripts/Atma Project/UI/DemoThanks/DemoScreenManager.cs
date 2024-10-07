using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;

namespace Atma.UI.Demo
{
    [DisallowMultipleComponent]
    public sealed class DemoScreenManager : MonoBehaviour
    {
        [SerializeField] private PauseMenuNavigator m_menuNav = null;
        [SerializeField, Scene] private string m_mainMenuSceneName = "MainMenu_SCENE";


        public void ReturnToMainMenu()
        {
            SceneLoader t_sceneLoader = SceneLoader.GetInstanceSafe();
            t_sceneLoader.LoadScene(m_mainMenuSceneName, false);
        }

        #region InputMessages
        private void OnSubmit(InputValue value)
        {
            if (value.isPressed)
            {
                m_menuNav.ChooseCurSelection();
            }
        }
        private void OnNavigate(InputValue value)
        {
            Vector2 t_inputVector = value.Get<Vector2>();
            m_menuNav.NavigateVertical(t_inputVector.y);
        }
        #endregion InputMessages
    }
}