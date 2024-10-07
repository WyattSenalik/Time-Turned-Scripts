using Helpers.Physics.Custom2D;
using UnityEngine;

namespace Helpers.Physics.Custom2DInt
{
    public struct CircleInt
    {
        public Vector2Int center { get; private set; }
        public int radius { get; private set; }
        public int x => center.x;
        public int y => center.y;


        public CircleInt(Vector2Int center, int radius)
        {
            this.center = center;
            this.radius = radius;
        }
        public CircleInt(int x, int y, int radius)
        {
            this.center = new Vector2Int(x, y);
            this.radius = radius;
        }


        public override string ToString() => $"center:{center}; radius:{radius}";
    }


    public static class CircleIntExtensions
    {
        public static Vector2 GetCircleCenterAsFloatPosition(this CircleInt circle)
        {
            return new Vector2(circle.x * CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT, circle.y * CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT);
        }
        public static float GetCircleRadiusAsFloat(this CircleInt circle)
        {
            return circle.radius * CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT;
        }
        public static Circle ToCircleFloat(this CircleInt circle)
        {
            return new Circle(circle.GetCircleCenterAsFloatPosition(), circle.GetCircleRadiusAsFloat());
        }

        public static void DrawDebug(this CircleInt circle, Color color, bool isDebugging = true) => DrawDebug(circle, Vector2.zero, color, isDebugging);
        public static void DrawDebug(this CircleInt circle, Vector2 positionOffset, Color color, bool isDebugging = true)
        {
            Vector2 t_center = positionOffset + GetCircleCenterAsFloatPosition(circle);
            float t_radius = GetCircleRadiusAsFloat(circle);
            CustomDebug.DrawCircle(t_center, t_radius, 32, color, isDebugging);
        }
        public static void DrawGizmos(this CircleInt circle, Color color) => DrawGizmos(circle, Vector2.zero, color);
        public static void DrawGizmos(this CircleInt circle, Vector2 positionOffset, Color color)
        {
            Gizmos.color = color;
            Vector2 t_center = GetCircleCenterAsFloatPosition(circle) + positionOffset;
            float t_radius = GetCircleRadiusAsFloat(circle);
            Gizmos.DrawWireSphere(t_center, t_radius);
        }
    }
}