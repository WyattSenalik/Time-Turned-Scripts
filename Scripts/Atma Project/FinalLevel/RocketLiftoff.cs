using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation;
using Helpers.Animation.BetterCurve;
using Timed;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class RocketLiftoff : TimedRecorder
    {
        public float stage3BeginTime => m_timeToBeginStage3;

        [SerializeField, Required] private RocketSounds m_rocketSounds = null; 
        [SerializeField, Required] private SpriteRenderer m_stage1SprRend1 = null;
        [SerializeField, Required] private SpriteRenderer m_stage1SprRend2 = null;
        [SerializeField, Required] private Transform m_puffsOffsetTrans = null;
        [SerializeField, Required] private SpriteRenderer m_rocketShadow = null;
        [SerializeField] private ManualSpriteAnimation m_stage1Anim = new ManualSpriteAnimation();
        [SerializeField] private Vector2 m_restingPos = new Vector2(-7.0f, 10.0f);

        [SerializeField, BoxGroup("Stage1")] private float m_timeToBeginStage1 = 0.125f;
        [SerializeField, BoxGroup("Stage1")] private float m_offsetBetweenInstancesStage1 = 0.0f;

        [SerializeField, BoxGroup("Stage2")] private float m_timeToBeginStage2 = 7.5f;
        [SerializeField, BoxGroup("Stage2")] private BetterCurve m_horizontalJitterCurve = new BetterCurve();
        [SerializeField, BoxGroup("Stage2")] private float m_offsetBetweenInstancesStage2 = 0.5f;

        [SerializeField, BoxGroup("Stage3")] private float m_timeToBeginStage3 = 15.0f;
        [SerializeField, BoxGroup("Stage3")] private SpriteRenderer m_stage3SprRend = null;
        [SerializeField, BoxGroup("Stage3")] private SpriteRenderer m_stage3PuffSprRend1 = null;
        [SerializeField, BoxGroup("Stage3")] private SpriteRenderer m_stage3PuffSprRend2 = null;
        [SerializeField, BoxGroup("Stage3")] private float m_offsetBetweenInstancesStage3 = 0.5f;
        [SerializeField, BoxGroup("Stage3")] private float m_timeToStopPuffsStage3 = 17.5f;
        [SerializeField, BoxGroup("Stage3")] private BetterCurve m_stage3HorizontalJitterCurve = new BetterCurve();
        [SerializeField, BoxGroup("Stage3")] private BetterCurve m_stage3VerticalCurve = new BetterCurve();
        [SerializeField, BoxGroup("Stage3")] private BetterCurve m_shadowFadeoutCurve = new BetterCurve();
        [SerializeField, BoxGroup("Stage3")] private ManualSpriteAnimation m_stage3Anim = new ManualSpriteAnimation();
        [SerializeField, BoxGroup("Stage3")] private ManualSpriteAnimation m_stage3PuffsAnim = new ManualSpriteAnimation();

        [SerializeField, ReadOnly] private eLiftoffStage m_lifoffStage = eLiftoffStage.None;

        private float m_prevTime = 0.0f;
        private float m_shadowYPos = 0.0f;
        private float m_stage3PuffsYPos = 0.0f;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rocketSounds, nameof(m_rocketSounds), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stage1SprRend1, nameof(m_stage1SprRend1), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stage1SprRend2, nameof(m_stage1SprRend2), this);
            #endregion Asserts

            m_shadowYPos = m_rocketShadow.transform.position.y;
            m_stage3PuffsYPos = m_stage3PuffSprRend1.transform.position.y;
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            TimeFrame t_noneFrame = new TimeFrame(0.0f, m_timeToBeginStage1);
            TimeFrame t_stage1Frame = new TimeFrame(m_timeToBeginStage1, m_timeToBeginStage2);
            TimeFrame t_stage2Frame = new TimeFrame(m_timeToBeginStage2, m_timeToBeginStage3);
            TimeFrame t_stage3Frame = new TimeFrame(m_timeToBeginStage3, float.PositiveInfinity);
            if (t_noneFrame.ContainsTime(time, eTimeFrameContainsOption.EndExclusive))
            {
                m_lifoffStage = eLiftoffStage.None;
            } 
            else if (t_stage1Frame.ContainsTime(time, eTimeFrameContainsOption.EndExclusive))
            {
                m_lifoffStage = eLiftoffStage.Stage1;
            }
            else if (t_stage2Frame.ContainsTime(time, eTimeFrameContainsOption.EndExclusive))
            {
                m_lifoffStage = eLiftoffStage.Stage2;
            }
            else if (t_stage3Frame.ContainsTime(time, eTimeFrameContainsOption.EndExclusive))
            {
                m_lifoffStage = eLiftoffStage.Stage3;
            }
            else
            {
                //CustomDebug.LogErrorForComponent($"No valid stage for liftoff", this);
            }


            switch (m_lifoffStage)
            {
                case eLiftoffStage.None:
                    NoneStageUpdate();
                    break;
                case eLiftoffStage.Stage1:
                    Stage1Update();
                    break;
                case eLiftoffStage.Stage2:
                    Stage2Update();
                    break;
                case eLiftoffStage.Stage3:
                    Stage3Update();
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_lifoffStage, this);
                    break;
            }

            m_prevTime = curTime;
        }


        private void NoneStageUpdate()
        {
            transform.position = m_restingPos;
            m_stage1SprRend1.sprite = null;
            m_stage1SprRend2.sprite = null;
            m_stage3SprRend.sprite = null;
            SetShadowAlpha(1.0f);
            UpdateShadowYPosition();
            HideStage3Puffs();
        }
        private void Stage1Update()
        {
            transform.position = m_restingPos;
            UpdatePoofAnimation(m_offsetBetweenInstancesStage1);

            m_stage3SprRend.sprite = null;
            SetShadowAlpha(1.0f);
            UpdateShadowYPosition();
            HideStage3Puffs();
        }
        private void Stage2Update()
        {
            float t_jitterValue = m_horizontalJitterCurve.Evaluate(curTime % m_horizontalJitterCurve.GetEndTime());
            transform.position = m_restingPos + new Vector2(t_jitterValue, 0);
            UpdatePoofAnimation(m_offsetBetweenInstancesStage2);

            if (isRecording && !m_rocketSounds.IsRumbleSoundPlaying())
            {
                // Play the rumble sound if its not playing.
                m_rocketSounds.PlayRumbleSound();
            }

            m_stage3SprRend.sprite = null;
            SetShadowAlpha(1.0f);
            UpdateShadowYPosition();
            HideStage3Puffs();
        }
        private void Stage3Update()
        {
            float t_timeSinceBegin = curTime - m_timeToBeginStage3;
            float t_xPos = m_stage3HorizontalJitterCurve.Evaluate(t_timeSinceBegin % m_stage3HorizontalJitterCurve.GetEndTime());
            float t_yPos = m_stage3VerticalCurve.EvaluateClamped(t_timeSinceBegin);
            ManualSpriteAnimation.Frame t_frame = m_stage3Anim.GetFrameForTime(m_timeToBeginStage3, curTime);
            m_stage3SprRend.sprite = t_frame.sprite;
            transform.position = m_restingPos + new Vector2(t_xPos, t_yPos);
            UpdateShadowYPosition();

            // Stage 3 Puffs animation
            if (curTime < m_timeToStopPuffsStage3)
            {
                ManualSpriteAnimation.Frame t_puff1Frame = m_stage3PuffsAnim.GetFrameForTime(m_timeToBeginStage3, curTime);
                if (t_puff1Frame != null)
                {
                    m_stage3PuffSprRend1.sprite = t_puff1Frame.sprite;
                    m_stage3PuffSprRend1.color = t_puff1Frame.color;
                }
                else
                {
                    m_stage3PuffSprRend1.sprite = null;
                }
                ManualSpriteAnimation.Frame t_puff2Frame = m_stage3PuffsAnim.GetFrameForTime(m_timeToBeginStage3 + m_offsetBetweenInstancesStage3, curTime);
                if (curTime + m_offsetBetweenInstancesStage3 < m_timeToStopPuffsStage3 && t_puff2Frame != null)
                {
                    m_stage3PuffSprRend2.sprite = t_puff2Frame.sprite;
                    m_stage3PuffSprRend2.color = t_puff2Frame.color;
                }
                else
                {
                    m_stage3PuffSprRend2.sprite = null;
                }
            }
            else
            {
                m_stage3PuffSprRend1.sprite = null;
                m_stage3PuffSprRend2.sprite = null;
            }
            Vector2 t_stage3PuffsPos2D = m_stage3PuffSprRend1.transform.position;
            t_stage3PuffsPos2D.y = m_stage3PuffsYPos;
            m_stage3PuffSprRend1.transform.position = t_stage3PuffsPos2D;
            m_stage3PuffSprRend2.transform.position = t_stage3PuffsPos2D;

            float t_shadowAlphaValue = m_shadowFadeoutCurve.EvaluateClamped(t_timeSinceBegin);
            SetShadowAlpha(t_shadowAlphaValue);

            // Non stage 3 puffs but during stage 3
            UpdatePoofAnimationStage3();

            if (isRecording && !m_rocketSounds.IsLiftoffSoundPlaying())
            {
                // Play the liftoff sound if its not playing.
                m_rocketSounds.PlayLiftoffSound();
            }
        }
        private void UpdatePoofAnimation(float offsetBetweenInstances)
        {
            float t_timeForSprRend1 = curTime % m_stage1Anim.animLength;
            float t_timeForSprRend2 = (curTime - offsetBetweenInstances) % m_stage1Anim.animLength;
            m_stage1Anim.ApplyFrameAtTime(m_stage1SprRend1, 0.0f, t_timeForSprRend1);
            m_stage1Anim.ApplyFrameAtTime(m_stage1SprRend2, 0.0f, t_timeForSprRend2);

            if (isRecording)
            {
                // Play a poof sound when animation begins to loop.
                float t_prevTimeForSprRend1 = m_prevTime % m_stage1Anim.animLength;
                float t_prevTimeForSprRend2 = (m_prevTime - offsetBetweenInstances) % m_stage1Anim.animLength;
                if (t_prevTimeForSprRend1 > t_timeForSprRend1 || t_prevTimeForSprRend2 > t_timeForSprRend2)
                {
                    m_rocketSounds.PlayPuffSound();
                }
            }
        }
        private void UpdatePoofAnimationStage3()
        {
            float t_animTime1AtStartOfStage3 = m_timeToBeginStage3 % m_stage1Anim.animLength;
            float t_animTime2AtStartOfStage3 = (m_timeToBeginStage3 - m_offsetBetweenInstancesStage2) % m_stage1Anim.animLength;
            float t_timeSinceStage3Started = curTime - m_timeToBeginStage3;

            m_stage1Anim.ApplyFrameAtTime(m_stage1SprRend1, 0.0f, t_animTime1AtStartOfStage3 + t_timeSinceStage3Started);
            m_stage1Anim.ApplyFrameAtTime(m_stage1SprRend2, 0.0f, t_animTime2AtStartOfStage3 + t_timeSinceStage3Started);
        }
        private void SetShadowAlpha(float alphaValue)
        {
            Color t_shadowColor = m_rocketShadow.color;
            t_shadowColor.a = alphaValue;
            m_rocketShadow.color = t_shadowColor;
        }
        private void UpdateShadowYPosition()
        {
            m_rocketShadow.transform.position = new Vector2(m_rocketShadow.transform.position.x, m_shadowYPos);
        }
        private void HideStage3Puffs()
        {
            m_stage3PuffSprRend1.sprite = null;
            m_stage3PuffSprRend2.sprite = null;
        }


        public enum eLiftoffStage { None, Stage1, Stage2, Stage3 }
    }
}