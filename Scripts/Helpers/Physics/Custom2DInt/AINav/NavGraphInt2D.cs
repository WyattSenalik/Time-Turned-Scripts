using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using Helpers.Singletons;
using Helpers.Math;
using Helpers.UnityEnums;

namespace Helpers.Physics.Custom2DInt.NavAI
{
    [Serializable]
    public sealed class NavGraphInt2D : SingletonMonoBehaviour<NavGraphInt2D>
    {
        private const bool IS_DEBUGGING = false;

        public int rowCount
        {
            get
            {
                if (m_gridPassableTable.Length % m_tilesInRow == 0)
                {
                    return m_gridPassableTable.Length / m_tilesInRow;
                }
                else
                {
                    return (m_gridPassableTable.Length + 1) / m_tilesInRow;
                }
            }
        }
        public int columnCount => m_tilesInRow;
        public Vector2Int tileSize => m_tileSize;
        public Vector2Int botLeftGridPos => m_botLeftGridPos;
        public int tileCount => m_gridPassableTable.Length;

        [SerializeField, FormerlySerializedAs("m_test")] private bool[] m_gridPassableTable = new bool[14]
        {
            true, false, true, true, true, true, true,
            true, false, true, false, false, false, true
        };
        [SerializeField] private int m_tilesInRow = 7;
        [SerializeField] private Vector2Int m_tileSize = new Vector2Int(64, 64);
        [SerializeField] private Vector2Int m_botLeftGridPos = Vector2Int.zero;

        [SerializeField] private bool m_alwaysShowGizmos = false;


