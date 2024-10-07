using Helpers.Physics.Custom2D;
using UnityEngine;

namespace Helpers.Physics.Custom2DInt
{
    public struct LineInt 
    {
        public Vector2Int[] points => new Vector2Int[2] { point1, point2 };
        public Vector2 normal => CustomPhysics2D.PerpendicularCounterClockwise(CustomPhysics2DInt.ConvertIntPositionToFloatPosition(point1 - point2)).normalized;
        public Vector2Int center => (point1 + point2) / 2;
        public Vector2Int point1 { get; set; }
        public Vector2Int point2 { get; set; }
        public CircleInt boundingCircle
        {
            get
            {
                if (!m_isBoundingCalculated)
                {
                    m_isBoundingCalculated = true;

                    Vector2Int t_center = center;
                    int t_sqDistToPoint1 = (t_center - point1).sqrMagnitude;
                    int t_sqDistToPoint2 = (t_center - point2).sqrMagnitude;
                    int t_largerSqDist = t_sqDistToPoint1 > t_sqDistToPoint2 ? t_sqDistToPoint1 : t_sqDistToPoint2;
                    m_boundingCircle = new CircleInt(center, Mathf.RoundToInt(Mathf.Sqrt(t_largerSqDist)));
                }
                return m_boundingCircle;
            }
        }
        private CircleInt m_boundingCircle;
        private bool m_isBoundingCalculated;

        public LineInt(Vector2Int point1, Vector2Int point2)
        {
            this.point1 = point1;
            this.point2 = point2;

            m_boundingCircle = new CircleInt();
            m_isBoundingCalculated = false;
        }

        public override string ToString() => $"point1:{point1}; point2:{point2}";
    }

    public static class LineIntExtensions
    {
        public static Vector2 GetPoint1AsFloatPosition(this LineInt line) => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(line.point1);
        public static Vector2 GetPoint2AsFloatPosition(this LineInt line) => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(line.point2);
        public static Vector2 GetCenterAsFloatPosition(this LineInt line) => CustomPhysics2DInt.ConvertIntPositionToFloatPosition(line.point1 + line.point2) * 0.5f;


        public static Line ToLineFloat(this LineInt line)
        {
            return new Line(line.GetPoint1AsFloatPosition(), line.GetPoint2AsFloatPosition());
        }

        public static void DrawDebug(this LineInt line, Color color, bool isDebugging = true, bool drawNormalArrow = true)
        {
            Line t_floatLine = line.ToLineFloat();
            t_floatLine.DrawDebug(color, isDebugging, drawNormalArrow);
        }

        public static void DrawGizmos(this LineInt line, Color color, bool drawNormalArrow = true)
        {
            Line t_floatLine = line.ToLineFloat();
            t_floatLine.DrawGizmos(color, drawNormalArrow);
        }
    }
}