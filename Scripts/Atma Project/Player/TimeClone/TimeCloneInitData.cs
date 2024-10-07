using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Data for initializing <see cref="TimeClone"/>.
    /// </summary>
    public sealed class TimeCloneInitData
    {
        /// <summary>
        /// The active player gameobject that the clone is replicating.
        /// </summary>
        public GameObject originalPlayerObj => m_originalPlayerObj;
        public CloneManager cloneManager => m_cloneManager;
        public float spawnTime => m_spawnTime;
        public float farthestTime => m_farthestTime;
        public int occupyingCharge => m_occupyingCharge;
        public float blinkTime => m_blinkTime;

        private readonly GameObject m_originalPlayerObj = null;
        private readonly CloneManager m_cloneManager = null;
        private readonly float m_spawnTime = -1.0f;
        private readonly float m_farthestTime = -1.0f;
        private readonly int m_occupyingCharge = -1;
        private readonly float m_blinkTime = -1.0f;


        public TimeCloneInitData(GameObject playerObject, CloneManager cloneMan, float cloneSpawnTime, float cloneDieTime, int occupyingChargeIndex, float blinkTime)
        {
            m_originalPlayerObj = playerObject;
            m_cloneManager = cloneMan;
            m_spawnTime = cloneSpawnTime;
            m_farthestTime = cloneDieTime;
            m_occupyingCharge = occupyingChargeIndex;
            m_blinkTime = blinkTime;
        }
    }
}