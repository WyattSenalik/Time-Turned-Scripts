using UnityEngine;
using UnityEngine.InputSystem.Samples.RebindUI;

using Atma.UI;
using Atma.Settings;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ControlsMenu : MonoBehaviour
    {
        public IEventPrimer onOpen => m_onOpen;
        public IEventPrimer onClose => m_onClose;

        [SerializeField] private NavigatableMenu m_keyboardMenu = null;
        [SerializeField] private NavigatableMenu m_controllerMenu = null;
        [SerializeField] private NavigatableMenuOption m_keyboardsControllerOption = null;
        [SerializeField] private NavigatableMenuOption m_controllersKeyboardOption = null;
        [SerializeField] private MixedEvent m_onOpen = new MixedEvent();
        [SerializeField] private MixedEvent m_onClose = new MixedEvent();

        private eControlMenuState m_openMenu = eControlMenuState.Keyboard;
        private RebindActionUI[] m_keyboardRebinds = null;
        private RebindActionUI[] m_controllerRebinds = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_keyboardMenu, nameof(m_keyboardMenu), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_controllerMenu, nameof(m_controllerMenu), this);
            #endregion Asserts
            m_keyboardRebinds = m_keyboardMenu.GetComponentsInChildren<RebindActionUI>();
            m_controllerRebinds = m_controllerMenu.GetComponentsInChildren<RebindActionUI>();
        }
        private void OnEnable()
        {
            m_onOpen.Invoke();
        }
        private void OnDisable()
        {
            m_onClose.Invoke();
        }


        public void OpenKeyboardControls()
        {
            m_openMenu = eControlMenuState.Keyboard;
            m_keyboardMenu.gameObject.SetActive(true);
            m_controllerMenu.gameObject.SetActive(false);
            m_keyboardMenu.SetBeginOptionAsCurOption();
            m_keyboardsControllerOption.OnUnhighlighted();
            m_controllersKeyboardOption.OnUnhighlighted();
        }
        public void OpenControllerControls()
        {
            m_openMenu = eControlMenuState.Controller;
            m_keyboardMenu.gameObject.SetActive(false);
            m_controllerMenu.gameObject.SetActive(true);
            m_controllerMenu.SetBeginOptionAsCurOption();
            m_keyboardsControllerOption.OnUnhighlighted();
            m_controllersKeyboardOption.OnUnhighlighted();
        }
        public void RestoreDefaults()
        {
            switch (m_openMenu)
            {
                case eControlMenuState.Keyboard:
                    foreach (RebindActionUI t_rebind in m_keyboardRebinds)
                    {
                        t_rebind.ResetToDefault();
                    }
                    break;
                case eControlMenuState.Controller:
                    foreach (RebindActionUI t_rebind in m_controllerRebinds)
                    {
                        t_rebind.ResetToDefault();
                    }
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_openMenu, this);
                    break;
            }
        }
        public void SaveControlMappings()
        {
            GameSettings.instance.SaveControlMappings();
        }

        public void Navigate(Vector2 navDir)
        {
            NavigatableMenu t_curMenu = GetActiveMenu();
            t_curMenu.Navigate(navDir);
        }
        public void Submit()
        {
            NavigatableMenu t_curMenu = GetActiveMenu();
            t_curMenu.Submit();
        }


        private NavigatableMenu GetActiveMenu()
        {
            switch (m_openMenu)
            {
                case eControlMenuState.Keyboard: return m_keyboardMenu;
                case eControlMenuState.Controller: return m_controllerMenu;
                default:
                    CustomDebug.UnhandledEnum(m_openMenu, this);
                    return null;
            }
        }


        public enum eControlMenuState { Keyboard,  Controller }
    }
}