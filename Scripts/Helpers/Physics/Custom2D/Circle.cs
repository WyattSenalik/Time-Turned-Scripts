using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public struct Circle
    {
        public Vector2 center { get; set; }
        public float radius { get; set; }


        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }


        public override string ToString() => $"center:{center}; radius:{radius}";
    }


    public static class CircleExtensions
    {
        public static void DrawDebug(this Circle circle, Color color, bool isDebugging = true)
        {
            CustomDebug.DrawCircle(circle.center, circle.radius, 32, color, isDebugging);
        }
        public static void DrawGizmos(this Circle circle, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(circle.center, circle.radius);
        }
    }
}