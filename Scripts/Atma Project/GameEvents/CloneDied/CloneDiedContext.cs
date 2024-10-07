// Original Authors - Wyatt Senalik
using UnityEngine;

namespace Atma.Events
{
    /// <summary>
    /// Parameter to pass in the CloneDied event.
    /// </summary>
    public class CloneDiedContext : ICloneDiedContext
    {
        public float deathTime => m_damageContext.damageTime;
        public GameObject killerObj => m_damageContext.damageSourceObj;
        public IDamageContext damageContext => m_damageContext;
        public Vector2 positionAtDeath => m_positionAtDeath;
        public GameObject cloneObj => m_cloneObj;

        private readonly IDamageContext m_damageContext = null;
        private readonly Vector2 m_positionAtDeath = Vector2.negativeInfinity;
        private readonly GameObject m_cloneObj = null;


        public CloneDiedContext(IDamageContext damageContext, Vector2 positionAtDeath, GameObject cloneObj)
        {
            m_damageContext = damageContext;
            m_positionAtDeath = positionAtDeath;
            m_cloneObj = cloneObj;
        }
    }
}