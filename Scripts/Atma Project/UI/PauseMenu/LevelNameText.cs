using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Helpers.Extensions;
using Atma.Translation;
using Atma.Settings;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class LevelNameText : MonoBehaviour
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

        [SerializeField] private bool m_hasOverrideName = false;
        [SerializeField, HideIf(nameof(m_hasOverrideName))] private string m_backupLevelName = "Missing Level Name";
        [SerializeField, ShowIf(nameof(m_hasOverrideName))] private string m_overrideName = "Override Name";

        private GameSettings m_settings = null;
        private TextMeshProUGUI m_textMesh = null;


        private void Awake()
        {
            m_textMesh = this.GetComponentSafe<TextMeshProUGUI>(this);
        }
        private void Start()
        {
            UpdateText();

            settings.onLanguageChanged.ToggleSubscription(UpdateText, true);
        }
        private void OnDestroy()
        {
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateText, false);
            }
        }


        private void UpdateText()
        {
            if (m_hasOverrideName)
            {
                m_textMesh.text = m_overrideName;
            }
            else
            {
                try
                {
                    LevelsList t_lvlList = LevelsList.GetMasterList();
                    int t_activeSceneSaveIndex = t_lvlList.GetSaveIndexOfActiveScene();
                    string t_lvlFlavorName;
                    if (t_activeSceneSaveIndex < 0)
                    {
                        t_lvlFlavorName = "";
                    }
                    else
                    {
                        t_lvlFlavorName = t_lvlList.GetFlavorNameForLevelAtSaveIndex(t_activeSceneSaveIndex);
                    }
                    m_textMesh.text = t_lvlFlavorName;
                }
                catch (System.Exception t_exception)
                {
                    //CustomDebug.LogError(t_exception.Message);
                    m_textMesh.text = m_backupLevelName;
                }
            }
            m_textMesh.ApplyFontToTextMeshForCurLanguage();
        }
    }
}