using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using Atma.Dialogue;
using Helpers;
using Helpers.Animation;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    [DisallowMultipleComponent]
    public sealed class SrAgentMover : MonoBehaviour
    {
        [SerializeField, Required] private RunCurvesSO m_runCurves = null;
        [SerializeField, Required] private SpriteRenderer m_sprRenderer = null;
        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_xVelAnimParamName = "xVelocity";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_yVelAnimParamName = "yVelocity";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_speedAnimParamName = "speed";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_teleportAnimParamName = "teleport";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_faceForwardTriggerAnimParamName = "faceForward";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_faceBackwardTriggerAnimParamName = "faceBackward";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_epicTurnTriggerAnimParamName = "epicTurnAround";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_removeDisguiseAnimParamName = "removeDisguise";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_spinAnimParamName = "spin";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_faceRightAnimParamName = "faceRight";
        [SerializeField, Min(1)] private int m_riemannSumRectangles = 100;

        [SerializeField] private MoveAnim[] m_moveToPositionsAnims = new MoveAnim[1];

        [SerializeField, Required] private FootstepController m_footstepCont = null;

        private float m_accelDist = 0.0f;
        private float m_decelDist = 0.0f;

        private bool m_isMoveAnimCoroutActive = false;
        private Coroutine m_moveAnimCorout = null;

        private float m_xVel = 0.0f;
        private float m_yVel = 0.0f;
        private float m_speed = 0.0f;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_footstepCont, nameof(m_footstepCont), this);
            #endregion Asserts
            m_accelDist = m_runCurves.ApproximateDistanceAccelerationTravels(m_riemannSumRectangles);
            m_decelDist = m_runCurves.ApproximateDistanceDeclerationTravels(m_riemannSumRectangles);
        }
        private void FixedUpdate()
        {
            if (m_isMoveAnimCoroutActive)
            {
                m_animator.SetFloat(m_xVelAnimParamName, m_xVel);
                m_animator.SetFloat(m_yVelAnimParamName, m_yVel);
                m_animator.SetFloat(m_speedAnimParamName, m_speed);
            }
            else
            {
                m_animator.SetFloat(m_xVelAnimParamName, 0.0f);
                m_animator.SetFloat(m_yVelAnimParamName, 0.0f);
                m_animator.SetFloat(m_speedAnimParamName, 0.0f);
            }
        }


        public void PlayMoveAnimation(int index) => PlayMoveAnimation(index, null);
        public void PlayMoveAnimation(int index, Action onFinished)
        {
            #region Logs
            if (index < 0 || index > m_moveToPositionsAnims.Length)
            {
                //CustomDebug.LogErrorForComponent($"Index ({index}) out of range for PlayMoveAnimation [0, {m_moveToPositionsAnims.Length}).", this);
            }
            #endregion Logs

            bool t_shouldSkipDialogue = ConversationSkipper.instance.ShouldSkipDialogue();
            if (t_shouldSkipDialogue)
            {
                MoveAnim t_moveAnim = m_moveToPositionsAnims[index];
                transform.position = t_moveAnim.moveToPositions[^1];
                onFinished?.Invoke();

                CoroutineSingleton.instance.InvokeAfterWaitForFixedUpdateDelay(t_moveAnim.InvokeOnFinishedEvent);
                if (t_moveAnim.faceForwardAfter)
                {
                    CoroutineSingleton.instance.InvokeAfterWaitForFixedUpdateDelay(() =>
                    {
                        m_animator.SetTrigger(m_faceForwardTriggerAnimParamName);
                    });
                }
                else if (t_moveAnim.faceBackwardAfter)
                {
                    CoroutineSingleton.instance.InvokeAfterWaitForFixedUpdateDelay(() =>
                    {
                        m_animator.SetTrigger(m_faceBackwardTriggerAnimParamName);
                    });
                }

                return;
            }
            
            if (m_isMoveAnimCoroutActive)
            {
                StopCoroutine(m_moveAnimCorout);
            }
            if (!t_shouldSkipDialogue)
            {
                m_moveAnimCorout = StartCoroutine(MoveAnimationCoroutine(index, onFinished));
            }
        }
        public void PlayMoveAnimAsLerpedWithoutAnimations(int index)
        {
            bool t_shouldSkipDialogue = ConversationSkipper.instance.ShouldSkipDialogue();
            if (t_shouldSkipDialogue)
            {
                MoveAnim t_moveAnim = m_moveToPositionsAnims[index];
                transform.position = t_moveAnim.moveToPositions[^1];

                CoroutineSingleton.instance.InvokeAfterWaitForFixedUpdateDelay(t_moveAnim.InvokeOnFinishedEvent);
                if (t_moveAnim.faceForwardAfter)
                {
                    CoroutineSingleton.instance.InvokeAfterWaitForFixedUpdateDelay(() =>
                    {
                        m_animator.SetTrigger(m_faceForwardTriggerAnimParamName);
                    });
                }
                else if (t_moveAnim.faceBackwardAfter)
                {
                    CoroutineSingleton.instance.InvokeAfterWaitForFixedUpdateDelay(() =>
                    {
                        m_animator.SetTrigger(m_faceBackwardTriggerAnimParamName);
                    });
                }

                return;
            }

            if (m_isMoveAnimCoroutActive)
            {
                StopCoroutine(m_moveAnimCorout);
            }
            if (!t_shouldSkipDialogue)
            {
                m_moveAnimCorout = StartCoroutine(MoveAnimationAsLerpedCoroutine(index));
            }
        }

        public void PlayTeleportLeave()
        {
            m_animator.SetTrigger(m_teleportAnimParamName);
        }
        public void PlayForwardIdle()
        {
            m_animator.SetTrigger(m_faceForwardTriggerAnimParamName);
        }
        public void PlayBackwardIdle()
        {
            m_animator.SetTrigger(m_faceBackwardTriggerAnimParamName);
        }
        public void PlayEpicTurn()
        {
            m_animator.SetTrigger(m_epicTurnTriggerAnimParamName);
        }
        public void PlayRemoveDisguise()
        {
            m_animator.SetTrigger(m_removeDisguiseAnimParamName);
        }
        public void PlaySpin()
        {
            m_animator.SetTrigger(m_spinAnimParamName);
        }
        public void PlayRightIdle()
        {
            m_animator.SetTrigger(m_faceRightAnimParamName);
        }


        private IEnumerator MoveAnimationCoroutine(int index, Action onFinished)
        {
            m_isMoveAnimCoroutActive = true;
            MoveAnim t_moveAnim = m_moveToPositionsAnims[index];
            IReadOnlyList<Vector2> t_moveToPositions = t_moveAnim.moveToPositions;

            for (int i = 0; i < t_moveToPositions.Count; ++i)
            {
                // Want the move animation to accelerate, coast at top speed, then decelerate over a fixed distance.
                // We know how long accelerating and decelerating will take and how far (distance-wise) they will move this transform.
                // We don't know how long we will be moving at top speed, which we need for the while loop's condition.
                // We need accelDist + decelDist + topSpeedDist = distWeWantToMove.
                // Rearranging this we can solve for the unknown (assuming accelDist + decelDist <= distWeWantToMove):
                //      topSpeedDist  = distWeWantToMove - accelDist - decelDist
                // Once we know how the distance we will move at top speed, we can figure out how long the whole thing should take using:
                //      topSpeedTime = topSpeedDist / topSpee,
                //      firstHalfTime = accelTime + topSpeedTime, and
                //      secondHalfTime = decelTime
                Vector2 t_startPos = transform.position;
                Vector2 t_endPos = t_moveToPositions[i];
                Vector2 t_diff = t_endPos - t_startPos;
                float t_dist = t_diff.magnitude;
                float t_topSpeedDist = t_dist - m_accelDist - m_decelDist;
                float t_topSpeedTimeToTake = t_topSpeedDist / m_runCurves.topSpeed;

                Vector2 t_moveDir = t_diff.normalized;
                float t_curTime = Time.time;
                // Part 1: Accel/topspeed (non-decel)
                float t_part1StartTime = Time.time;
                float t_part1FinishTime = t_part1StartTime + m_runCurves.accelTime + t_topSpeedTimeToTake;
                while (t_curTime < t_part1FinishTime)
                {
                    float t_speed = m_runCurves.EvaluateAccelMove(t_part1StartTime, t_curTime);
                    MoveInDirection(t_moveDir, t_speed, t_endPos);
                    yield return new WaitForFixedUpdate();

                    t_curTime = Time.time;
                }
                // Part 2: Decel
                float t_part2StartTime = Time.time;
                float t_part2FinishTime = t_part2StartTime + m_runCurves.decelTime;
                while (t_curTime < t_part2FinishTime)
                {
                    float t_speed = m_runCurves.EvaluateDecel(t_part2StartTime, t_curTime);
                    MoveInDirection(t_moveDir, t_speed, t_endPos);
                    yield return new WaitForFixedUpdate();

                    t_curTime = Time.time;
                }

                StopMoving(t_endPos);
            }

            if (t_moveAnim.faceForwardAfter)
            {
                m_animator.SetTrigger(m_faceForwardTriggerAnimParamName);
                yield return new WaitForFixedUpdate();
            }
            else if (t_moveAnim.faceBackwardAfter)
            {
                m_animator.SetTrigger(m_faceBackwardTriggerAnimParamName);
                yield return new WaitForFixedUpdate();
            }

            onFinished?.Invoke();
            t_moveAnim.InvokeOnFinishedEvent();

            m_isMoveAnimCoroutActive = false;
        }
        private void MoveInDirection(Vector2 normalizedDir, float speed, Vector2 endPos)
        {
            Vector2 t_pos2D = transform.position;
            Vector2 t_velocity = normalizedDir * speed;
            Vector2 t_afterMovePos = t_pos2D + t_velocity * Time.deltaTime;
            Vector2 t_afterMoveDiff = endPos - t_afterMovePos;
            if (Vector2.Dot(t_afterMoveDiff, normalizedDir) <= 0.0f)
            {
                // Overshot the position with the last move.
                transform.position = endPos;
            }
            else
            {
                // Normally move
                transform.position = t_afterMovePos;
            }

            m_xVel = t_velocity.x;
            m_yVel = t_velocity.y;
            m_speed = speed / m_runCurves.topSpeed;

            // We no longer flip because we have walk right and walk left animations
            //if (t_velocity.x < 0.0f)
            //{
            //    m_sprRenderer.flipX = true;
            //}
            //else if (t_velocity.x > 0.0f)
            //{
            //    m_sprRenderer.flipX = false;
            //}

            m_footstepCont.RequestFootstep();
        }
        private void StopMoving(Vector2 finalPos)
        {
            transform.position = finalPos;

            m_speed = 0.0f;
            m_yVel = 0.0f;
            m_speed = 0.0f;
        }
        private IEnumerator MoveAnimationAsLerpedCoroutine(int index)
        {
            m_isMoveAnimCoroutActive = true;
            MoveAnim t_moveAnim = m_moveToPositionsAnims[index];
            IReadOnlyList<Vector2> t_moveToPositions = t_moveAnim.moveToPositions;

            for (int i = 0; i < t_moveToPositions.Count; ++i)
            {
                Vector2 t_startPos = transform.position;
                Vector2 t_endPos = t_moveToPositions[i];
                Vector2 t_diff = t_endPos - t_startPos;
                float t_dist = t_diff.magnitude;
                float t_timeToTake = t_dist / m_runCurves.topSpeed;
                float t_startTime = Time.time;
                float t_curTime = t_startTime;
                // Part 1: Accel/topspeed (non-decel)
                while (t_curTime < t_timeToTake)
                {
                    float t = (t_curTime - t_startTime) / t_timeToTake;
                    transform.position = Vector2.Lerp(t_startPos, t_endPos, t);
                    yield return new WaitForFixedUpdate();

                    t_curTime = Time.time;
                }

                StopMoving(t_endPos);
            }

            if (t_moveAnim.faceForwardAfter)
            {
                m_animator.SetTrigger(m_faceForwardTriggerAnimParamName);
                yield return new WaitForFixedUpdate();
            }
            else if (t_moveAnim.faceBackwardAfter)
            {
                m_animator.SetTrigger(m_faceBackwardTriggerAnimParamName);
                yield return new WaitForFixedUpdate();
            }

            t_moveAnim.InvokeOnFinishedEvent();

            m_isMoveAnimCoroutActive = false;
        }


        [Serializable]
        public sealed class MoveAnim
        {
            public IReadOnlyList<Vector2> moveToPositions => m_moveToPositions;
            public bool faceForwardAfter => m_faceForwardAfter;
            public bool faceBackwardAfter => m_faceBackwardAfter;

            [SerializeField] private Vector2[] m_moveToPositions = new Vector2[1];
            [SerializeField] private UnityEvent m_onFinished = new UnityEvent();
            [SerializeField] private UnityEvent m_onFinishedAfterDelay = new UnityEvent();
            [SerializeField] private bool m_faceForwardAfter = false;
            [SerializeField] private bool m_faceBackwardAfter = false;

            public MoveAnim(params Vector2[] positions)
            {
                m_moveToPositions = positions;
            }


            public void InvokeOnFinishedEvent()
            {
                m_onFinished.Invoke();

                CoroutineSingleton.instance.StartCoroutine(InvokeOnFinishedEventAfterDelay());
            }

            private IEnumerator InvokeOnFinishedEventAfterDelay()
            {
                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
                m_onFinishedAfterDelay.Invoke();
            }
        }
    }
}