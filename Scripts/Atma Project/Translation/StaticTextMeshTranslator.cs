using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Atma.Settings;
// Original Authors - Wyatt Senalik

namespace Atma.Translation
{
    [DisallowMultipleComponent]
    public sealed class StaticTextMeshTranslator : MonoBehaviour
    {
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

        [SerializeField] private eTranslationKey m_key = eTranslationKey.None;
        [SerializeField, Required] private TextMeshProUGUI m_textToTranslate = null;

        [SerializeField] private eLanguage m_previewLanguage = eLanguage.EN;

        private TranslatorFileReader m_translator = null;
        private GameSettings m_settings = null;


        private void Awake()
        {
            if (m_key == eTranslationKey.None)
            {
                CustomDebug.LogErrorForComponent($"Forgot to set key to translate textmesh ({m_textToTranslate}).", this);
            }
        }
        private void OnEnable()
        {
            UpdateTextMesh();

            settings.onLanguageChanged.ToggleSubscription(UpdateTextMesh, true);
        }
        private void OnDisable()
        {
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateTextMesh, false);
            }
        }



        private void UpdateTextMesh()
        {
            m_textToTranslate.text = translator.GetTranslatedTextForKey(m_key);
            m_textToTranslate.ApplyFontToTextMeshForCurLanguage();
        }
        private void UpdateTextMesh(eLanguage previewLanguage)
        {
            // Override the current language
            m_textToTranslate.text = translator.GetTranslatedTextForKey(m_key, previewLanguage);
            m_textToTranslate.ApplyFontToTextMeshForLanguage(previewLanguage);
        }


        [Button]
        private void UpdateTextToPreviewLanguage()
        {
            UpdateTextMesh(m_previewLanguage);
        }
    }
}