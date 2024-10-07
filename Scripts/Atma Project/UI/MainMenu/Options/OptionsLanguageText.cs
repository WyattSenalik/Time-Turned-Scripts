using UnityEngine;

using TMPro;

using Atma.Settings;
using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class OptionsLanguageText : MonoBehaviour
    {
        private GameSettings settings
        {
            get
            {
                if (m_settings == null)
                {
                    m_settings = GameSettings.instance;
                }
                return m_settings;
            }
        }
        private TranslatorFileReader translator
        {
            get
            {
                if (m_translator == null)
                {
                    m_translator = TranslatorFileReader.instance;
                }
                return m_translator;
            }
        }

        [SerializeField] private TextMeshProUGUI m_languageTextMesh = null;
        [SerializeField] private TextMeshProUGUI m_flagTextMesh = null;
        [SerializeField] private string m_spriteAtlasName = "Flags";

        private GameSettings m_settings = null;
        private TranslatorFileReader m_translator = null;


        private void OnEnable()
        {
            UpdateText();
            settings.onLanguageChanged.ToggleSubscription(UpdateText, true);
        }
        private void OnDisable()
        {
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateText, false);
            }
        }


        private void UpdateText()
        {
            string t_languageText = translator.GetTranslatedTextForKey(eTranslationKey.MENU_LANGUAGE);
            m_languageTextMesh.text = t_languageText;
            m_languageTextMesh.ApplyFontToTextMeshForCurLanguage();

            string t_flagName = settings.language.ToString();
            m_flagTextMesh.text = $"<sprite=\"{m_spriteAtlasName}\" name=\"{t_flagName}\">";
            // We don't update the flag's font for size reasons
        }
    }
}