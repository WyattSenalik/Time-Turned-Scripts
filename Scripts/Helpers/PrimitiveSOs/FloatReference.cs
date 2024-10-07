using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    [CreateAssetMenu(fileName = "new FloatRef", menuName = "ScriptableObjects/Primitives/Float")]
    public sealed class FloatReference : ScriptableObject
    {
        public float value => m_value;
        [SerializeField] private float m_value = 0;
    }
}