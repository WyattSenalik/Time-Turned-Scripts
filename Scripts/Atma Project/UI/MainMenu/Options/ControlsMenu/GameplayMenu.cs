using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Atma.Settings;
using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class GameplayMenu : MonoBehaviour
    {
        [SerializeField, Required] private PauseMenuNavigator m_gameplayMenuNav = null;
        [SerializeField, Required] private TextMeshProUGUI m_skipDialogueHeaderTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_skipDialogueChoiceTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_textSpeedHeaderTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_textSpeedChoiceTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_runInBackgroundHeaderTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_runInBackgroundChoiceTextMesh = null;

        [SerializeField] private Color m_fadedDefaultColor = new Color(0.854902f, 0.8745098f, 0.9019608f);
        [SerializeField] private Color m_fadedSelectedColor = new Color(0.8f, 0.6980392f, 0.2392157f);

        [SerializeField] private bool m_isPauseMenu = false;

        private bool m_curSkipDialogueOpt = false;
        private eTextSpeed m_textSpeedOpt = eTextSpeed.Normal;
        private bool m_curRunInBackgroundOpt = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_gameplayMenuNav, nameof(m_gameplayMenuNav), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_skipDialogueChoiceTextMesh, nameof(m_skipDialogueChoiceTextMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_textSpeedHeaderTextMesh, nameof(m_textSpeedHeaderTextMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_textSpeedChoiceTextMesh, nameof(m_textSpeedChoiceTextMesh), this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            GameSettings t_settings = GameSettings.instance;
            m_curSkipDialogueOpt = t_settings.skipDialogue;
            m_textSpeedOpt = t_settings.textSpeed;
            m_curRunInBackgroundOpt = t_settings.runInBackground;

            UpdateTextMeshes();
            UpdateTextSpeedColors();
        }
        private void Start()
        {
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public void SwapSkipDialogueOption()
        {
            m_curSkipDialogueOpt = !m_curSkipDialogueOpt;
            GameSettings.instance.SetSkipDialogue(m_curSkipDialogueOpt);
            UpdateDialogueChoiceTextMesh();
            UpdateTextSpeedColors();
        }
        public void ChangeTextSpeedToNext(float horiDir)
        {
            if (horiDir > 0.1f)
            {
                m_textSpeedOpt += 1;
                if ((int)m_textSpeedOpt > (int)eTextSpeed.Fast)
                {
                    m_textSpeedOpt = eTextSpeed.Slow;
                }
            }
            else if (horiDir < -0.1f)
            {
                m_textSpeedOpt -= 1;
                if ((int)m_textSpeedOpt < 0)
                {
                    m_textSpeedOpt = eTextSpeed.Fast;
                }
            }
            else { return; }

            GameSettings.instance.SetTextSpeed(m_textSpeedOpt);
            UpdateTextSpeedChoiceTextMesh();
            UpdateTextSpeedColors();
        }
        public void SwapRunInBackgroundOption()
        {
            m_curRunInBackgroundOpt = !m_curRunInBackgroundOpt;
            GameSettings.instance.SetRunInBackground(m_curRunInBackgroundOpt);
            UpdateRunInBackgroundTextMesh();
            UpdateTextSpeedColors();
        }


        private void UpdateTextMeshes()
        {
            UpdateDialogueChoiceTextMesh();
            UpdateTextSpeedChoiceTextMesh();
            UpdateRunInBackgroundTextMesh();
        }
        private void UpdateDialogueChoiceTextMesh()
        {
            TranslatorFileReader t_translator = TranslatorFileReader.instance;
            string t_choice;
            if (m_curSkipDialogueOpt)
            {
                t_choice = t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_YES);
            }
            else
            {
                t_choice = t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_NO);
            }

            m_skipDialogueChoiceTextMesh.text = t_choice;
            m_skipDialogueChoiceTextMesh.ApplyFontToTextMeshForCurLanguage();
        }
        private void UpdateTextSpeedChoiceTextMesh()
        {
            m_textSpeedChoiceTextMesh.text = m_textSpeedOpt.ToText();
            m_textSpeedChoiceTextMesh.ApplyFontToTextMeshForCurLanguage();
        }
        private void UpdateRunInBackgroundTextMesh()
        {
            TranslatorFileReader t_translator = TranslatorFileReader.instance;
            string t_choice;
            if (m_curRunInBackgroundOpt)
            {
                t_choice = t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_YES);
            }
            else
            {
                t_choice = t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_NO);
            }

            m_runInBackgroundChoiceTextMesh.text = t_choice;
            m_runInBackgroundChoiceTextMesh.ApplyFontToTextMeshForCurLanguage();
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_gameplayMenuNav != null)
            {
                m_gameplayMenuNav.onSelectionUpdated.ToggleSubscription(UpdateTextSpeedColors, cond);
            }
        }
        private void UpdateTextSpeedColors()
        {
            if (m_curSkipDialogueOpt)
            {
                UpdateTextColorsFromChecksToDesired(m_textSpeedHeaderTextMesh, m_textSpeedChoiceTextMesh, m_gameplayMenuNav.selectedTextColor, m_gameplayMenuNav.defaultTextColor, m_fadedSelectedColor, m_fadedDefaultColor);
            }
            else
            {
                UpdateTextColorsFromChecksToDesired(m_textSpeedHeaderTextMesh, m_textSpeedChoiceTextMesh, m_fadedSelectedColor, m_fadedDefaultColor, m_gameplayMenuNav.selectedTextColor, m_gameplayMenuNav.defaultTextColor);
            }

            if (m_isPauseMenu)
            {
                UpdateTextColorsFromChecksToDesired(m_skipDialogueHeaderTextMesh, m_skipDialogueChoiceTextMesh, m_gameplayMenuNav.selectedTextColor, m_gameplayMenuNav.defaultTextColor, m_fadedSelectedColor, m_fadedDefaultColor);
            }
            else
            {
                UpdateTextColorsFromChecksToDesired(m_skipDialogueHeaderTextMesh, m_skipDialogueChoiceTextMesh, m_fadedSelectedColor, m_fadedDefaultColor, m_gameplayMenuNav.selectedTextColor, m_gameplayMenuNav.defaultTextColor);
            }
        }


        private void UpdateTextColorsFromChecksToDesired(TextMeshProUGUI headerMesh, TextMeshProUGUI choiceMesh, Color selectedCheckColor, Color defaultCheckColor, Color desiredSelectedColor, Color desiredDefaultColor)
        {
            if (headerMesh.color.Equals(selectedCheckColor))
            {
                headerMesh.color = desiredSelectedColor;
            }
            else if (headerMesh.color.Equals(defaultCheckColor))
            {
                headerMesh.color = desiredDefaultColor;
            }

            choiceMesh.color = desiredDefaultColor;
        }
    }
}