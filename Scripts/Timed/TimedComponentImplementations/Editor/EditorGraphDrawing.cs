using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// For drawing points and lines for a custom inspector.
    /// </summary>
    public static class EditorGraphDrawing
    {
        private const float TWO_PI = 2 * Mathf.PI;
        private const int POINT_LOD = 12;
        private const float POINT_INC = TWO_PI / POINT_LOD;


        public static void DrawLine(Vector2 pos0, Vector2 pos1, Color col)
        {
            GL.Begin(GL.LINES);
            GL.Color(col);
            GL.Vertex(pos0);
            GL.Vertex(pos1);
            GL.End();
        }
        public static void DrawPoint(Vector2 pos, Color col, float radius)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(col);
            for (float t_rad = 0; t_rad < TWO_PI; t_rad += POINT_INC)
            {
                float t_x = radius * Mathf.Cos(t_rad) + pos.x;
                float t_y = radius * Mathf.Sin(t_rad) + pos.y;

                float t_nextRad = t_rad + POINT_INC;
                float t_nX = radius * Mathf.Cos(t_nextRad) + pos.x;
                float t_nY = radius * Mathf.Sin(t_nextRad) + pos.y;

                GL.Vertex(pos);
                GL.Vertex3(t_x, t_y, 0);
                GL.Vertex3(t_nX, t_nY, 0);
            }
            GL.End();
        }
    }
}