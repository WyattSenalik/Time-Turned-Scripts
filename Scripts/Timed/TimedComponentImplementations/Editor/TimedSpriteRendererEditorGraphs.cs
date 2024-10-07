using System;
using UnityEngine;

using Atma;
using Helpers.Editor;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Editor Graphs for a <see cref="TimedSpriteRenderer"/>.
    /// </summary>
    public sealed class TimedSpriteRendererEditorGraphs : ScrapbookEditorGraphs<TimedSpriteRenderer>
    {
        public override bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg)
        {
            // If fail to get the currently selected recorder, stop.
            if (!TryGetSelectedRecorder(out TimedSpriteRenderer t_sprRend, out errorMsg)) { return false; }

            // Enabled
            TimedBool t_enabled = t_sprRend.timedRendEnabled;
            DrawGraph(0, t_enabled, drawRect, dataPointSize, timeDotSize);
            // Sprite
            TimedSprite t_sprite = t_sprRend.timedSpr;
            DrawGraph(1, t_sprite, drawRect, dataPointSize, timeDotSize);
            // FlipX
            TimedBool t_flipX = t_sprRend.timedFlipX;
            DrawGraph(2, t_flipX, drawRect, dataPointSize, timeDotSize);
            // Sorting Order
            TimedInt t_sortingOrder = t_sprRend.timedSortingOrder;
            DrawGraph(3, t_sortingOrder, drawRect, dataPointSize, timeDotSize);

            return true;
        }
        public override void DisplayCurrentData()
        {
            if (!TryGetSelectedRecorder(out TimedSpriteRenderer t_sprRend, out _)) { return; }

            float t_curTime = t_sprRend.timedRendEnabled.curTime;
            TimedBool t_enabled = t_sprRend.timedRendEnabled;
            TimedSprite t_sprite = t_sprRend.timedSpr;
            TimedBool t_flipX = t_sprRend.timedFlipX;
            TimedInt t_sortingOrder = t_sprRend.timedSortingOrder;

            GUILayout.BeginVertical();
            //
            // Time
            GUILayout.Label($"Time {t_curTime}");
            // Enabled
            GUILayout.Label($"Enabled {t_enabled.curData}");
            // Sprite
            GUILayout.Label($"Sprite {t_sprite.curData.sprite.name}");
            // FlipX
            GUILayout.Label($"FlipX {t_flipX.curData}");
            // Sorting Order
            GUILayout.Label($"Sorting Order {t_sortingOrder.curData}");
            //
            GUILayout.EndVertical();
        }


        private bool TryGetSelectedRecorder(out TimedSpriteRenderer timedSprRend, out string errorMsg)
        {
            // Try to get the component off the current object.
            if (!EditorSelection.TryGetEditorSelectedComponent(out timedSprRend, out errorMsg))
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
                else if (timedSprRend.timedRendEnabled == null || timedSprRend.timedSpr == null || timedSprRend.timedFlipX == null || timedSprRend.timedSortingOrder == null)
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
            if (!TryGetSelectedRecorder(out TimedSpriteRenderer t_sprRend, out _))
            {
                //CustomDebug.LogError($"Failed to get TimeCloneTransform when updating min max");
                return;
            }

            TimedSprite t_sprite = t_sprRend.timedSpr;
            TimedInt t_sortingOrder = t_sprRend.timedSortingOrder;

            float t_spriteGraphVal = t_sprite.curData.sprite.GetHashCode();
            float t_sortingOrderGraphVal = t_sortingOrder.curData;

            curMin = Mathf.Min(curMin, t_spriteGraphVal, t_sortingOrderGraphVal);
            curMax = Mathf.Max(curMax, t_spriteGraphVal, t_sortingOrderGraphVal);
        }
        protected override float GetVerticalAxisData(int graphIndex, object data)
        {
            const float TWO_THIRDS = 2.0f / 3.0f;
            const float ONE_THIRD = 1.0f / 3.0f;
            switch (graphIndex)
            {
                // Enabled
                case 0: return GetBoolVerticalAxisData((bool)data);
                // Sprite
                case 1: return ((SpriteData)data).sprite.GetHashCode();
                // FlipX
                case 2: return GetBoolVerticalAxisData((bool)data);
                // Sorting Order
                case 3: return (int)data;
                default:
                {
                    CustomDebug.UnhandledEnum(graphIndex, GetType().Name);
                    return 0;
                }
            }

            float GetBoolVerticalAxisData(bool data)
            {
                float t_multiplyAm = data ? TWO_THIRDS : ONE_THIRD;
                return (minMaxDiff * t_multiplyAm) + recentMinValue;
            }
        }
        protected override Color GetGraphColor(int graphIndex)
        {
            switch (graphIndex)
            {
                // Enabled
                case 0: return Color.green;
                // Sprite
                case 1: return Color.red;
                // FlipX
                case 2: return Color.cyan;
                // Sorting Order
                case 3: return Color.yellow;
                default:
                {
                    CustomDebug.UnhandledEnum(graphIndex, GetType().Name);
                    return Color.white;
                }
            }
        }
    }
}