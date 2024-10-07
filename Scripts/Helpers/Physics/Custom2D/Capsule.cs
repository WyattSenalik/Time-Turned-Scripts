using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public struct Capsule
    {
        public Vector2 point1 { get; set; }
        public Vector2 point2 { get; set; }
        public float radius { get; set; }
        public Vector2 center => (point1 + point2) * 0.5f;
        public Line innerLine => new Line(point1, point2);
        public Line outerLine1 => new Line(innerLine.point1 + innerLine.normal * radius, innerLine.point2 + innerLine.normal * radius);
        public Line outerLine2 => new Line(innerLine.point1 - innerLine.normal * radius, innerLine.point2 - innerLine.normal * radius);

        public Capsule(Vector2 point1, Vector2 point2, float radius)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.radius = radius;
        }

        public override string ToString() => $"point1:{point1}; point2:{point2}; radius:{radius}";
    }

    public static class CapsuleExtensions
    {
        public static void DrawDebug(this Capsule capsule, Color color, bool isDebugging = true)
        {
            // Draw the inner line.
            CustomDebug.DrawLine(capsule.point1, capsule.point2, color, isDebugging);
            // Draw the end circles.
            CustomDebug.DrawCircle(capsule.point1, capsule.radius, 32, color, isDebugging);
            CustomDebug.DrawCircle(capsule.point2, capsule.radius, 32, color, isDebugging);
            // Draw the circle connector lines.
            Vector2 t_dir = (capsule.point2 - capsule.point1).normalized;
            Vector2 t_offset1 = CustomPhysics2D.PerpendicularClockwise(t_dir) * capsule.radius;
            Vector2 t_offset2 = CustomPhysics2D.PerpendicularCounterClockwise(t_dir) * capsule.radius;
            CustomDebug.DrawLine(capsule.point1 + t_offset1, capsule.point2 + t_offset1, color, isDebugging);
            CustomDebug.DrawLine(capsule.point1 + t_offset2, capsule.point2 + t_offset2, color, isDebugging);
        }
    }
}