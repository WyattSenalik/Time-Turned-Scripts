using UnityEngine;

using NaughtyAttributes;

using Dialogue.Sound;

namespace Dialogue.ConvoActions.Text
{
    [CreateAssetMenu(fileName = "new CharacterInfo", menuName = "ScriptableObjects/Dialogue/ConvoActions/DialogueText/CharacterInfo")]
    public sealed class CharacterInfo : ScriptableObject
    {
        public Sprite characterPortrait => m_characterPortrait;
        public string characterName => m_characterName.refString;
        public AlphabetAudioCollection audioCollection => m_audioCollection;

        [SerializeField] private Sprite m_characterPortrait = null;
        [SerializeField, Required] private ConstStringReference m_characterName = null;
        [SerializeField] private AlphabetAudioCollection m_audioCollection = null; 
    }
}