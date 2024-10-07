using System.Collections.Generic;
using UnityEngine;

using Helpers.Math;
using Helpers.Editor;
using Helpers.Extensions;

using Atma;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Editor graph for hold pressure plates to show the hold windows.
    /// </summary>
    public sealed class HoldPPEditorGraphs : ITimedRecorderEditorGraphs
    {
        private static readonly Color[] HOLD_OBJ_COLORS = { Color.blue, Color.green, Color.red, Color.cyan, Color.yellow, Color.magenta };
        private static readonly Color TIME_DOT_COLOR = Color.white;

        public bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the hold pressure plate, stop.
            if (!TryGetHoldPP(out HoldPressurePlate t_holdPP, out errorMsg)) { return false; }
            if (t_holdPP.timedObject == null)
            {
                errorMsg = "timed object is null";
                return false;
            }

            // Get all the objects currently involved in the hold pressure plate's windows.
            List<GameObject> t_holdObjs = new List<GameObject>();
            foreach (HoldPressurePlate.HeldWindow t_window in t_holdPP.heldWindows)
            {
                // If the hold objects does not yet contain the hold object for this held window, put in it coach.
                if (!t_holdObjs.Contains(t_window.holdingObject))
                {
                    t_holdObjs.Add(t_window.holdingObject);
                }
            }

            // Get spawn time and furthest time
            float t_spawnTime = t_holdPP.spawnTime;
            float t_furthestTime = t_holdPP.furthestTime;
            // Draw each held window with each object being on its own horizontal line.
            foreach (HoldPressurePlate.HeldWindow t_window in t_holdPP.heldWindows)
            {
                // Get index to determine where to draw and what color.
                int t_index = t_holdObjs.IndexOf(t_window.holdingObject);
                Color t_color = GetHoldObjColor(t_index);
                // Get vertical position.
                float t_yPos = (drawRect.height / (Mathf.Max(0, t_holdObjs.Count) + 1)) * (t_index + 1) + drawRect.yMin;
                // Get horizontal positions.
                float t_startXPos = MathHelpers.ChangeBase(t_window.startTime, t_spawnTime, t_furthestTime, drawRect.min.x, drawRect.max.x);
                float t_endXPos = MathHelpers.ChangeBase(Mathf.Min(t_window.endTime, t_holdPP.furthestTime), t_spawnTime, t_furthestTime, drawRect.min.x, drawRect.max.x);
                // Points
                Vector2 t_startPoint = new Vector2(t_startXPos, t_yPos);
                Vector2 t_endPoint = new Vector2(t_endXPos, t_yPos);
                // Draw points and line between them.
                EditorGraphDrawing.DrawPoint(t_startPoint, t_color, dataPointSize);
                if (t_endXPos != t_holdPP.furthestTime)
                {
                    EditorGraphDrawing.DrawPoint(t_endPoint, t_color, dataPointSize);
                }
                EditorGraphDrawing.DrawLine(t_startPoint, t_endPoint, t_color);
            }

            // Draw a vertical line for the current time.
            float t_curTimeX = MathHelpers.ChangeBase(t_holdPP.curTime, t_spawnTime, t_furthestTime, drawRect.min.x, drawRect.max.x);
            Vector2 t_topOfLine = new Vector2(t_curTimeX, drawRect.max.y);
            Vector2 t_botOfLine = new Vector2(t_curTimeX, drawRect.min.y);
            EditorGraphDrawing.DrawLine(t_topOfLine, t_botOfLine, TIME_DOT_COLOR);

            return true;
        }

        public void DisplayCurrentData()
        {
            // If fail to get the hold pressure plate, stop.
            if (!TryGetHoldPP(out HoldPressurePlate t_holdPP, out _)) { return; }

            GUILayout.BeginVertical();
            foreach (HoldPressurePlate.HeldWindow t_window in t_holdPP.heldWindows)
            {
                GUILayout.Label($"Start ({t_window.startTime}); " +
                    $"End({ t_window.endTime}); " +
                    $"HoldObj ({t_window.holdingObject.name})");
            }
            GUILayout.EndVertical();
        }


        private bool TryGetHoldPP(out HoldPressurePlate holdPP, out string errorMsg)
        {
            // Try to get the selected component off the object.
            if (!EditorSelection.TryGetEditorSelectedComponent(out holdPP, out errorMsg))
            {
                return false;
            }

            errorMsg = "";
            return true;
        }
        private Color GetHoldObjColor(int index)
        {
            int t_wrappedIndex = HOLD_OBJ_COLORS.WrapIndex(index);
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(t_wrappedIndex, HOLD_OBJ_COLORS, this);
            #endregion Asserts
            return HOLD_OBJ_COLORS[t_wrappedIndex];
        }
    }
}