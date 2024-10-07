
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class CircleHighlightSounds : MonoBehaviour
    {
        [SerializeField] private UIntReference m_highlightInSoundID = null;
        [SerializeField] private UIntReference m_highlightOutSoundID = null;


        public void PlayHighlightInSound() => PostEventWithID(m_highlightInSoundID);
        public void PlayHighlightOutSound() => PostEventWithID(m_highlightOutSoundID);


        private void PostEventWithID(UIntReference soundIDRef)
        {
            if (soundIDRef != null)
            {
                AkSoundEngine.PostEvent(soundIDRef.value, gameObject);
            }
        }
    }
}