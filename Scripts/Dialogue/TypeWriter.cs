using System;
using System.Collections;
using UnityEngine;

using TMPro;

using Helpers.Events;
// Original Authors - Wyatt Senalik
// Stolen from SquaredUp (and then tweaked slightly)

namespace Dialogue
{
    /// <summary>Controls the text on this object to append characters one at a time</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(TypeTextVertexController))]
    public sealed class TypeWriter : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;
        private const bool IS_DEBUGGING_FRAME_RATE = false;

        public IEventPrimer<char> onSingleCharacterTyped => m_onSingleCharacterTyped;
        public IEventPrimer<TextChangedEventData> onTextChanged => m_onTextChanged;
        public IEventPrimer onStartedNewLine => m_onStartedNewLine;
        public IEventPrimer onFinishedTypingLine => m_onFinishedTypingLine;
        public TextMeshProUGUI typeText
        {
            get
            {
                if (m_typeText == null)
                {
                    m_typeText = GetComponent<TextMeshProUGUI>();
                    #region Asserts
                    //CustomDebug.AssertComponentIsNotNull(m_typeText, this);
                    #endregion Asserts
                }
                return m_typeText;
            }
        }
        public bool isTyping => m_isTypeCoroutineActive;


        [SerializeField, Range(0, 1)] private float m_startTransparency = 0.1f;
        
        [SerializeField] private MixedEvent<char> m_onSingleCharacterTyped = new MixedEvent<char>();
        [SerializeField] private MixedEvent<TextChangedEventData> m_onTextChanged = new MixedEvent<TextChangedEventData>();
        [SerializeField] private MixedEvent m_onStartedNewLine = new MixedEvent();
        [SerializeField] private MixedEvent m_onFinishedTypingLine = new MixedEvent();

        // Text to write to
        private TextMeshProUGUI m_typeText = null;
        // Controller for directly editing the text's vertices
        private TypeTextVertexController m_textVertexCont = null;

        // Curent line being written
        private RichText m_currentText = new RichText();
        // Function to call when finished tpying the current line.
        private Action m_onFinishedTyping = null;

        // Reference to running coroutine
        private Coroutine m_typeWriteCoroutine = null;
        private bool m_isTypeCoroutineActive = false;


        // Called 0th. Set references.
        private void Awake()
        {
            if (m_typeText == null)
            {
                m_typeText = GetComponent<TextMeshProUGUI>();
                m_textVertexCont = GetComponent<TypeTextVertexController>();
                #region Asserts
                //CustomDebug.AssertComponentIsNotNull(m_typeText, this);
                //CustomDebug.AssertComponentIsNotNull(m_textVertexCont, this);
                #endregion Asserts
            }
        }


        /// <summary>Type the current line</summary>
        public void TypeLine(RichText text, Action onFinishedTyping)
        {
            // So, we need to murder children here because TMPro makes submeshes and is very bad at cleaning them up. If we render a sprite atlas sprite in text, it will persist across lines due to how we are updating the mesh in the TypeTextVertexController.
            // But if we destroy the submesh here, it fixes it.
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform t_child = transform.GetChild(i);
                Destroy(t_child.gameObject);
            }

