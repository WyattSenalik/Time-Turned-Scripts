using System;
using UnityEngine;

using Helpers.Editor;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Abstract TimedRecorderEditorGraphs specifically for SnapshotRecorders.
    /// </summary>
    /// <typeparam name="TRecorder">Which snapshot recorder this is for.</typeparam>
    /// <typeparam name="TSnap">Kind of Snapshots used by the recorder.</typeparam>
    /// <typeparam name="TSnapData">Data held in the snapshots.</typeparam>
    public abstract class SnapshotRecorderEditorGraphs<TRecorder, TSnap, TSnapData> : ScrapbookEditorGraphs<SnapshotRecorder<TSnap, TSnapData>>
        where TRecorder : SnapshotRecorder<TSnap, TSnapData>
        where TSnap : ISnapshot<TSnapData, TSnap>
        where TSnapData : IEquatable<TSnapData>
    {
        protected abstract int amountGraphs { get; }

        public override bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently selected recorder, stop.
            if (!TryGetSelectedRecorder(out TRecorder t_curRecorder, out errorMsg)) { return false; }

            for (int i = 0; i < amountGraphs; ++i)
            {
                DrawGraph(i, t_curRecorder.scrapbook, t_curRecorder.spawnTime, t_curRecorder.furthestTime, t_curRecorder.curTime, drawRect, dataPointSize, timeDotSize);
            }

            return true;
        }

        protected override void CompareMinMaxToData(float curTime, ref float curMin, ref float curMax)
        {
            if (!TryGetSelectedRecorder(out TRecorder t_curRecorder, out _))
            {
                //CustomDebug.LogError($"Failed to get recorder when updating min max");
                return;
            }
            TSnapData t_snapData = t_curRecorder.scrapbook.GetSnapshot(curTime).data;
            CompareMinMaxToData(t_snapData, ref curMin, ref curMax);
        }
        protected override float GetVerticalAxisData(int graphIndex, object data)
        {
            TSnapData t_snapData = (TSnapData)data;
            return GetVerticalAxisData(graphIndex, t_snapData);
        }

        protected abstract void CompareMinMaxToData(TSnapData data, ref float curMin, ref float curMax);
        protected abstract float GetVerticalAxisData(int graphIndex, TSnapData data);


        protected bool TryGetSelectedRecorder(out TRecorder retrievedRecorder, out string errorMsg)
        {
            // Try to get the component off the current object.
            if (!EditorSelection.TryGetEditorSelectedComponent(out retrievedRecorder, out errorMsg))
            {
                return false;
            }
            try
            {
                // No snaps on the timed transform.
                if (retrievedRecorder.scrapbook.Count <= 0)
                {
                    errorMsg = $"Scrapbook for {typeof(TRecorder).Name} ({retrievedRecorder}) has no entries";
                    return false;
                }
            }
            catch (Exception t_exception)
            {
                errorMsg = $"{t_exception}";
                return false;
            } 

            errorMsg = "";
            return true;
        }
    }
}