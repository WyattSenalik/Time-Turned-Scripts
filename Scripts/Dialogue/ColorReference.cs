using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// A color that can be accessed by sub texts that can easily be changed by editing the reference instead of having to find it in every sub text.
    /// Usually used alongside string references.
    /// </summary>
    [CreateAssetMenu(fileName = "new ColorReference", menuName = "ScriptableObjects/Dialogue/ConvoActions/DialogueText/ColorReference")]
    public sealed class ColorReference : ScriptableObject
    {
        public Color color => m_color;
        [SerializeField] private Color m_color = Color.white;
    }
}