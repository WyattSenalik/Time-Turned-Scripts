using System;
using UnityEngine;

using Helpers.Physics.Custom2D;
using System.Collections.Generic;

namespace Atma
{
    public sealed class BulletCollider : MonoBehaviour
    {
        public bool isStatic => m_isStatic;
        public Vector2 pos2D => transform.position;
        public Circle boundingCircle => new Circle(m_boundingCircleLocal.center + pos2D, m_boundingCircleLocal.radius);
        public IReadOnlyList<Line> edges => new Line[4] { leftEdgeLine, upEdgeLine, rightEdgeLine, downEdgeLine };
        public bool checkContained => m_checkContained;
        public Line leftEdgeLine => new Line(m_leftEdgeLineLocal.point1 + pos2D, m_leftEdgeLineLocal.point2 + pos2D);
        public Line rightEdgeLine => new Line(m_rightEdgeLineLocal.point1 + pos2D, m_rightEdgeLineLocal.point2 + pos2D);
        public Line upEdgeLine => new Line(m_upEdgeLineLocal.point1 + pos2D, m_upEdgeLineLocal.point2 + pos2D);
        public Line downEdgeLine => new Line(m_downEdgeLineLocal.point1 + pos2D, m_downEdgeLineLocal.point2 + pos2D);
        public eBulletTarget targetType => m_targetType;

        [SerializeField] private bool m_isStatic = false; 
        [SerializeField] private bool m_checkContained = false;
        [SerializeField] private BulletColliderEdge m_leftEdge = new BulletColliderEdge(Vector2.left, 1.0f);
        [SerializeField] private BulletColliderEdge m_rightEdge = new BulletColliderEdge(Vector2.right, 1.0f);
        [SerializeField] private BulletColliderEdge m_upEdge = new BulletColliderEdge(Vector2.up, 1.0f);
        [SerializeField] private BulletColliderEdge m_downEdge = new BulletColliderEdge(Vector2.down, 1.0f);

        [SerializeField] private eBulletTarget m_targetType = eBulletTarget.NotBox;

        private BulletPhysicsManager m_physicsMan = null;

        [SerializeField] private bool m_gizmosDrawBoundingCircle = false;

        private Circle m_boundingCircleLocal = new Circle();
        private Line m_leftEdgeLineLocal = new Line();
        private Line m_rightEdgeLineLocal = new Line();
        private Line m_upEdgeLineLocal = new Line();
        private Line m_downEdgeLineLocal = new Line();

        private bool m_hasPrecalculatedContainingRectangle = false;
        private Rectangle m_precalculatedContainingRectangle = new Rectangle();


        private void Awake()
        {
            InitializePhysicsShapes();

            // It's okay to get this in awake since its a dynamic singleton.
            m_physicsMan = BulletPhysicsManager.instance;
        }
        private void OnEnable()
        {
            m_physicsMan.RegisterCollider(this);
        }
        private void OnDisable()
        {
            if (m_physicsMan != null)
            {
                m_physicsMan.UnregisterCollider(this);
            }
        }


        public Rectangle GetContainingRectangle()
        {
            if (m_isStatic)
            {
                if (!m_hasPrecalculatedContainingRectangle)
                {
                    m_precalculatedContainingRectangle = CalculateContainingRectangle();
                    m_hasPrecalculatedContainingRectangle = true;
                }
                return m_precalculatedContainingRectangle;
            }
            else
            {
                return CalculateContainingRectangle();
            }
            
        }
        public void OverrideEdges(BulletColliderEdge leftEdgeOverride, BulletColliderEdge rightEdgeOverride, BulletColliderEdge upEdgeOverride, BulletColliderEdge downEdgeOverride)
        {
            m_leftEdge = leftEdgeOverride;
            m_rightEdge = rightEdgeOverride;
            m_upEdge = upEdgeOverride;
            m_downEdge = downEdgeOverride;

            InitializePhysicsShapes();
        }


