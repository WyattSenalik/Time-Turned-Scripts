using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using NaughtyAttributes;
using TMPro;

using Atma.UI;
using Dialogue;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    [RequireComponent(typeof(PlayerInput))]
    [DisallowMultipleComponent]
    public sealed class ConfirmNameInput : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] private NavigatableMenu m_navMenu = null;
        [SerializeField, Required] private TMP_InputField m_inputField = null;
        [SerializeField, Required] private DynamicStringReference m_dynRefToPlayerName = null;
        [SerializeField] private UnityEvent m_onConfirmName = new UnityEvent();

        private PlayerInput m_playerInp = null;
        

        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputField, nameof(m_inputField), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_dynRefToPlayerName, nameof(m_dynRefToPlayerName), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe(this);
            m_playerInp = t_playerSingleton.GetComponentSafe<PlayerInput>();
        }

        public void SubmitName()
        {
            if (m_inputField.text.Length <= 0)
            {
                // No name given, so ignore.
                #region Logs
                //CustomDebug.LogForComponent($"No name given.", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }
            #region Logs
            //CustomDebug.LogForComponent($"Name Confirmed to be {m_inputField.text}.", this, IS_DEBUGGING);
            #endregion Logs
            m_dynRefToPlayerName.SetDynamicStringValue(m_inputField.text);
            m_onConfirmName.Invoke();
        }

        private void OnNavigate(InputValue value)
        {
            if (m_playerInp.IsCurrentControlSchemeController())
            {
                m_navMenu.Navigate(value.Get<Vector2>());
            }
        }
        private void OnSubmit(InputValue value)
        {
            eControlScheme t_curScheme = m_playerInp.GetCurrentControlScheme();

            switch (t_curScheme)
            {
                case eControlScheme.MouseKeyboard:
                {
                    if (!value.isPressed)
                    {
                        SubmitName();
                    }
                    break;
                }
                case eControlScheme.GamePad:
                {
                    if (value.isPressed)
                    {
                        m_navMenu.Submit();
                    }
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(t_curScheme, this);
                    break;
                }
            }
        }
    }
}