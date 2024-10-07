using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// ReadOnly version of <see cref="DynamicStringReference"/>. Extensions for this don't allow for setting or removing.
    /// </summary>
    public abstract class DynamicStringReferenceReadOnly : StringReference
    {
        public override string refString => this.GetCurrentDynamicStringValue();

        public string uniqueID => m_uniqueID;
        public string defaultValue => m_defaultValue;

        [SerializeField]
        private string m_uniqueID = "";

#if UNITY_EDITOR
        [Button]
        private void RandomizeID() => m_uniqueID = UnityEditor.GUID.Generate().ToString();
#endif

        [SerializeField] private string m_defaultValue = "Default";
    }
}
