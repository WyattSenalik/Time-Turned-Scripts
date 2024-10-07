using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Helpers.Physics.Custom2DInt.NavAI;
using Helpers.UnityEnums;
using Timed;
// Original Author - Sam Smith
// Tweaked by Wyatt Senalik (Added SmartWallMovement)

namespace Atma
{
    /// <summary>
    /// Sets turret's velocity to follow the ModularTurretController's target if it matches following conditions
    /// Pre Conditions:
    [RequireComponent(typeof(ModularTurretController))]
    [RequireComponent(typeof(CharacterMover))]
    [RequireComponent(typeof(Int2DTransform))]
    /// </summary>
    public class DirectFollowModule : TimedRecorder
    {
        private const float UPDATE_FREQUENCY = 0.2f;
        private const float UPDATE_OFFSET_TIME = 0.04f;
        private const float CLOSE_ENOUGH_DIST = 0.1f;

        private static int s_velocityUpdateOffset = 0;

        [Tooltip("Units per FixedUpdate to move")]
        [SerializeField] private float m_speed = 1f;

        [SerializeField, Required] private SpriteRenderer m_rendererThatDeterminesIfSoundPlays;
        [SerializeField, Required] private PushableBoxCollider m_pusherCollider = null;
        [SerializeField, Required] private FootstepController m_footstepCont = null;

        [SerializeField, BoxGroup("Debugging")] private bool m_shouldDrawDebug = false;
        [SerializeField, BoxGroup("Debugging"), ReadOnly] private Vector2 m_debugVelocity = Vector2.zero;

        private NavGraphInt2D m_navGraph = null;

        private ModularTurretController m_parentTurret = null;
        private CharacterMover m_charMover = null;
        private Int2DTransform m_intTransform = null;

        private Vector2Int m_prevPos = Vector2Int.zero;

        private int m_myVelocityUpdateOffset = 0;
        private readonly List<float> m_updateTimes = new List<float>();


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pusherCollider, nameof(m_pusherCollider), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_footstepCont, nameof(m_footstepCont), this);
            #endregion Asserts
            m_parentTurret = this.GetComponentSafe<ModularTurretController>();
            m_charMover = this.GetComponentSafe<CharacterMover>();
            m_intTransform = this.GetComponentSafe<Int2DTransform>();

            m_prevPos = m_intTransform.localPosition;

