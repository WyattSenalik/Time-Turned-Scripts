using Atma.Dialogue;
using Dialogue;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Extension;

namespace Atma
{
    public class CheckForName : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;
        [SerializeField] private TMP_InputField m_inputField = null;
        [SerializeField] private InputMapStack m_mapStack = null;
        [SerializeField] private GameObject m_enterPlayerName = null;
        [SerializeField] private UnityEventEndpointProgrammedConvoAction m_convoAction = null;
        [SerializeField, Required] private DynamicStringReference m_dynRefToPlayerName = null;

        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputField, nameof(m_inputField), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_mapStack, nameof(m_mapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_enterPlayerName, nameof(m_enterPlayerName), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_convoAction, nameof(m_convoAction), this);
            #endregion Asserts
        }

        public void CheckForNameAndEnter()
        {
            if (m_inputField.text.Length <= 0)
            {
                // No name given, so ignore.
                #region Logs
                //CustomDebug.LogForComponent($"No name given.", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }

            m_dynRefToPlayerName.SetDynamicStringValue(m_inputField.text);
            m_mapStack.PopInputMap("EnterName");
            m_enterPlayerName.SetActive(false);
            m_convoAction.Finish();

        }
    }
}