        public bool IsTileAtFloatPositionPassable(Vector2 floatPos)
        {
            Vector2Int t_intPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(floatPos);
            return IsTileAtIntPositionPassable(t_intPos);
        }
        public bool IsTileAtIntPositionPassable(Vector2Int intPos)
        {
            int t_index = GetIndexOfTileThatContainsIntPoint(intPos);
            return IsTileAtIndexPassable(t_index);
        }
        public bool IsTileAtIndexPassable(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(IsTileAtIndexPassable)}");
                }
                return false;
            }
            return m_gridPassableTable[index];
        }
        public int GetIndexOfTileThatContainsIntPoint(Vector2Int intPos, bool allowNotOnGraphPoint = false)
        {
            RectangleInt t_boundingRect = GetBoundsOfGraph();
            if (CustomPhysics2DInt.IsPointInRectangle(t_boundingRect, intPos))
            {
                Vector2Int t_offsetFromBotLeft = intPos - m_botLeftGridPos;
                int t_xIndex = t_offsetFromBotLeft.x / m_tileSize.x;
                int t_yIndex = t_offsetFromBotLeft.y / m_tileSize.y;
                return t_yIndex * m_tilesInRow + t_xIndex;
            }
            else
            {
                if (!allowNotOnGraphPoint)
                {
                    //CustomDebug.LogWarning($"No tile contains this intPos {intPos}");
                }
                return -1;
            }
        }
        public int GetIndexOfTileRightOfTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileRightOfTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_rightIndex = index + 1;
            if (t_rightIndex % m_tilesInRow == 0 || t_rightIndex >= m_gridPassableTable.Length)
            {
                // No tile to the right of this tile.
                return -1;
            }
            else
            {
                return t_rightIndex;
            }
        }
        public int GetIndexOfTileLeftOfTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileLeftOfTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_leftIndex = index - 1;
            if (t_leftIndex % m_tilesInRow == m_tilesInRow - 1 || t_leftIndex < 0)
            {
                // No tile to the left of this tile.
                return -1;
            }
            else
            {
                return t_leftIndex;
            }
        }
        public int GetIndexOfTileAboveTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileAboveTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_aboveIndex = index + m_tilesInRow;
            if (t_aboveIndex >= m_gridPassableTable.Length)
            {
                // No tile above this tile.
                return -1;
            }
            else
            {
                return t_aboveIndex;
            }
        }
        public int GetIndexOfTileBelowTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileBelowTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_belowIndex = index - m_tilesInRow;
            if (t_belowIndex < 0)
            {
                // No tile below this tile.
                return -1;
            }
            else
            {
                return t_belowIndex;
            }
        }
        public int GetIndexOfTileUpRightOfTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileBelowTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_upRightIndex = index + m_tilesInRow + 1;
            if (t_upRightIndex % m_tilesInRow == 0 || t_upRightIndex >= m_gridPassableTable.Length)
            {
                // No tile below this tile.
                return -1;
            }
            else
            {
                return t_upRightIndex;
            }
        }
        public int GetIndexOfTileUpLeftOfTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileBelowTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_upRightIndex = index + m_tilesInRow - 1;
            if (t_upRightIndex % m_tilesInRow == m_tilesInRow - 1 || t_upRightIndex >= m_gridPassableTable.Length)
            {
                // No tile up right of this tile.
                return -1;
            }
            else
            {
                return t_upRightIndex;
            }
        }
        public int GetIndexOfTileDownRightOfTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileBelowTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_downRightIndex = index - m_tilesInRow + 1;
            if (t_downRightIndex % m_tilesInRow == 0 || t_downRightIndex < 0)
            {
                // No tile down right of this tile.
                return -1;
            }
            else
            {
                return t_downRightIndex;
            }
        }
        public int GetIndexOfTileDownLeftOfTileAtGivenIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetIndexOfTileBelowTileAtGivenIndex)}");
                }
                return -1;
            }

            int t_downLeftIndex = index - m_tilesInRow - 1;
            if (t_downLeftIndex % m_tilesInRow == m_tilesInRow - 1 || t_downLeftIndex < 0)
            {
                // No tile down left of this tile.
                return -1;
            }
            else
            {
                return t_downLeftIndex;
            }
        }
        public int GetIndexOfTileInDirOfTileAtGivenIndex(int index, eEightDirection dir, bool allowOutOfBounds = false)
        {
            switch (dir)
            {
                case eEightDirection.Left: return GetIndexOfTileLeftOfTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.Right: return GetIndexOfTileRightOfTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.Up: return GetIndexOfTileAboveTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.Down: return GetIndexOfTileBelowTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.LeftUp: return GetIndexOfTileUpLeftOfTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.RightUp: return GetIndexOfTileUpRightOfTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.LeftDown: return GetIndexOfTileDownLeftOfTileAtGivenIndex(index, allowOutOfBounds);
                case eEightDirection.RightDown: return GetIndexOfTileDownRightOfTileAtGivenIndex(index, allowOutOfBounds);
                default:
                {
                    CustomDebug.UnhandledEnum(dir, this);
                    return -1;
                }
            }
        }
        public Vector2Int GetBotLeftIntPositionOfTileAtIndex(int index, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(GetBotLeftIntPositionOfTileAtIndex)}");
                }
                return new Vector2Int(int.MinValue, int.MinValue);
            }

            int t_xIndex = index % m_tilesInRow;
            int t_yIndex = index / m_tilesInRow;

            return m_botLeftGridPos + new Vector2Int(t_xIndex * m_tileSize.x, t_yIndex * m_tileSize.y);
        }
        public Vector2Int GetCenterIntPositionOfTileAtIndex(int index, bool allowOutOfBounds = false)
        {
            return GetBotLeftIntPositionOfTileAtIndex(index, allowOutOfBounds) + (MathHelpers.FastDivideBy2(m_tileSize));
        }
        /// <summary>
        /// Only works for the tile the point is in and adjacent tiles.
        /// </summary>
        public int GetIndexOfClosestPassableTileToIntPosition(Vector2Int intPos)
        {
            int t_indexOfTileAtPos = GetIndexOfTileThatContainsIntPoint(intPos);
            if (t_indexOfTileAtPos < 0)
            {
                //CustomDebug.Log($"NO CLOSEST FOUND", IS_DEBUGGING);
            }
            // The actual tile is the closest.
            else if (IsTileAtIndexPassable(t_indexOfTileAtPos))
            {
                return t_indexOfTileAtPos;
            }
            int t_curDist = 1;
            int t_maxDist = Mathf.Max(rowCount, columnCount);
            while (t_curDist <= t_maxDist)
            {
                int t_curX = 0;
                int t_curY = t_curDist;
                // Top right check
                while (t_curY > 0)
                {
                    if (IsTileAtPositionWithOffsetPassable(intPos, t_curX, t_curY, out int t_tileIndex))
                    {
                        return t_tileIndex;
                    }
                    --t_curY;
                    ++t_curX;
                }
                // Bot right check
                while (t_curX > 0)
                {
                    if (IsTileAtPositionWithOffsetPassable(intPos, t_curX, t_curY, out int t_tileIndex))
                    {
                        return t_tileIndex;
                    }
                    --t_curY;
                    --t_curX;
                }
                // Bot left check
                while (t_curY < 0)
                {
                    if (IsTileAtPositionWithOffsetPassable(intPos, t_curX, t_curY, out int t_tileIndex))
                    {
                        return t_tileIndex;
                    }
                    ++t_curY;
                    --t_curX;
                }
                // Top left check
                while (t_curX < 0)
                {
                    if (IsTileAtPositionWithOffsetPassable(intPos, t_curX, t_curY, out int t_tileIndex))
                    {
                        return t_tileIndex;
                    }
                    ++t_curY;
                    ++t_curX;
                }

                ++t_curDist;
            }
            // No tiles are passable.
            return -1;
        }
        private bool IsTileAtPositionWithOffsetPassable(Vector2Int centerPos, int xOffset, int yOffset, out int tileIndex)
        {
            Vector2Int t_newCheckPos = centerPos + new Vector2Int(xOffset * m_tileSize.x, yOffset * m_tileSize.y);
            tileIndex = GetIndexOfTileThatContainsIntPoint(t_newCheckPos);
            return IsTileAtIndexPassable(tileIndex, true);
        }
        private int GetIndexOfClosestPassableTileToIntPositionCheckAdjacents(Vector2Int intPos, List<int> indicesToCheckAdjacentsFor, List<int> alreadyCheckedIndices, int[] preallocatedAdjIndices)
        {
            int t_curTileIndex = indicesToCheckAdjacentsFor[0];

            preallocatedAdjIndices[0] = GetIndexOfTileLeftOfTileAtGivenIndex(t_curTileIndex);
            preallocatedAdjIndices[1] = GetIndexOfTileRightOfTileAtGivenIndex(t_curTileIndex);
            preallocatedAdjIndices[2] = GetIndexOfTileAboveTileAtGivenIndex(t_curTileIndex);
            preallocatedAdjIndices[3] = GetIndexOfTileBelowTileAtGivenIndex(t_curTileIndex);

            int t_closestIndex = -1;
            int t_closestSqDist = int.MaxValue;
            for (int i = 0; i < 4; ++i)
            {
                int t_singleAdjIndex = preallocatedAdjIndices[i];
                if (alreadyCheckedIndices.Contains(t_singleAdjIndex))
                { continue; }
                alreadyCheckedIndices.Add(t_singleAdjIndex);
                if (!IsTileAtIndexPassable(t_singleAdjIndex, true))
                {
                    indicesToCheckAdjacentsFor.Add(t_singleAdjIndex);
                    continue;
                }

                Vector2Int t_tileCenterPos = GetCenterIntPositionOfTileAtIndex(t_singleAdjIndex);
                Vector2Int t_diff = t_tileCenterPos - intPos;
                int t_sqrDist = t_diff.sqrMagnitude;
                if (t_sqrDist < t_closestSqDist)
                {
                    t_closestIndex = t_singleAdjIndex;
                    t_closestSqDist = t_sqrDist;
                }
            }
            if (t_closestIndex >= 0)
            {
                return t_closestIndex;
            }
            indicesToCheckAdjacentsFor.RemoveAt(0);
            return -1;
        }
        public void SetTileAtIndexAsImpassable(int index, bool allowOutOfBounds = false)
        {
            SetStateOfTileAtIndex(index, false, allowOutOfBounds);
        }
        public void SetTileAtIndexAsPassable(int index, bool allowOutOfBounds = false)
        {
            SetStateOfTileAtIndex(index, true, allowOutOfBounds);
        }
        public void SetStateOfTileAtIndex(int index, bool impassableOrPassable, bool allowOutOfBounds = false)
        {
            if (index < 0 || index >= m_gridPassableTable.Length)
            {
                if (!allowOutOfBounds)
                {
                    //CustomDebug.LogError($"Index out of bounds for {nameof(IsTileAtIndexPassable)}");
                }
                return;
            }
            m_gridPassableTable[index] = impassableOrPassable;
        }
        public void SetTileThatContainsIntPositionAsImpassable(Vector2Int intPos)
        {
            int t_index = GetIndexOfTileThatContainsIntPoint(intPos);
            SetTileAtIndexAsImpassable(t_index);
        }
        public void SetTileThatContainsIntPositionAsPassable(Vector2Int intPos)
        {
            int t_index = GetIndexOfTileThatContainsIntPoint(intPos);
            SetTileAtIndexAsPassable(t_index);
        }
        public void SetStateOfTileThatContainsIntPosition(Vector2Int intPos, bool impassableOrPassable)
        {
            int t_index = GetIndexOfTileThatContainsIntPoint(intPos);
            SetStateOfTileAtIndex(t_index, impassableOrPassable);
        }
        public void SetTileThatContainsFloatPositionAsImpassable(Vector2 floatPos)
        {
            Vector2Int t_intPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(floatPos);
            SetTileThatContainsIntPositionAsImpassable(t_intPos);
        }
        public void SetTileThatContainsFloatPositionAsPassable(Vector2 floatPos)
        {
            Vector2Int t_intPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(floatPos);
            SetTileThatContainsIntPositionAsPassable(t_intPos);
        }
        public void SetStateOfTileThatContainsFloatPosition(Vector2 floatPos, bool impassableOrPassable)
        {
            Vector2Int t_intPos = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(floatPos);
            SetStateOfTileThatContainsIntPosition(t_intPos, impassableOrPassable);
        }

        public RectangleInt GetBoundsOfGraph()
        {
            return new RectangleInt(m_botLeftGridPos, new Vector2Int(m_tileSize.x * columnCount, m_tileSize.y * rowCount));
        }


        private void OnDrawGizmosSelected()
        {
            if (!m_alwaysShowGizmos)
            {
                DrawGizmos();
            }
        }
        private void OnDrawGizmos()
        {
            if (m_alwaysShowGizmos)
            {
                DrawGizmos();
            }
        }
        private void DrawGizmos()
        {
            int t_rowIndex = -1;
            for (int i = 0; i < m_gridPassableTable.Length; ++i)
            {
                int t_colIndex = i % m_tilesInRow;
                if (t_colIndex == 0)
                {
                    ++t_rowIndex;
                }

                Vector2Int t_rectBotLeftPos = m_botLeftGridPos + new Vector2Int(t_colIndex * m_tileSize.x, t_rowIndex * m_tileSize.y);
                RectangleInt t_curRect = new RectangleInt(t_rectBotLeftPos, m_tileSize);
                bool t_isNavigatable = m_gridPassableTable[i];
                Color t_col = t_isNavigatable ? Color.blue : Color.red;
                t_curRect.DrawOutlineGizmos(t_col);
                if (!t_isNavigatable)
                {
                    t_curRect.DrawInsideGizmos(t_col);
                }
            }

            //CircleInt t_testPointCirc = new CircleInt(m_testPoint, 8);
            //Color t_testCol = IsTileAtIntPositionPassable(m_testPoint) ? Color.green : Color.magenta;
            //t_testPointCirc.DrawGizmos(t_testCol);
            //int t_testPosTileIndex = GetIndexOfTileThatContainsIntPoint(m_testPoint);

            //int t_rightCheckIndex = GetIndexOfTileRightOfTileAtGivenIndex(t_testPosTileIndex);
            //DebugTileAtIndex(t_rightCheckIndex);
            //int t_leftCheckIndex = GetIndexOfTileLeftOfTileAtGivenIndex(t_testPosTileIndex);
            //DebugTileAtIndex(t_leftCheckIndex);
            //int t_aboveCheckIndex = GetIndexOfTileAboveTileAtGivenIndex(t_testPosTileIndex);
            //DebugTileAtIndex(t_aboveCheckIndex);
            //int t_belowCheckIndex = GetIndexOfTileBelowTileAtGivenIndex(t_testPosTileIndex);
            //DebugTileAtIndex(t_belowCheckIndex);

            //void DebugTileAtIndex(int tileIndex)
            //{
            //    if (tileIndex < 0) { return; }

            //    Vector2Int t_debugTileCenterPos = GetCenterIntPositionOfTileAtIndex(tileIndex);
            //    CircleInt t_debugTilePointCirc = new CircleInt(t_debugTileCenterPos, 8);
            //    Color t_debugTileColor = IsTileAtIndexPassable(tileIndex) ? Color.green : Color.magenta;
            //    t_debugTilePointCirc.DrawGizmos(t_debugTileColor);
            //}
        }
    }
}