using UnityEngine;

using NaughtyAttributes;
using Helpers.Extensions;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Sets the variables of the animator.
    /// </summary>
    [RequireComponent(typeof(CharacterMover))]
    public sealed class ChaserAnimator : TimedRecorder
    {
        public CharacterMover charMover { get; private set; }

        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, Required] private SpriteRenderer m_sprRend = null;
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_xSpeedFloatAnimParam = "xSpeed";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_yVelocityFloatAnimParam = "yVelocity";
        [SerializeField] private bool m_flipValueWhenMovingRight = true;

        [SerializeField, Required] private Transform m_handsParentTransform = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_sprRend, nameof(m_sprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_handsParentTransform, nameof(m_handsParentTransform), this);
            #endregion Asserts

            charMover = this.GetComponentSafe<CharacterMover>();
        }
        private void Update()
        {
            if (isRecording)
            {
                Vector2 t_velocity = charMover.velocity;
                if (m_sprRend.enabled)
                {
                    if (t_velocity.x > 0.0f)
                    {
                        m_sprRend.flipX = m_flipValueWhenMovingRight;
                    }
                    else if (t_velocity.x < 0.0f)
                    {
                        m_sprRend.flipX = !m_flipValueWhenMovingRight;
                    }
                }
                if (m_animator.enabled)
                {
                    m_animator.SetFloat(m_xSpeedFloatAnimParam, Mathf.Abs(t_velocity.x));
                    m_animator.SetFloat(m_yVelocityFloatAnimParam, t_velocity.y);
                }
            }

            if (m_sprRend.flipX)
            {
                m_handsParentTransform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                m_handsParentTransform.localScale = Vector3.one;
            }
        }
    }
}