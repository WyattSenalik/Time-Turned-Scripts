using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Dialogue.Sound;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Collection of references that many ConvoActions want to access.
    /// </summary>
    public sealed class ConvoData
    {
        public ConvoDefaults defaults { get; private set; }
        public TypeWriter typeWriter { get; private set; }
        public TypeTextSFX typeTextSFX { get; private set; }
        public TextMeshProUGUI speakerNameTextMesh { get; private set; }
        public Image portraitImg { get; private set; }
        public DialogueBoxDisplay dialougeBoxDisplay { get; private set; }
        public ConversationDriver convoDriver { get; private set; }


        public ConvoData(ConvoDefaults defaults, TypeWriter typeWriter, TypeTextSFX typeTextSFX, TextMeshProUGUI speakerNameTextMesh, Image portraitImg, DialogueBoxDisplay dialougeBoxDisplay, ConversationDriver convoDriver)
        {
            this.defaults = defaults;
            this.typeWriter = typeWriter;
            this.typeTextSFX = typeTextSFX;
            this.speakerNameTextMesh = speakerNameTextMesh;
            this.portraitImg = portraitImg;
            this.dialougeBoxDisplay = dialougeBoxDisplay;
            this.convoDriver = convoDriver;

        }
    }
}