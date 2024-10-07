using System.Collections.Generic;
using UnityEngine;

namespace Timed.TimedComponentImplementations.Editor
{
    public sealed class TimedIntTransformEditorGraphs : ScrapbookEditorGraphs<TimedIntTransform>
    {
        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedComponent(out TimedIntTransform t_intTrans, out _)) { return; }
            float t_curTime = t_intTrans.timedPosX.curTime;
            IntSnapshot t_snapPosX = t_intTrans.timedPosX.scrapbook.GetSnapshot(t_curTime);
            IntSnapshot t_snapPosY = t_intTrans.timedPosY.scrapbook.GetSnapshot(t_curTime);

            int t_angle = 0;
            if (t_intTrans.shouldRecordRotation)
            {
                IntSnapshot t_snapAngle = t_intTrans.timedAngle.scrapbook.GetSnapshot(t_curTime);
                t_angle = t_snapAngle.data;
            }

            Vector2Int t_size = Vector2Int.zero;
            if (t_intTrans.shouldRecordSize)
            {
                IntSnapshot t_snapSizeX = t_intTrans.timedSizeX.scrapbook.GetSnapshot(t_curTime);
                IntSnapshot t_snapSizeY = t_intTrans.timedSizeY.scrapbook.GetSnapshot(t_curTime);
                t_size.x = t_snapSizeX.data;
                t_size.y = t_snapSizeY.data;
            }

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_curTime}");
            // Position
            GUILayout.Label($"Position ({t_snapPosX.data}, {t_snapPosY.data})");
            // Rotation
            GUILayout.Label($"Rotation {t_angle}");
            // Scale
            GUILayout.Label($"Scale ({t_size.x}, {t_size.y})");
            //
            GUILayout.EndVertical();
        }


        public override bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently selected recorder, stop.
            if (!TryGetSelectedComponent(out TimedIntTransform t_intTrans, out errorMsg)) { return false; }
            float t_startTime = t_intTrans.timedPosX.startTime;
            float t_farTime = t_intTrans.timedPosX.farthestTime;
            float t_curTime = t_intTrans.timedPosX.curTime;

            IReadOnlySnapshotScrapbook<IntSnapshot, int> t_scrapbookPosX = t_intTrans.timedPosX.scrapbook;
            IReadOnlySnapshotScrapbook<IntSnapshot, int> t_scrapbookPosY = t_intTrans.timedPosY.scrapbook;
            DrawGraph(0, t_scrapbookPosX, t_startTime, t_farTime, t_curTime, drawRect, dataPointSize, timeDotSize);
            DrawGraph(1, t_scrapbookPosY, t_startTime, t_farTime, t_curTime, drawRect, dataPointSize, timeDotSize);

            if (t_intTrans.shouldRecordRotation)
            {
                IReadOnlySnapshotScrapbook<IntSnapshot, int> t_scrapbookAngle = t_intTrans.timedAngle.scrapbook;
                DrawGraph(2, t_scrapbookAngle, t_startTime, t_farTime, t_curTime, drawRect, dataPointSize, timeDotSize);
            }
            if (t_intTrans.shouldRecordSize)
            {
                IReadOnlySnapshotScrapbook<IntSnapshot, int> t_scrapbookSizeX = t_intTrans.timedSizeX.scrapbook;
                IReadOnlySnapshotScrapbook<IntSnapshot, int> t_scrapbookSizeY = t_intTrans.timedSizeY.scrapbook;
                DrawGraph(3, t_scrapbookSizeX, t_startTime, t_farTime, t_curTime, drawRect, dataPointSize, timeDotSize);
                DrawGraph(4, t_scrapbookSizeY, t_startTime, t_farTime, t_curTime, drawRect, dataPointSize, timeDotSize);
            }

            return true;
        }
        protected override void CompareMinMaxToData(float curTime, ref float curMin, ref float curMax)
        {
            if (!TryGetSelectedComponent(out TimedIntTransform t_intTrans, out string t_errorMsg))
            {
                //CustomDebug.LogError(t_errorMsg);
                return;
            }

            IntSnapshot t_snapPosX = t_intTrans.timedPosX.scrapbook.GetSnapshot(curTime);
            IntSnapshot t_snapPosY = t_intTrans.timedPosY.scrapbook.GetSnapshot(curTime);

            List<float> t_potentialMins = new List<float>() { curMin, t_snapPosX.data, t_snapPosY.data };
            List<float> t_potentialMaxs = new List<float>() { curMax, t_snapPosX.data, t_snapPosY.data };
            if (t_intTrans.shouldRecordRotation)
            {
                IntSnapshot t_snapAngle = t_intTrans.timedAngle.scrapbook.GetSnapshot(curTime);

                t_potentialMins.Add(t_snapAngle.data);

                t_potentialMaxs.Add(t_snapAngle.data);
            }

            if (t_intTrans.shouldRecordSize)
            {
                IntSnapshot t_snapSizeX = t_intTrans.timedSizeX.scrapbook.GetSnapshot(curTime);
                IntSnapshot t_snapSizeY = t_intTrans.timedSizeY.scrapbook.GetSnapshot(curTime);

                t_potentialMins.Add(t_snapSizeX.data);
                t_potentialMins.Add(t_snapSizeY.data);

                t_potentialMaxs.Add(t_snapSizeX.data);
                t_potentialMaxs.Add(t_snapSizeY.data);
            }

            curMin = Mathf.Min(t_potentialMins.ToArray());
            curMax = Mathf.Max(t_potentialMaxs.ToArray());
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                case 0: return Color.green;
                case 1: return Color.red;
                case 2: return Color.yellow;
                case 3: return Color.blue;
                case 4: return Color.magenta;
                default:
                {
                    CustomDebug.UnhandledEnum(graphIndex, nameof(TimedIntTransformEditorGraphs));
                    return Color.black;
                }
            }
        }
        protected override float GetVerticalAxisData(int graphIndex, object data)
        {
            if (data is not int t_castedData)
            {
                //CustomDebug.LogError($"Failed to cast {data} to an int");
                return float.NaN;
            }
            if (!TryGetSelectedComponent(out TimedIntTransform t_intTrans, out string t_errorMsg))
            {
                //CustomDebug.LogError(t_errorMsg);
                return float.NaN;
            }

            return t_castedData;
        }
    }
}