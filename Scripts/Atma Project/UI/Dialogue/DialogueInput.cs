using UnityEngine;
using UnityEngine.InputSystem;

using Dialogue;
using Helpers.Extensions;
using UnityEngine.InputSystem.Extension;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma.Dialogue
{
    /// <summary>
    /// Input on the player for advancing the dialogue.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputMapStack))]
    public sealed class DialogueInput : MonoBehaviour
    {
        public bool isHolding { get; private set; } = false;
        public float timeHeld { get; private set; } = 0.0f;

        [SerializeField, Min(0.0f)] private float m_requiredHoldTimeBeforeSkip = 1.0f;
        [SerializeField, Min(0.0f)] private float m_delayBeforeSkipStart = 0.5f;
        [SerializeField] private string m_dialogueInputMapName = "Dialogue";

        [SerializeField] private MixedEvent m_onAdvance = new MixedEvent();

        private ConversationDriver m_convoDriver = null;
        private SkipButton m_skipButt = null;
        private InputMapStack m_inputMapStack = null;


        private void Start()
        {
            m_convoDriver = ConversationDriver.GetInstanceSafe(this);
            m_skipButt = SkipButton.GetInstanceSafe(this);
            m_inputMapStack = this.GetComponentSafe<InputMapStack>(this);
        }
        private void Update()
        {
            if (!m_inputMapStack.GetActiveInputMapName().Equals(m_dialogueInputMapName))
            {
                // Input map is not the dialogue one, reset the input.
                isHolding = false;
                timeHeld = 0.0f;
                return;
            }

            if (m_skipButt.isSkipping)
            {
                timeHeld = 0.0f;
                return;
            }
            else if (!m_skipButt.gameObject.activeInHierarchy)
            {
                timeHeld = 0.0f;
                return;
            }

            if (isHolding)
            {
                if (timeHeld >= m_delayBeforeSkipStart)
                {
                    float t_totalRequiredHoldTime = m_delayBeforeSkipStart + m_requiredHoldTimeBeforeSkip;
                    if (timeHeld >= t_totalRequiredHoldTime)
                    {
                        m_skipButt.SkipCurrentDialogue();
                    }
                    float t_barPercent = (timeHeld - m_delayBeforeSkipStart) / m_requiredHoldTimeBeforeSkip;
                    m_skipButt.UpdateTimeHeld(t_barPercent);
                }
                timeHeld += Time.unscaledDeltaTime;
            }
            else
            {
                m_skipButt.UpdateTimeHeld(0.0f);
            }

        }


        #region InputMessages
        private void OnAdvance(InputValue value)
        {
            // When released
            if (!value.isPressed)
            {
                m_convoDriver.AdvanceDialogue();
                m_onAdvance.Invoke();
            }

            isHolding = value.isPressed;

            if (isHolding && m_skipButt.isHovered)
            {
                // Might be that the advance input was a mouse click, we need to check that.
                string t_path = eInputAction.AdvanceDialogue.ExtractBindingPathFromCurrentMappings();
                if (t_path.Contains("Mouse"))
                {
                    // It was a mouse click, so just skip dialogue right away.
                    m_skipButt.SkipCurrentDialogue();
                }
            }

            if (!isHolding)
            {
                timeHeld = 0.0f;
            }
        }
        #endregion InputMessages
    }
}