using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Atma.Settings;
using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class FullscreenOption : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI m_displayTextMesh = null;

        private GameSettings m_settings = null;
        private TranslatorFileReader m_translator = null;
        private eFullScreenOption m_curOption = eFullScreenOption.Borderless;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_displayTextMesh, nameof(m_displayTextMesh), this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            m_translator = TranslatorFileReader.instance;
            m_settings = GameSettings.instance;
            m_curOption = m_settings.fullScreenOption;
            UpdateTextMesh();

            m_settings.onLanguageChanged.ToggleSubscription(UpdateTextMesh, true);
        }
        private void OnDisable()
        {
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateTextMesh, false);
            }
        }


        public void GoToNextFullscreenOption()
        {
            eFullScreenOption t_newOpt = GetNextFullScreenOption(m_curOption);
            if (m_curOption == t_newOpt) { return; }

            m_curOption = t_newOpt;
            Vector2Int t_res = new Vector2Int(Screen.width, Screen.height);
            if (t_newOpt != eFullScreenOption.Windowed)
            {
                t_res = new Vector2Int(1920, 1080);
            }
            UpdateTextMesh();
            Screen.SetResolution(t_res.x, t_res.y, m_curOption.GetCorrespondingMode());

            m_settings.SetFullScreen(t_newOpt);
            m_settings.SetResolution(t_res);
        }


        private void UpdateTextMesh()
        {
            m_displayTextMesh.text = m_translator.GetTranslatedTextForKey(m_curOption.GetTranslationKey());
            m_displayTextMesh.ApplyFontToTextMeshForCurLanguage();
        }

        private static eFullScreenOption GetNextFullScreenOption(eFullScreenOption prevOption)
        {
            switch (prevOption)
            {
                case eFullScreenOption.Borderless:
                {
                    if (eFullScreenOption.FullScreen.IsSupportedByCurrentPlatform())
                    {
                        return eFullScreenOption.FullScreen;
                    }
                    else if (eFullScreenOption.Windowed.IsSupportedByCurrentPlatform())
                    {
                        return eFullScreenOption.Windowed;
                    }
                    else
                    {
                        // Only borderless supported
                        return eFullScreenOption.Borderless;
                    }
                }
                case eFullScreenOption.FullScreen:
                {
                    if (eFullScreenOption.Windowed.IsSupportedByCurrentPlatform())
                    {
                        return eFullScreenOption.Windowed;
                    }
                    else
                    {
                        return eFullScreenOption.Borderless;
                    }
                }
                case eFullScreenOption.Windowed:
                {
                    return eFullScreenOption.Borderless;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(prevOption, nameof(FullscreenOption.GetNextFullScreenOption));
                    return eFullScreenOption.Borderless;
                }
            }
        }
    }


    public enum eFullScreenOption { Borderless, FullScreen, Windowed }
    public static class FullScreenOptionsExtensions
    {
        public static FullScreenMode GetCorrespondingMode(this eFullScreenOption option)
        {
            switch (option)
            {
                case eFullScreenOption.Borderless: return FullScreenMode.FullScreenWindow;
                case eFullScreenOption.FullScreen: return FullScreenMode.ExclusiveFullScreen;
                case eFullScreenOption.Windowed: return FullScreenMode.Windowed;
                default:
                {
                    CustomDebug.UnhandledEnum(option, nameof(FullScreenOptionsExtensions.GetCorrespondingMode));
                    return FullScreenMode.FullScreenWindow;
                }
            }
        }
        public static bool IsSupportedByCurrentPlatform(this eFullScreenOption option)
        {
            switch (option)
            {
                // Supported by all platforms.
                case eFullScreenOption.Borderless: return true;
                // Only supported by windows.
                case eFullScreenOption.FullScreen:
                    switch (Application.platform)
                    {
                        case RuntimePlatform.WindowsEditor: return true;
                        case RuntimePlatform.WindowsPlayer: return true;
                        case RuntimePlatform.WindowsServer: return true;
                        default: return false;
                    }
                // Only supported by desktop.
                case eFullScreenOption.Windowed:
                    switch (Application.platform)
                    {
                        case RuntimePlatform.OSXEditor: return true;
                        case RuntimePlatform.OSXPlayer: return true;

                        case RuntimePlatform.WindowsPlayer: return true;
                        case RuntimePlatform.WindowsEditor: return true;

                        case RuntimePlatform.LinuxPlayer: return true;
                        case RuntimePlatform.LinuxEditor: return true;
                        
                        case RuntimePlatform.LinuxServer: return true;
                        case RuntimePlatform.WindowsServer: return true;
                        case RuntimePlatform.OSXServer: return true;
                        default: return false;
                    }
                default:
                    CustomDebug.UnhandledEnum(option, nameof(FullScreenOptionsExtensions.IsSupportedByCurrentPlatform));
                    return false;
            }
        }
        public static eTranslationKey GetTranslationKey(this eFullScreenOption option)
        {
            switch (option)
            {
                case eFullScreenOption.Borderless: return eTranslationKey.MENU_RES_OPT_BORDERLESS;
                case eFullScreenOption.FullScreen: return eTranslationKey.MENU_RES_OPT_FULL_SCREEN;
                case eFullScreenOption.Windowed: return eTranslationKey.MENU_RES_OPT_WINDOWED;
                default:
                {
                    CustomDebug.UnhandledEnum(option, nameof(FullScreenOptionsExtensions.GetTranslationKey));
                    return eTranslationKey.None;
                }
            }
        }
    }
}