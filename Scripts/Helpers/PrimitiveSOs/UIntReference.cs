using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    [CreateAssetMenu(fileName = "new UIntRef", menuName = "ScriptableObjects/Primitives/UnsignedInteger")]
    public sealed class UIntReference : ScriptableObject
    {
        public uint value => m_value;
        [SerializeField] private uint m_value = 0;
    }
}