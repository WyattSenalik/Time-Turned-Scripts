using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class TimedNPCMover : TimedRecorder
    {
        public bool isMoving
        {
            get
            {
                Vector2 t_pos2D = transform.position;
                return t_pos2D != curTargetPos;
            }
        }
        public Vector2 curTargetPos => m_timedTargetPos.curData;

        [SerializeField, Required] private RunCurvesSO m_runCurves = null;
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
        [SerializeField, Min(1)] private int m_riemannSumRectangles = 100;

        [SerializeField, Required] private FootstepController m_footstepCont = null;

        [SerializeField, ReadOnly, BoxGroup("Debugging")] private Vector2 m_debugTargetPos = Vector2.zero;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private Vector2 m_debugPosAtStartOfMove = Vector2.zero;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private float m_debugXVel = 0.0f;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private float m_debugYVel = 0.0f;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private float m_debugSpeed = 0.0f;

        private TimedVector2 m_timedTargetPos = null;
        private TimedVector2 m_timedPosAtStartOfMove = null;

        private TimedFloat m_timedXVel = null;
        private TimedFloat m_timedYVel = null;
        private TimedFloat m_timedSpeed = null;

        private float m_accelDist = 0.0f;
        private float m_decelDist = 0.0f;

        private bool m_initialized = false;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_runCurves, nameof(m_runCurves), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_footstepCont, nameof(m_footstepCont), this);
            #endregion Asserts
            m_accelDist = m_runCurves.ApproximateDistanceAccelerationTravels(m_riemannSumRectangles);
            m_decelDist = m_runCurves.ApproximateDistanceDeclerationTravels(m_riemannSumRectangles);
        }
        private void Start()
        {
            m_timedTargetPos = new TimedVector2(transform.position, eInterpolationOption.Step);
            m_timedPosAtStartOfMove = new TimedVector2(transform.position, eInterpolationOption.Step);
            m_timedXVel = new TimedFloat(0.0f, eInterpolationOption.Linear);
            m_timedYVel = new TimedFloat(0.0f, eInterpolationOption.Linear);
            m_timedSpeed = new TimedFloat(0.0f, eInterpolationOption.Linear);

            m_initialized = true;
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (!m_initialized) { return; }

            m_debugTargetPos = m_timedTargetPos.curData;
            m_debugPosAtStartOfMove = m_timedPosAtStartOfMove.curData;
            m_debugXVel = m_timedXVel.curData;
            m_debugYVel = m_timedYVel.curData;
            m_debugSpeed = m_timedSpeed.curData;

            if (isRecording)
            {
                // Move towards target position
                MoveTowardsTarget();
                // Update the animator
                m_animator.SetFloat(m_xVelAnimParamName, m_timedXVel.curData);
                m_animator.SetFloat(m_yVelAnimParamName, m_timedYVel.curData);
                m_animator.SetFloat(m_speedAnimParamName, m_timedSpeed.curData);
            }
        }


        public void SetTargetPosition(Vector2 targetPos)
        {
            if (m_timedTargetPos.isRecording)
            {
                if (m_timedTargetPos.curData != targetPos)
                {
                    m_timedTargetPos.curData = targetPos;
                    m_timedPosAtStartOfMove.curData = transform.position;
                }
            }
        }
        public void PlayForwardIdle()
        {
            m_animator.SetTrigger(m_faceForwardTriggerAnimParamName);
        }
        public void PlayBackwardIdle()
        {
            m_animator.SetTrigger(m_faceBackwardTriggerAnimParamName);
        }


        private void MoveTowardsTarget()
        {
            Vector2 t_startPos = m_timedPosAtStartOfMove.curData;
            Vector2 t_endPos = m_timedTargetPos.curData;
            Vector2 t_curPos = transform.position;
            if (t_startPos == t_endPos || t_curPos == t_endPos)
            {
                ResetAnimatorValues();
                return;
            }
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
            Vector2 t_diff = t_endPos - t_startPos;
            float t_dist = t_diff.magnitude;
            float t_topSpeedDist = t_dist - m_accelDist - m_decelDist;
            float t_topSpeedTimeToTake = t_topSpeedDist / m_runCurves.topSpeed;

            Vector2 t_moveDir = t_diff.normalized;
            float t_moveStartTime = GetMostRecentMoveTime();

            // Part 1: Accel/topspeed (non-decel)
            float t_part1StartTime = t_moveStartTime;
            float t_part1FinishTime = t_part1StartTime + m_runCurves.accelTime + t_topSpeedTimeToTake;
            if (curTime >= t_part1StartTime && curTime < t_part1FinishTime)
            {
                float t_speed = m_runCurves.EvaluateAccelMove(t_part1StartTime, curTime);
                MoveInDirection(t_moveDir, t_speed, t_endPos);
            }
            else
            {
                // Part 2: Decel
                float t_part2StartTime = t_part1FinishTime;
                float t_part2FinishTime = t_part2StartTime + m_runCurves.decelTime;
                if (curTime >= t_part2StartTime && curTime < t_part2FinishTime)
                {
                    float t_speed = m_runCurves.EvaluateDecel(t_part2StartTime, curTime);
                    MoveInDirection(t_moveDir, t_speed, t_endPos);
                }
                else if (curTime >= t_part2FinishTime)
                {
                    // Done moving.
                    FinishMoving(t_endPos);
                }
            }
        }
        private void MoveInDirection(Vector2 normalizedDir, float speed, Vector2 endPos)
        {
            Vector2 t_pos2D = transform.position;
            Vector2 t_velocity = normalizedDir * speed;
            Vector2 t_afterMovePos = t_pos2D + t_velocity * deltaTime;
            Vector2 t_afterMoveDiff = endPos - t_afterMovePos;
            if (Vector2.Dot(t_afterMoveDiff, normalizedDir) <= 0.0f)
            {
                // Would overshoot the position with the last move.
                transform.position = endPos;
            }
            else
            {
                // Normally move
                transform.position = t_afterMovePos;
            }

            m_timedXVel.curData = t_velocity.x;
            m_timedYVel.curData = t_velocity.y;
            m_timedSpeed.curData = speed / m_runCurves.topSpeed;

            m_footstepCont.RequestFootstep();
        }
        private void FinishMoving(Vector2 endPos)
        {
            transform.position = endPos;
            ResetAnimatorValues();
        }
        private float GetMostRecentMoveTime()
        {
            if (m_timedTargetPos.scrapbook.Count == 0)
            {
                return -1.0f;
            }
            return m_timedTargetPos.scrapbook.GetLatestTime();
        }
        private void ResetAnimatorValues()
        {
            m_timedXVel.curData = 0.0f;
            m_timedYVel.curData = 0.0f;
            m_timedSpeed.curData = 0.0f;
        }
    }
}