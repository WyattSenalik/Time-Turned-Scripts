using System;
using System.Collections;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TimeCloneDisappearAnimation : MonoBehaviour
    {
        public float curTime => m_timeCloneBlink.curTime;
        public float animStartTime => m_timeCloneBlink.blinkTimeEnd - m_animStartTimeOffset;
        public float animLength => m_redFrames.Length / ((float)m_fps);

        [SerializeField] private Sprite[] m_redFrames = new Sprite[0];
        [SerializeField] private Sprite[] m_blueFrames = new Sprite[0];
        [SerializeField] private Sprite[] m_yellowFrames = new Sprite[0];
        [SerializeField, Min(1)] private int m_fps = 12;
        [SerializeField, Tooltip("Time before the blink ends to start animation")] private float m_animStartTimeOffset = 1.0f;
        [SerializeField] private Vector2 m_positionOffset = new Vector2(0.0f, 0.375f);
        [SerializeField, Min(1)] private int m_earlyDeathFrameIndex = 11;

        private TimeClone m_timeClone = null;
        private TimeCloneBlink m_timeCloneBlink = null;
        private TimeCloneHealth m_timeCloneHealth = null;
        private SpriteRenderer m_sprRend = null;

        private float m_realTimeAnimStartTime = float.PositiveInfinity;


        private void Awake()
        {
            m_sprRend = GetComponent<SpriteRenderer>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_sprRend, this);
            #endregion Asserts
        }
        private void Update()
        {
            if (!float.IsPositiveInfinity(m_realTimeAnimStartTime))
            {
                m_sprRend.sprite = GetFrame(m_realTimeAnimStartTime, Time.time);
                UpdatePosition();
            }
            else if (curTime >= m_timeCloneHealth.hitAfterDeathTime)
            {
                UpdatePosition();
                m_sprRend.sprite = GetCurrentFrameForEarlyDeath();
            }
            else if (curTime >= animStartTime)
            {
                UpdatePosition();
                m_sprRend.sprite = GetCurrentFrame();
            }
            else //if (curTime < animStartTime)
            {
                m_sprRend.sprite = null;
            }
        }

        public void Initialize(TimeClone clone, TimeCloneBlink cloneBlink, TimeCloneHealth cloneHealth)
        {
            m_timeClone = clone;
            m_timeCloneBlink = cloneBlink;
            m_timeCloneHealth = cloneHealth;
        }
        public void PlayAsUnityTimeAnimation()
        {
            m_realTimeAnimStartTime = Time.time;
        }


        private void UpdatePosition()
        {
            Vector2 t_timeClonePos2D = m_timeClone.transform.position;
            transform.position = t_timeClonePos2D + m_positionOffset;
        }
        private Sprite GetCurrentFrame() => GetFrame(animStartTime, curTime);
        private Sprite GetCurrentFrameForEarlyDeath() => GetFrame(m_timeCloneHealth.hitAfterDeathTime, curTime, m_earlyDeathFrameIndex);
        private Sprite GetFrame(float animStartTime, float animCurTime, int frameOffset = 0)
        {
            Sprite[] t_frames = GetFramesForTimeClone();
            float t_timeSinceBegin = animCurTime - animStartTime;
            int t_frameIndex = Mathf.FloorToInt(t_timeSinceBegin * m_fps) + frameOffset;
            if (t_frameIndex < t_frames.Length)
            {
                return t_frames[t_frameIndex];
            }
            else
            {
                return null;
            }
        }
        private Sprite[] GetFramesForTimeClone()
        {
            int t_charge = m_timeClone.cloneData.occupyingCharge;
            switch (t_charge)
            {
                case 0: return m_redFrames;
                case 1: return m_blueFrames;
                case 2: return m_yellowFrames;
                case -1: return m_blueFrames;
                default:
                {
                    CustomDebug.UnhandledValue(t_charge, this);
                    return new Sprite[0];
                }
            }
        }
    }
}