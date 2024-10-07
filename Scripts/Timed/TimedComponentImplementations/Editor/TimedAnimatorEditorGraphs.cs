using UnityEngine;

namespace Timed.TimedComponentImplementations.Editor
{
    public sealed class TimedAnimatorEditorGraphs : SnapshotRecorderEditorGraphs<TimedAnimator, AnimatorSnapshot, AnimatorSnapshotData>
    {
        protected override int amountGraphs => 3;

        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedRecorder(out TimedAnimator t_animator, out _)) { return; }
            AnimatorSnapshot t_snap = t_animator.scrapbook.GetSnapshot(t_animator.curTime);

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_animator.curTime}");
            // StateHash
            GUILayout.Label($"StateHash {t_snap.data.stateHash}");
            // NormTime
            GUILayout.Label($"NormTime {t_snap.data.normTime}");
            // Params
            string t_paramValuesStr = "";
            if (t_snap.data.paramCount > 0)
            {
                t_paramValuesStr = $"{t_snap.data.paramValues[0]}";
                for (int i = 1; i < t_snap.data.paramCount; ++i)
                {
                    t_paramValuesStr += $", {t_snap.data.paramValues[i]}";
                }
            }
            GUILayout.Label($"Params ({t_snap.data.paramCount}) [{t_paramValuesStr}]");
            //
            GUILayout.EndVertical();
        }
        protected override void CompareMinMaxToData(AnimatorSnapshotData data, ref float curMin, ref float curMax)
        {
            float t_stateHash = ConvertStateHashToMoreReadableValue(data.stateHash);
            int t_paramCount = data.paramCount;
            float t_normTime = data.normTime;
            curMin = Mathf.Min(curMin, t_stateHash, t_paramCount, t_normTime);
            curMax = Mathf.Max(curMax, t_stateHash, t_paramCount, t_normTime);
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.green;
                case 2:
                    return Color.blue;
                default:
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedAnimatorEditorGraphs));
                    return Color.magenta;
            }
        }
        protected override float GetVerticalAxisData(int graphIndex, AnimatorSnapshotData data)
        {
            switch (graphIndex)
            {
                case 0:
                    return ConvertStateHashToMoreReadableValue(data.stateHash);
                case 2:
                    return data.normTime;
                case 1:
                    return data.paramCount;
                default:
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedAnimatorEditorGraphs));
                    return 0;
            }
        }


        private float ConvertStateHashToMoreReadableValue(int stateHash)
        {
            return (stateHash / ((float)int.MaxValue)) * 16.0f;
        }
    }
}