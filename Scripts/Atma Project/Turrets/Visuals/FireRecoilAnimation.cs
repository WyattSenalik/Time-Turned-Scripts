using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Animation;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class FireRecoilAnimation : MonoBehaviour
    {
        public BulletController bulletCont => m_bulletCont;


        [SerializeField, Required] private EnemyHealth m_enemyHealth = null;
        [SerializeField, Required] private AutoShootBullet m_autoShootCont = null;
        [SerializeField, Required] private BulletController m_bulletCont = null;
        [SerializeField, Required] private SpriteRenderer m_sprRend = null;
        [SerializeField] private ManualSpriteAnimation m_fireAnimation = new ManualSpriteAnimation();
        [SerializeField, Required] private Sprite m_idleSprite = null;
        [SerializeField, Min(0.0f)] private float m_desiredSpawnTimeFromEndOfAnim = 1.0f / 12.0f;

        private TimedFloat m_playTimes = null;
        private float m_adjustedAnimLength = float.NaN;
        private float m_playbackSpeed = float.NaN;
        private float m_adjustedDesiredSpawnTimeFromEndofAnim = float.NaN;
        private float m_firstPlayTime = float.NaN;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_autoShootCont, nameof(m_autoShootCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_bulletCont, nameof(m_bulletCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_sprRend, nameof(m_sprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_idleSprite, nameof(m_idleSprite), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_playbackSpeed = 1.0f;
            m_adjustedAnimLength = m_fireAnimation.animLength;
            m_adjustedDesiredSpawnTimeFromEndofAnim = m_desiredSpawnTimeFromEndOfAnim;
            if (m_fireAnimation.animLength > m_autoShootCont.fireCooldown)
            {
                m_playbackSpeed = m_fireAnimation.animLength / m_autoShootCont.fireCooldown;
                m_adjustedAnimLength = m_autoShootCont.fireCooldown;
                m_adjustedDesiredSpawnTimeFromEndofAnim = m_desiredSpawnTimeFromEndOfAnim / m_playbackSpeed;
            }
            m_firstPlayTime = m_autoShootCont.initialFireDelay - m_adjustedAnimLength + m_adjustedDesiredSpawnTimeFromEndofAnim;
            m_playTimes = new TimedFloat(m_firstPlayTime, eInterpolationOption.Step);

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void Update()
        {
            float t_curTime = m_playTimes.curTime;
            if (t_curTime >= m_enemyHealth.deathTime) { return; }

            float t_curPlayTime = m_playTimes.curData;
            TimeFrame t_curPlayWindow = new TimeFrame(t_curPlayTime, t_curPlayTime + m_adjustedAnimLength);
            if (t_curPlayWindow.ContainsTime(t_curTime, eTimeFrameContainsOption.EndExclusive))
            {
                float t_timeElapsed = t_curTime - t_curPlayTime;
                float t_correctedTimeElapsed = t_timeElapsed * m_playbackSpeed;
                float t_correctedCurTime = t_curPlayTime + t_correctedTimeElapsed;
                ManualSpriteAnimation.Frame t_frame = m_fireAnimation.GetFrameForTime(t_curPlayTime, t_correctedCurTime);

                m_sprRend.sprite = t_frame.sprite;
            }
            else
            {
                m_sprRend.sprite = m_idleSprite;
            }
        }


        private void ToggleSubscriptions(bool cond)
        {
            m_bulletCont?.onFireBullet.ToggleSubscription(OnFireBullet, cond);
        }
        private void OnFireBullet()
        {
            float t_nextPlayTime = m_playTimes.curTime + m_autoShootCont.fireCooldown - m_adjustedAnimLength + m_adjustedDesiredSpawnTimeFromEndofAnim;
            if (m_playTimes.isRecording)
            {
                m_playTimes.curData = t_nextPlayTime;
            }
            else
            {
                // TimedObjects update before TimedClasses, so its possible that the PlayTime isn't recording when the BulletController tells us it fired.
                StartCoroutine(SetPlayTimeAfterDelayCoroutine(t_nextPlayTime));
            }
        }


        private IEnumerator SetPlayTimeAfterDelayCoroutine(float playTime)
        {
            yield return new WaitForEndOfFrame();
            m_playTimes.curData = playTime;
        }
    }
}