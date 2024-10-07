using UnityEngine;

using NaughtyAttributes;

using Dialogue;
using Timed;
using UnityEngine.InputSystem.Extension;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class InWorldTVCollider : MonoBehaviour
    {
        [SerializeField, Required] private Conversation m_tvConvo = null;
        [SerializeField] private string m_dialogueInputMapName = "Dialogue";
        [SerializeField, Min(0.0f)] private float m_interactRadius = 1.5f;
        [SerializeField] private Vector2 m_indicatorOffset = new Vector2(0.0f, 0.75f);

        private PlayerSingleton m_playerSingleton = null;
        private ConversationDriver m_convoDriver = null;
        private GlobalTimeManager m_timeMan = null;
        private InteractIndicator m_interactIndicator = null;
        private InputMapStack m_playerInputMapStack = null;
        private TVInteractInput m_playerInput = null;
        private float m_interactRadiusSqrd = float.NaN;
        private bool m_isPlayerInRange = false;
        private bool m_isInteractable = true;


        private void Awake()
        {
            m_interactRadiusSqrd = m_interactRadius * m_interactRadius;
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_tvConvo, nameof(m_tvConvo), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_convoDriver = ConversationDriver.GetInstanceSafe();
            m_timeMan = GlobalTimeManager.instance;
            m_interactIndicator = InteractIndicator.GetInstanceSafe();
            m_playerInput = m_playerSingleton.GetComponentSafe<TVInteractInput>();
            m_playerInputMapStack = m_playerSingleton.GetComponentSafe<InputMapStack>();
            ToggleSubscriptions(true);

            // Starts off true
            m_isInteractable = true;
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            Vector2 t_diff = m_playerSingleton.transform.position - transform.position;
            if (t_diff.sqrMagnitude <= m_interactRadiusSqrd)
            {
                m_isPlayerInRange = true;
            }
            else
            {
                m_isPlayerInRange = false;
            }
            UpdateInteractionVisual();
        }


        public void OnDialogueEnd()
        {
            m_timeMan.timeScale = 1.0f;
            m_playerInputMapStack.PopInputMap(m_dialogueInputMapName);
        }


        private void UpdateInteractionVisual()
        {
            if (m_isInteractable && m_isPlayerInRange)
            {
                // Show input prompt
                m_interactIndicator.MoveTo(transform.position, m_indicatorOffset, true);
            }
            else
            {
                // Hide input prompt
                m_interactIndicator.Hide();
            }
        }

        private void ToggleSubscriptions(bool cond)
        {
            m_playerInput?.onTriedToInteractWithTV.ToggleSubscription(OnPlayerTriedToInteractWithTV, cond);
        }
        private void OnPlayerTriedToInteractWithTV()
        {
            // Can't interact with tv right now.
            if (!m_isInteractable) { return; }
            // Player is out of range.
            if (!m_isPlayerInRange) { return; }

            m_isInteractable = false;
            UpdateInteractionVisual();

            m_timeMan.timeScale = 0.0f;
            m_playerInputMapStack.SwitchInputMap(m_dialogueInputMapName);
            m_convoDriver.StartConversation(m_tvConvo);
        }
    }
}