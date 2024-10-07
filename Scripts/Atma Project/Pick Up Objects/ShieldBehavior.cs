using System;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Physics;
using Helpers.UnityEnums;
using Timed;

using static Atma.BulletCollider;
// Original Authors - Bryce Cernohous-Schrader
// Wasn't being used anymore, so modified heavily by Wyatt,

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ShieldBehavior : PickupBehavior
    {
        [SerializeField, Required] private BulletCollider m_shieldCol1 = null;
        [SerializeField, Required] private SpriteRenderer m_sprRend = null;
        [SerializeField, Required] private Sprite m_onGroundSpr = null;
        [SerializeField, Required] private Sprite m_frontFacingSpr = null;
        [SerializeField, Required] private Sprite m_frontRightFacingSpr = null;
        [SerializeField, Required] private Sprite m_rightFacingSpr = null;
        [SerializeField, Required] private Sprite m_backRightFacingSpr = null;
        [SerializeField, Required] private Sprite m_backFacingSpr = null;
        [SerializeField, Required] private Sprite m_backLeftFacingSpr = null;
        [SerializeField, Required] private Sprite m_leftFacingSpr = null;
        [SerializeField, Required] private Sprite m_frontLeftFacingSpr = null;

        [SerializeField] private int m_pickupOrderInLayer = 0;
        [SerializeField] private int m_behindOrderInLayer = -1;
        [SerializeField] private int m_frontOrderInLayer = 1;

        [SerializeField]
        private BulletColliderSpecs m_frontFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_frontRightFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_rightFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_backRightFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_backFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_backLeftFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_leftFacingColSpecs1 = new BulletColliderSpecs();
        [SerializeField]
        private BulletColliderSpecs m_frontLeftFacingColSpecs1 = new BulletColliderSpecs();

        private GlobalTimeManager m_timeMan = null;


        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
            #endregion Asserts
        }
        private void Update()
        {
            // Only do stuff when not rewinding.
            if (!m_timeMan.shouldRecord) { return; }

            if (!isHeld)
            {
                m_shieldCol1.enabled = false;
                m_sprRend.sprite = m_onGroundSpr;
                m_sprRend.sortingOrder = m_pickupOrderInLayer;
            }
            else
            {
                eEightDirection t_holdDir = GetHoldingDirection();
                Sprite t_directionalSpr = GetSpriteFromHoldDir(t_holdDir);
                int t_orderInLayer = GetOrderInLayerFromHoldDir(t_holdDir);
                BulletColliderSpecs t_colSpecs = GetColldierSpecsFromHoldDir(t_holdDir);
                // Update the sprite.
                m_sprRend.sprite = t_directionalSpr;
                m_sprRend.sortingOrder = t_orderInLayer;
                // Turn on collider and set it's size to be accurate.
                m_shieldCol1.enabled = true;
                t_colSpecs.ApplyToCollider(m_shieldCol1);
            }
        }


        private eEightDirection GetHoldingDirection()
        {
            // For why comparisons are made with these specific values, consult a unit circle.
            Vector2 t_rawDir = pickUpObject.aimDirection;
            return t_rawDir.ToClosestEightDirection();
        }
        private Sprite GetSpriteFromHoldDir(eEightDirection dir)
        {
            switch (dir)
            {
                case eEightDirection.Down: return m_frontFacingSpr;
                case eEightDirection.RightDown: return m_frontRightFacingSpr;
                case eEightDirection.Right: return m_rightFacingSpr;
                case eEightDirection.RightUp: return m_backRightFacingSpr;
                case eEightDirection.Up: return m_backFacingSpr;
                case eEightDirection.LeftUp: return m_backLeftFacingSpr;
                case eEightDirection.Left: return m_leftFacingSpr;
                case eEightDirection.LeftDown: return m_frontLeftFacingSpr;
                default:
                {
                    CustomDebug.UnhandledEnum(dir, this);
                    return null;
                }
            }
        }
        private int GetOrderInLayerFromHoldDir(eEightDirection dir)
        {
            switch (dir)
            {
                case eEightDirection.Down: return m_frontOrderInLayer;
                case eEightDirection.RightDown: return m_frontOrderInLayer;
                case eEightDirection.Right: return m_frontOrderInLayer;
                case eEightDirection.RightUp: return m_behindOrderInLayer;
                case eEightDirection.Up: return m_behindOrderInLayer;
                case eEightDirection.LeftUp: return m_behindOrderInLayer;
                case eEightDirection.Left: return m_frontOrderInLayer;
                case eEightDirection.LeftDown: return m_frontOrderInLayer;
                default:
                {
                    CustomDebug.UnhandledEnum(dir, this);
                    return m_frontOrderInLayer;
                }
            }
        }
        private BulletColliderSpecs GetColldierSpecsFromHoldDir(eEightDirection dir)
        {
            switch (dir)
            {
                case eEightDirection.Down: return m_frontFacingColSpecs1;
                case eEightDirection.RightDown: return m_frontRightFacingColSpecs1;
                case eEightDirection.Right: return m_rightFacingColSpecs1;
                case eEightDirection.RightUp: return m_backRightFacingColSpecs1;
                case eEightDirection.Up: return m_backFacingColSpecs1;
                case eEightDirection.LeftUp: return m_backLeftFacingColSpecs1;
                case eEightDirection.Left: return m_leftFacingColSpecs1;
                case eEightDirection.LeftDown: return m_frontLeftFacingColSpecs1;
                default:
                {
                    CustomDebug.UnhandledEnum(dir, this);
                    return null;
                }
            }
        }


        [Serializable]
        public sealed class BulletColliderSpecs
        {
            [SerializeField] private BulletColliderEdge m_leftEdge = new BulletColliderEdge(Vector2.left, 1.0f);
            [SerializeField] private BulletColliderEdge m_rightEdge = new BulletColliderEdge(Vector2.right, 1.0f);
            [SerializeField] private BulletColliderEdge m_upEdge = new BulletColliderEdge(Vector2.up, 1.0f);
            [SerializeField] private BulletColliderEdge m_downEdge = new BulletColliderEdge(Vector2.down, 1.0f);


            public void ApplyToCollider(BulletCollider collider)
            {
                collider.OverrideEdges(m_leftEdge, m_rightEdge, m_upEdge, m_downEdge);
            }
        }
    }
}
