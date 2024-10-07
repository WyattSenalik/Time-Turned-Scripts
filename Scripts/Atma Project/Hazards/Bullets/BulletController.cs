using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using NaughtyAttributes;

using Helpers.Events;
using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Handles the firing of the bullets and trimming them if the player goes back in time.
    /// </summary>
    public sealed class BulletController : MonoBehaviour
    {
        public bool overrideStaticColliders => m_overrideStaticColliders;
        public List<BulletCollider> staticCollidersOverride => m_staticCollidersOverride;
        public IEventPrimer onFireBullet => m_onFireBullet;
        public IReadOnlyList<BulletCollider> ignoreColliders => m_ignoreColliders;
        public float latestBulletFireTime
        {
            get
            {
                float t_latestTime = float.NegativeInfinity;
                foreach (LinearBullet t_bullet in m_firedBullets)
                {
                    float t_bulletLatestTime = t_bullet.GetLatestFireTime();
                    if (t_bulletLatestTime > t_latestTime)
                    {
                        t_latestTime = t_bulletLatestTime;
                    }
                }
                return t_latestTime;
            }
        }

        //private GlobalTimeManager timeManager
        //{
        //    get
        //    {
        //        TryInitializeTimeManager();
        //        return m_timeManager;
        //    }
        //}
        //private float curTime => timeManager.curTime;

        [SerializeField, Required] private GameObject m_bulletPrefab = null;
        [SerializeField, Required] private Transform m_defaultFirePos = null;
        [SerializeField, FormerlySerializedAs("m_bulletVelocity")]
        private Vector2 m_defaultBulletVelocity = Vector2.right;

        [SerializeField] private List<BulletCollider> m_ignoreColliders = new List<BulletCollider>();

        [SerializeField] private MixedEvent m_onFireBullet = new MixedEvent();

        [SerializeField, ReadOnly] private List<GameObject> m_debugBullets = new List<GameObject>();

        [SerializeField] private int m_bulletsToPrespawn = 1;

        [SerializeField] private bool m_overrideStaticColliders = false;
        [SerializeField, ShowIf(nameof(m_overrideStaticColliders))] private List<BulletCollider> m_staticCollidersOverride = new List<BulletCollider>();

        [SerializeField] private bool m_isDebugging = false;


        //private GlobalTimeManager m_timeManager = null;
        private Transform m_bulletParent = null;

        private readonly List<LinearBullet> m_firedBullets = new List<LinearBullet>();


        private void Awake()
        {
            m_bulletParent = new GameObject($"Bullet Parent ({name})").transform;
        }
        private void Start()
        {
            for (int i = 0; i < m_bulletsToPrespawn; ++i)
            {
                SpawnNewBullet();
            }
        }
        private void OnDestroy()
        {
            if (m_bulletParent != null)
            {
                Destroy(m_bulletParent);
            }
        }


        public void FireBullet() => FireBullet(m_defaultFirePos.position, m_defaultBulletVelocity);
        public void FireBullet(Vector2 pos, Vector2 vel)
        {
            LinearBullet t_bullet = GetAvailableBullet();
            t_bullet.AddFireTime(pos, vel);

            m_onFireBullet.Invoke();
        }

        public void AddIgnoreCollider(BulletCollider colliderToIgnore)
        {
            m_ignoreColliders.Add(colliderToIgnore);
        }
        public void ClearIgnoreColliders()
        {
            m_ignoreColliders.Clear();
        }


        private LinearBullet GetAvailableBullet()
        {
            foreach (LinearBullet t_existingBullet in m_firedBullets)
            {
                if (t_existingBullet.IsAvailable())
                {
                    return t_existingBullet;
                }
            }
            return SpawnNewBullet();
        }
        private LinearBullet SpawnNewBullet()
        {
            GameObject t_bulletInst = Instantiate(m_bulletPrefab, Vector3.zero, Quaternion.identity, m_bulletParent);
            LinearBullet t_bullet = t_bulletInst.GetComponentSafe<LinearBullet>();
            t_bullet.owner = this;
            t_bullet.isDebugging = m_isDebugging;
            // Assumes that this bullet is being spawned after all other bullets.
            m_firedBullets.Add(t_bullet);
            m_debugBullets.Add(t_bulletInst);
            return t_bullet;
        }
    }
}
