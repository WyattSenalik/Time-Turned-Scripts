// Original Authors - Wyatt Senalik
using UnityEngine;

namespace Atma
{
    /// <summary>
    /// Default implementation of <see cref="IDamageContext"/>.
    /// </summary>
    public sealed class DamageContext : IDamageContext
    {
        public float damageTime => m_damageTime;
        public GameObject damageSourceObj => m_damageSourceObj;

        private readonly float m_damageTime = 0.0f;
        private readonly GameObject m_damageSourceObj = null;


        public DamageContext(float damageTime, GameObject damageSourceObj)
        {
            m_damageTime = damageTime;
            m_damageSourceObj = damageSourceObj;
        }
    }
}