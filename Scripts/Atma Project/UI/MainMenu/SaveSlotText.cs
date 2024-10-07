using UnityEngine;

using TMPro;

using Atma.Saving;
using Atma.Settings;
using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class SaveSlotText : MonoBehaviour
    {
        private SaveManager saveMan
        {
            get
            {
                if (m_saveMan == null)
                {
                    m_saveMan = SaveManager.instance;
                    #region Asserts
                    //CustomDebug.AssertSingletonIsNotNull(m_saveMan, this);
                    #endregion Asserts
                }
                return m_saveMan;
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

        [SerializeField, Range(0, 2)] private int m_saveSlot = 0;
        [SerializeField] private eTranslationKey m_newGameKey = eTranslationKey.MENU_NEW_GAME;

        private SaveManager m_saveMan = null;
        private TranslatorFileReader m_translator = null;
        private GameSettings m_settings = null;
        private TextMeshProUGUI m_textMesh = null;


        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            #endregion Asserts
        }
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


        public void ForceUpdateText()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (saveMan.DoesExistingSaveDataExist(m_saveSlot))
            {
                ISaveData t_slotData = saveMan.PeekSaveData(m_saveSlot);
                m_textMesh.text = t_slotData.playerName;
            }
            else
            {
                // No save data
                m_textMesh.text = translator.GetTranslatedTextForKey(m_newGameKey);
            }
            m_textMesh.ApplyFontToTextMeshForCurLanguage();
        }
    }
}