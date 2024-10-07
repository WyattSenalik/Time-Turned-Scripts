using System;
using UnityEngine;

using NaughtyAttributes;
using Helpers;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Text
{
    [CreateAssetMenu(fileName = "new DialogueText", menuName = "ScriptableObjects/Dialogue/ConvoActions/DialogueText/DialogueText")]
    public sealed class DialogueText : ConvoActionObject
    {
        public override bool autoAdvance => m_autoAdvance;

        private const bool IS_DEBUGGING = false;

        public IReadOnlyCollection<DialogueSubText> subTexts => m_subTexts;

        [SerializeField] private bool m_autoAdvance = false;
        [SerializeField] private bool m_showDialogueBoxAtBottom = false;
        [SerializeField, Required, Expandable] private CharacterInfo m_characterInfo = null;
        [SerializeField, Expandable] private DialogueSubText[] m_subTexts = new DialogueSubText[1];


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            if (convoData.convoDriver.shouldSkip)
            {
                onFinished?.Invoke();
                return;
            }

            // Set character portrait and name.
            convoData.speakerNameTextMesh.text = m_characterInfo.characterName;
            convoData.portraitImg.sprite = m_characterInfo.characterPortrait;

            // Set isAlt for if should show at bottom
            convoData.dialougeBoxDisplay.SetIsAlt(m_showDialogueBoxAtBottom);

            // Set the sound
            convoData.typeTextSFX.SetWwiseTypeSoundEventID(m_characterInfo.audioCollection);

            if (!convoData.dialougeBoxDisplay.isShown)
            {
                // Clear previous text before showing.
                convoData.typeWriter.ClearText();
                // Show dialogue box if hidden.
                convoData.dialougeBoxDisplay.Show(() => StartTypingLine(convoData, onFinished));
            }
            else
            {
                // Box is already shown, just start typing right away.
                StartTypingLine(convoData, onFinished);
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            if (!convoData.convoDriver.shouldSkip)
            {
                // If dialogue box is still in the process of showing itself, don't advance yet.
                if (!convoData.dialougeBoxDisplay.isShown)
                { return false; }
                TypeWriter t_typeWriter = convoData.typeWriter;
                // If the line is still being typed, finish it early.
                if (convoData.typeWriter.isTyping)
                {
                    t_typeWriter.PreemptiveLineFinish();
                    return false;
                }
            }
            // Otherwise, line is not still being typed and we should go to next action.
            // Before advancing, remove animations.
            UnregisterSubTextsFromAnimator();
            return true;
        }

        private void StartTypingLine(ConvoData convoData, Action onFinished)
        {
            // Wait until dialogue box is shown before starting the dialogue.
            // Begin typing first text box.
            RichText t_richTextToType = BuildRichTextFromSubTexts(convoData.defaults);
            #region Logs
            //CustomDebug.LogForObject($"Typing RichText. Literal ({t_richTextToType.GetLiteralText()}). Formatted ({t_richTextToType.GetFormattedText()})", this, IS_DEBUGGING);
            #endregion Logs
            convoData.typeWriter.TypeLine(t_richTextToType, onFinished);
            // Register the animations
            RegisterSubTextsToAnimator();
        }
        public RichText BuildRichTextFromSubTexts(ConvoDefaults convoDefaults)
        {
            RichText t_richText = new RichText();
            foreach (DialogueSubText t_diagSubText in m_subTexts)
            {
                if (t_diagSubText == null) { continue; }
                RichSubText t_richSubText = t_diagSubText.ToRichSubText(convoDefaults);
                #region Logs
                //CustomDebug.LogForObject($"Appending ST. Formatted ({t_richSubText.GetFormattedText()}). Literal ({t_richSubText.literalText}). Raw ({t_diagSubText.text}).", this, IS_DEBUGGING);
                #endregion Logs
                t_richText.Add(t_richSubText);
            }
            return t_richText;
        } 
        private void RegisterSubTextsToAnimator()
        {
            SubTextAnimator t_animator = SubTextAnimator.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_animator, this);
            #endregion Asserts
            int t_traversedLength = 0;
            foreach (DialogueSubText t_diagSubText in m_subTexts)
            {
                if (t_diagSubText.isAnimated)
                {
                    t_animator.RegisterAnimatedSubText(t_diagSubText, t_traversedLength);
                }
                t_traversedLength += t_diagSubText.text.Length;
            }
        }
        private void UnregisterSubTextsFromAnimator()
        {
            SubTextAnimator t_animator = SubTextAnimator.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_animator, this);
            #endregion Asserts
            foreach (DialogueSubText t_diagSubText in m_subTexts)
            {
                if (t_diagSubText.isAnimated)
                {
                    t_animator.UnregisterAnimatedSubText(t_diagSubText);
                }
            }
        }
    }
}