            m_currentText = text;
            m_typeText.text = "";
            m_onFinishedTyping = onFinishedTyping;
            if (enabled && gameObject.activeInHierarchy)
            {
                m_typeWriteCoroutine = StartCoroutine(TypeLineCoroutine());
                m_onStartedNewLine.Invoke();
            }
        }
        /// <summary>Show the whole line before the typewriter is done</summary>
        public void PreemptiveLineFinish()
        {
            if (m_isTypeCoroutineActive)
            {
                StopCoroutine(m_typeWriteCoroutine);
                m_isTypeCoroutineActive = false;
            }
            string t_formattedText = m_currentText.GetFormattedText();
            m_typeText.text = t_formattedText;

            // Invoke on text changed because of the preemptive finish.
            string t_literalText = m_currentText.GetLiteralText();
            int t_lastIndex = t_literalText.Length - 1;
            char t_curChar = t_literalText[t_lastIndex];
            float t_charDelay = m_currentText.GetCurrentCharacterDelay(t_lastIndex + 1);
            m_onTextChanged.Invoke(new TextChangedEventData(m_currentText.GetLiteralText(), t_formattedText, t_lastIndex, t_curChar, t_charDelay, 1.0f));

            InvokeFinishTyping();
        }
        /// <summary>Deletes any text currently being displayed.</summary>
        public void ClearText()
        {
            m_typeText.text = "";
        }


        /// <summary>Coroutine to make the letters appear slowly</summary>
        private IEnumerator TypeLineCoroutine()
        {
            m_isTypeCoroutineActive = true;

            float t_startTime = Time.time;
            float t_expectedTotalDelayTime = 0.0f;

            string t_literalText = m_currentText.GetLiteralText();
            int t_literalTextLength = t_literalText.Length;
            string t_formattedText = m_currentText.GetFormattedText();
            // We will show text by making the "untyped" text transparent first and then slowly moving over characters.
            RichSubText t_shownText = new RichSubText();
            RichSubText t_curCharText = new RichSubText();
            RichSubText t_hiddenText = new RichSubText(t_formattedText, -1.0f);
            // Give hidden text transparent color
            t_hiddenText.SetColorTag(new Color(0.0f, 0.0f, 0.0f, 0.0f));
            // Display text is the combination of all 3 of the texts.
            RichText t_displayText = new RichText(t_shownText, t_curCharText, t_hiddenText);

            // Set initial text. If its multi line, then make it left-aligned. If not, make it centered.
            m_typeText.text = t_displayText.GetFormattedText();
            m_typeText.alignment = IsMultiLine() ? TextAlignmentOptions.Left : TextAlignmentOptions.Center;

            // Iterate over the length of the text.
            for (int i = 0; i < t_literalTextLength; ++i)
            {
                float t_curCharStartTime = Time.time;

                char t_curChar = t_literalText[i];
                CustomDebug.RunDebugFunction(() =>
                {
                    if (t_curChar == '’')
                    {
                        //CustomDebug.LogError($"Found an incorrect comma (’ instead of '). Please correct this in the corresponding dialogue sub text.");
                        CustomDebug.Break();
                    }
                }, true);
                float t_charDelay = m_currentText.GetCurrentCharacterDelay(i + 1);
                #region Logs
                //CustomDebug.LogForComponent($"Literal Index ({i}). FormattedText ({t_formattedText}). CharDelay ({t_charDelay}).", this, IS_DEBUGGING);
                #endregion Logs
                t_shownText.literalText = m_currentText.GetFormattedText(i);
                // We are going to need to make the current character's color change to fade it in, so retrieve that character's color.
                RichSubText t_curCharsSubText = m_currentText.GetRichSubTextAtCharacterIndex(i);
                Color t_curCharColor = t_curCharsSubText.color;
                t_curCharText.literalText = m_currentText.GetFormattedTextSubstring(i, 1, eRichTag.Color);
                // Ignore color tags because we need to override color tags with the outer-wrapping invisible color tag.
                t_hiddenText.literalText = m_currentText.GetFormattedTextSubstring(i + 1, eRichTag.Color);

                // We need to adjust character delay to account for it the character was typed fast/slow because its possible that 1 frame is longer than the char delay.
                float t_desiredCharFinTime = t_startTime + t_expectedTotalDelayTime + t_charDelay;
                float t_adjustedCharDelay = t_desiredCharFinTime - Time.time;
                float t_endAlpha = t_curCharColor.a;
                float t = 0.0f;
                string t_newFormattedText;
                if (t_adjustedCharDelay > 0.0f)
                {
                    float t_inverseCharDelay = 1.0f / t_adjustedCharDelay;
                    float t_elapsedTime = 0.0f;
                    while (t < 1.0f)
                    {
                        t = t_elapsedTime * t_inverseCharDelay;
                        // Set the tag values for transparency.
                        t_curCharColor.a = Mathf.Lerp(m_startTransparency, t_endAlpha, t);
                        t_curCharText.SetColorTag(t_curCharColor);

                        // Update the text.
                        t_newFormattedText = t_displayText.GetFormattedText();
                        m_typeText.text = t_newFormattedText;
                        m_onTextChanged.Invoke(new TextChangedEventData(t_literalText, t_newFormattedText, i, t_curChar, t_charDelay, t));

                        // Wait until next frame.
                        yield return null;
                        // Increment time.
                        t_elapsedTime += Time.deltaTime;
                    }
                }
                t = 1.0f;
                // Set the tag values for transparency.
                t_curCharColor.a = t_endAlpha;
                t_curCharText.SetColorTag(t_curCharColor);
                // Update the text.
                t_newFormattedText = m_currentText.GetFormattedText();
                m_typeText.text = t_newFormattedText;
                m_onTextChanged.Invoke(new TextChangedEventData(t_literalText, t_newFormattedText, i, t_curChar, t_charDelay, t));

                t_expectedTotalDelayTime += t_charDelay;

                m_onSingleCharacterTyped.Invoke(t_curChar);

                #region Logs
                //CustomDebug.LogForComponent($"Char delay ({t_charDelay}) for char ({t_curChar}). Actually delay ({Time.time - t_curCharStartTime}).", this, IS_DEBUGGING_FRAME_RATE);
                #endregion Logs
            }

            m_isTypeCoroutineActive = false;
            // Finish Typing.
            InvokeFinishTyping();
        }
        private bool IsMultiLine()
        {
            m_typeText.ForceMeshUpdate();
            return m_typeText.textInfo.lineCount > 1;
        }
        /// <summary>Calls the function for onFinishedTyping and discards it.</summary>
        private void InvokeFinishTyping()
        {
            Action t_tempFinishedAction = m_onFinishedTyping;
            m_onFinishedTyping = null;

            m_onFinishedTypingLine.Invoke();
            t_tempFinishedAction?.Invoke();
        }


        public sealed class TextChangedEventData
        {
            public string newLiteralText { get; private set; }
            public string newText { get; private set; }
            public int curCharIndex { get; private set; }
            public char curChar { get; private set; }
            public float curCharDelay { get; private set; }
            public float percentWaitUntilTextChar { get; private set; }


            public TextChangedEventData(string newLiteralText, string newText, int curCharIndex, char curChar, float curCharDelay, float percentWaitUntilTextChar)
            {
                this.newLiteralText = newLiteralText;
                this.newText = newText;
                this.curCharIndex = curCharIndex;
                this.curChar = curChar;
                this.curCharDelay = curCharDelay;
                this.percentWaitUntilTextChar = percentWaitUntilTextChar;
            }
        }
    }
}
