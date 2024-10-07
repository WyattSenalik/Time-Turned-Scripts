using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;

using Timed;
using Helpers.UnityEnums;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(IMovementSuspender))]
    [RequireComponent(typeof(CharacterMover))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(HoldHandPositionController))]
    public sealed class PlayerMovement : TimedRecorder
    {
        private const bool IS_DEBUGGING = false;
        public const float MOVE_JOYSTICK_DEADZONE = 0.1f;

        public TimedVector2 timedMoveDir => m_timedMoveDir;

        [SerializeField, Min(0.0f)] private float m_speed = 2;
        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_xSpeedAnimParamName = "xSpeed";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_yVelocityAnimParamName = "yVelocity";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_walkCycleAnimParamName = "walkCycle";
        [SerializeField] private bool m_flipValueWhenMovingRight = false;

        [SerializeField, Required] private FootstepController m_footstepCont = null;

        private PlayerStateManager m_playerStateMan = null;
        private IMovementSuspender m_movementSuspender = null;
        private CharacterMover m_mover = null;
        private SpriteRenderer m_sprRend = null;
        private HoldHandPositionController m_windupHandCont = null;

        private Vector2 m_rawInput = Vector2.zero;
        private Vector2 m_fakeMoveInput = Vector2.zero;

        private TimedVector2 m_timedMoveDir = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_footstepCont, nameof(m_footstepCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            #endregion Asserts
            m_playerStateMan = this.GetComponentSafe<PlayerStateManager>();
            m_movementSuspender = this.GetIComponentSafe<IMovementSuspender>();
            m_mover = this.GetComponentSafe<CharacterMover>();
            m_sprRend = this.GetComponentSafe<SpriteRenderer>();
            m_windupHandCont = this.GetComponentSafe<HoldHandPositionController>();
        }
        private void Start()
        {
            m_timedMoveDir = new TimedVector2(Vector2.zero, eInterpolationOption.Step);
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            m_mover.velocity = Vector2.zero;

            // Don't do anything if dead
            if (m_playerStateMan.curState == ePlayerState.Dead) { return; }
            // Don't do any moving if movement is suspended
            if (m_movementSuspender.isMovementSuspended)
            {
                // Reset animator
                m_animator.SetFloat(m_xSpeedAnimParamName, 0.0f);
                m_animator.SetFloat(m_yVelocityAnimParamName, 0.0f);
                // Reset movement
                m_mover.velocity = Vector2.zero;
                if (m_movementSuspender.shouldClearMovementMemory)
                {
                    m_rawInput = Vector2.zero;
                }
                return;
            }
            // Also don't do any moving if currently throwing
            if (m_windupHandCont.isPlayingWindupAnim)
            {
                // Reset animator
                m_animator.SetFloat(m_xSpeedAnimParamName, 0.0f);
                m_animator.SetFloat(m_yVelocityAnimParamName, 0.0f);
                // Reset movement
                m_mover.velocity = Vector2.zero;
                return;
            }

            Vector2 t_moveDir = UpdateMoveDirFromInput();
            m_mover.velocity = t_moveDir * m_speed;

            // Footstep sounds
            if (m_mover.velocity != Vector2.zero)
            {
                m_footstepCont.RequestFootstep();
            }

            // Update animation
            m_animator.SetFloat(m_xSpeedAnimParamName, Mathf.Abs(m_mover.velocity.x));
            m_animator.SetFloat(m_yVelocityAnimParamName, m_mover.velocity.y);
            // We don't set walk cycle anymore cause it makes it look weird.
            //// We multiple it by 2/3s because walk animation takes 8 frames at 12 frames per second, so seconds it takes it 8/12=2/3
            //float t_walkTimeElapsed = m_animator.GetFloat(m_walkCycleAnimParamName) * 0.6666666667f;
            //t_walkTimeElapsed = t_walkTimeElapsed + deltaTime;
            //// Similarly we multiply it by 3/2s because walk animation takes 8 frames at 12 frames per second, so to normalize between [0, 1] we need to multiply by 1.5 (since original range is [0, 8/12=2/3])
            //float t_cycleOffset = (t_walkTimeElapsed * 1.5f) % 1.0f;
            //m_animator.SetFloat(m_walkCycleAnimParamName, t_cycleOffset);
            if (m_mover.velocity.x > 0)
            {
                m_sprRend.flipX = m_flipValueWhenMovingRight;
            }
            else if (m_mover.velocity.x < 0)
            {
                m_sprRend.flipX = !m_flipValueWhenMovingRight;
            }
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            m_mover.velocity = Vector2.zero;
            m_rawInput = Vector2.zero;
            m_fakeMoveInput = Vector2.zero;
            #region Logs
            //CustomDebug.LogForComponent($"Stop moving OnRecordingStop", this, IS_DEBUGGING);
            #endregion Logs
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            if (m_fakeMoveInput != Vector2.zero)
            {
                m_rawInput = m_fakeMoveInput;
            }
        }

        public Vector2 GetMostRecentNonZeroMoveDirectionBeforeTime(float time)
        {
            IReadOnlySnapshotScrapbook<Vector2Snapshot, Vector2> t_moveDirScrapbook = m_timedMoveDir.scrapbook;
            for (int i = t_moveDirScrapbook.Count - 1; i >= 0; --i)
            {
                Vector2Snapshot t_curSnap = t_moveDirScrapbook.GetSnapshotAtIndex(i);
                if (t_curSnap.time < time && t_curSnap.data != Vector2.zero)
                {
                    return t_curSnap.data;
                }
            }
            return Vector2.zero;
        }
        public Vector2 GetMostRecentNonZeroMoveDirectionBeforeCurTime()
        {
            return GetMostRecentNonZeroMoveDirectionBeforeTime(curTime);
        }

        public void FakeMoveInput(Vector2 newMoveInput)
        {
            m_rawInput = newMoveInput.normalized;
        }

        #region InputMessages
        private void OnMove(InputValue value)
        {
            m_rawInput = value.Get<Vector2>();
            //CustomDebug.LogForComponent($"Raw move input is {m_rawInput}", this, IS_DEBUGGING);
        }
        private void OnMoveFake(InputValue value)
        {
            m_fakeMoveInput = value.Get<Vector2>();
        }
        #endregion InputMessages

        private Vector2 UpdateMoveDirFromInput()
        {
            Vector2 t_moveDir;
            if (Mathf.Abs(m_rawInput.x) < 0.1f && Mathf.Abs(m_rawInput.y) < 0.1f)
            {
                t_moveDir = Vector2.zero;
            }
            else
            {
                t_moveDir = m_rawInput.ToClosestEightDirection().ToOffsetInt().ToVector2().normalized;
            }
            AddMoveDirToMemoryIfNeeded(t_moveDir);
            return t_moveDir;
        }
        private void AddMoveDirToMemoryIfNeeded(Vector2 moveDir)
        {
            if (m_timedMoveDir != null)
            {
                m_timedMoveDir.curData = moveDir;
            }
        }
    }
}
