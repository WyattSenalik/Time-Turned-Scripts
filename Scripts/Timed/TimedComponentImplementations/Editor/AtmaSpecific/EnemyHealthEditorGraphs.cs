using UnityEngine;

using Helpers.Math;
using Helpers.Editor;

using Atma;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Editor graph for enemy health (when they die).
    /// </summary>
    public sealed class EnemyHealthEditorGraphs : ITimedRecorderEditorGraphs
    {
        private static readonly Color DEATH_DOT_COLOR = Color.blue;
        private static readonly Color TIME_DOT_COLOR = Color.white;


        public bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently enemy health, stop.
            if (!TryGetEnemyHealth(out EnemyHealth t_enemyHealth, out errorMsg)) { return false; }
            if (t_enemyHealth.timedObject == null)
            {
                errorMsg = "timed object is null";
                return false;
            }

            // Draw a line for the dots to rest on.
            float t_spawnTime = t_enemyHealth.spawnTime;
            float t_furthestTime = t_enemyHealth.furthestTime;
            EditorGraphDrawing.DrawLine(new Vector2(t_spawnTime, drawRect.center.y), new Vector2(t_furthestTime, drawRect.center.y), Color.gray);
            // Draw a dot for the death time.
            DrawPointForDeath(t_enemyHealth, t_spawnTime, t_furthestTime, drawRect, DEATH_DOT_COLOR, dataPointSize);

            // Highlight the death point if we are past it.
            if (t_enemyHealth.curTime >= t_enemyHealth.deathTime)
            {
                DrawPointForDeath(t_enemyHealth, t_spawnTime, t_furthestTime, drawRect, TIME_DOT_COLOR, timeDotSize);
            }

            // Draw a vertical line for the current time.
            float t_curTimeX = MathHelpers.ChangeBase(t_enemyHealth.curTime, t_spawnTime, t_furthestTime, drawRect.min.x, drawRect.max.x);
            Vector2 t_topOfLine = new Vector2(t_curTimeX, drawRect.max.y);
            Vector2 t_botOfLine = new Vector2(t_curTimeX, drawRect.min.y);
            EditorGraphDrawing.DrawLine(t_topOfLine, t_botOfLine, TIME_DOT_COLOR);

            return true;
        }
        public void DisplayCurrentData()
        {
            // If fail to get the currently enemy health, stop.
            if (!TryGetEnemyHealth(out EnemyHealth t_enemyHealth, out _)) { return; }
            if (t_enemyHealth.timedObject == null) { return; }

            GUILayout.BeginVertical();
            //
            // Scale
            GUILayout.Label($"Death time {t_enemyHealth.deathTime}");
            //
            GUILayout.EndVertical();
        }


        private bool TryGetEnemyHealth(out EnemyHealth enemyHealth, out string errorMsg)
        {
            // Try to get the object off the selected component.
            if (!EditorSelection.TryGetEditorSelectedComponent(out enemyHealth, out errorMsg))
            {
                return false;
            }

            errorMsg = "";
            return true;
        }
        private void DrawPointForDeath(EnemyHealth enemyHealth, float spawnTime, float furthestTime, Rect drawRect, Color pointColor, float pointSize)
        {
            Vector2 t_pos;
            t_pos.x = MathHelpers.ChangeBase(enemyHealth.deathTime, spawnTime, furthestTime, drawRect.min.x, drawRect.max.x);
            t_pos.y = drawRect.center.y;
            EditorGraphDrawing.DrawPoint(t_pos, pointColor, pointSize);
        }
    }
}
