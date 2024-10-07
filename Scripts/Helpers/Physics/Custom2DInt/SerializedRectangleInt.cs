using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Physics.Custom2DInt
{
    [Serializable]
    public struct SerializedRectangleInt
    {
        /// <summary>0 - leftEdge, 1 - topEdge, 2 - rightEdge, 3 - botEdge</summary>
        public LineInt[] edges => rectangle.edges;
        public LineInt topEdge => rectangle.topEdge;
        public LineInt botEdge => rectangle.botEdge;
        public LineInt leftEdge => rectangle.leftEdge;
        public LineInt rightEdge => rectangle.rightEdge;
        /// <summary>0 - botLeftPoint, 1 - topLeftPoint, 2 - topRightPoint, 3 - botRightPoint</summary>
        public Vector2Int[] points => rectangle.points;
        public Vector2Int topRightPoint => rectangle.topRightPoint;
        public Vector2Int botRightPoint => rectangle.botRightPoint;
        public Vector2Int topLeftPoint => rectangle.topLeftPoint;
        public Vector2Int max => rectangle.max;
        public Vector2Int min => rectangle.min;
        public Vector2Int center => rectangle.center;
        public int height => rectangle.height;
        public int width => rectangle.width;
        public Vector2Int botLeftPoint => rectangle.botLeftPoint;
        public Vector2Int size => rectangle.size;
        public RectangleInt rectangle => new RectangleInt(m_botLeftPoint, m_size);

        [SerializeField] private Vector2Int m_botLeftPoint;
        [SerializeField] private Vector2Int m_size;


        public SerializedRectangleInt(Vector2Int botLeftPoint, Vector2Int size)
        {
            m_botLeftPoint = botLeftPoint;
            m_size = size;
        }
        public SerializedRectangleInt(RectangleInt rectangle)
        {
            m_botLeftPoint = rectangle.botLeftPoint;
            m_size = rectangle.size;
        }
    }
}