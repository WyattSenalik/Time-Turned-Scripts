using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public struct Line
    {
        public Vector2[] points => new Vector2[2] { point1, point2 };
        public Vector2 normal => CustomPhysics2D.PerpendicularCounterClockwise(point2 - point1).normalized;
        public Vector2 center => (point1 + point2) * 0.5f;
        public Vector2 point1 { get; set; }
        public Vector2 point2 { get; set; }

        public Line(Vector2 point1, Vector2 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }

        public override string ToString() => $"point1:{point1}; point2:{point2}";
    }

    public static class LineExtensions
    {
        public static Circle DetermineBoundingCircle(this Line line)
        {
            return new Circle(line.center, Vector2.Distance(line.point1, line.point2) * 0.5f);
        }

        public static void DrawDebug(this Line line, Color color, bool isDebugging = true, bool drawNormalArrow = true)
        {
            // Draw the actual line.
            CustomDebug.DrawLine(line.point1, line.point2, color, isDebugging);
            if (drawNormalArrow)
            {
                // Draw Normal
                float t_lineLength = Vector2.Distance(line.point1, line.point2);
                float t_normalLength = 0.5f * t_lineLength;
                t_normalLength = Mathf.Clamp(t_normalLength, 0.0f, 2.0f);
                Color t_normalColor = color;
                t_normalColor.a *= 0.5f;
                Vector2 t_center = line.center;
                Vector2 t_normal = line.normal;
                Vector2 t_normalEndPos = t_center + (t_normal * t_normalLength);
                CustomDebug.DrawLine(t_center, t_normalEndPos, t_normalColor, isDebugging);
                float t_arrowHeadLength = t_normalLength * 0.25f;
                Vector2 t_arrowHead1Dir = (Quaternion.Euler(0.0f, 0.0f, 135.0f) * t_normal) * t_arrowHeadLength;
                Vector2 t_arrowHead2Dir = (Quaternion.Euler(0.0f, 0.0f, -135.0f) * t_normal) * t_arrowHeadLength;
                CustomDebug.DrawLine(t_normalEndPos, t_normalEndPos + t_arrowHead1Dir, t_normalColor, isDebugging);
                CustomDebug.DrawLine(t_normalEndPos, t_normalEndPos + t_arrowHead2Dir, t_normalColor, isDebugging);
            }
        }

        public static void DrawGizmos(this Line line, Color color, bool drawNormalArrow = true)
        {
            Gizmos.color = color;
            // Draw the actual line.
            Gizmos.DrawLine(line.point1, line.point2);
            if (drawNormalArrow)
            {
                // Draw Normal
                float t_lineLength = Vector2.Distance(line.point1, line.point2);
                float t_normalLength = 0.5f * t_lineLength;
                t_normalLength = Mathf.Clamp(t_normalLength, 0.0f, 2.0f);
                Color t_normalColor = color;
                t_normalColor.a *= 0.5f;
                Vector2 t_center = line.center;
                Vector2 t_normal = line.normal;
                Vector2 t_normalEndPos = t_center + (t_normal * t_normalLength);
                Gizmos.DrawLine(t_center, t_normalEndPos);
                float t_arrowHeadLength = t_normalLength * 0.25f;
                Vector2 t_arrowHead1Dir = (Quaternion.Euler(0.0f, 0.0f, 135.0f) * t_normal) * t_arrowHeadLength;
                Vector2 t_arrowHead2Dir = (Quaternion.Euler(0.0f, 0.0f, -135.0f) * t_normal) * t_arrowHeadLength;
                Gizmos.DrawLine(t_normalEndPos, t_normalEndPos + t_arrowHead1Dir);
                Gizmos.DrawLine(t_normalEndPos, t_normalEndPos + t_arrowHead2Dir);
            }
        }
    }
}