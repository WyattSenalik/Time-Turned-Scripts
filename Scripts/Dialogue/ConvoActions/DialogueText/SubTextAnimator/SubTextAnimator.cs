using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Helpers.Animation.BetterCurve;
using Helpers.Singletons;
using static Dialogue.TypeTextVertexController;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Text
{
    [DisallowMultipleComponent]
    public sealed class SubTextAnimator : SingletonMonoBehaviour<SubTextAnimator>
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private TypeTextVertexController m_textVertCont = null;

        private readonly Dictionary<DialogueSubText, SubTextDictValue> m_textsToAnimate = new Dictionary<DialogueSubText, SubTextDictValue>();


        protected override void Awake()
        {
            base.Awake();
            #region Logs
            //CustomDebug.AssertSerializeFieldIsNotNull(m_textVertCont, nameof(m_textVertCont), this);
            #endregion Logs
        }
        private void Start()
        {
            m_textVertCont.onVertexContUpdate.ToggleSubscription(OnTextMeshUpdate, true);
        }
        private void OnDestroy()
        {
            m_textVertCont?.onVertexContUpdate.ToggleSubscription(OnTextMeshUpdate, false);
        }

        private void OnTextMeshUpdate(VertexControllerUpdateEventData eventData)
        {
            // Nothing to animate.
            if (m_textsToAnimate.Count <= 0) { return; }

            foreach (KeyValuePair<DialogueSubText, SubTextDictValue> t_kvp in m_textsToAnimate)
            {
                SubTextDictValue t_dictValue = t_kvp.Value;
                DialogueSubTextAnimationSpecs t_animSpecs = t_dictValue.animSpecs;
                int t_charIndex = t_dictValue.charIndex;
                int t_textLength = t_dictValue.textLength;
                float t_curAnimTime = t_dictValue.curAnimTime;
                string t_text = t_kvp.Key.text;

                // If the animation isn't supposed to loop and the animation has finished.
                if (!t_animSpecs.loop && t_curAnimTime > t_animSpecs.GetLongestAnimationTime())
                {
                    continue;
                } 

                TMP_CharacterInfo t_charInfo = m_textVertCont.GetCharacterInfo(t_charIndex);
                // Stop animating this character if it hasn't even been typed yet.
                if (eventData.mostRecentlyTypedCharIndex < t_charIndex)
                {
                    continue;
                }
                // Stop if we want to wait until after the sub text is fully typed.
                if (t_animSpecs.waitUntilAfterFullSubTyped)
                {
                    if (eventData.mostRecentlyTypedCharIndex < (t_charIndex + t_textLength - 1))
                    {
                        continue;
                    }
                }
                // Define the positional offset for the whole subtext.
                Vector3 t_offset = Vector3.zero;
                // Define the positional offset for each individual character.
                Vector3[] t_offsetPerCharacter = new Vector3[t_textLength];
                for (int i = 0; i < t_textLength; ++i)
                {
                    t_offsetPerCharacter[i] = Vector3.zero;
                }

                /// Animate Rotation
                if (t_animSpecs.hasRotationalAnimation)
                {
                    AnimateRotation(t_animSpecs.rotationAnimSpecs, t_text, t_charIndex, t_curAnimTime);
                }

                /// Animate Position
                // Add offset vertically.
                if (t_animSpecs.hasVerticalAnimation)
                {
                    AnimateVertical(t_animSpecs.verticalAnimSpecs, t_text, ref t_charInfo, t_curAnimTime);
                }
                // Add offset horizontally.
                if (t_animSpecs.hasHorizontalAnimation)
                {
                    AnimateHorizontal(t_animSpecs.horizontalAnimSpecs, t_text, ref t_charInfo, t_curAnimTime);
                }

                /// Color.
                if (t_animSpecs.hasColorAnimation)
                {
                    AnimateColor(t_animSpecs.colorAnimSpecs, t_text, t_charIndex, t_curAnimTime, eventData.mostRecentlyTypedCharIndex);
                }

                // Apply offset for the whole sub text.
                for (int i = 0; i < 4 * t_textLength; ++i)
                {
                    int t_index = t_charInfo.vertexIndex + i;
                    m_textVertCont.AddOffsetToVertex(t_index, t_offset);
                }
                // Apply offset per each character
                for (int i = 0; i < t_textLength; ++i)
                {
                    Vector3 t_curCharOffset = t_offsetPerCharacter[i];
                    for (int k = 0; k < 4; ++k)
                    {
                        int t_index = t_charInfo.vertexIndex + (i * 4) + k;
                        m_textVertCont.AddOffsetToVertex(t_index, t_curCharOffset);
                    }
                }

                // Update the time.
                t_dictValue.curAnimTime += Time.deltaTime;
            }
        }


        public void RegisterAnimatedSubText(DialogueSubText subText, int charIndexInWholeLine)
        {
            #region Logs
            //CustomDebug.LogForComponent($"Registered SubText ({subText}) to be animated with index ({charIndexInWholeLine})", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(!m_textsToAnimate.ContainsKey(subText), $"Subtext ({subText}) has already been registered.", this);
            #endregion Asserts
            int t_length = subText.text.Length;
            m_textsToAnimate.Add(subText, new SubTextDictValue(subText.animSpecs, charIndexInWholeLine, t_length));
        }
        public bool UnregisterAnimatedSubText(DialogueSubText subText)
        {
            #region Logs
            //CustomDebug.LogForComponent($"Unregistered SubText ({subText}) from being animated.", this, IS_DEBUGGING);
            #endregion Logs
            return m_textsToAnimate.Remove(subText);
        }


        private void AnimateVertical(MotionAnimationSpecs animSpecs, string text, ref TMP_CharacterInfo charInfo, float time)
        {
            BetterCurve t_curve = animSpecs.animCurve;
            int t_textLength = text.Length;
            int t_spacesInText = GetAmountOfSpacesInText(text);
            // Apply animation separately to each char
            if (animSpecs.isPerChar)
            {
                for (int i = 0; i < t_textLength - t_spacesInText; ++i)
                {
                    int t_vertexIndex = charInfo.vertexIndex + i * 4;

                    float t_wrappedTime = CalculateWrappedEvaluationTime(t_curve, animSpecs.delayBetweenChars, i, time);
                    float t_curVertOffset = t_curve.Evaluate(t_wrappedTime);
                    Vector3 t_offset = new Vector3(0.0f, t_curVertOffset, 0.0f);
                    for (int k = 0; k < 4; ++k)
                    {
                        m_textVertCont.AddOffsetToVertex(t_vertexIndex + k, t_offset);
                    }
                }
            }
            // Apply animation to all chars at same time
            else
            {
                float t_wrappedTime = time % t_curve.GetEndTime();
                float t_curVertOffset = t_curve.Evaluate(t_wrappedTime);
                Vector3 t_offset = new Vector3(0.0f, t_curVertOffset, 0.0f);
                for (int i = 0; i < t_textLength - t_spacesInText; ++i)
                {
                    int t_vertexIndex = charInfo.vertexIndex + i * 4;
                    for (int k = 0; k < 4; ++k)
                    {
                        m_textVertCont.AddOffsetToVertex(t_vertexIndex + k, t_offset);
                    }
                }
            }
        }
        private void AnimateHorizontal(MotionAnimationSpecs animSpecs, string text, ref TMP_CharacterInfo charInfo, float time)
        {
            BetterCurve t_curve = animSpecs.animCurve;
            int t_textLength = text.Length;
            int t_spacesInText = GetAmountOfSpacesInText(text);
            // Apply animation separately to each char
            if (animSpecs.isPerChar)
            {
                for (int i = 0; i < t_textLength - t_spacesInText; ++i)
                {
                    int t_vertexIndex = charInfo.vertexIndex + i * 4;

                    float t_wrappedTime = CalculateWrappedEvaluationTime(t_curve, animSpecs.delayBetweenChars, i, time);
                    float t_curHoriOffset = t_curve.Evaluate(t_wrappedTime);
                    Vector3 t_offset = new Vector3(t_curHoriOffset, 0.0f, 0.0f);
                    for (int k = 0; k < 4; ++k)
                    {
                        m_textVertCont.AddOffsetToVertex(t_vertexIndex + k, t_offset);
                    }
                }
            }
            // Apply animation to all chars at same time
            else
            {
                float t_wrappedTime = time % t_curve.GetEndTime();
                float t_curHoriOffset = t_curve.Evaluate(t_wrappedTime);
                Vector3 t_offset = new Vector3(t_curHoriOffset, 0.0f, 0.0f);
                for (int i = 0; i < t_textLength - t_spacesInText; ++i)
                {
                    int t_vertexIndex = charInfo.vertexIndex + i * 4;
                    for (int k = 0; k < 4; ++k)
                    {
                        m_textVertCont.AddOffsetToVertex(t_vertexIndex + k, t_offset);
                    }
                }
            }
        }
        private void AnimateRotation(MotionAnimationSpecs animSpecs, string text, int charIndex, float time)
        {
            BetterCurve t_curve = animSpecs.animCurve;
            int t_textLength = text.Length;
            int t_spacesInText = GetAmountOfSpacesInText(text);
            int t_textLengthAccountingForSpaces = t_textLength - t_spacesInText;
            // Apply animation separately to each char
            if (animSpecs.isPerChar)
            {
                for (int i = 0; i < t_textLengthAccountingForSpaces; ++i)
                {
                    TMP_CharacterInfo t_curCharInfo = m_textVertCont.GetCharacterInfo(charIndex + i);

                    float t_wrappedTime = CalculateWrappedEvaluationTime(t_curve, animSpecs.delayBetweenChars, i, time);
                    float t_curRotAngle = t_curve.Evaluate(t_wrappedTime);
                    Vector2 t_botLeftVertex = t_curCharInfo.bottomLeft;
                    Vector2 t_topRightVertex = t_curCharInfo.topRight;
                    Vector3 t_centerOfChar = new Vector2((t_botLeftVertex.x + t_topRightVertex.x) * 0.5f, (t_botLeftVertex.y + t_topRightVertex.y) * 0.5f); 
                    Matrix4x4 t_rotMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, 0.0f, t_curRotAngle), Vector3.one);
                    for (int k = 0; k < 4; ++k)
                    {
                        int t_index = t_curCharInfo.vertexIndex + k;
                        Vector3 t_vertex = m_textVertCont.GetVertex(t_index);
                        Vector3 t_newVertex = t_vertex;
                        t_newVertex -= t_centerOfChar;
                        t_newVertex = t_rotMat.MultiplyPoint3x4(t_newVertex);
                        t_newVertex += t_centerOfChar;
                        Vector3 t_offset = t_newVertex - t_vertex;
                        m_textVertCont.AddOffsetToVertex(t_index, t_offset);
                    }
                }
            }
            // Apply animation to all chars at same time
            else
            {
                TMP_CharacterInfo t_charInfo = m_textVertCont.GetCharacterInfo(charIndex);
                float t_wrappedTime = time % t_curve.GetEndTime();
                float t_curRotAngle = t_curve.Evaluate(t_wrappedTime);
                // Find the center of the whole subtext.
                Vector2 t_firstCharBotLeft = m_textVertCont.GetVertex(t_charInfo.vertexIndex);
                Vector2 t_lastCharTopRight = m_textVertCont.GetVertex(t_charInfo.vertexIndex + t_textLengthAccountingForSpaces * 4 - 2);
                // Find the tallest character and get that things height.
                float t_tallestCharY = t_lastCharTopRight.y;
                for (int i = 1; i < 4 * t_textLengthAccountingForSpaces; i += 4)
                {
                    Vector2 t_vert = m_textVertCont.GetVertex(t_charInfo.vertexIndex + i);
                    if (t_tallestCharY < t_vert.y)
                    {
                        t_tallestCharY = t_vert.y;
                    }
                } 
                Vector3 t_centerOfSubtext = new Vector2((t_firstCharBotLeft.x + t_lastCharTopRight.x) * 0.5f, (t_firstCharBotLeft.y + t_tallestCharY) * 0.5f);

                // Apply the rotation to all the char in the sub text.
                Matrix4x4 t_rotMat = Matrix4x4.TRS(Vector2.zero, Quaternion.Euler(0.0f, 0.0f, t_curRotAngle), Vector3.one);
                for (int i = 0; i < 4 * t_textLengthAccountingForSpaces; ++i)
                {
                    int t_index = t_charInfo.vertexIndex + i;
                    Vector3 t_curVertex = m_textVertCont.GetVertex(t_index);
                    Vector3 t_newVertex = t_curVertex;
                    t_newVertex -= t_centerOfSubtext;
                    t_newVertex = t_rotMat.MultiplyPoint3x4(t_newVertex);
                    t_newVertex += t_centerOfSubtext;
                    Vector3 t_offset = t_newVertex - t_curVertex;
                    m_textVertCont.AddOffsetToVertex(t_index, t_offset);
                }
            }
        }
        private void AnimateColor(ColorAnimationSpecs animSpecs, string text, int charIndex, float time, int mostRecentlyTypedCharIndex)
        {
            int t_textLength = text.Length;
            int t_spacesInText = GetAmountOfSpacesInText(text);
            int t_textLengthAccountingForSpaces = t_textLength;
            if (t_spacesInText > 0)
            {
                // Why do we do color different than the others? \_(0.0)_/
                t_textLengthAccountingForSpaces -= t_spacesInText - 1;
            }
            float t_charDelay = animSpecs.isPerChar ? animSpecs.delayBetweenChars : 0.0f;

            if (animSpecs.isPerVertex)
            {
                ColorCurve t_curveBL = animSpecs.curveBL;
                ColorCurve t_curveTL = animSpecs.curveTL;
                ColorCurve t_curveTR = animSpecs.curveTR;
                ColorCurve t_curveBR = animSpecs.curveBR;

                SetVertexColorFromCurve(t_textLengthAccountingForSpaces, charIndex, time, t_charDelay, t_curveBL, 0);
                SetVertexColorFromCurve(t_textLengthAccountingForSpaces, charIndex, time, t_charDelay, t_curveTL, 1);
                SetVertexColorFromCurve(t_textLengthAccountingForSpaces, charIndex, time, t_charDelay, t_curveTR, 2);
                SetVertexColorFromCurve(t_textLengthAccountingForSpaces, charIndex, time, t_charDelay, t_curveBR, 3);
            }
            else
            {
                ColorCurve t_curve = animSpecs.animCurve;
                for (int i = 0; i < t_textLengthAccountingForSpaces; ++i)
                {
                    int t_newCharIndex = charIndex + i;
                    // Don't color anything past the most recently typed index because then it shows it all at once.
                    if (t_newCharIndex > mostRecentlyTypedCharIndex) { break; }
                    #region Logs
                    //CustomDebug.LogForComponent($"Coloring {i}", this, IS_DEBUGGING);
                    #endregion Logs

                    float t_wrappedTime = CalculateWrappedEvaluationTime(t_curve, t_charDelay, i, time);
                    Color t_curColor = t_curve.Evaluate(t_wrappedTime);
                    m_textVertCont.SetCharacterColor(charIndex + i, t_curColor);
                }
            }
        }
        private void SetVertexColorFromCurve(int textLength, int charIndex, float time, float charDelay, ColorCurve curve, int vertexIndexOffset)
        {
            for (int i = 0; i < textLength; ++i)
            {
                float t_wrappedTime = CalculateWrappedEvaluationTime(curve, charDelay, i, time);
                Color t_curColor = curve.Evaluate(t_wrappedTime);
                int t_index = m_textVertCont.ConvertCharIndexToVertexIndex(charIndex + i);
                m_textVertCont.SetVertexColor(t_index + vertexIndexOffset, t_curColor);
            }
        }
        private float CalculateWrappedEvaluationTime(ICurve curve, float delayBetweenChars, int iterationIndex, float time)
        {
            float t_charDisplacement = iterationIndex * delayBetweenChars;
            float t_curveEndTime = curve.GetEndTime();
            float t_wrappedTime = (time + t_charDisplacement) % t_curveEndTime;
            int t_infProtect = 0;
            while (t_wrappedTime < 0)
            {
                t_wrappedTime += t_curveEndTime;
                #region InfinityProtection
                if (t_infProtect > 1000)
                {
                    //CustomDebug.LogErrorForComponent($"Infinite Loop Detected when wrapping time.", this);
                    return 0.0f;
                }
                #endregion InfinityProtection
            }
            return t_wrappedTime;
        }
        private int GetAmountOfSpacesInText(string text)
        {
            int t_spaces = 0;
            for (int i = 0; i < text.Length; ++i)
            {
                if (text[i] == ' ')
                {
                    ++t_spaces;
                }
            }
            return t_spaces;
        }

        public class SubTextDictValue
        {
            public DialogueSubTextAnimationSpecs animSpecs { get; private set; }
            public int charIndex { get; private set; }
            public int textLength { get; private set; }
            public float curAnimTime { get; set; }


            public SubTextDictValue(DialogueSubTextAnimationSpecs animSpecs, int charIndex, int textLength)
            {
                this.animSpecs = animSpecs;
                this.charIndex = charIndex;
                this.textLength = textLength;
                this.curAnimTime = 0.0f;
            }
        }
    }
}