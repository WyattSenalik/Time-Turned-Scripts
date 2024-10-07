using System;
using System.Collections.Generic;
using UnityEngine;

using Helpers.Editor;
using Helpers.Math;

namespace Timed.TimedComponentImplementations.Editor
{
    public abstract class ScrapbookEditorGraphs<TComponent> : ITimedRecorderEditorGraphs
        where TComponent : MonoBehaviour
    {

        private static readonly Color TIME_DOT_COLOR = Color.white;

        public float recentMaxValue { get; private set; } = float.MaxValue;
        public float recentMinValue { get; private set; } = float.MinValue;
        public float minMaxDiff => recentMaxValue - recentMinValue;


        public abstract bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg);
        public abstract void DisplayCurrentData();

        protected abstract void CompareMinMaxToData(float curTime, ref float curMin, ref float curMax);
        protected abstract float GetVerticalAxisData(int graphIndex, object data);
        protected abstract Color GetGraphColor(int graphIndex);


        protected bool TryGetSelectedComponent(out TComponent retrievedComponent, out string errorMsg)
        {
            // Try to get the component off the current object.
            if (!EditorSelection.TryGetEditorSelectedComponent(out retrievedComponent, out errorMsg))
            {
                return false;
            }

            errorMsg = "";
            return true;
        }


        protected void DrawGraph<TSnap, TSnapData>(int graphIndex, TimedVariable<TSnap, TSnapData> timedVar, Rect drawRect, float dataPointSize, float timeDotSize)
            where TSnap : ISnapshot<TSnapData, TSnap>
            where TSnapData : IEquatable<TSnapData>
        {
            DrawGraph(graphIndex, timedVar.scrapbook, timedVar.startTime, timedVar.farthestTime, timedVar.curTime, drawRect, dataPointSize, timeDotSize);
        }
        protected void DrawGraph<TSnap, TSnapData>(int graphIndex, IReadOnlySnapshotScrapbook<TSnap, TSnapData> scrapbook, float spawnTime, float farthestTime, float curTime, Rect drawRect, float dataPointSize, float timeDotSize)
            where TSnap : ISnapshot<TSnapData, TSnap>
            where TSnapData : IEquatable<TSnapData>
        {
            // Colors
            Color t_graphColor = GetGraphColor(graphIndex);

            // Get mins and maxes for the snapshots
            float t_minCoord = float.MaxValue;
            float t_maxCoord = float.MinValue;
            ICollection<TSnap> t_rawScrapbook = scrapbook.GetInternalData();
            foreach (TSnap t_singleSnap in t_rawScrapbook)
            {
                // Compare the data to the min and max coordinates respectively, to determine the true min and max values for the y axis.
                CompareMinMaxToData(t_singleSnap.time, ref t_minCoord, ref t_maxCoord);
            }
            recentMaxValue = t_maxCoord;
            recentMinValue = t_minCoord;
            bool t_isFirst = true;
            eInterpolationOption t_prevInterpolationOpt = eInterpolationOption.Linear;
            Vector2 t_prevPoint = Vector2.negativeInfinity;
            Vector2 t_curPoint;
            // Actually draw the points.
            foreach (TSnap t_singleSnap in t_rawScrapbook)
            {
                t_curPoint = GetSnapGraphPoint(t_singleSnap);
                Color t_curPointColor = t_singleSnap.interpolationOpt == eInterpolationOption.Step ? Color.blue : t_graphColor;

                if (!t_isFirst)
                {
                    switch (t_prevInterpolationOpt)
                    {
                        case eInterpolationOption.Linear:
                        {
                            EditorGraphDrawing.DrawLine(t_prevPoint, t_curPoint, t_graphColor);
                            break;
                        }
                        case eInterpolationOption.Step:
                        {
                            Vector2 t_steppedPoint = new Vector2(t_curPoint.x, t_prevPoint.y);
                            EditorGraphDrawing.DrawLine(t_prevPoint, t_steppedPoint, t_graphColor);
                            EditorGraphDrawing.DrawPoint(t_steppedPoint, t_graphColor * 0.5f, dataPointSize);
                            EditorGraphDrawing.DrawLine(t_steppedPoint, t_curPoint, t_graphColor * 0.5f);
                            break;
                        }
                        default:
                        {
                            CustomDebug.UnhandledEnum(t_prevInterpolationOpt, nameof(DrawGraph));
                            break;
                        }
                    }
                }
                EditorGraphDrawing.DrawPoint(t_curPoint, t_curPointColor, dataPointSize);

                t_prevPoint = t_curPoint;
                t_prevInterpolationOpt = t_singleSnap.interpolationOpt;
                t_isFirst = false;
            }
            // Extrapolate to farthest time.
            TSnap t_extraSnap = scrapbook.GetSnapshot(farthestTime);
            t_curPoint = GetSnapGraphPoint(t_extraSnap);
            // Draw lines to connect last snap to extrapolated last.
            EditorGraphDrawing.DrawLine(t_prevPoint, t_curPoint, t_graphColor);

            // Draw a dot on both graphs for the current time.
            TSnap t_curSnap = scrapbook.GetSnapshot(curTime);
            t_curPoint = GetSnapGraphPoint(t_curSnap);
            // Draw Points to represent where we are in time on the graphs.
            EditorGraphDrawing.DrawPoint(t_curPoint, TIME_DOT_COLOR, timeDotSize);

            Vector2 GetSnapGraphPoint(TSnap snap)
            {
                float t_timeHori = MathHelpers.ChangeBase(snap.time, spawnTime, farthestTime, drawRect.xMin, drawRect.xMax);
                float t_rawData = GetVerticalAxisData(graphIndex, snap.data);
                float t_dataVert = MathHelpers.ChangeBase(t_rawData, t_minCoord, t_maxCoord, drawRect.yMax, drawRect.yMin);
                return new Vector2(t_timeHori, t_dataVert);
            }
        }
    }
}