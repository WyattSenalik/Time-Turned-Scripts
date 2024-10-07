using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class OptionsMenu : MonoBehaviour
    {
        public bool isOpen => m_curState != eOptionsMenuState.Closed;
        public IEventPrimer onOptionsMenuClose => m_onOptionsMenuClose;

        [SerializeField, Required] private PauseMenuNavigator m_optionsMenuNavigator = null;
        [SerializeField, Required] private PauseMenuNavigator m_languageMenuNav = null;
        [SerializeField, Required] private ControlsMenu m_controlsMenu = null;
        [SerializeField, Required] private PauseMenuNavigator m_gameplayMenuNav = null;
        [SerializeField, Required] private PauseMenuNavigator m_audioMenuNav = null;
        [SerializeField] private MixedEvent m_onOptionsMenuClose = new MixedEvent();

        private readonly GamePauser m_gamePauser = new GamePauser();
        private eOptionsMenuState m_curState = eOptionsMenuState.Closed;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_optionsMenuNavigator, nameof(m_optionsMenuNavigator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_languageMenuNav, nameof(m_languageMenuNav), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_controlsMenu, nameof(m_controlsMenu), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_gameplayMenuNav, nameof(m_gameplayMenuNav), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_audioMenuNav, nameof(m_audioMenuNav), this);
            #endregion Asserts
        }


        public void Navigate(Vector2 navDir)
        {
            switch (m_curState)
            {
                case eOptionsMenuState.Options:
                    m_optionsMenuNavigator.Vector2InputForMovingAndHorizontalInput(navDir);
                    break;
                case eOptionsMenuState.Language:
                    m_languageMenuNav.Vector2InputForMovingAndHorizontalInput(navDir);
                    break;
                case eOptionsMenuState.Controls:
                    m_controlsMenu.Navigate(navDir);
                    break;
                case eOptionsMenuState.Gameplay:
                    m_gameplayMenuNav.Vector2InputForMovingAndHorizontalInput(navDir);
                    break;
                case eOptionsMenuState.Audio:
                    m_audioMenuNav.Vector2InputForMovingAndHorizontalInput(navDir);
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_curState, this);
                    break;
            }
        }
        public void Submit()
        {
            switch (m_curState)
            {
                case eOptionsMenuState.Options:
                    m_optionsMenuNavigator.ChooseCurSelection();
                    break;
                case eOptionsMenuState.Language:
                    m_languageMenuNav.ChooseCurSelection();
                    break;
                case eOptionsMenuState.Controls:
                    m_controlsMenu.Submit();
                    break;
                case eOptionsMenuState.Gameplay:
                    m_gameplayMenuNav.ChooseCurSelection();
                    break;
                case eOptionsMenuState.Audio:
                    m_audioMenuNav.ChooseCurSelection();
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_curState, this);
                    break;
            }
        }
        public void Cancel()
        {
            switch (m_curState)
            {
                case eOptionsMenuState.Options: CloseOptionsMenu(); break;
                case eOptionsMenuState.Language: CloseLanguageMenu(); break;
                case eOptionsMenuState.Controls: CloseControlsMenu(); break;
                case eOptionsMenuState.Gameplay: CloseGameplayMenu(); break;
                case eOptionsMenuState.Audio: CloseAudioMenu(); break;
                default:
                    CustomDebug.UnhandledEnum(m_curState, this);
                    break;
            }
        }


        public void OpenOptionsMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(true);
            m_curState = eOptionsMenuState.Options;

            m_optionsMenuNavigator.SetSelected(0);
            m_gamePauser.RequestToPause();
        }
        public void CloseOptionsMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(false);
            m_curState = eOptionsMenuState.Closed;

            m_gamePauser.CancelRequestToPause();

            m_onOptionsMenuClose.Invoke();
        }

        public void OpenLanguageMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(false);
            m_languageMenuNav.gameObject.SetActive(true);
            m_curState = eOptionsMenuState.Language;
        }
        public void CloseLanguageMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(true);
            m_languageMenuNav.gameObject.SetActive(false);
            m_curState = eOptionsMenuState.Options;
        }

        public void OpenControlsMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(false);
            m_controlsMenu.gameObject.SetActive(true);
            m_curState = eOptionsMenuState.Controls;
        }
        public void CloseControlsMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(true);
            m_controlsMenu.gameObject.SetActive(false);
            m_curState = eOptionsMenuState.Options;
        }

        public void OpenGameplayMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(false);
            m_gameplayMenuNav.gameObject.SetActive(true);
            m_curState = eOptionsMenuState.Gameplay;
        }
        public void CloseGameplayMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(true);
            m_gameplayMenuNav.gameObject.SetActive(false);
            m_curState = eOptionsMenuState.Options;
        }

        public void OpenAudioMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(false);
            m_audioMenuNav.gameObject.SetActive(true);
            m_curState = eOptionsMenuState.Audio;
        }
        public void CloseAudioMenu()
        {
            m_optionsMenuNavigator.gameObject.SetActive(true);
            m_audioMenuNav.gameObject.SetActive(false);
            m_curState = eOptionsMenuState.Options;
        }


        public enum eOptionsMenuState { Closed, Options, Language, Controls, Gameplay, Audio }
    }
}