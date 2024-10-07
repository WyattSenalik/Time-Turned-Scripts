using Helpers.Singletons;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atma.Tutorial
{
    public class ButtonsAddText : SingletonMonoBehaviour<ButtonsAddText>
    {
        [SerializeField] private TMP_InputField m_inputField = null;

        public void AddCharToText(string button_char)
        {
            if (m_inputField.text.Length < 12)
            {
                m_inputField.text += button_char;
            }

        }

        public void RemoveCharFromText()
        {
            if (m_inputField.text.Length > 0)
            {
                m_inputField.text = m_inputField.text.Remove(m_inputField.text.Length - 1);
            }
        }
    }
}
