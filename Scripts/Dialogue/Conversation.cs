using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using System.Linq;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    [CreateAssetMenu(fileName = "new Conversation", menuName = "ScriptableObjects/Dialogue/Conversation")]
    public sealed class Conversation : ScriptableObject
    {
        public Color defaultTextColor => m_defaultTextColor;
        public float textSize => m_textSize;
        public float characterDelay => m_characterDelay;
        public ConvoDefaults defaults => new ConvoDefaults(m_defaultTextColor, m_textSize, m_characterDelay);
        public IReadOnlyList<ConvoActionObject> conversationActions => m_conversationActions;

        [SerializeField] private Color m_defaultTextColor = Color.white;
        [SerializeField, Min(0.0f)] private float m_textSize = 36.0f;
        [SerializeField, Min(0.0f)] private float m_characterDelay = 0.02f;

        [SerializeField, Expandable] private ConvoActionObject[] m_conversationActions = new ConvoActionObject[1];


        public void SetDefaultTextColor(Color newDefaultTextColor) => m_defaultTextColor = newDefaultTextColor;
        public void SetTextSize(float newTextSize) => m_textSize = newTextSize;
        public void SetCharacterDelay(float newCharacterDelay) => m_characterDelay = newCharacterDelay;

        public void AddActionToList(ConvoActionObject action)
        {
            m_conversationActions = conversationActions.Append(action).ToArray();
        }
    }
}