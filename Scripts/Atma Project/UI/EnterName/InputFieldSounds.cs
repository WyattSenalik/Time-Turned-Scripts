using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Helpers;
using Helpers.Extensions;

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_InputField))]
    public sealed class InputFieldSounds : MonoBehaviour
    {
        [SerializeField, Required] private UIntReference m_typingSoundEventID = null;
        [SerializeField, Required] private UIntReference m_deleteSoundEventID = null;

        private TMP_InputField m_inputField = null;
        private string m_prevValue = "";


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_typingSoundEventID, nameof(m_typingSoundEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_deleteSoundEventID, nameof(m_deleteSoundEventID), this);
            #endregion Asserts
            m_inputField = this.GetComponentSafe<TMP_InputField>();
        }
        private void Start()
        {
            ToggleSubscription(true);
        }
        private void OnDestroy()
        {
            ToggleSubscription(false);
        }


        private void ToggleSubscription(bool cond)
        {
            if (cond)
            {
                m_inputField.onValueChanged.AddListener(OnInputValueChanged);
            }
            else
            {
                if (m_inputField != null)
                {
                    m_inputField.onValueChanged.RemoveListener(OnInputValueChanged);
                }
            }
        }
        private void OnInputValueChanged(string newValue)
        {
            if (m_prevValue == newValue) { return; }
            if (newValue.Length >= m_prevValue.Length)
            {
                AkSoundEngine.PostEvent(m_typingSoundEventID.value, gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent(m_deleteSoundEventID.value, gameObject);
            }
            m_prevValue = newValue;
        }
    }
}