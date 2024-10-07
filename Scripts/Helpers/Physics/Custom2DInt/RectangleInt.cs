using UnityEngine;

using Helpers.Physics.Custom2D;

namespace Helpers.Physics.Custom2DInt
{
    public struct RectangleInt
    {
        /// <summary>0 - leftEdge, 1 - topEdge, 2 - rightEdge, 3 - botEdge</summary>
        public LineInt[] edges => new LineInt[4] { leftEdge, topEdge, rightEdge, botEdge };
        public LineInt topEdge => new LineInt(topLeftPoint, topRightPoint);
        public LineInt botEdge => new LineInt(botRightPoint, botLeftPoint);
        public LineInt leftEdge => new LineInt(botLeftPoint, topLeftPoint);
        public LineInt rightEdge => new LineInt(topRightPoint, botRightPoint);
        /// <summary>0 - botLeftPoint, 1 - topLeftPoint, 2 - topRightPoint, 3 - botRightPoint</summary>
        public Vector2Int[] points => new Vector2Int[4] { botLeftPoint, topLeftPoint, topRightPoint, botRightPoint };
        public Vector2Int topRightPoint => max;
        public Vector2Int botRightPoint => new Vector2Int(topRightPoint.x, botLeftPoint.y);
        public Vector2Int topLeftPoint => new Vector2Int(botLeftPoint.x, topRightPoint.y);
        public Vector2Int max => botLeftPoint + size;
        public Vector2Int min => botLeftPoint;
        public Vector2Int size { get; private set; }
        public Vector2Int halfSize { get; private set; }
        public Vector2Int center => botLeftPoint + halfSize;
        public Vector2Int botLeftPoint { get; private set; }
        public int height => size.y;
        public int width => size.x;
        public CircleInt boundingCircle
        {
            get
            {
                if (!m_isBoundingCalculated)
                {
                    // Okay, so this is wrong, BUT is always slightly bigger than the calculation below and avoids square roots.
                    int t_slightlyTooBigRadius = (size.x + size.y) >> 1;
                    m_boundingCircle = new CircleInt(botLeftPoint + halfSize, t_slightlyTooBigRadius);
                    //int t_radius = Mathf.CeilToInt(Mathf.Sqrt(width * width + height * height) * 0.5f);
                    //m_boundingCircle = new CircleInt(botLeftPoint + halfSize, t_radius);

                    m_isBoundingCalculated = true;
                }
                return m_boundingCircle;
            }
        }

        private CircleInt m_boundingCircle;
        private bool m_isBoundingCalculated;


        public RectangleInt(Vector2Int botLeftPoint, int width, int height)
        {
            this.botLeftPoint = botLeftPoint;
            size = new Vector2Int(width, height);
            halfSize = new Vector2Int(width >> 1, height >> 1);

            m_boundingCircle = new CircleInt();
            m_isBoundingCalculated = false;
        }
        public RectangleInt(Vector2Int botLeftPoint, Vector2Int size)
        {
            this.botLeftPoint = botLeftPoint;
            this.size = size;
            halfSize = new Vector2Int(size.x >> 1, size.y >> 1);

            m_boundingCircle = new CircleInt();
            m_isBoundingCalculated = false;
        }
        public override string ToString()
        {
            return $"botLeftPoint:{botLeftPoint}; size:{size}";
        }
    }

    public static class RectangleIntExtensions
    {
        public static void DrawOutlineDebug(this RectangleInt rectangle, Color color, bool isDebugging = true)
        {
            Rectangle t_floatRect = rectangle.ToFloatRectangle();
            t_floatRect.DrawOutlineDebug(color, isDebugging);
        }
        public static void DrawInsideDebug(this RectangleInt rectangle, Color color, int segments = 7, bool isDebugging = true)
        {
            Rectangle t_floatRect = rectangle.ToFloatRectangle();
            t_floatRect.DrawInsideDebug(color, segments, isDebugging);
        }

        public static void DrawOutlineGizmos(this RectangleInt rectangle, Color color)
        {
            Rectangle t_floatRect = rectangle.ToFloatRectangle();
            t_floatRect.DrawOutlineGizmos(color);
        }
        public static void DrawInsideGizmos(this RectangleInt rectangle, Color color, int segments = 7)
        {
            Rectangle t_floatRect = rectangle.ToFloatRectangle();
            t_floatRect.DrawInsideGizmos(color, segments);
        }

        public static Vector2 GetBotLeftPointAsFloatPosition(this RectangleInt rectangle)
        {
            return CustomPhysics2DInt.ConvertIntPositionToFloatPosition(rectangle.botLeftPoint);
        }
        public static Vector2 GetSizeAsFloatSize(this RectangleInt rectangle)
        {
            return CustomPhysics2DInt.ConvertIntPositionToFloatPosition(rectangle.size);
        }
        public static float GetWidthAsFloatWidth(this RectangleInt rectangle)
        {
            return CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT * rectangle.width;
        }
        public static float GetHeightAsFloatHeight(this RectangleInt rectangle)
        {
            return CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT * rectangle.height;
        }
        public static Vector2 GetCenterAsFloatPosition(this RectangleInt rectangle)
        {
            return CustomPhysics2DInt.ConvertIntPositionToFloatPosition(rectangle.center);
        }

        public static Rectangle ToFloatRectangle(this RectangleInt rectangle)
        {
            Vector2 t_floatSize = rectangle.GetSizeAsFloatSize();
            Vector2 t_floatCenter = rectangle.GetBotLeftPointAsFloatPosition() + t_floatSize * 0.5f;
            return new Rectangle(t_floatCenter, t_floatSize);
        }
    }
}