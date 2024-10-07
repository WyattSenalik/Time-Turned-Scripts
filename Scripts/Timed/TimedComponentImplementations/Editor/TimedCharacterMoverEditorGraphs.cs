using Atma;
using Helpers.Editor;
using System;
using UnityEngine;

namespace Timed.TimedComponentImplementations.Editor
{
    public sealed class TimedCharacterMoverEditorGraphs : ScrapbookEditorGraphs<CharacterMover>
    {
        public override bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently selected recorder, stop.
            if (!TryGetSelectedRecorder(out CharacterMover t_charMover, out errorMsg)) { return false; }

            // Velocity
            TimedVector2 t_velocity = t_charMover.internalVelocity;
            for (int i = 0; i < 1; ++i)
            {
                DrawGraph(i, t_velocity.scrapbook, t_velocity.startTime, t_velocity.farthestTime, t_velocity.curTime, drawRect, dataPointSize, timeDotSize);
            }
            // Position
            TimedVector2Int t_position = t_charMover.internalPosition;
            for (int i = 2; i < 4; ++i)
            {
                DrawGraph(i, t_position.scrapbook, t_position.startTime, t_position.farthestTime, t_position.curTime, drawRect, dataPointSize, timeDotSize);
            }

            return true;
        }
        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedRecorder(out CharacterMover t_charMover, out _))
            {
                return;
            }

            float t_curTime = t_charMover.internalPosition.curTime;
            Vector2Int t_curPosInt = t_charMover.internalPosition.curData;
            Vector2 t_curVelocity = t_charMover.internalVelocity.curData;

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_curTime}");
            // Position
            GUILayout.Label($"Position {t_curPosInt}");
            // Rotation
            GUILayout.Label($"Velocity {t_curVelocity}");
            //
            GUILayout.EndVertical();
        }


        private bool TryGetSelectedRecorder(out CharacterMover charMover, out string errorMsg)
        {
            // Try to get the component off the current object.
            if (!EditorSelection.TryGetEditorSelectedComponent(out charMover, out errorMsg))
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
                else if (charMover.internalPosition == null || charMover.internalVelocity == null)
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
            if (!TryGetSelectedRecorder(out CharacterMover t_charMover, out _))
            {
                //CustomDebug.LogError($"Failed to get CharMover when updating min max");
                return;
            }

            Vector2 t_velocity = t_charMover.internalVelocity.scrapbook.GetSnapshot(curTime).data;
            Vector2 t_position = t_charMover.internalPosition.scrapbook.GetSnapshot(curTime).data;

            curMin = Mathf.Min(curMin, t_velocity.x, t_velocity.y, t_position.x,  t_position.y);
            curMax = Mathf.Max(curMax, t_velocity.x, t_velocity.y, t_position.x,  t_position.y);
        }
        protected override float GetVerticalAxisData(int graphIndex, object data)
        {
            Vector2 t_velocity = Vector2.zero;
            Vector2Int t_position = Vector2Int.zero;
            if (data is Vector2 t_vel)
            {
                t_velocity = t_vel;
            }
            else if (data is Vector2Int t_pos)
            {
                t_position = t_pos;
            }
            else
            {
                //CustomDebug.LogError($"Could not cast data to velocity or position");
            }

            switch (graphIndex)
            {
                case 0: return t_velocity.x;
                case 1: return t_velocity.y;
                case 2: return t_position.x;
                case 3: return t_position.y;
                default:
                {
                    CustomDebug.UnhandledEnum(graphIndex, GetType().Name);
                    return 0.0f;
                }
            }
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                case 0: return Color.yellow;
                case 1: return Color.magenta;
                case 2: return Color.green;
                case 3: return Color.red;
                default:
                {
                    CustomDebug.UnhandledEnum(graphIndex, GetType().Name);
                    return Color.white;
                }
            }
        }
    }
}