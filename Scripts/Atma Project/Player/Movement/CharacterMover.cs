using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Helpers.UnityEnums;

using static Atma.PushableBoxCollider;
using Helpers;

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Int2DTransform))]
    public sealed class CharacterMover : MonoBehaviour
    {
        private bool IS_DEBUGGING_BOX_PUSHING = false;

        public Vector2 velocity
        {
            get => m_velocity.curData * timeMan.timeScale;
            set => m_externalVelocitySet = value;
        }
        public TimedVector2 internalVelocity => m_velocity;
        [System.Obsolete]
        public TimedVector2Int internalPosition => null;

        private GlobalTimeManager timeMan
        {
            get
            {
                if (m_timeMan == null)
                {
                    m_timeMan = GlobalTimeManager.instance;
                }
                return m_timeMan;
            }
        }

        [SerializeField, Required] private PushableBoxCollider m_boxCollider = null;
        [SerializeField] private Leaper m_leaper = null;

        [SerializeField, Range(0.0f, 1.0f)] private float m_boxPushSpeedReduce = 0.75f;
        [SerializeField] private bool m_applyDeltaTime = true;
        [SerializeField] private bool m_onlyManualUpdate = false;

        [SerializeField, Tag] private string m_cloneTag = "Clone";

        [SerializeField, BoxGroup("Debugging")] private bool m_isDebugging = false;

        private GlobalTimeManager m_timeMan = null;
        private BoxPhysicsManager m_boxPhysicsMan = null;

        private Vector2? m_externalVelocitySet = null;
        private TimedVector2 m_velocity = null;
        private Int2DTransform m_intTransform = null;

        private Vector2 m_estimatedPosition = Vector2.zero;
        private bool m_didHitBoxLastTime = false;


        private void Awake()
        {
            m_intTransform = this.GetComponentSafe<Int2DTransform>();

            m_velocity = new TimedVector2(Vector2.zero, eInterpolationOption.Step);
            Vector2 t_pos2D = transform.position;

            m_estimatedPosition = t_pos2D;

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_boxCollider, nameof(m_boxCollider), this);
            #endregion Asserts
        }
        private void Start()
        {
            if (m_timeMan == null)
            {
                m_timeMan = GlobalTimeManager.instance;
            }
            m_boxPhysicsMan = BoxPhysicsManager.instance;
        }
        private void FixedUpdate()
        {
            if (m_leaper != null)
            {
                if (m_leaper.IsLeapingAtTime(timeMan.curTime))
                {
                    ResetEstimatedPosition();
                    return;
                }
            }
            if (!m_onlyManualUpdate)
            {
                DoPhysicsUpdate();
            }
        }


        /// <summary>
        /// Returns true if something was hit during move.
        /// </summary>
        public bool DoPhysicsUpdate()
        {
            bool t_wasStopped = false;
            if (!m_velocity.isRecording)
            {
                ResetEstimatedPosition();
            }
            else
            {
                if (m_externalVelocitySet.HasValue)
                {
                    m_velocity.curData = m_externalVelocitySet.Value;
                }

                Vector2 t_curVel = m_velocity.curData;
                float t_moveDist = t_curVel.magnitude;
                if (t_moveDist == 0.0f)
                {
                    // Don't need to move since we've be moving nothing.
                    ResetEstimatedPosition();
                }
                else
                {
                    Vector2 t_moveDir = t_curVel / t_moveDist;
                    t_moveDist *= timeMan.timeScale;
                    if (m_applyDeltaTime)
                    {
                        t_moveDist *= Time.fixedDeltaTime;
                    }
                    if (m_didHitBoxLastTime)
                    {
                        t_moveDist *= m_boxPushSpeedReduce;
                    }
                    m_didHitBoxLastTime = false;
                    t_wasStopped = MoveCharacter(t_moveDir, t_moveDist);
                }

                m_externalVelocitySet = null;
            }
            return t_wasStopped;
        }
        public void ResetEstimatedPosition()
        {
            m_estimatedPosition = m_intTransform.localPositionFloat;
        }


        /// <summary>
        /// Returns if the character hit something and was stopped.
        /// </summary>
        private bool MoveCharacter(Vector2 dir, float dist)
        {
            RectangleInt t_preMoveCharRect = m_boxCollider.rectangleCollider.rectangle;
            Vector2Int t_preMoveCharPosition = m_intTransform.localPosition;

            float t_singleMoveStepDist = dist;
            int t_amIncrements = 1;
            Vector2 t_singleMoveStepVel = dir * t_singleMoveStepDist;
            float t_leftOverDist = dist % t_singleMoveStepDist;

            bool t_wasStopped = false;
            int t_incrementsElapsed = 0;
            for (int i = 1; i <= t_amIncrements; ++i)
            {
                if (!MoveCharacterSingleIncrement(dir, t_preMoveCharRect, t_preMoveCharPosition, t_singleMoveStepVel, t_singleMoveStepDist))
                {
                    // Failed to move, so stop.
                    t_wasStopped = true;
                    break;
                }
                ++t_incrementsElapsed;
            }
            if (t_wasStopped && dir.x != 0.0f && dir.y != 0.0f)
            {
                // We were stopped, so if moving diagonally, try to move only in the component parts (vertical and horizontal).
                // First try to move in the vertical.
                int t_leftOverIncrements = t_amIncrements - t_incrementsElapsed;
                Vector2 t_vertDir = new Vector2(0.0f, dir.y);
                Vector2 t_vertSingleStepMoveVel = t_vertDir * t_singleMoveStepDist;
                t_wasStopped = false;
                for (int i = 1; i <= t_leftOverIncrements; ++i)
                {
                    if (!MoveCharacterSingleIncrement(t_vertDir, t_preMoveCharRect, t_preMoveCharPosition, t_vertSingleStepMoveVel, t_singleMoveStepDist))
                    {
                        t_wasStopped = true;
                        break;
                    }
                    ++t_incrementsElapsed;
                }

                if (t_wasStopped)
                {
                    // If the vertical movement was also stopped, try to move only in the horizontal.
                    t_leftOverIncrements = t_amIncrements - t_incrementsElapsed;
                    Vector2 t_horiDir = new Vector2(dir.x, 0.0f);
                    Vector2 t_horiSingleStepMoveVel = t_horiDir * t_singleMoveStepDist;
                    t_wasStopped = false;
                    for (int i = 1; i <= t_leftOverIncrements; ++i)
                    {
                        if (!MoveCharacterSingleIncrement(t_horiDir, t_preMoveCharRect, t_preMoveCharPosition, t_horiSingleStepMoveVel, t_singleMoveStepDist))
                        {
                            t_wasStopped = true;
                            break;
                        }
                    }
                }
            }

            // If we were not stopped, move the left over distance.
            if (!t_wasStopped)
            {
                if (t_leftOverDist > 0.0f)
                {
                    MoveCharacterSingleIncrement(dir, t_preMoveCharRect, t_preMoveCharPosition, dir * t_leftOverDist, t_leftOverDist);
                }
            }
            return t_wasStopped;
        }
        private bool MoveCharacterSingleIncrement(Vector2 dir, RectangleInt preMoveCharRect, Vector2Int preMoveCharPosition, Vector2 singleMoveStepVel, float singleMoveStepDist)
        {
            Vector2 t_curIncrFutureEstPosition = m_estimatedPosition + singleMoveStepVel;
            Vector2Int t_curIncrFutureEstPositionInt = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(t_curIncrFutureEstPosition);

            if (t_curIncrFutureEstPositionInt == m_intTransform.localPosition)
            {
                // Increment position did not increase enough to enter the next position, no need to check for hitting anything.
                m_estimatedPosition = t_curIncrFutureEstPosition;
                m_intTransform.localPosition = t_curIncrFutureEstPositionInt;
                return true;
            }

            Vector2Int t_curIncrMoveDiff = t_curIncrFutureEstPositionInt - preMoveCharPosition;
            RectangleInt t_curIncrFutureMoveCharRect = new RectangleInt(preMoveCharRect.botLeftPoint + t_curIncrMoveDiff, preMoveCharRect.size);

            // Look for overlap
            if (m_boxPhysicsMan.DoesRectangleOverlapWalls(t_curIncrFutureMoveCharRect))
            {
                // Hit wall, next increment is bad, stop incrementing.
                return false;
            }
            else
            {
                List<PushableBoxCollider> t_hitBoxes = m_boxPhysicsMan.GetBoxesRectangleOverlapsIgnoreEdgeTouch(t_curIncrFutureMoveCharRect);
                if (t_hitBoxes.Count > 0)
                {
                    m_didHitBoxLastTime = true;
                    bool t_didHitUnmovingBox = true;
                    foreach (PushableBoxCollider t_singleBox in t_hitBoxes)
                    {
                        m_boxCollider.InvokeCollisionEvent(new CollisionInfo(t_singleBox, dir));
                        t_singleBox.InvokeCollisionEvent(new CollisionInfo(m_boxCollider, dir));
                        if (!MoveBox(t_singleBox, dir, singleMoveStepDist, t_curIncrFutureMoveCharRect, m_boxCollider, new List<PushableBoxCollider>()))
                        {
                            // Failed to move the box, can't move.
                            t_didHitUnmovingBox = false;
                            return false;
                        }
                    }
                    if (!t_didHitUnmovingBox)
                    {
                        // Hit an un moving box, so we need to stop.
                        return false;
                    }
                }
            }

            // Made it to the end, we are good to increment.
            m_estimatedPosition = t_curIncrFutureEstPosition;
            m_intTransform.localPosition = t_curIncrFutureEstPositionInt;
            return true;
        }
        private bool MoveBox(PushableBoxCollider box, Vector2 dir, float dist, RectangleInt pusherPostMoveRect, PushableBoxCollider pusherColToIgnore, List<PushableBoxCollider> boxCollidersToIgnore)
        {
            RectangleInt t_boxPreMoveRect = box.rectangleCollider.rectangle;

            bool t_hasValidMoveDir = DetermineValidBoxMoveDirection(dir, dist, pusherPostMoveRect, t_boxPreMoveRect, out eEightDirection t_moveDir);
            if (!t_hasValidMoveDir)
            {
                // Box can't move.
                return false;
            }
            Vector2Int t_boxPostMoveBotLeft = DetermineDesiredBoxBotLeftPosition(pusherPostMoveRect, t_boxPreMoveRect, t_moveDir);

            RectangleInt t_boxPostMoveRect = new RectangleInt(t_boxPostMoveBotLeft, t_boxPreMoveRect.size);
            if (m_boxPhysicsMan.DoesRectangleOverlapWalls(t_boxPostMoveRect))
            {
                // Hit a wall, can't move the box.
                return false;
            }
            else if (m_boxPhysicsMan.DoesCheckBoxRectangleOverlapPushersAndHandleHitBlinkingClones(box, t_boxPostMoveRect, pusherColToIgnore))
            {
                // Hit a pusher, can't move the box.
                return false;
            }
            else
            {
                boxCollidersToIgnore.Add(box);
                List<PushableBoxCollider> t_hitBoxes = m_boxPhysicsMan.GetBoxesRectangleOverlapsIgnoreEdgeTouch(t_boxPostMoveRect, boxCollidersToIgnore);
                if (t_hitBoxes.Count > 0)
                {
                    // Hit some boxes, see if any of them will stop this box.
                    foreach (PushableBoxCollider t_singleBoxHit in t_hitBoxes)
                    {
                        if (!MoveBox(t_singleBoxHit, dir, dist, t_boxPostMoveRect, pusherColToIgnore, boxCollidersToIgnore))
                        {
                            // Hit an unmovable box, stop.
                            return false;
                        }
                    }

                    // Boxes hit, but they can move, so its okay.
                }
                else
                {
                    // Nothing hit, allow the boxes pusher to move
                }
                Vector2Int t_diffToBoxColCenterFromBotLeft = box.rootTransform.localPosition - t_boxPreMoveRect.botLeftPoint;
                box.rootTransform.localPosition = t_boxPostMoveBotLeft + t_diffToBoxColCenterFromBotLeft;
                return true;
            }
        }

        private bool DetermineValidBoxMoveDirection(Vector2 dir, float dist, RectangleInt pusherPostMoveRect, RectangleInt boxRect, out eEightDirection validDir)
        {
            Vector2Int t_moveVector = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(dir * dist);
            RectangleInt t_preMovePusherRect = new RectangleInt(pusherPostMoveRect.botLeftPoint - t_moveVector, pusherPostMoveRect.size);
            eEightDirection t_eightDir = dir.ToClosestEightDirection();

            int t_boxBot = boxRect.min.y;
            int t_boxTop = boxRect.max.y;
            int t_boxLeft = boxRect.min.x;
            int t_boxRight = boxRect.max.x;

            int t_preMovePusherBot = t_preMovePusherRect.min.y;
            int t_preMovePusherTop = t_preMovePusherRect.max.y;
            int t_preMovePusherLeft = t_preMovePusherRect.min.x;
            int t_preMovePusherRight = t_preMovePusherRect.max.x;

            int t_postMovePusherBot = pusherPostMoveRect.min.y;
            int t_postMovePusherTop = pusherPostMoveRect.max.y;
            int t_postMovePusherLeft = pusherPostMoveRect.min.x;
            int t_postMovePusherRight = pusherPostMoveRect.max.x;


            bool t_isPreMoveBelowBoxBot = t_preMovePusherTop <= t_boxBot;
            bool t_isPostMoveAboveBoxBot = t_postMovePusherTop > t_boxBot;

            bool t_isPreMoveAboveBoxTop = t_preMovePusherBot >= t_boxTop;
            bool t_isPostMoveBelowBoxTop = t_postMovePusherBot < t_boxTop;

            bool t_isPreMoveRightOfBoxRight = t_preMovePusherLeft >= t_boxRight;
            bool t_isPostMoveLeftOfBoxRight = t_postMovePusherLeft < t_boxRight;

            bool t_isPreMoveLeftOfBoxLeft = t_preMovePusherRight <= t_boxLeft;
            bool t_isPostMoveRightOfBoxLeft = t_postMovePusherRight > t_boxLeft;

            #region Logs
            //CustomDebug.Log($"[{Time.time}] (<color={(t_isPreMoveBelowBoxBot ? "green" : "red")}>{nameof(t_isPreMoveBelowBoxBot)}</color>, <color={(t_isPostMoveAboveBoxBot ? "green" : "red")}>{nameof(t_isPostMoveAboveBoxBot)}</color>); (<color={(t_isPreMoveAboveBoxTop ? "green" : "red")}>{nameof(t_isPreMoveAboveBoxTop)}</color>, <color={(t_isPostMoveBelowBoxTop ? "green" : "red")}>{nameof(t_isPostMoveBelowBoxTop)}</color>); (<color={(t_isPreMoveRightOfBoxRight ? "green" : "red")}>{nameof(t_isPreMoveRightOfBoxRight)}</color>, <color={(t_isPostMoveLeftOfBoxRight ? "green" : "red")}>{nameof(t_isPostMoveLeftOfBoxRight)}</color>); (<color={(t_isPreMoveLeftOfBoxLeft ? "green" : "red")}>{nameof(t_isPreMoveLeftOfBoxLeft)}</color>, <color={(t_isPostMoveRightOfBoxLeft ? "green" : "red")}>{nameof(t_isPostMoveRightOfBoxLeft)}</color>)\nDir={dir}. Dist={dist}.", IS_DEBUGGING_BOX_PUSHING);
            #endregion Logs

            // Pusher Top hits Box Bot (potentially move up)
            if (t_isPreMoveBelowBoxBot && t_isPostMoveAboveBoxBot)
            {
                #region Logs
                //CustomDebug.Log($"[{Time.time}] Pusher Top hit Box Bot", IS_DEBUGGING_BOX_PUSHING);
                #endregion Logs
                validDir = eEightDirection.Up;
                switch (t_eightDir)
                {
                    case eEightDirection.Up: return true;
                    case eEightDirection.LeftUp: return true;
                    case eEightDirection.RightUp: return true;
                    default: return false;
                }
            }
            // Pusher Bot hits Box Top (potentially move down)
            else if (t_isPreMoveAboveBoxTop && t_isPostMoveBelowBoxTop)
            {
                #region Logs
                //CustomDebug.Log($"[{Time.time}] Pusher Bot hit Box Top", IS_DEBUGGING_BOX_PUSHING);
                #endregion Logs
                validDir = eEightDirection.Down;
                switch (t_eightDir)
                {
                    case eEightDirection.Down: return true;
                    case eEightDirection.LeftDown: return true;
                    case eEightDirection.RightDown: return true;
                    default: return false;
                }
            }
            // Pusher Left hits Box Right (potentially move left)
            else if (t_isPreMoveRightOfBoxRight && t_isPostMoveLeftOfBoxRight)
            {
                #region Logs
                //CustomDebug.Log($"[{Time.time}] Pusher Left hit Box Right", IS_DEBUGGING_BOX_PUSHING);
                #endregion Logs
                validDir = eEightDirection.Left;
                switch (t_eightDir)
                {
                    case eEightDirection.Left: return true;
                    case eEightDirection.LeftUp: return true;
                    case eEightDirection.LeftDown: return true;
                    default: return false;
                }
            }
            // Pusher Right hits Box Left (potentially move right)
            else if (t_isPreMoveLeftOfBoxLeft && t_isPostMoveRightOfBoxLeft)
            {
                #region Logs
                //CustomDebug.Log($"[{Time.time}] Pusher Right hit Box Left", IS_DEBUGGING_BOX_PUSHING);
                #endregion Logs
                validDir = eEightDirection.Right;
                switch (t_eightDir)
                {
                    case eEightDirection.Right: return true;
                    case eEightDirection.RightUp: return true;
                    case eEightDirection.RightDown: return true;
                    default: return false;
                }
            }
            else
            {
                #region Logs
                //CustomDebug.Log($"[{Time.time}] No valid dir found", IS_DEBUGGING_BOX_PUSHING);
                #endregion Logs
                validDir = t_eightDir;

                //pusherPostMoveRect.DrawOutlineDebug(Color.blue, IS_DEBUGGING_BOX_PUSHING);
                //t_preMovePusherRect.DrawOutlineDebug(Color.cyan, IS_DEBUGGING_BOX_PUSHING);
                //pusherPostMoveRect.DrawInsideDebug(Color.blue, 7, IS_DEBUGGING_BOX_PUSHING);
                //boxRect.DrawOutlineDebug(Color.yellow, IS_DEBUGGING_BOX_PUSHING);
                return false;
            }
        }
        private Vector2Int DetermineDesiredBoxBotLeftPosition(RectangleInt pusherRect, RectangleInt boxRect, eEightDirection boxMoveDir)
        {
            switch (boxMoveDir)
            {
                case eEightDirection.Right:
                {
                    // Want the botLeft point of the box to be at the right of the pusher
                    return new Vector2Int(pusherRect.botRightPoint.x, boxRect.botLeftPoint.y);
                }
                case eEightDirection.Left:
                {
                    // Want the botRight point of the box to be at the left of the pusher
                    Vector2Int t_desiredBoxBotRight = new Vector2Int(pusherRect.botLeftPoint.x, boxRect.botLeftPoint.y);
                    return new Vector2Int(t_desiredBoxBotRight.x - boxRect.width, t_desiredBoxBotRight.y);
                }
                case eEightDirection.Up:
                {
                    // Want the botLeft point of the box to be at the top of the pusher
                    return new Vector2Int(boxRect.botLeftPoint.x, pusherRect.topLeftPoint.y);
                }
                case eEightDirection.Down:
                {
                    // Want the topLeft point of the box to be at the bottom of the pusher
                    Vector2Int t_desiredBoxTopLeft = new Vector2Int(boxRect.botLeftPoint.x, pusherRect.botLeftPoint.y);
                    return new Vector2Int(t_desiredBoxTopLeft.x, t_desiredBoxTopLeft.y - boxRect.height);
                }
                default:
                {
                    CustomDebug.UnhandledEnum(boxMoveDir, this);
                    return Vector2Int.zero;
                }
            }
        }
    }
}