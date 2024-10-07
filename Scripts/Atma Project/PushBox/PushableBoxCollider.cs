using UnityEngine;

using Helpers.Physics.Custom2DInt;
using Helpers.Extensions;
using NaughtyAttributes;
using Helpers.Events;

namespace Atma
{
    [RequireComponent(typeof(RectangleIntCollider))]
    public sealed class PushableBoxCollider : MonoBehaviour
    {
        public RectangleIntCollider rectangleCollider => m_collider;
        public bool isPusherOnly => m_isPusherOnly;
        public bool isImmobile => m_isImmobile;
        public Int2DTransform rootTransform => m_rootTransform;
        public Vector2 rootPos2D => rootTransform.position;
        public IEventPrimer<CollisionInfo> onCollision => m_onCollision;

        [SerializeField] private bool m_isPusherOnly = false;
        [SerializeField] private bool m_isImmobile = false;

        [SerializeField, ShowIf(nameof(ShouldShowRootTransform))] private Int2DTransform m_rootTransform = null;
        [SerializeField] private MixedEvent<CollisionInfo> m_onCollision = new MixedEvent<CollisionInfo>();

        private BoxPhysicsManager m_boxPhysicsMan = null;
        private RectangleIntCollider m_collider = null;


        private void Awake()
        {
            m_collider = this.GetComponentSafe<RectangleIntCollider>();
        }
        private void OnEnable()
        {
            if (m_boxPhysicsMan == null)
            {
                m_boxPhysicsMan = BoxPhysicsManager.instance;
            }
            m_boxPhysicsMan.Register(this);
        }
        private void OnDisable()
        {
            if (m_boxPhysicsMan != null)
            {
                m_boxPhysicsMan.Unregister(this);
            }
        }


        public void InvokeCollisionEvent(CollisionInfo collisionInfo)
        {
            m_onCollision.Invoke(collisionInfo);
        }


        private bool ShouldShowRootTransform()
        {
            return !m_isImmobile;
        }


        public sealed class CollisionInfo
        {
            public PushableBoxCollider otherCollider { get; private set; }
            public Vector2 directionIMoved { get; private set; }


            public CollisionInfo(PushableBoxCollider otherCollider, Vector2 directionIMoved)
            {
                this.otherCollider = otherCollider;
                this.directionIMoved = directionIMoved;
            }

            public override string ToString()
            {
                string t_otherColStr = "null";
                if (otherCollider != null)
                {
                    t_otherColStr = otherCollider.gameObject.GetFullName();
                }
                return $"OtherCollider:{t_otherColStr}; MoveDir:{directionIMoved}";
            }
        }
    }
}