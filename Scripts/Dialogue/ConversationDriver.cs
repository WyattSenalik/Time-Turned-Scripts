using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
using TMPro;

using Helpers.Singletons;
using Helpers.Events;
using Dialogue.Sound;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DialogueBoxDisplay))]
    public sealed class ConversationDriver : SingletonMonoBehaviour<ConversationDriver>
    {
        private const bool IS_DEBUGGING = false;

        public DialogueBoxDisplay dialogueBoxDisplay => m_dialogueBoxDisplay;
        public ConvoActionObject curAction => m_curAction;
        public IEventPrimer onConversationStarted => m_onConversationStarted;
        public IEventPrimer onConversationEnded => m_onConversationEnded;
        public bool isConversationActive { get; private set; }
        public bool shouldSkip { get; set; }

        [SerializeField, Required] private TypeWriter m_typeWriter = null;
        [SerializeField, Required] private TypeTextSFX m_typeTextSFX = null;
        [SerializeField, Required] private TextMeshProUGUI m_speakerNameTextMesh = null;
        [SerializeField, Required] private Image m_portraitImg = null;

        [SerializeField] private bool m_autoBeginConversation = false;
        [SerializeField, ShowIf(nameof(m_autoBeginConversation))]
        private Conversation m_convoToAutoStart = null;

        [SerializeField] private MixedEvent m_onConversationStarted = new MixedEvent();
        [SerializeField] private MixedEvent m_onConversationEnded = new MixedEvent();

        private DialogueBoxDisplay m_dialogueBoxDisplay = null;

        private ConvoData m_curConvoData = null;
        private ConvoActionObject m_curAction = null;
        private Conversation m_curConversation = null;
        private int m_curActionIndex = -1;


        protected override void Awake()
        {
            base.Awake();

            m_dialogueBoxDisplay = GetComponent<DialogueBoxDisplay>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_typeWriter, nameof(m_typeWriter), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_typeTextSFX, nameof(m_typeTextSFX), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_speakerNameTextMesh, nameof(m_speakerNameTextMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_portraitImg, nameof(m_portraitImg), this);
            //CustomDebug.AssertComponentIsNotNull(m_dialogueBoxDisplay, this);
            #endregion Asserts
        }
        private void Start()
        {
            if (m_autoBeginConversation)
            {
                StartConversation(m_convoToAutoStart);
            }
        }


        public void StartConversation(Conversation conversation) => StartConversation(conversation, null);
        public void StartConversation(Conversation conversation, Action onConvoEnd)
        {
            isConversationActive = true;
            m_curConversation = conversation;
            m_curConvoData = new ConvoData(conversation.defaults, m_typeWriter, m_typeTextSFX, m_speakerNameTextMesh, m_portraitImg, m_dialogueBoxDisplay, this);
            SetTextDefaults();

            IReadOnlyList<ConvoActionObject> t_actions = conversation.conversationActions;
            // Do first action.
            m_curActionIndex = 0;
            DoNextAction(t_actions, onConvoEnd);

            m_onConversationStarted.Invoke();
        }
        public void AdvanceDialogue()
        {
            if (m_curAction.Advance(m_curConvoData))
            {
                ++m_curActionIndex;
                DoNextAction(m_curConversation.conversationActions);
            }
        }

        private void SetTextDefaults()
        {
            ConvoDefaults t_defaults = m_curConvoData.defaults;

            TextMeshProUGUI t_textMesh = m_typeWriter.typeText;
            t_textMesh.color = t_defaults.color;
            t_textMesh.fontSize = t_defaults.textSize;
        }
        private void DoNextAction(IReadOnlyList<ConvoActionObject> actions, Action onConvoEnd=null)
        {
            // Do the action if its not the last.
            if (m_curActionIndex < actions.Count)
            {
                m_curAction = actions[m_curActionIndex];
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(m_curAction != null, $"Convo Action specified on {m_curConversation.name} ({m_curConversation.GetType()}) with index {m_curActionIndex} to not be null.", this);
                #endregion Asserts
                #region Logs
                //CustomDebug.LogForComponent($"[{Time.frameCount}] Doing next action with index ({m_curActionIndex}) [{m_curAction.name}].", this, IS_DEBUGGING);
                #endregion Logs
                Action t_onActionFinished = m_curAction.autoAdvance ? () => AdvanceDialogue() : null;
                m_curAction.Begin(m_curConvoData, t_onActionFinished);
            }
            else
            {
                #region Logs
                //CustomDebug.LogForComponent($"Index ({m_curActionIndex}) was larger than action count ({actions.Count}).", this, IS_DEBUGGING);
                #endregion Logs
                // Last action was the last.
                onConvoEnd?.Invoke();
                OnConversationEnd();
            }
        }


        private void OnConversationEnd()
        {
            isConversationActive = false;
            m_dialogueBoxDisplay.Hide();
            m_onConversationEnded.Invoke();
        }
    }
}