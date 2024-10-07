using UnityEngine;

using Helpers.Editor;
using Helpers.Math;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Abstract TimedRecorderEditorGraphs specifically for MomentRecorders.
    /// </summary>
    /// <typeparam name="TRecorder">Which moment recorder this is for.</typeparam>
    public abstract class MomentRecorderEditorGraphs<TRecorder, TMomentSelf> : ITimedRecorderEditorGraphs
        where TRecorder : MomentRecorder<TMomentSelf>
        where TMomentSelf : IMoment<TMomentSelf>
    {
        private static readonly Color MOMENT_DOT_COLOR = Color.blue;
        private static readonly Color TIME_DOT_COLOR = Color.white;

        public bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently selected recorder, stop.
            if (!TryGetSelectedRecorder(out TRecorder t_curRecorder, out errorMsg)) { return false; }

            // Draw a line for the dots to rest on.
            float t_spawnTime = t_curRecorder.spawnTime;
            float t_furthestTime = t_curRecorder.furthestTime;
            EditorGraphDrawing.DrawLine(new Vector2(t_spawnTime, drawRect.center.y), new Vector2(t_furthestTime, drawRect.center.y), Color.gray);
            // Draw dots for each of the moments
            foreach (TMomentSelf t_moment in t_curRecorder.album.GetInternalData())
            {
                DrawPointForMoment(t_moment, t_spawnTime, t_furthestTime, drawRect, MOMENT_DOT_COLOR, dataPointSize);
            }

            // Draw a different dot on most recent moment.
            TMomentSelf t_latestMom = t_curRecorder.album.GetMomentBeforeOrAtTime(t_curRecorder.curTime);
            DrawPointForMoment(t_latestMom, t_spawnTime, t_furthestTime, drawRect, TIME_DOT_COLOR, timeDotSize);

            // Draw a vertical line for the current time.
            float t_curTimeX = MathHelpers.ChangeBase(t_curRecorder.curTime, t_spawnTime, t_furthestTime, drawRect.min.x, drawRect.max.x);
            Vector2 t_topOfLine = new Vector2(t_curTimeX, drawRect.max.y);
            Vector2 t_botOfLine = new Vector2(t_curTimeX, drawRect.min.y);
            EditorGraphDrawing.DrawLine(t_topOfLine, t_botOfLine, TIME_DOT_COLOR);

            return true;
        }

        public abstract void DisplayCurrentData();


        protected bool TryGetSelectedRecorder(out TRecorder retrievedRecorder, out string errorMsg)
        {
            // Try to get the object off the selected component.
            if (!EditorSelection.TryGetEditorSelectedComponent(out retrievedRecorder, out errorMsg))
            {
                return false;
            }
            // No moments in the album.
            if (retrievedRecorder.album.Count <= 0)
            {
                errorMsg = $"Album for {typeof(TRecorder).Name} ({retrievedRecorder}) has no entries";
                return false;
            }

            errorMsg = "";
            return true;
        }


        private void DrawPointForMoment(TMomentSelf moment, float spawnTime, float furthestTime, Rect drawRect, Color pointColor, float pointSize)
        {
            Vector2 t_pos;
            t_pos.x = MathHelpers.ChangeBase(moment.time, spawnTime, furthestTime, drawRect.min.x, drawRect.max.x);
            t_pos.y = drawRect.center.y;
            EditorGraphDrawing.DrawPoint(t_pos, pointColor, pointSize);
        }
    }
}