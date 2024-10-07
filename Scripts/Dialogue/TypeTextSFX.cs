using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Dialogue.Sound
{
    /// <summary>
    /// For playing a sound when the type writer types a single character.
    /// </summary>
  
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TypeWriter))]
    public sealed class TypeTextSFX : MonoBehaviour
    {
        [SerializeField] private char[] m_ignoreCharacters = new char[] { ' ', '.', ',', '?', '!', '"', '\'' };
        [SerializeField] private AlphabetAudioCollection m_defaultWwiseTypeSoundEventID = null;
        [SerializeField, Range(0.0f, 1.0f)] private float m_minimumWaitTimeBeforeNextPlay = 0.1f;

        private TypeWriter m_typeWriter = null;
        private AlphabetAudioCollection m_wwiseTypeSoundEventCollection = null;
        private float m_lastPostTime = float.NegativeInfinity;


        private void Awake()
        {
            ResetWwiseTypeSoundEventID();

            m_typeWriter = this.GetComponentSafe<TypeWriter>();
        }
        private void Start()
        {
            m_typeWriter.onSingleCharacterTyped.ToggleSubscription(OnSingleCharacterTyped, true);
        }
        private void OnDestroy()
        {
            if (m_typeWriter != null)
            {
                m_typeWriter.onSingleCharacterTyped.ToggleSubscription(OnSingleCharacterTyped, false);
            }
        }


        public void SetWwiseTypeSoundEventID(AlphabetAudioCollection newTypeSoundEventCollection)
        {
            m_wwiseTypeSoundEventCollection = newTypeSoundEventCollection;
        }
        public void ResetWwiseTypeSoundEventID()
        {
            m_wwiseTypeSoundEventCollection = m_defaultWwiseTypeSoundEventID;
        }


        private void OnSingleCharacterTyped(char typedChar)
        {
            if (IsIgnoreCharacter(typedChar)) { return; }

            if (m_wwiseTypeSoundEventCollection == null)
            {
                //CustomDebug.LogWarningForComponent($"No sound specified for typing sound", this);
            }
            else
            {
                float t_curTime = Time.time;
                if (t_curTime >= m_lastPostTime + m_minimumWaitTimeBeforeNextPlay)
                {
                    uint t_soundEventID = m_wwiseTypeSoundEventCollection.GetSoundEventValueForCharacter(typedChar);
                    AkSoundEngine.PostEvent(t_soundEventID, gameObject);
                    m_lastPostTime = t_curTime;
                }
            }
        }
        private bool IsIgnoreCharacter(char charToCheck)
        {
            foreach (char c in m_ignoreCharacters)
            {
                if (c == charToCheck)
                {
                    return true;
                }
            }
            return false;
        }
    }
}