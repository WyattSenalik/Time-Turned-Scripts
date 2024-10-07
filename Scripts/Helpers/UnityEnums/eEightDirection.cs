using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityEnums
{
    public enum eEightDirection { Left = -1, Right = 1, Down = -2, Up = 2, LeftUp = -3, RightUp = -4, LeftDown = 4, RightDown = 3 }

    public static class EightDirectionExtensions
    {
        public const float SQRT_TWO_OVER_TWO = 0.70710678118f;

        public static bool GetComponentDirectionsIfDiagonal(this eEightDirection thisDir, out eEightDirection horiCompDir, out eEightDirection vertCompDir)
        {
            horiCompDir = eEightDirection.Left;
            vertCompDir = eEightDirection.Left;
            switch (thisDir)
            {
                case eEightDirection.Left: return false;
                case eEightDirection.Right: return false;
                case eEightDirection.Down: return false;
                case eEightDirection.Up: return false;
                case eEightDirection.LeftUp:
                    horiCompDir = eEightDirection.Left;
                    vertCompDir = eEightDirection.Up;
                    return true;
                case eEightDirection.RightUp:
                    horiCompDir = eEightDirection.Right;
                    vertCompDir = eEightDirection.Up;
                    return true;
                case eEightDirection.LeftDown:
                    horiCompDir = eEightDirection.Left;
                    vertCompDir = eEightDirection.Down;
                    return true;
                case eEightDirection.RightDown:
                    horiCompDir = eEightDirection.Right;
                    vertCompDir = eEightDirection.Down;
                    return true;
                default:
                {
                    CustomDebug.UnhandledEnum(thisDir, nameof(ToOffsetInt));
                    return false;
                }
            }
        }
        /// <summary>
        /// Tests if the dirToTest is a component direction of thisDir. Returns false always if thisDir is not a diagonal.
        /// </summary>
        public static bool IsGivenDirComponentDirectionOfThis(this eEightDirection thisDir, eEightDirection dirToTest)
        {
            if (!thisDir.GetComponentDirectionsIfDiagonal(out eEightDirection t_horiCompDir, out eEightDirection t_vertCompDir))
            {
                // Not even diagonal
                return false;
            }
            return dirToTest == t_horiCompDir || dirToTest == t_vertCompDir;
        }
        /// <summary>
        /// Returns true if an overlap was found between the directions.
        /// If the directions are the same, returns true and the overlapResult will also have the same value as dirA and dirB.
        /// Otherwise, will either return Left, Right, Up, or Down.
        /// Example: RightUp and LeftUp have the overlap direction of Up, so true is returned and overlapResult is Up.
        /// Example: RightUp and Left have no overlap direction, so false is returned.
        /// </summary>
        public static bool TryGetOverlap(this eEightDirection dirA, eEightDirection dirB, out eEightDirection overlapResult)
        {
            if (dirA == dirB)
            {
                overlapResult = dirA;
                return true;
            }
            if (dirA.IsLeftward() && dirB.IsLeftward())
            {
                overlapResult = eEightDirection.Left;
                return true;
            }
            if (dirA.IsRightward() && dirB.IsRightward())
            {
                overlapResult = eEightDirection.Right;
                return true;
            }
            if (dirA.IsUpward() && dirB.IsUpward())
            {
                overlapResult = eEightDirection.Up;
                return true;
            }
            if (dirA.IsDownward() && dirB.IsDownward())
            {
                overlapResult = eEightDirection.Down;
                return true;
            }

            overlapResult = dirA;
            return false;
        }
        /// <summary>
        /// Returns true if given direction is left, leftdown, or leftup.
        /// </summary>
        public static bool IsLeftward(this eEightDirection thisDir)
        {
            return thisDir == eEightDirection.LeftDown || thisDir == eEightDirection.LeftUp || thisDir == eEightDirection.Left;
        }
        /// <summary>
        /// Returns true if given direction is right, rightdown, or rightup.
        /// </summary>
        public static bool IsRightward(this eEightDirection thisDir)
        {
            return thisDir == eEightDirection.RightDown || thisDir == eEightDirection.RightUp || thisDir == eEightDirection.Right;
        }
        /// <summary>
        /// Returns true if given direction is up, rightup, or leftup.
        /// </summary>
        public static bool IsUpward(this eEightDirection thisDir)
        {
            return thisDir == eEightDirection.Up || thisDir == eEightDirection.RightUp || thisDir == eEightDirection.LeftUp;
        }
        /// <summary>
        /// Returns true if given direction is down, rightdown, or leftdown.
        /// </summary>
        public static bool IsDownward(this eEightDirection thisDir)
        {
            return thisDir == eEightDirection.Down || thisDir == eEightDirection.RightDown || thisDir == eEightDirection.LeftDown;
        }
        /// <summary>
        /// Gets the two directions that are adjacent to this direction. For example if the direction is left, the adjacent directions are leftUp and leftDown. For diagonals, this is the same as the directions component directions.
        /// </summary>
        public static void GetAdjacentDirections(this eEightDirection thisDir, out eEightDirection counterClockwiseAdjDir, out eEightDirection clockwiseAdjDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Left:
                {
                    counterClockwiseAdjDir = eEightDirection.LeftDown;
                    clockwiseAdjDir = eEightDirection.LeftUp;
                    break;
                }
                case eEightDirection.LeftUp:
                {
                    counterClockwiseAdjDir = eEightDirection.Left;
                    clockwiseAdjDir = eEightDirection.Up;
                    break;
                }
                case eEightDirection.Up:
                {
                    counterClockwiseAdjDir = eEightDirection.LeftUp;
                    clockwiseAdjDir = eEightDirection.RightUp;
                    break;
                }
                case eEightDirection.RightUp:
                {
                    counterClockwiseAdjDir = eEightDirection.Up;
                    clockwiseAdjDir = eEightDirection.Right;
                    break;
                }
                case eEightDirection.Right:
                {
                    counterClockwiseAdjDir = eEightDirection.RightUp;
                    clockwiseAdjDir = eEightDirection.RightDown;
                    break;
                }
                case eEightDirection.RightDown:
                {
                    counterClockwiseAdjDir = eEightDirection.Right;
                    clockwiseAdjDir = eEightDirection.Down;
                    break;
                }
                case eEightDirection.Down:
                {
                    counterClockwiseAdjDir = eEightDirection.RightDown;
                    clockwiseAdjDir = eEightDirection.LeftDown;
                    break;
                }
                case eEightDirection.LeftDown:
                {
                    counterClockwiseAdjDir = eEightDirection.Down;
                    clockwiseAdjDir = eEightDirection.Left;
                    break;
                }
                default:
                {
                    counterClockwiseAdjDir = eEightDirection.Left;
                    clockwiseAdjDir = eEightDirection.Left;
                    CustomDebug.UnhandledEnum(thisDir, nameof(GetAdjacentDirections));
                    break;
                }
            }
        }
        /// <summary>
        /// Returns true when the dirToTest is an adjacent direction to thisDir.
        /// </summary>
        public static bool IsGivenDirAdjacentToThis(this eEightDirection thisDir, eEightDirection dirToTest)
        {
            thisDir.GetAdjacentDirections(out eEightDirection t_ccwAdjDir, out eEightDirection t_cwAdjDir);
            return dirToTest == t_ccwAdjDir || dirToTest == t_cwAdjDir;
        }
        /// <summary>
        /// Gets the two directions that are perpendicular to this direction. For example if the direction is left, the perpendicular directions are Up and Down.
        /// </summary>
        public static void GetPerpendicularDirections(this eEightDirection thisDir, out eEightDirection counterClockwisePerpDir, out eEightDirection clockwisePerpDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Left:
                {
                    counterClockwisePerpDir = eEightDirection.Down;
                    clockwisePerpDir = eEightDirection.Up;
                    break;
                }
                case eEightDirection.LeftUp:
                {
                    counterClockwisePerpDir = eEightDirection.LeftDown;
                    clockwisePerpDir = eEightDirection.RightUp;
                    break;
                }
                case eEightDirection.Up:
                {
                    counterClockwisePerpDir = eEightDirection.Left;
                    clockwisePerpDir = eEightDirection.Right;
                    break;
                }
                case eEightDirection.RightUp:
                {
                    counterClockwisePerpDir = eEightDirection.LeftUp;
                    clockwisePerpDir = eEightDirection.RightDown;
                    break;
                }
                case eEightDirection.Right:
                {
                    counterClockwisePerpDir = eEightDirection.Up;
                    clockwisePerpDir = eEightDirection.Down;
                    break;
                }
                case eEightDirection.RightDown:
                {
                    counterClockwisePerpDir = eEightDirection.RightUp;
                    clockwisePerpDir = eEightDirection.LeftDown;
                    break;
                }
                case eEightDirection.Down:
                {
                    counterClockwisePerpDir = eEightDirection.Right;
                    clockwisePerpDir = eEightDirection.Left;
                    break;
                }
                case eEightDirection.LeftDown:
                {
                    counterClockwisePerpDir = eEightDirection.RightDown;
                    clockwisePerpDir = eEightDirection.LeftUp;
                    break;
                }
                default:
                {
                    counterClockwisePerpDir = eEightDirection.Left;
                    clockwisePerpDir = eEightDirection.Left;
                    CustomDebug.UnhandledEnum(thisDir, nameof(GetPerpendicularDirections));
                    break;
                }
            }
        }
        /// <summary>
        /// Returns true if this direction is leftUp, leftDown, rightUp, or rightDown.
        /// Returns false if this direction is left, right, down, or up.
        /// </summary>
        public static bool IsDiagonal(this eEightDirection thisDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Left: return false;
                case eEightDirection.Right: return false;
                case eEightDirection.Down: return false;
                case eEightDirection.Up: return false;
                case eEightDirection.LeftUp: return true;
                case eEightDirection.RightUp: return true;
                case eEightDirection.LeftDown: return true;
                case eEightDirection.RightDown: return true;
                default:
                {
                    CustomDebug.UnhandledEnum(thisDir, nameof(ToOffsetInt));
                    return false;
                }
            }
        }
        public static bool IsOpposite(this eEightDirection thisDir, eEightDirection other)
        {
            return (int)thisDir + (int)other == 0;
        }
        public static bool IsPerpendicular(this eEightDirection thisDir, eEightDirection other)
        {
            thisDir.GetPerpendicularDirections(out eEightDirection t_ccwPerpDir, out eEightDirection t_cwPerpDir);
            return other == t_ccwPerpDir || other == t_cwPerpDir;
        }
        /// <summary>
        /// Returns true if this direction is down or up.
        /// Returns false if this direction is left or right.
        /// Also returns true for all diagonals.
        /// </summary>
        public static bool IsVertical(this eEightDirection thisDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Left:
                case eEightDirection.Right:
                    return false;
                default:
                    return true;
            }
        }
        /// <summary>
        /// Returns true if this direction is left or right.
        /// Returns false if this direction is up or down.
        /// Also returns true for all diagonals.
        /// </summary>
        public static bool IsHorizontal(this eEightDirection thisDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Up:
                case eEightDirection.Down:
                    return false;
                default:
                    return true;
            }
        }
        public static Vector2Int ToOffsetInt(this eEightDirection thisDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Left: return new Vector2Int(-1, 0);
                case eEightDirection.Right: return new Vector2Int(1, 0);
                case eEightDirection.Down: return new Vector2Int(0, -1);
                case eEightDirection.Up: return new Vector2Int(0, 1);
                case eEightDirection.LeftUp: return new Vector2Int(-1, 1);
                case eEightDirection.RightUp: return new Vector2Int(1, 1);
                case eEightDirection.LeftDown: return new Vector2Int(-1, -1);
                case eEightDirection.RightDown: return new Vector2Int(1, -1);
                default:
                {
                    CustomDebug.UnhandledEnum(thisDir, nameof(ToOffsetInt));
                    return Vector2Int.zero;
                }
            }
        }
        public static Vector2 ToOffset(this eEightDirection thisDir)
        {
            switch (thisDir)
            {
                case eEightDirection.Left:
                    return new Vector2(-1, 0);
                case eEightDirection.Right:
                    return new Vector2(1, 0);
                case eEightDirection.Down:
                    return new Vector2(0, -1);
                case eEightDirection.Up:
                    return new Vector2(0, 1);
                case eEightDirection.LeftUp:
                    return new Vector2(-SQRT_TWO_OVER_TWO, SQRT_TWO_OVER_TWO);
                case eEightDirection.RightUp:
                    return new Vector2(SQRT_TWO_OVER_TWO, SQRT_TWO_OVER_TWO);
                case eEightDirection.LeftDown:
                    return new Vector2(-SQRT_TWO_OVER_TWO, -SQRT_TWO_OVER_TWO);
                case eEightDirection.RightDown:
                    return new Vector2(SQRT_TWO_OVER_TWO, -SQRT_TWO_OVER_TWO);
                default:
                {
                    CustomDebug.UnhandledEnum(thisDir, nameof(ToOffset));
                    return Vector2Int.zero;
                }
            }
        }
        public static eEightDirection GetOpposite(this eEightDirection thisDir)
        {
            return (eEightDirection)(-(int)thisDir);
        }
        public static eEightDirection ToClosestEightDirection(this Vector2 vector)
        {
            float t_angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            if (t_angle <= -157.5f)
            {
                return eEightDirection.Left;
            }
            else if (t_angle <= -112.5f)
            {
                return eEightDirection.LeftDown;
            }
            else if (t_angle <= -67.5f)
            {
                return eEightDirection.Down;
            }
            else if (t_angle <= -22.5f)
            {
                return eEightDirection.RightDown;
            }
            else if (t_angle <= 22.5f)
            {
                return eEightDirection.Right;
            }
            else if (t_angle <= 67.5f)
            {
                return eEightDirection.RightUp;
            }
            else if (t_angle <= 112.5f)
            {
                return eEightDirection.Up;
            }
            else if (t_angle <= 157.5f)
            {
                return eEightDirection.LeftUp;
            }
            else if (t_angle <= 180.0f)
            {
                return eEightDirection.Left;
            }
            else
            {
                #region Logs
                //CustomDebug.LogError($"Unhandled angle ({t_angle}). Expected Mathf.Atan2 to return angles in range [-180.0f, 180.0f].");
                #endregion Logs
                return eEightDirection.Down;
            }
        }
        public static eEightDirection ToClosestFourDirection(this Vector2 vector)
        {
            float t_angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            if (t_angle <= -112.5f)
            {
                return eEightDirection.Left;
            }
            else if (t_angle <= -22.5f)
            {
                return eEightDirection.Down;
            }
            else if (t_angle <= 67.5f)
            {
                return eEightDirection.Right;
            }
            else if (t_angle <= 157.5f)
            {
                return eEightDirection.Up;
            }
            else if (t_angle <= 180.0f)
            {
                return eEightDirection.Left;
            }
            else
            {
                #region Logs
                //CustomDebug.LogError($"Unhandled angle ({t_angle}). Expected Mathf.Atan2 to return angles in range [-180.0f, 180.0f].");
                #endregion Logs
                return eEightDirection.Down;
            }
        }


        /// <summary>
        /// Finds the direction in the list that are closest to this direction.
        /// For example: if this direction is left and the list contains all directions but left, then it will return leftUp and leftDown.
        /// </summary>
        public static eEightDirection[] GetClosestInList(this eEightDirection thisDir, IReadOnlyList<eEightDirection> queryList)
        {
            Vector2 t_thisOffset = thisDir.ToOffset();

            List<eEightDirection> t_closestDirs = new List<eEightDirection>(2);
            float t_mostPositiveDotResult = float.NegativeInfinity;
            for (int i = 0; i < queryList.Count; ++i)
            {
                eEightDirection t_curDir = queryList[i];
                if (t_curDir == thisDir)
                {
                    return new eEightDirection[] { thisDir };
                }
                Vector2 t_curOffset = t_curDir.ToOffset();
                float t_dotResult = Vector2.Dot(t_thisOffset, t_curOffset);
                if (t_dotResult > t_mostPositiveDotResult)
                {
                    t_closestDirs.Clear();
                    t_closestDirs.Add(t_curDir);
                    t_mostPositiveDotResult = t_dotResult;
                }
                else if (t_dotResult == t_mostPositiveDotResult)
                {
                    t_closestDirs.Add(t_curDir);
                }
            }
            return t_closestDirs.ToArray();
        }
    }
}