using UnityEditor;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Editor Graphs for a <see cref="TimedRigidbody2D"/>.
    /// </summary>
    public sealed class TimedRigidbody2DEditorGraphs : SnapshotRecorderEditorGraphs<TimedRigidbody2D, Rigidbody2DDataSnapshot, Rigidbody2DData>
    {
        protected override int amountGraphs => 2;


        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedRecorder(out TimedRigidbody2D t_rb2D, out _)) { return; }
            Rigidbody2DDataSnapshot t_snap = t_rb2D.scrapbook.GetSnapshot(t_rb2D.curTime);

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_rb2D.curTime}");
            // Velocity
            GUILayout.Label($"Velocity {t_snap.data.velocity}");
            //
            GUILayout.EndVertical();
        }

        protected override void CompareMinMaxToData(Rigidbody2DData data, ref float curMin, ref float curMax)
        {
            Vector2 t_vel = data.velocity;
            float t_x = t_vel.x;
            float t_y = t_vel.y;
            curMin = Mathf.Min(curMin, t_x, t_y);
            curMax = Mathf.Max(curMax, t_x, t_y);
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                case 0:
                    return Color.green;
                case 1:
                    return Color.red;
                default:
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedRigidbody2DEditorGraphs));
                    return Color.magenta;
            }
        }
        protected override float GetVerticalAxisData(int graphIndex, Rigidbody2DData data)
        {
            switch (graphIndex)
            {
                case 0:
                    return data.velocity.x;
                case 1:
                    return data.velocity.y;
                default:
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedRigidbody2DEditorGraphs));
                    return 0;
            }
        }
    }
}
