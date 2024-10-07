using System;
using UnityEngine;

namespace Helpers.Physics.Custom2D
{
    public struct Rectangle
    {
        /// <summary>0 - leftEdge, 1 - topEdge, 2 - rightEdge, 3 - botEdge</summary>
        public Line[] edges => new Line[4] { leftEdge, topEdge, rightEdge, botEdge };
        public Line topEdge => new Line(center + 0.5f * new Vector2(-width, height), center + 0.5f * new Vector2(width, height));
        public Line botEdge => new Line(center + 0.5f * new Vector2(width, -height), center + 0.5f * new Vector2(-width, -height));
        public Line leftEdge => new Line(center + 0.5f * new Vector2(-width, -height), center + 0.5f * new Vector2(-width, height));
        public Line rightEdge => new Line(center + 0.5f * new Vector2(width, height), center + 0.5f * new Vector2(width, -height));
        /// <summary>0 - botLeftPoint, 1 - topLeftPoint, 2 - topRightPoint, 3 - botRightPoint</summary>
        public Vector2[] points => new Vector2[4] { botLeftPoint, topLeftPoint, topRightPoint, botRightPoint };
        public Vector2 botLeftPoint => min;
        public Vector2 topRightPoint => max;
        public Vector2 botRightPoint => new Vector2(topRightPoint.x, botLeftPoint.y);
        public Vector2 topLeftPoint => new Vector2(botLeftPoint.x, topRightPoint.y);
        public Vector2 max => center + size * 0.5f;
        public Vector2 min => center - size * 0.5f;
        public Vector2 size  => new Vector2(width, height);
        public Vector2 center { get; set; }
        public float height { get; set; }
        public float width { get; set; }


        public Rectangle(Vector2 center, float width, float height)
        {
            this.center = center;
            this.width = width;
            this.height = height;
        }
        public Rectangle(Vector2 center, Vector2 size)
        {
            this.center = center;
            this.width = size.x;
            this.height = size.y;
        }
        public Rectangle(float xMin, float xMax, float yMin, float yMax)
        {
            this.center = 0.5f * new Vector2(xMin + xMax, yMin + yMax);
            this.width = xMax - xMin;
            this.height = yMax - yMin;
        }


        public override string ToString()
        {
            return $"center:{center}; size:{size}";
        }
    }


    public static class RectangleExtensions
    {
        public static Circle DetermineBoundingCircle(this Rectangle rectangle)
        {
            return new Circle(rectangle.center, Mathf.Sqrt(rectangle.width * rectangle.width + rectangle.height * rectangle.height) * 0.5f);
        }

        public static void DrawOutlineDebug(this Rectangle rectangle, Color color, bool isDebugging = true)
        {
            rectangle.leftEdge.DrawDebug(color, isDebugging, false);
            rectangle.rightEdge.DrawDebug(color, isDebugging, false);
            rectangle.topEdge.DrawDebug(color, isDebugging, false);
            rectangle.botEdge.DrawDebug(color, isDebugging, false);
        }
        public static void DrawInsideDebug(this Rectangle rectangle, Color color, int segments = 7, bool isDebugging = true)
        {
            DrawInsideHelper(rectangle, (Vector2 botPoint, Vector2 topPoint) =>
            {
                CustomDebug.DrawLine(botPoint, topPoint, color, isDebugging);
            }, segments);
        }

        public static void DrawOutlineGizmos(this Rectangle rectangle, Color color)
        {
            rectangle.leftEdge.DrawGizmos(color, false);
            rectangle.rightEdge.DrawGizmos(color, false);
            rectangle.topEdge.DrawGizmos(color, false);
            rectangle.botEdge.DrawGizmos(color, false);
        }
        public static void DrawInsideGizmos(this Rectangle rectangle, Color color, int segments = 7)
        {
            Gizmos.color = color;

            DrawInsideHelper(rectangle, (Vector2 botPoint, Vector2 topPoint) =>
            {
                Gizmos.DrawLine(botPoint, topPoint);
            }, segments);
        }

        private static void DrawInsideHelper(Rectangle rectangle, Action<Vector2, Vector2> drawLineFunc, int segments = 7)
        {
            Vector2 t_diagonalDiff = rectangle.max - rectangle.min;
            Vector2 t_dir = new Vector2(1, -1).normalized;
            for (int i = 1; i < segments + 1; ++i)
            {
                float t = ((float)i) / (segments + 1);
                Vector2 t_curDiff = t_diagonalDiff * t;
                Vector2 t_curMidPoint = rectangle.min + t_curDiff;

                Line t_infSlashLine = new Line(t_curMidPoint, t_curMidPoint + t_dir);
                if (CustomPhysics2D.InfiniteLinesIntersect(t_infSlashLine, new Line(rectangle.min, rectangle.min + Vector2.right), out Vector2 t_botPoint))
                {
                    // Restrict the point to not be past the right
                    if (t_botPoint.x > rectangle.max.x)
                    {
                        Vector2 t_oldBotPoint = t_botPoint;
                        if (!CustomPhysics2D.InfiniteLinesIntersect(new Line(t_oldBotPoint, t_oldBotPoint + t_dir), rectangle.rightEdge, out t_botPoint))
                        {
                            // Failed to find intersect (unlikely, but eh)
                            t_botPoint = t_oldBotPoint;
                        }
                    }

                    if (CustomPhysics2D.InfiniteLinesIntersect(t_infSlashLine, new Line(rectangle.min, rectangle.min + Vector2.up), out Vector2 t_topPoint))
                    {
                        // Restrict the point to not be above the top
                        if (t_topPoint.y > rectangle.max.y)
                        {
                            Vector2 t_oldTopPoint = t_topPoint;
                            if (!CustomPhysics2D.InfiniteLinesIntersect(new Line(t_oldTopPoint, t_oldTopPoint + t_dir), rectangle.topEdge, out t_topPoint))
                            {
                                // Failed to find intersect (unlikely, but eh)
                                t_topPoint = t_oldTopPoint;
                            }
                        }

                        drawLineFunc.Invoke(t_botPoint, t_topPoint);
                    }
                }
            }
        }
    }
}