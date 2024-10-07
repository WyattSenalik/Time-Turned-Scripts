using UnityEngine;

using TMPro;

using Atma.Settings;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma.Translation
{
    [DisallowMultipleComponent]
    public sealed class FontsByLanguageManager : DynamicSingletonMonoBehaviourPersistant<FontsByLanguageManager>
    {
        private const float TIME_UNTIL_UNLOAD = 10.0f;

        private FontsByLanguage m_fontsByLang = null;
        private bool m_isLoaded = false;
        private float m_lastLoadTime = 0.0f;
        private float m_lastRefTime = 0.0f;

        protected override void OnSingletonCreated()
        {
            base.OnSingletonCreated();

            // Load the FontsByLanguage from resources
            LoadResourcesIfNotAlreadyLoaded();
            m_lastLoadTime = Time.time;
            m_lastRefTime = Time.time;
        }
        private void Update()
        {
            if (m_lastRefTime - m_lastLoadTime >= TIME_UNTIL_UNLOAD)
            {
                Resources.UnloadAsset(m_fontsByLang);
                m_isLoaded = false;
            }
        }


        public TMP_FontAsset GetFontForLanguage(eLanguage language)
        {
            foreach (FontsByLanguage.FontLanguage t_singleFontLanguage in m_fontsByLang.fontLanguagePairs)
            {
                if (t_singleFontLanguage.language == language)
                {
                    return t_singleFontLanguage.font;
                }
            }
            return m_fontsByLang.defaultFont;
        }


        private void LoadResourcesIfNotAlreadyLoaded()
        {
            if (!m_isLoaded)
            {
                m_fontsByLang = Resources.Load<FontsByLanguage>("Language Assets/FontLanguageList");
                if (m_fontsByLang == null)
                {
                    CustomDebug.LogError($"Failed to load FontLanguageList");
                }
                else
                {
                    m_isLoaded = true;
                }
            }
        }
    }


    public static class FontsByLanguageExtensions
    {
        public static void ApplyFontToTextMeshForLanguage(this TextMeshProUGUI textMesh, eLanguage language)
        {
            FontsByLanguageManager t_fontLangMan = FontsByLanguageManager.instance;
            textMesh.font = t_fontLangMan.GetFontForLanguage(language);
        }
        public static void ApplyFontToTextMeshForCurLanguage(this TextMeshProUGUI textMesh)
        {
            GameSettings t_settings = GameSettings.instance;
            ApplyFontToTextMeshForLanguage(textMesh, t_settings.language);
        }
    }
}