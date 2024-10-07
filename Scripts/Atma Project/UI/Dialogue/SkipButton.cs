using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using NaughtyAttributes;
using TMPro;

using Dialogue;
using Helpers.Singletons;
using TMPro.Extension;
using Atma.Translation;
using UnityEngine.UI;
using Atma.Settings;

namespace Atma.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class SkipButton : SingletonMonoBehaviour<SkipButton>, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool isHovered { get; private set; } = false;
        public bool isSkipping { get; private set; } = false;

        [SerializeField, Required] private DialogueBoxDisplay m_dialogueBox = null;
        [SerializeField, Required] private TextMeshProUGUI m_skipTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_preHoldTextMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_postHoldTextMesh = null;
        [SerializeField, Required] private Image m_holdInputImg = null;
        [SerializeField] private Color m_defaultColor = Color.white;
        [SerializeField] private Color m_hoveredColor = Color.yellow;
        [SerializeField] private Color m_activeColor = Color.green;

        private ConversationDriver m_convoDriver = null;
        private ConversationSkipper m_skipper = null;
        private PlayerSingleton m_player = null;
        private TranslatorFileReader m_translator = null;
        private GameSettings m_settings = null;

        private bool m_isDialogueBoxShowing = false;

        private float m_percentFilled = 0;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_dialogueBox, nameof(m_dialogueBox), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_skipTextMesh, nameof(m_skipTextMesh), this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            m_settings = GameSettings.instance;
            m_settings.onLanguageChanged.ToggleSubscription(UpdateVisuals, true);
        }
        private void OnDisable()
        {
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateVisuals, false);
            }
        }
        private void Start()
        {
            m_convoDriver = ConversationDriver.GetInstanceSafe(this);
            m_skipper = ConversationSkipper.GetInstanceSafe(this);
            m_translator = TranslatorFileReader.instance;
            // This is a new dialogue/level, don't want it skipping.
            m_skipper.SetSkipButtonValue(false);

            m_player = PlayerSingleton.GetInstanceSafe();
            ToggleSubscriptions(true);

            UpdateVisuals();
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public void SkipCurrentDialogue()
        {
            isSkipping = true;
            UpdateVisuals();

            m_skipper.SetSkipButtonValue(isSkipping);
        }
        public void UpdateTimeHeld(float percentHeld)
        {
            if (m_percentFilled == percentHeld) { return; }
            m_percentFilled = Mathf.Clamp01(percentHeld);
            UpdateVisuals();
        }
        public void ResetSkipButton()
        {
            isSkipping = false;
            UpdateVisuals();

            m_skipper.SetSkipButtonValue(isSkipping);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            UpdateVisuals();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            UpdateVisuals();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            SkipCurrentDialogue();
        }


        private void UpdateVisuals()
        {
            if (m_skipper.ShouldSkipDialogue() || !m_isDialogueBoxShowing)
            {
                ToggleVisualEnabled(false);
                return;
            }
            ToggleVisualEnabled(true);

            // Skip Text
            m_skipTextMesh.text = DetermineSkipText();
            m_skipTextMesh.ApplyFontToTextMeshForCurLanguage();
            // PreHold Text
            // If the display is "Hold A down" where A is the image for pressing the A button, then "Hold" is the PreHold Text.
            m_preHoldTextMesh.text = m_translator.GetTranslatedTextForKey(eTranslationKey.UI_PROMPT_HOLD_PRE);
            m_preHoldTextMesh.ApplyFontToTextMeshForCurLanguage();
            // PostHold Text
            // If the display is "Hold A down" where A is the image for pressing the A button, then "down" is the PostHold Text.
            // In reality this is only used for specific non-english languages. In english it is empty.
            m_postHoldTextMesh.text = m_translator.GetTranslatedTextForKey(eTranslationKey.UI_PROMPT_HOLD_POST);
            m_preHoldTextMesh.ApplyFontToTextMeshForCurLanguage();

            // Update the image to reflect current bindings.
            m_holdInputImg.sprite = eInputAction.AdvanceDialogue.GetSpriteBasedOnCurrentBindings();
        }
        private string DetermineSkipText()
        {
            Color t_nonActiveColor = DetermineTextColor();
            string t_textString = m_translator.GetTranslatedTextForKey(eTranslationKey.UI_PROMPT_SKIP);

            // Filled start
            int t_charactersFilled = Mathf.FloorToInt(t_textString.Length * m_percentFilled);
            RichSubText t_filledSubText = new RichSubText(t_textString.Substring(0, t_charactersFilled), 0.0f);
            t_filledSubText.SetColorTag(m_activeColor);

            // Unfilled start
            RichSubText t_unfilledSubText = new RichSubText(t_textString.Substring(t_charactersFilled), 0.0f);
            t_unfilledSubText.SetColorTag(t_nonActiveColor);

            RichText t_displayText = new RichText(t_filledSubText, t_unfilledSubText);
            return t_displayText.GetFormattedText();
        }
        private void ToggleVisualEnabled(bool cond)
        {
            m_skipTextMesh.enabled = cond;
            m_preHoldTextMesh.enabled = cond;
            m_postHoldTextMesh.enabled = cond;
            m_holdInputImg.enabled = cond;
        }
        private Color DetermineTextColor()
        {
            if (isHovered)
            {
                return m_hoveredColor;
            }
            else if (isSkipping)
            {
                return m_activeColor;
            }
            else
            {
                return m_defaultColor;
            }
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_player != null)
            {
                m_player.onControlsChanged.ToggleSubscription(OnControlsChanged, cond);
            }
            if (m_dialogueBox != null)
            {
                m_dialogueBox.onShowAnimFin.ToggleSubscription(OnDialogueBoxShowAnimEnd, cond);
                m_dialogueBox.onHide.ToggleSubscription(OnDialogueBoxHideAnimBegin, cond);
            }
            if (m_convoDriver != null)
            {
                m_convoDriver.onConversationStarted.ToggleSubscription(OnConversationStarted, cond);
            }
        }
        private void OnControlsChanged(PlayerInput playerInp)
        {
            UpdateVisuals();
        }
        private void OnDialogueBoxShowAnimEnd()
        {
            m_isDialogueBoxShowing = true;
            UpdateVisuals();
        }
        private void OnDialogueBoxHideAnimBegin()
        {
            m_isDialogueBoxShowing = false;
            UpdateVisuals();
        }
        private void OnConversationStarted()
        {
            UpdateTimeHeld(0.0f);
            isSkipping = false;
        }
    }
}