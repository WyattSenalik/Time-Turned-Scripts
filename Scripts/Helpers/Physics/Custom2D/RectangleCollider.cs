using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public sealed class RectangleCollider : MonoBehaviour
    {
        public Vector2 pos2D => transform.position;
        public Vector2 centerPosWorld => m_centerPos + pos2D;
        public Vector2 size => m_size;
        public Rectangle rectangle => new Rectangle(centerPosWorld, m_size);
        public Circle circleBounds => rectangle.DetermineBoundingCircle();

        [SerializeField] private Vector2 m_centerPos = Vector2.zero;
        [SerializeField] private Vector2 m_size = Vector2.one;

        [SerializeField] private bool m_showCircleBoundsGizmos = false;


        private void OnDrawGizmosSelected()
        {
            if (m_showCircleBoundsGizmos)
            {
                circleBounds.DrawGizmos(Color.green);
            }

            rectangle.DrawInsideGizmos(Color.green);
            rectangle.DrawOutlineGizmos(Color.green);
        }
    }
}