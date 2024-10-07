using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Atma;
using Helpers.Editor;
using System;
using Helpers.Physics.Custom2DInt;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    public sealed class TimeCloneTransformEditorGraphs : ScrapbookEditorGraphs<TimeCloneTransform>
    {
        public override bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently selected recorder, stop.
            if (!TryGetSelectedRecorder(out TimeCloneTransform t_cloneTrans, out errorMsg)) { return false; }

            // Position
            TimedInt t_posX = t_cloneTrans.internalPosX;
            TimedInt t_posY = t_cloneTrans.internalPosY;
            DrawGraph(0, t_posX);
            DrawGraph(1, t_posY);
            // Angle
            TimedInt t_angle = t_cloneTrans.internalAngle;
            if (t_angle != null)
            {
                DrawGraph(2, t_angle);
            }
            // Scale
            TimedInt t_scaleX = t_cloneTrans.internalSizeX;
            TimedInt t_scaleY = t_cloneTrans.internalSizeY;
            if (t_scaleX != null)
            {
                DrawGraph(3, t_scaleX);
            }
            if (t_scaleY != null)
            {
                DrawGraph(4, t_scaleY);
            }
            // Replay position
            TimedInt t_replayPosX = t_cloneTrans.internalReplayPosX;
            TimedInt t_replayPosY = t_cloneTrans.internalReplayPosY;
            DrawGraph(5, t_replayPosX);
            DrawGraph(6, t_replayPosY);

            return true;

            void DrawGraph(int index, TimedInt timedInt)
            {
                this.DrawGraph(index, timedInt.scrapbook, timedInt.startTime, timedInt.farthestTime, timedInt.curTime, drawRect, dataPointSize, timeDotSize);
            }
        }
        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedRecorder(out TimeCloneTransform t_cloneTrans, out _)) { return; }

            float t_curTime = t_cloneTrans.internalPosX.curTime;
            TimedInt t_posX = t_cloneTrans.internalPosX;
            TimedInt t_posY = t_cloneTrans.internalPosY;
            Vector2Int t_curPos = new Vector2Int(t_posX.curData, t_posY.curData);
            TimedInt t_angle = t_cloneTrans.internalAngle;
            TimedInt t_scaleX = t_cloneTrans.internalSizeX;
            TimedInt t_scaleY = t_cloneTrans.internalSizeY;
            TimedInt t_replayPosX = t_cloneTrans.internalReplayPosX;
            TimedInt t_replayPosY = t_cloneTrans.internalReplayPosY;
            Vector2Int t_curReplayPos = new Vector2Int(t_replayPosX.curData, t_replayPosY.curData);

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_curTime}");
            // Position
            GUILayout.Label($"Position int:({t_curPos}) float:({CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_curPos)})");
            // Angle
            if (t_angle != null)
            {
                GUILayout.Label($"Angle {t_angle.curData}");
            }
            // Scale
            if (t_scaleX != null && t_scaleY != null)
            {
                Vector2Int t_curScale = new Vector2Int(t_scaleX.curData, t_scaleY.curData);
                GUILayout.Label($"Scale int:({t_curScale}) float:({CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_curScale)})");
            }
            // Replay Position
            GUILayout.Label($"Replay Position int:({t_curReplayPos}) float:({CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_curReplayPos)})");
            //
            GUILayout.EndVertical();
        }


        private bool TryGetSelectedRecorder(out TimeCloneTransform cloneTrans, out string errorMsg)
        {
            // Try to get the component off the current object.
            if (!EditorSelection.TryGetEditorSelectedComponent(out cloneTrans, out errorMsg))
            {
                return false;
            }
            try
            {
                // Not runtime
                if (!Application.isPlaying)
                {
                    errorMsg = $"Game is not in playmode";
                    return false;
                }
                // Not yet initialized
                else if (cloneTrans.internalPosX == null || cloneTrans.internalPosY == null || cloneTrans.internalReplayPosX == null || cloneTrans.internalReplayPosY == null)
                {
                    errorMsg = $"Not yet initialized";
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

        protected override void CompareMinMaxToData(float curTime, ref float curMin, ref float curMax)
        {
            if (!TryGetSelectedRecorder(out TimeCloneTransform t_cloneTrans, out _))
            {
                //CustomDebug.LogError($"Failed to get TimeCloneTransform when updating min max");
                return;
            }

            TimedInt t_posX = t_cloneTrans.internalPosX;
            TimedInt t_posY = t_cloneTrans.internalPosY;
            TimedInt t_angle = t_cloneTrans.internalAngle;
            TimedInt t_scaleX = t_cloneTrans.internalSizeX;
            TimedInt t_scaleY = t_cloneTrans.internalSizeY;
            TimedInt t_replayPosX = t_cloneTrans.internalReplayPosX;
            TimedInt t_replayPosY = t_cloneTrans.internalReplayPosY;

            curMin = Mathf.Min(curMin, t_posX.curData, t_posY.curData, t_replayPosX.curData, t_replayPosY.curData);
            curMax = Mathf.Max(curMax, t_posX.curData, t_posY.curData, t_replayPosX.curData, t_replayPosY.curData);

            if (t_angle != null)
            {
                curMin = Mathf.Min(curMin, t_angle.curData);
                curMax = Mathf.Max(curMax, t_angle.curData);
            }
            if (t_scaleX != null)
            {
                curMin = Mathf.Min(curMin, t_scaleX.curData);
                curMax = Mathf.Max(curMax, t_scaleX.curData);
            }
            if (t_scaleY != null)
            {
                curMin = Mathf.Min(curMin, t_scaleY.curData);
                curMax = Mathf.Max(curMax, t_scaleY.curData);
            }
        }
        protected override float GetVerticalAxisData(int graphIndex, object data)
        {
            return (int)data;
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                case 0: return Color.green;
                case 1: return Color.red;
                case 2: return new Color(1.0f, 0.64705882352f, 0.0f);
                case 3: return Color.cyan;
                case 4: return Color.blue;
                case 5: return Color.yellow;
                case 6: return Color.magenta;
                default:
                {
                    CustomDebug.UnhandledEnum(graphIndex, GetType().Name);
                    return Color.white;
                }
            }
        }
    }
}