using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Text
{
    [CreateAssetMenu(fileName = "new DialogueSubText", menuName = "ScriptableObjects/Dialogue/ConvoActions/DialogueText/DialogueSubText")]
    public sealed class DialogueSubText : ScriptableObject
    {
        public string text
        {
            get
            {
                if (m_useTextReference)
                {
                    return m_textReference.refString;
                }
                return m_text;
            }
        }
        public bool isAnimated => m_isAnimated;
        public DialogueSubTextAnimationSpecs animSpecs => m_animSpecs;

        [SerializeField] private bool m_useTextReference = false;
        [SerializeField, ResizableTextArea, HideIf(nameof(m_useTextReference))]
        private string m_text = "Placeholder";
        [SerializeField, Required, ShowIf(nameof(m_useTextReference))]
        private StringReference m_textReference = null;

        [SerializeField] private bool m_overrideColor = false;
        [SerializeField, ShowIf(nameof(m_overrideColor))]
        private bool m_useColorReference = false;
        [SerializeField, ShowIf(nameof(ShouldShowColor))]
        private Color m_color = Color.white;
        [SerializeField, Required, ShowIf(nameof(ShouldShowColorReference))]
        private ColorReference m_colorRef = null;

        [SerializeField] private bool m_overrideTextSize = false;
        [SerializeField, Min(0.0f), ShowIf(nameof(m_overrideTextSize))]
        private float m_textSize = 12.0f;

        [SerializeField] private bool m_ignoreBaseWaitTime = false;
        [SerializeField, Min(0.0f)]
        private float m_additionalCharacterDelay = 0.0f;

        [SerializeField] private bool m_isBold = false;
        [SerializeField] private bool m_isItalic = false;

        [SerializeField] private bool m_isAnimated = false;
        [SerializeField, ShowIf(nameof(m_isAnimated)), AllowNesting]
        private DialogueSubTextAnimationSpecs m_animSpecs = new DialogueSubTextAnimationSpecs();


        public Color GetColor(ConvoDefaults defaultData)
        {
            if (m_overrideColor)
            {
                if (m_useColorReference)
                {
                    return m_colorRef.color;
                }
                else
                {
                    return m_color;
                }
            }
            return defaultData.color;
        }
        public float GetTextSize(ConvoDefaults defaultData)
        {
            if (m_overrideTextSize)
            {
                return m_textSize;
            }
            return defaultData.textSize;
        }
        public float GetCharacterDelay(ConvoDefaults defaultData)
        {
            if (m_ignoreBaseWaitTime)
            {
                return m_additionalCharacterDelay;
            }
            return (defaultData.characterDelay + m_additionalCharacterDelay) * DialogueSettings.charDelayMultiplier;
        }
        public RichSubText ToRichSubText(ConvoDefaults defaultData)
        {
            Color t_color = GetColor(defaultData);
            float t_textSize = GetTextSize(defaultData);
            float t_charDelay = GetCharacterDelay(defaultData);

            RichSubText t_richSubText = new RichSubText(text, t_charDelay);
            if (m_overrideColor)
            {
                t_richSubText.SetColorTag(t_color);
            }
            if (m_overrideTextSize)
            {
                int t_percentSize = Mathf.RoundToInt((t_textSize / defaultData.textSize) * 100);
                t_richSubText.SetPercentSizeTag(t_percentSize);
            }
            if (m_isBold)
            {
                t_richSubText.SetBoldTag(true);
            }
            if (m_isItalic)
            {
                t_richSubText.SetItalicTag(true);
            }

            return t_richSubText;
        }


        private bool ShouldShowColor() => m_overrideColor && !m_useColorReference;
        private bool ShouldShowColorReference() => m_overrideColor && m_useColorReference;
    }
}
