using UnityEngine;

using Helpers.Transforms;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Editor Graphs for a <see cref="TimedTransform"/>.
    /// </summary>
    public sealed class TimedTransformEditorGraphs : SnapshotRecorderEditorGraphs<TimedTransform, TransformDataSnapshot, TransformData>
    {
        protected override int amountGraphs => 3;

        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedRecorder(out TimedTransform t_trans, out _))
            { return; }
            TransformDataSnapshot t_snap = t_trans.scrapbook.GetSnapshot(t_trans.curTime);

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_trans.curTime}");
            // Position
            GUILayout.Label($"Position {t_snap.data.position}");
            // Rotation
            GUILayout.Label($"Rotation {t_snap.data.angle}");
            // Scale
            GUILayout.Label($"Scale {t_snap.data.scale}");
            //
            GUILayout.EndVertical();
        }

        protected override void CompareMinMaxToData(TransformData data, ref float curMin, ref float curMax)
        {
            Vector2 t_pos = data.position;
            float t_x = t_pos.x;
            float t_y = t_pos.y;
            float t_angle = data.angle * Mathf.Deg2Rad;
            curMin = Mathf.Min(curMin, t_x, t_y, t_angle);
            curMax = Mathf.Max(curMax, t_x, t_y, t_angle);
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                case 0:
                    return Color.green;
                case 1:
                    return Color.red;
                case 2:
                    return Color.yellow;
                default:
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedTransformEditorGraphs));
                    return Color.magenta;
            } 
        }
        protected override float GetVerticalAxisData(int graphIndex, TransformData data)
        {
            switch (graphIndex)
            {
                case 0:
                    return data.position.x;
                case 1:
                    return data.position.y;
                case 2:
                    return data.angle * Mathf.Deg2Rad;
                default:
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedTransformEditorGraphs));
                    return 0;
            }
        }
    }
}