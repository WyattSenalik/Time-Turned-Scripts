using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// A string that can be accessed by sub texts that can easily be changed by edited the reference instead of having to find it in every sub text.
    /// Meant to be used for names and important nouns.
    /// </summary>
    [CreateAssetMenu(fileName = "new StringReference (Constant)", menuName = "ScriptableObjects/Dialogue/ConvoActions/DialogueText/StringReference (Constant)")]
    public sealed class ConstStringReference : StringReference
    {
        public override string refString => m_refString;

        [SerializeField, TextArea] private string m_refString = "???";
    }
}