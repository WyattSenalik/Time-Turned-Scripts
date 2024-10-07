using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public struct Triangle
    {
        public Vector2 point1 { get; set; }
        public Vector2 point2 { get; set; }
        public Vector2 point3 { get; set; }
        public Vector2 center => (point1 + point2 + point3) / 3.0f;

        public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
        }
    }
}