        private void InitializePhysicsShapes()
        {
            m_leftEdgeLineLocal = new Line(m_leftEdge.center - new Vector2(0.0f, m_leftEdge.length * 0.5f), m_leftEdge.center + new Vector2(0.0f, m_leftEdge.length * 0.5f));
            m_rightEdgeLineLocal = new Line(m_rightEdge.center + new Vector2(0.0f, m_rightEdge.length * 0.5f), m_rightEdge.center - new Vector2(0.0f, m_rightEdge.length * 0.5f));
            m_upEdgeLineLocal = new Line(m_upEdge.center - new Vector2(m_upEdge.length * 0.5f, 0.0f), m_upEdge.center + new Vector2(m_upEdge.length * 0.5f, 0.0f));
            m_downEdgeLineLocal = new Line(m_downEdge.center + new Vector2(m_downEdge.length * 0.5f, 0.0f), m_downEdge.center - new Vector2(m_downEdge.length * 0.5f, 0.0f));

            List<Vector2> t_points = new List<Vector2>();
            t_points.AddRange(m_leftEdgeLineLocal.points);
            t_points.AddRange(m_rightEdgeLineLocal.points);
            t_points.AddRange(m_upEdgeLineLocal.points);
            t_points.AddRange(m_downEdgeLineLocal.points);

            Vector2 t_min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 t_max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            foreach (Vector2 t_singlePoint in t_points)
            {
                if (t_singlePoint.x < t_min.x)
                {
                    t_min.x = t_singlePoint.x;
                }
                if (t_singlePoint.x > t_max.x)
                {
                    t_max.x = t_singlePoint.x;
                }

                if (t_singlePoint.y < t_min.y)
                {
                    t_min.y = t_singlePoint.y;
                }
                if (t_singlePoint.y > t_max.y)
                {
                    t_max.y = t_singlePoint.y;
                }
            }
            Rectangle t_boundingRect = new Rectangle(t_min.x, t_max.x, t_min.y, t_max.y);
            m_boundingCircleLocal = t_boundingRect.DetermineBoundingCircle();
        }
        private Rectangle CalculateContainingRectangle()
        {
            float t_tLMinX = Mathf.Max(Mathf.Min(upEdgeLine.point1.x, upEdgeLine.point2.x), leftEdgeLine.center.x);
            float t_tLMaxX = Mathf.Max(Mathf.Max(upEdgeLine.point1.x, upEdgeLine.point2.x), leftEdgeLine.center.x);
            float t_tLMinY = Mathf.Min(Mathf.Min(leftEdgeLine.point1.y, leftEdgeLine.point2.y), upEdgeLine.center.y);
            float t_tLMaxY = Mathf.Min(Mathf.Max(leftEdgeLine.point1.y, leftEdgeLine.point2.y), upEdgeLine.center.y);
            Rectangle t_topLeftRectangle = new Rectangle(t_tLMinX, t_tLMaxX, t_tLMinY, t_tLMaxY);

            float t_bRMinX = Mathf.Min(Mathf.Min(downEdgeLine.point1.x, downEdgeLine.point2.x), rightEdgeLine.center.x);
            float t_bRMaxX = Mathf.Min(Mathf.Max(downEdgeLine.point1.x, downEdgeLine.point2.x), rightEdgeLine.center.x);
            float t_bRMinY = Mathf.Max(Mathf.Min(rightEdgeLine.point1.y, rightEdgeLine.point2.y), downEdgeLine.center.y);
            float t_bRMaxY = Mathf.Max(Mathf.Max(rightEdgeLine.point1.y, rightEdgeLine.point2.y), downEdgeLine.center.y);
            Rectangle t_botRightRectangle = new Rectangle(t_bRMinX, t_bRMaxX, t_bRMinY, t_bRMaxY);

            if (CustomPhysics2D.RectangleRectangleOverlap(t_topLeftRectangle, t_botRightRectangle))
            {
                float t_xMax = Mathf.Min(t_topLeftRectangle.max.x, t_botRightRectangle.max.x);
                float t_xMin = Mathf.Max(t_topLeftRectangle.min.x, t_botRightRectangle.min.x);
                float t_yMax = Mathf.Min(t_topLeftRectangle.max.y, t_botRightRectangle.max.y);
                float t_yMin = Mathf.Max(t_topLeftRectangle.min.y, t_botRightRectangle.min.y);
                return new Rectangle(t_xMin, t_xMax, t_yMin, t_yMax);
            }
            else
            {
                return new Rectangle(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }


        private void OnDrawGizmosSelected()
        {
            if (enabled)
            {
                InitializePhysicsShapes();

                leftEdgeLine.DrawGizmos(Color.green);
                rightEdgeLine.DrawGizmos(Color.green);
                upEdgeLine.DrawGizmos(Color.green);
                downEdgeLine.DrawGizmos(Color.green);

                if (m_checkContained)
                {
                    Rectangle t_interesectRect = CalculateContainingRectangle();
                    t_interesectRect.DrawInsideGizmos(new Color(0.0f, 1.0f, 0.0f, 0.5f));
                }
                if (m_gizmosDrawBoundingCircle)
                {
                    boundingCircle.DrawGizmos(Color.white);
                }
            }
        }


        [Serializable]
        public sealed class BulletColliderEdge
        {
            public Vector2 center => m_center;
            public float length => m_length;

            [SerializeField] private Vector2 m_center = Vector2.left;
            [SerializeField] private float m_length = 1.0f;


            public BulletColliderEdge(Vector2 center, float length)
            {
                m_center = center;
                m_length = length;
            }
        }

        public enum eBulletTarget { NotBox, Box }
    }
}