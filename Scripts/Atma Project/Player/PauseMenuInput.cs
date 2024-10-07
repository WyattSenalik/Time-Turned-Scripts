using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;

using UnityEngine.InputSystem.Extension;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Player input for controlling the <see cref="PauseMenuNavigator"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputMapStack))]
    public sealed class PauseMenuInput : MonoBehaviour
    {
        [SerializeField, Required] private PauseMenuNavigator m_menuNavigator = null;
        [SerializeField, Required] private PauseMenuExecutor m_menuController = null;
        [SerializeField, Required] private OptionsMenu m_optionsMenu = null;
        [SerializeField, Required] private LevelSelectOptionsMenu m_levelSelectMenu = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_menuNavigator, nameof(m_menuNavigator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_menuController, nameof(m_menuController), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_optionsMenu, nameof(m_optionsMenu), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_levelSelectMenu, nameof(m_levelSelectMenu), this);
            #endregion Asserts
        }


        #region Input Messages
        private void OnPause(InputValue value)
        {
            if (value.isPressed)
            {
                m_menuController.Pause();
            }
        }
        private void OnUnpause(InputValue value)
        {
            if (value.isPressed)
            {
                if (m_optionsMenu.isOpen)
                {
                    m_optionsMenu.Cancel();
                    if (!m_optionsMenu.isOpen)
                    {
                        m_menuController.ToggleMenuVisibility(true);
                    }
                }
                else if (m_levelSelectMenu.isOpen)
                {
                    m_levelSelectMenu.CloseMenu();
                }
                else
                {
                    m_menuController.Resume();
                }
            }
        }

        private void OnNavigatePauseMenu(InputValue value)
        {
            if (!m_menuNavigator.enabled) { return; }

            Vector2 t_dir = value.Get<Vector2>();
            if (m_optionsMenu.isOpen)
            {
                m_optionsMenu.Navigate(t_dir);
            }
            else if (m_levelSelectMenu.isOpen)
            {
                m_levelSelectMenu.Navigate(t_dir);
            }
            else
            {
                m_menuNavigator.Vector2InputForMovingAndHorizontalInput(t_dir);
            }
        }
        private void OnSubmitPauseMenu(InputValue value)
        {
            if (!m_menuNavigator.enabled) { return; }

            if (value.isPressed)
            {
                if (m_optionsMenu.isOpen)
                {
                    m_optionsMenu.Submit();
                }
                else if (m_levelSelectMenu.isOpen)
                {
                    m_levelSelectMenu.Submit();
                }
                else
                {
                    m_menuNavigator.ChooseCurSelection();
                }
            }
        }
        private void OnScrollWheel(InputValue value)
        {
            if (!m_menuNavigator.enabled) { return; }

            if (m_levelSelectMenu.isOpen)
            {
                m_levelSelectMenu.Scroll(value.Get<Vector2>().y);
            }
        }
        #endregion Input Messages
    }
}