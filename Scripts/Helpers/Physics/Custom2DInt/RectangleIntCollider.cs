using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Helpers.Physics.Custom2DInt
{
    [RequireComponent(typeof(Int2DTransform))]
    public sealed class RectangleIntCollider : MonoBehaviour
    {
        public Int2DTransform intTransform
        {
            get
            {
#if UNITY_EDITOR
                // If accessing from editor, this happens
                if (!Application.isPlaying)
                {
                    return GetComponent<Int2DTransform>();
                }
#endif 
                return m_intTransform;
            }
        }
        public Vector2Int bottomLeftPos => CustomPhysics2DInt.ConvertFloatPositionToIntPosition(intTransform.transform.TransformPoint(CustomPhysics2DInt.ConvertIntPositionToFloatPosition(m_bottomLeftPos)));
        public Vector2Int size => intTransform.lossySize * m_size;
        public RectangleInt rectangle
        {
            get
            {
                if (m_isStatic)
                {
                    if (!m_hasCalculatedStaticRectangle)
                    {
                        m_staticRectangle = new RectangleInt(bottomLeftPos, size);
                        m_hasCalculatedStaticRectangle = true;
                    }
                    return m_staticRectangle;
                }
                else
                {
                    return new RectangleInt(bottomLeftPos, size);
                }
            }
        }
        public CircleInt circleBounds => rectangle.boundingCircle;

        [SerializeField] private bool m_isStatic = false;
        [SerializeField] private Vector2Int m_bottomLeftPos = Vector2Int.zero;
        [SerializeField] private Vector2Int m_size = Vector2Int.one;

        [SerializeField] private bool m_showCircleBoundsGizmos = false;

        private Int2DTransform m_intTransform = null;

        private bool m_hasCalculatedStaticRectangle = false;
        private RectangleInt m_staticRectangle = new RectangleInt();


        private void Awake()
        {
            m_intTransform = this.GetComponentSafe<Int2DTransform>();
        }


        private void OnDrawGizmosSelected()
        {
            if (m_showCircleBoundsGizmos)
            {
                circleBounds.DrawGizmos(Color.green);
            }

            RectangleInt t_previewRect = new RectangleInt(bottomLeftPos, size);
            t_previewRect.DrawInsideGizmos(Color.green);
            t_previewRect.DrawOutlineGizmos(Color.green);
        }
    }
}