            m_myVelocityUpdateOffset = s_velocityUpdateOffset++;
        }
        private void Start()
        {
            m_navGraph = NavGraphInt2D.GetInstanceSafe();
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (isRecording)
            {
                Vector2Int t_curPos = m_intTransform.localPosition;

                if (m_parentTurret.health <= 0)
                {
                    m_debugVelocity = Vector2.zero;
                    m_charMover.velocity = Vector2.zero;
                }
                else
                {
                    float t_mostRecentUpdateTime;
                    if (m_updateTimes.Count > 0)
                    {
                        t_mostRecentUpdateTime = m_updateTimes[^1];
                    }
                    else
                    {
                        t_mostRecentUpdateTime = float.NegativeInfinity;
                    }
                    float t_nextUpdateTime = t_mostRecentUpdateTime + UPDATE_FREQUENCY + UPDATE_OFFSET_TIME * m_myVelocityUpdateOffset;
                    if (t_nextUpdateTime <= curTime)
                    {
                        // Update update velocity if we've waited enough.

                        // Set turret velocity to calculated direction and speed toward target.
                        Vector2 t_nextMoveVect = NextNormalMoveVect();
                        float t_moveDist = m_speed * timeScale;

                        //m_debugStr += $"Time: {curTime}. TargetPos:{m_parentTurret.target.position}. MyPos:{m_intTransform.position}. Dir:{t_nextMoveVect}. Dist:{t_moveDist}.\n";

                        m_debugVelocity = t_nextMoveVect;
                        m_charMover.velocity = t_nextMoveVect * t_moveDist;

                        Vector2Int t_posDelta = t_curPos - m_prevPos;
                        if (t_posDelta.sqrMagnitude > 1 && m_rendererThatDeterminesIfSoundPlays.isVisible)
                        {
                            m_footstepCont.RequestFootstep();
                        }
                    }
                }

                m_prevPos = t_curPos;
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            m_prevPos = m_intTransform.localPosition;

            for (int i = m_updateTimes.Count - 1; i >= 0; ++i)
            {
                if (m_updateTimes[i] > curTime)
                {
                    m_updateTimes.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Compare the turret's target against follow conditions and returns a normal Vector2 of the direction the turret should move
        /// Pre Conditions: m_parentTurret must not be null
        /// </summary>
        public Vector2 NextNormalMoveVect()
        {
            if (m_parentTurret.target == null)
            {
                return Vector2.right;
            }

            Vector2 t_diffToTarget = m_parentTurret.target.positionFloat - m_intTransform.positionFloat;
            eEightDirection t_desiredDir = t_diffToTarget.ToClosestEightDirection();
            eEightDirection t_actualDir;
            switch (t_desiredDir)
            {
                case eEightDirection.Left:
                {
                    GetActualDirWhenLeftIsDesired(t_diffToTarget, out t_actualDir);
                    break;
                }
                case eEightDirection.Right:
                {
                    GetActualDirWhenRightIsDesired(t_diffToTarget, out t_actualDir);
                    break;
                }
                case eEightDirection.Up:
                {
                    GetActualDirWhenUpIsDesired(t_diffToTarget, out t_actualDir);
                    break;
                }
                case eEightDirection.Down:
                {
                    GetActualDirWhenDownIsDesired(t_diffToTarget, out t_actualDir);
                    break;
                }
                case eEightDirection.LeftUp:
                {
                    t_actualDir = GetActualDirWhenLeftUpIsDesired(t_diffToTarget);
                    break;
                }
                case eEightDirection.LeftDown:
                {
                    t_actualDir = GetActualDirWhenLeftDownIsDesired(t_diffToTarget);
                    break;
                }
                case eEightDirection.RightUp:
                {
                    t_actualDir = GetActualDirWhenRightUpIsDesired(t_diffToTarget);
                    break;
                }
                case eEightDirection.RightDown:
                {
                    t_actualDir = GetActualDirWhenRightDownIsDesired(t_diffToTarget);
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(t_desiredDir, this);
                    t_actualDir = eEightDirection.Right;
                    break;
                }
            }

            Vector2 t_rtnDir;
            if (t_actualDir == t_desiredDir)
            {
                t_rtnDir = t_diffToTarget.normalized;
            }
            else
            {
                t_rtnDir = t_actualDir.ToOffset();
            }
            //CustomDebug.DrawRay(transform.position, t_desiredDir.ToOffset(), Color.magenta, m_shouldDrawDebug);
            //CustomDebug.DrawRay(transform.position, t_rtnDir, Color.white, m_shouldDrawDebug);
            return t_rtnDir;
        }
        /// <summary>
        /// Returns true if can go in the actual direction returned.
        /// </summary>
        private bool GetActualDirWhenLeftIsDesired(Vector2 diffToTarget, out eEightDirection actualDir)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_topLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topLeftPoint);
            int t_botLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botLeftPoint);
            return GetActualDirWhenNonDiagonalIsDesired(diffToTarget.y, t_botLeftTileIndex, t_topLeftTileIndex, eEightDirection.Left, out actualDir);
        }
        private bool GetActualDirWhenRightIsDesired(Vector2 diffToTarget, out eEightDirection actualDir)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_topRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topRightPoint);
            int t_botRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botRightPoint);
            return GetActualDirWhenNonDiagonalIsDesired(-diffToTarget.y, t_topRightTileIndex, t_botRightTileIndex, eEightDirection.Right, out actualDir);
        }
        private bool GetActualDirWhenUpIsDesired(Vector2 diffToTarget, out eEightDirection actualDir)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_topLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topLeftPoint);
            int t_topRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topRightPoint);
            return GetActualDirWhenNonDiagonalIsDesired(diffToTarget.x, t_topLeftTileIndex, t_topRightTileIndex, eEightDirection.Up, out actualDir);
        }
        private bool GetActualDirWhenDownIsDesired(Vector2 diffToTarget, out eEightDirection actualDir)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_botLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botLeftPoint);
            int t_botRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botRightPoint);
            return GetActualDirWhenNonDiagonalIsDesired(-diffToTarget.x, t_botRightTileIndex, t_botLeftTileIndex, eEightDirection.Down, out actualDir);
        }
        private eEightDirection GetActualDirWhenLeftUpIsDesired(Vector2 diffToTarget)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_botLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botLeftPoint);
            int t_topLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topLeftPoint);
            int t_topRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topRightPoint);

            return GetActualDirWhenDiagonalIsDesiredHelper(diffToTarget, eEightDirection.LeftUp, t_topLeftTileIndex, t_botLeftTileIndex, t_topRightTileIndex);
        }
        private eEightDirection GetActualDirWhenLeftDownIsDesired(Vector2 diffToTarget)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_botLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botLeftPoint);
            int t_topLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topLeftPoint);
            int t_botRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botRightPoint);

            return GetActualDirWhenDiagonalIsDesiredHelper(diffToTarget, eEightDirection.LeftDown, t_botLeftTileIndex, t_topLeftTileIndex, t_botRightTileIndex);
        }
        private eEightDirection GetActualDirWhenRightUpIsDesired(Vector2 diffToTarget)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_topRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topRightPoint);
            int t_topLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topLeftPoint);
            int t_botRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botRightPoint);

            return GetActualDirWhenDiagonalIsDesiredHelper(diffToTarget, eEightDirection.RightUp, t_topRightTileIndex, t_botRightTileIndex, t_topLeftTileIndex);
        }
        private eEightDirection GetActualDirWhenRightDownIsDesired(Vector2 diffToTarget)
        {
            RectangleInt t_rectangle = m_pusherCollider.rectangleCollider.rectangle;
            int t_botRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botRightPoint);
            int t_topRightTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.topRightPoint);
            int t_botLeftTileIndex = m_navGraph.GetIndexOfTileThatContainsIntPoint(t_rectangle.botLeftPoint);

            return GetActualDirWhenDiagonalIsDesiredHelper(diffToTarget, eEightDirection.RightDown, t_botRightTileIndex, t_topRightTileIndex, t_botLeftTileIndex);
        }
        private bool GetActualDirWhenNonDiagonalIsDesired(float concernedDiffToTarget, int ccwTileIndex, int cwTileIndex, eEightDirection desiredDir, out eEightDirection actualDir)
        {
            desiredDir.GetAdjacentDirections(out eEightDirection t_ccwDiagonalDir, out eEightDirection t_cwDiagonalDir);
            desiredDir.GetPerpendicularDirections(out eEightDirection t_ccwPerpDir, out eEightDirection t_cwPerpDir);

            if (cwTileIndex == ccwTileIndex)
            {
                int t_tileInDesiredDir = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(ccwTileIndex, desiredDir, true);
                if (m_navGraph.IsTileAtIndexPassable(t_tileInDesiredDir, true))
                {
                    // Both concerning points can move in desired dir.
                    actualDir = desiredDir;
                    return true;
                }
                else
                {
                    // Neither concerining point can move in desired dir, check if we want to go positive or negative of perpendicular.
                    if (concernedDiffToTarget > CLOSE_ENOUGH_DIST)
                    {
                        // Want to go in direction of cw perpendicular
                        int t_cwPerpTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(ccwTileIndex, t_cwPerpDir, true);
                        if (m_navGraph.IsTileAtIndexPassable(t_cwPerpTileIndex, true))
                        {
                            actualDir = t_cwPerpDir;
                            return true;
                        }
                        else
                        {
                            actualDir = t_cwDiagonalDir;
                            return false;
                        }
                    }
                    else if (concernedDiffToTarget < -CLOSE_ENOUGH_DIST)
                    {
                        // Want to go in direction of ccw perpendicular
                        int t_ccwPerpTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(ccwTileIndex, t_ccwPerpDir, true);
                        if (m_navGraph.IsTileAtIndexPassable(t_ccwPerpTileIndex, true))
                        {
                            actualDir = t_ccwPerpDir;
                            return true;
                        }
                        else
                        {
                            actualDir = t_ccwDiagonalDir;
                            return false;
                        }
                    }
                    else
                    {
                        // Want to go nowhere.
                        actualDir = desiredDir;
                        return false;
                    }
                }
            }
            else
            {
                // They are in different tiles.
                int t_cwPointDesiredDirTile = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(cwTileIndex, desiredDir, true);
                int t_ccwPointDesiredDirTile = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(ccwTileIndex, desiredDir, true);
                bool t_isCWPointDesiredDirPassable = m_navGraph.IsTileAtIndexPassable(t_cwPointDesiredDirTile, true);
                bool t_isCCWPointDesiredDirPassable = m_navGraph.IsTileAtIndexPassable(t_ccwPointDesiredDirTile, true);
                if (t_isCWPointDesiredDirPassable)
                {
                    if (t_isCCWPointDesiredDirPassable)
                    {
                        // Both are passable, go left.
                        actualDir = desiredDir;
                        return true;
                    }
                    else
                    {
                        // Only top is passable, go up.
                        actualDir = t_cwDiagonalDir;
                        return true;
                    }
                }
                else if (t_isCCWPointDesiredDirPassable)
                {
                    // Only bot is passable, go down.
                    actualDir = t_ccwDiagonalDir;
                    return true;
                }
                else
                {
                    // Neither left point can move left, check if we want to go up or down.
                    if (concernedDiffToTarget > CLOSE_ENOUGH_DIST)
                    {
                        // Want to go up
                        actualDir = t_cwPerpDir;
                        return false;
                    }
                    else if (concernedDiffToTarget < -CLOSE_ENOUGH_DIST)
                    {
                        // Want to go down
                        actualDir = t_ccwPerpDir;
                        return false;
                    }
                    else
                    {
                        // Want to go nowhere.
                        actualDir = desiredDir;
                        return false;
                    }
                }
            }
        }
        private eEightDirection GetActualDirWhenDiagonalIsDesiredHelper(Vector2 diffToTarget, eEightDirection desiredDiagonalDir, int desiredDiagonalCornerTileIndex, int tileIndexThatSharesHorizontal, int tileIndexThatSharesVertical)
        {
            desiredDiagonalDir.GetComponentDirectionsIfDiagonal(out eEightDirection t_horiDir, out eEightDirection t_vertDir);

            if (desiredDiagonalCornerTileIndex == tileIndexThatSharesHorizontal && desiredDiagonalCornerTileIndex == tileIndexThatSharesVertical)
            {
                // They are all the in same tile.
                int t_horiAdjTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(desiredDiagonalCornerTileIndex, t_horiDir, true);
                int t_vertAdjTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(desiredDiagonalCornerTileIndex, t_vertDir, true);

                bool t_isHoriTilePassable = m_navGraph.IsTileAtIndexPassable(t_horiAdjTileIndex, true);
                bool t_isVertTilePassable = m_navGraph.IsTileAtIndexPassable(t_vertAdjTileIndex, true);

                //CustomDebug.DrawRay(transform.position, t_horiDir.ToOffset(), t_isHoriTilePassable ? Color.green : Color.black, m_shouldDrawDebug);
                //CustomDebug.DrawRay(transform.position, t_vertDir.ToOffset(), t_isVertTilePassable ? Color.red : Color.black, m_shouldDrawDebug);

                if (t_isHoriTilePassable)
                {
                    if (t_isVertTilePassable)
                    {
                        int t_diagonalAdjTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(t_horiAdjTileIndex, t_vertDir, true);
                        bool t_isDiagonalAdjTilePassable = m_navGraph.IsTileAtIndexPassable(t_diagonalAdjTileIndex, true);
                        //CustomDebug.DrawRay(transform.position, desiredDiagonalDir.ToOffset(), t_isVertTilePassable ? Color.blue : Color.black, m_shouldDrawDebug);
                        if (t_isDiagonalAdjTilePassable)
                        {
                            // All are passable, go direct.
                            return desiredDiagonalDir;
                        }
                        else
                        {
                            // Only hori and vert are passable, digonal is not.
                            // Either go horizontal or vertical.
                            if (Mathf.Abs(diffToTarget.x) > Mathf.Abs(diffToTarget.y))
                            {
                                // Move horizontal
                                return t_horiDir;
                            }
                            else
                            {
                                // Move vertical
                                return t_vertDir;
                            }
                        }
                    }
                    else
                    {
                        // Hori is passable, but vert is not.
                        return t_horiDir;
                    }
                }
                else if (t_isVertTilePassable)
                {
                    // Vert is passable, but hori is not.
                    return t_vertDir;
                }
                else
                {
                    // Neither hori nor vert is passable, move diagonal fruitlesslessly.
                    return desiredDiagonalDir;
                }
            }
            else if (tileIndexThatSharesHorizontal == desiredDiagonalCornerTileIndex)
            {
                // Horizontals are in the same tile, but the opposite horizontal point is in another tile.
                int t_horiAdjTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(desiredDiagonalCornerTileIndex, t_horiDir, true);
                int t_vertAdjTileToPositiveHoriIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(desiredDiagonalCornerTileIndex, t_vertDir, true);
                int t_vertAdjTileToNegativeHoriIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(tileIndexThatSharesVertical, t_vertDir, true);

                bool t_isHoriAdjTilePassable = m_navGraph.IsTileAtIndexPassable(t_horiAdjTileIndex, true);
                bool t_isVertAdjTileToPositiveHoriPassable = m_navGraph.IsTileAtIndexPassable(t_vertAdjTileToPositiveHoriIndex, true);
                bool t_isVertAdjTileToNegativeHoriPassable = m_navGraph.IsTileAtIndexPassable(t_vertAdjTileToNegativeHoriIndex, true);

                if (t_isHoriAdjTilePassable)
                {
                    if (t_isVertAdjTileToPositiveHoriPassable)
                    {
                        if (t_isVertAdjTileToNegativeHoriPassable)
                        {
                            // All are passable, go direct.
                            return desiredDiagonalDir;
                        }
                        else
                        {
                            // Positive hori point's vertical adj tile is unpassable, so want to go in hori.
                            return t_horiDir;
                        }
                    }
                    else if (t_isVertAdjTileToNegativeHoriPassable)
                    {
                        // Tile to hori dir is passable, tile in vert of tile that shares vertical is passable, but tile in vert of hori is not.
                        // We go hori because we know we are on the opposite hori side of the tile.
                        return t_horiDir;
                    }
                    else
                    {
                        // Only tile to hori is passable, so move hori.
                        return t_horiDir;
                    }
                }
                else if (t_isVertAdjTileToPositiveHoriPassable)
                {
                    if (t_isVertAdjTileToNegativeHoriPassable)
                    {
                        // Hori is not passable but both above tiles are.
                        // And we know that we are on the opposite hori side of the tile, so straight diagonal will put us move in the h tile in the vert dir of the hori point.
                        return desiredDiagonalDir;
                    }
                    else
                    {
                        // Only tile in the vert of the hori tile is passable, so move hori to move fully into the hori tile.
                        return t_horiDir;
                    }
                }
                else if (t_isVertAdjTileToNegativeHoriPassable)
                {
                    // Only the tile in the vert of the hori is passable, move fruitlessly in desired direction.
                    // TODO: MAYBE MOVE IN OPP HORI???
                    return desiredDiagonalDir;
                }
                else
                {
                    // Non of the tiles are passable, move fruitlessly in desired direction.
                    return desiredDiagonalDir;
                }
            }
            else if (desiredDiagonalCornerTileIndex == tileIndexThatSharesVertical)
            {
                // Tops are in the same tile, but the bottom is in another tile.
                int t_vertAdjTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(desiredDiagonalCornerTileIndex, t_vertDir, true);
                int t_horiAdjTileToDesiredDiagonalIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(desiredDiagonalCornerTileIndex, t_horiDir, true);
                int t_horiAdjTileToSharedHorizontalIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(tileIndexThatSharesHorizontal, t_horiDir, true);

                bool t_isVertTilePassable = m_navGraph.IsTileAtIndexPassable(t_vertAdjTileIndex, true);
                bool t_isHoriTileToDesiredDiagonalPointPassable = m_navGraph.IsTileAtIndexPassable(t_horiAdjTileToDesiredDiagonalIndex, true);
                bool t_isHoriTileToSharedHorizontalPointPassable = m_navGraph.IsTileAtIndexPassable(t_horiAdjTileToSharedHorizontalIndex, true);

                if (t_isVertTilePassable)
                {
                    if (t_isHoriTileToDesiredDiagonalPointPassable)
                    {
                        if (t_isHoriTileToSharedHorizontalPointPassable)
                        {
                            // All are passable, move straight there.
                            return desiredDiagonalDir;
                        }
                        else
                        {
                            // Hori tile to the shared horizontal point is not passable, move vertically
                            return t_vertDir;
                        }
                    }
                    else if (t_isHoriTileToSharedHorizontalPointPassable)
                    {
                        // Vert tile and hori tile to the neg vert tile is passable, but the hori tile to the pos vert tile is not, move fruitlessly in desired direction.
                        // TODO: MAYBE MOVE NEG VERT???
                        return desiredDiagonalDir;
                    }
                    else
                    {
                        // Vert is the only tile that is passable, both hori points are not
                        return t_vertDir;
                    }
                }
                else if (t_isHoriTileToDesiredDiagonalPointPassable)
                {
                    if (t_isHoriTileToSharedHorizontalPointPassable)
                    {
                        // Both hori points are passable but vert is not.
                        // And we know that we are on the opp vert side of the tile, so straight diagonal will put us move in the vert points hori tile.
                        return desiredDiagonalDir;
                    }
                    else
                    {
                        // Only the tile hori of the vert point is passable.
                        // Go pos vert until we get fully in the pos vert point's tile.
                        return t_vertDir;
                    }
                }
                else if (t_isHoriTileToSharedHorizontalPointPassable)
                {
                    // The only tile passable is the one to the hori of the neg vert point.
                    // Move fruitlessly in the direction we want to go.
                    return desiredDiagonalDir;
                }
                else
                {
                    // No tile is passable, so move fruitlessly in the direction we want to go.
                    return desiredDiagonalDir;
                }
            }
            else
            {
                // None are in the same tile (this means that we are in the opposite diagonal of a tile and are free to move straight diagonal).
                return desiredDiagonalDir;
            }
        }

        private bool IsAdjacentTilePassable(int fromTileIndex, eEightDirection dirOfTile) => IsAdjacentTilePassable(fromTileIndex, dirOfTile, out _);
        private bool IsAdjacentTilePassable(int fromTileIndex, eEightDirection dirOfTile, out int adjTileIndex)
        {
            adjTileIndex = m_navGraph.GetIndexOfTileInDirOfTileAtGivenIndex(fromTileIndex, dirOfTile, true);
            return m_navGraph.IsTileAtIndexPassable(adjTileIndex, true);
        }
    }
}
