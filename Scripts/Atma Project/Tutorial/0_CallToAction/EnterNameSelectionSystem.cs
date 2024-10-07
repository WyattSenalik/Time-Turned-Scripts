using Atma.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Atma.Tutorial
{
    public class EnterNameSelectionSystem : MonoBehaviour
    {
        [SerializeField] private PlayerInput m_playerInput = null;
        private PlayerSingleton m_player = null;

        [SerializeField] private TMP_InputField m_inputField = null;
        [SerializeField] private NavigatableMenu m_onScreenKeyboard = null;
        [SerializeField] private GameObject m_confirmText = null;

        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerInput, nameof(m_playerInput), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_button, nameof(m_button), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputField, nameof(m_inputField), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_onScreenKeyboard, nameof(m_onScreenKeyboard), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_confirmText, nameof(m_confirmText), this);
            #endregion Asserts
        }

        private void Start()
        {
            m_player = PlayerSingleton.GetInstanceSafe();
            ToggleSubscriptions(true);
        }

        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }

        public void InputSelection()
        {

            if (m_playerInput.IsCurrentControlSchemeController())
            {
                m_onScreenKeyboard.gameObject.SetActive(true);
                m_confirmText.SetActive(false);
                m_onScreenKeyboard.SetBeginOptionAsCurOption();
            }
            else
            {
                m_onScreenKeyboard.gameObject.SetActive(false);
                m_confirmText.SetActive(true);
                m_inputField.Select();
            }
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_player != null)
            {
                m_player.onControlsChanged.ToggleSubscription(OnControlsChanged, cond);
            }
        }

        private void OnControlsChanged(PlayerInput playerInp)
        {
            InputSelection();
        }
    }
}
