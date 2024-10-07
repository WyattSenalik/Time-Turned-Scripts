using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Animation;
using System.Collections.Generic;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SoundRecorder))]
    public sealed class RoomExplosion : TimedRecorder
    {
        public IReadOnlyList<SpriteRenderer> explosionsSpriteRends => m_explosionSpriteRends;
        private LevelOptions options
        {
            get { InitializeLevelOptionsReference(); return m_options; }
        }

        [SerializeField, MinMaxSlider(0.0f, 10.0f)] private Vector2 m_offsetRange = new Vector2(0.0f, 1.0f);
        [SerializeField] private ManualSpriteAnimation m_explosionAnim = new ManualSpriteAnimation();

        private LevelOptions m_options = null;
        private SoundRecorder m_soundRecorder = null;
        private SpriteRenderer[] m_explosionSpriteRends = new SpriteRenderer[1];
        private float[] m_instanceOffsets = null;

        private bool m_areExplosionsPlaying = false;


        protected override void Awake()
        {
            base.Awake();

            m_explosionSpriteRends = GetComponentsInChildren<SpriteRenderer>();
            m_instanceOffsets = new float[m_explosionSpriteRends.Length - 1];
            for (int i = 0; i < m_instanceOffsets.Length; ++i)
            {
                m_instanceOffsets[i] = Random.Range(m_offsetRange.x, m_offsetRange.y);
            }

            m_soundRecorder = this.GetComponentSafe<SoundRecorder>(this);
        }
        private void Start()
        {
            InitializeLevelOptionsReference();
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (options.noTimeLimit) { return; }
            if (m_explosionSpriteRends.Length <= 0) { return; }

            float t_explosionLength = m_explosionAnim.animLength;
            float t_explosionStartTime = options.time - t_explosionLength;
            if (time >= t_explosionStartTime)
            {
                if ( !m_areExplosionsPlaying ) {
                    m_areExplosionsPlaying = true;
                    SteamAchievementManager.instance.GrantAchievement( AchievementId.OUT_OF_TIME );
                }

                m_explosionAnim.ApplySpriteAndPositionAtTime(m_explosionSpriteRends[0], t_explosionStartTime, time);
                for (int i = 1; i < m_explosionSpriteRends.Length; ++i)
                {
                    float t_offsetStart = t_explosionStartTime + m_instanceOffsets[i - 1];
                    if (time >= t_offsetStart)
                    {
                        m_explosionAnim.ApplySpriteAndPositionAtTime(m_explosionSpriteRends[i], t_offsetStart, time);
                    }
                    else
                    {
                        m_explosionSpriteRends[i].sprite = null;
                    }
                }

                // Play the sound for the room explosion if its not playing yet.
                if (!m_soundRecorder.IsPlaying())
                {
                    m_soundRecorder.Play();
                }
            }
            else
            {
                if (m_areExplosionsPlaying)
                {
                    foreach (SpriteRenderer t_explosionRend in m_explosionSpriteRends)
                    {
                        t_explosionRend.sprite = null;
                    }
                    m_areExplosionsPlaying = false;
                }
            }
        }

        private void InitializeLevelOptionsReference()
        {
            if (m_options == null)
            {
                m_options = LevelOptions.instance;
                #region Asserts
                //CustomDebug.AssertSingletonIsNotNull(m_options, this);
                #endregion Asserts
            }
        }
    }
}