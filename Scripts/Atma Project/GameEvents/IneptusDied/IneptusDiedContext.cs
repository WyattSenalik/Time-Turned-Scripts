using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Parameter to pass in the IneptusDied event.
    /// </summary>
    public sealed class IneptusDiedContext : IIneptusDiedContext
    {
        public float deathTime => m_damageContext.damageTime;
        public GameObject killerObj => m_damageContext.damageSourceObj;
        public IDamageContext damageContext => m_damageContext;
        public Vector2 positionAtDeath => m_positionAtDeath;
        public GameObject ineptusObj => m_ineptusObj;

        private readonly IDamageContext m_damageContext = null;
        private readonly Vector2 m_positionAtDeath = Vector2.negativeInfinity;
        private readonly GameObject m_ineptusObj = null;


        public IneptusDiedContext(IDamageContext damageContext, Vector2 positionAtDeath, GameObject ineptusObj)
        {
            m_damageContext = damageContext;
            m_positionAtDeath = positionAtDeath;
            m_ineptusObj = ineptusObj;
        }
    }
}