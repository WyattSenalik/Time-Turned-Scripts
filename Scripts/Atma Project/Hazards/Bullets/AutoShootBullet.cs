using UnityEngine;

using Timed;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Automatically fires a continuous stream of bullets.
    /// </summary>
    [RequireComponent(typeof(BulletController))]
    public sealed class AutoShootBullet : TimedObserver
    {
        public float initialFireDelay => m_initialFireDelay;
        public float fireCooldown => m_fireCooldown;
        public BulletController bulletController { get; private set; }

        [SerializeField, Required] private EnemyHealth m_enemyHealth = null;

        [SerializeField, Min(0.0f)] private float m_initialFireDelay = 0.1f;
        [SerializeField, Min(0.0f)] private float m_fireCooldown = 1.0f;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();
            bulletController = GetComponent<BulletController>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_enemyHealth, nameof(m_enemyHealth), this);

            //CustomDebug.AssertComponentIsNotNull(bulletController, this);
            #endregion Asserts
        }


        public override void TimedUpdate(float deltaTime)
        {
            if (curTime >= m_enemyHealth.deathTime) { return; }

            // Initial delay
            if (m_initialFireDelay >= curTime)
            {
                return;
            }

            UpdateFire();
        }


        private void UpdateFire()
        {
            float t_prevFireTime = bulletController.latestBulletFireTime;
            if (t_prevFireTime + m_fireCooldown <= curTime)
            {
                bulletController.FireBullet();
            }
        }
    }
}
