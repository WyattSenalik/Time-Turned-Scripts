using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    public sealed class ConvoDefaults
    {
        public Color color { get; private set; }
        public float textSize { get; private set; }
        public float characterDelay { get; private set; }

        
        public ConvoDefaults(Color color, float textSize, float characterDelay)
        {
            this.color = color;
            this.textSize = textSize;
            this.characterDelay = characterDelay;
        }
    }
}