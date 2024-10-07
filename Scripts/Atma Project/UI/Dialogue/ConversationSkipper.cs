using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Atma.Settings;
using Dialogue;
using Dialogue.ConvoActions.Programmed;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma.Dialogue
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConversationDriver))]
    public sealed class ConversationSkipper : SingletonMonoBehaviour<ConversationSkipper>
    {
        [SerializeField, Required] private DialogueBoxDisplay m_dialogueBox = null;

        private ConversationDriver m_convoDriver = null;

        private bool m_isSkipButtonDown = false;


        protected override void Awake()
        {
            base.Awake();

            m_convoDriver = GetComponent<ConversationDriver>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_convoDriver, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_dialogueBox, nameof(m_dialogueBox), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_dialogueBox.shouldSkip = ShouldSkipDialogue();
        }
        private void Update()
        {
            bool t_shouldSkip = ShouldSkipDialogue();
            m_convoDriver.shouldSkip = t_shouldSkip;
            m_dialogueBox.shouldSkip = t_shouldSkip;
            if (m_convoDriver.isConversationActive)
            {
                if (t_shouldSkip)
                {
                    if (m_convoDriver.curAction is not ProgrammableConvoAction)
                    {
                        m_convoDriver.AdvanceDialogue();
                    }
                }
            }
            else
            {
                // If there is an inactive conversation, reset the skip button value
                SetSkipButtonValue(false);
            }
        }


        public bool ShouldSkipDialogue()
        {
            if (GameSettings.instance.skipDialogue)
            {
                return true;
            }
            return m_isSkipButtonDown;
        }
        public void SetSkipButtonValue(bool isSkipButtonSkipping)
        {
            m_isSkipButtonDown = isSkipButtonSkipping;
        }
    }
}