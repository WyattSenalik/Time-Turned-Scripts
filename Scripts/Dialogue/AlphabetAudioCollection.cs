using System.Collections.Generic;
using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Dialogue.Sound
{
    [CreateAssetMenu(fileName = "new AlphabetAudioCollection", menuName = "ScriptableObjects/Dialogue/Sound/AlphabetAudio")]
    public sealed class AlphabetAudioCollection : ScriptableObject
    {
        public IReadOnlyList<UIntReference> soundEvents => m_soundEvents;

        [SerializeField] private UIntReference[] m_soundEvents = new UIntReference[0];


        public UIntReference GetSoundEventForCharacter(char c)
        {
            int t_index = c;
            t_index %= m_soundEvents.Length;
            if (t_index < 0)
            {
                t_index += m_soundEvents.Length;
            }
            return m_soundEvents[t_index];
        }
        public uint GetSoundEventValueForCharacter(char c)
        {
            return GetSoundEventForCharacter(c).value;
        }
    }
}