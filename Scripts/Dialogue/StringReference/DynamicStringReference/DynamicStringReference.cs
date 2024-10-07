using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// ID to reference a dynamic string that is to be set during runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "new StringReference (Dynamic)", menuName = "ScriptableObjects/Dialogue/ConvoActions/DialogueText/StringReference (Dynamic)")]
    public sealed class DynamicStringReference : DynamicStringReferenceReadOnly
    {
        // See DynamicStringReferenceReadOnly and DynamicStringReferenceExtensions for functionality.
    }
}
