using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    [Serializable]
    public sealed class ManualSpriteAnimation
    {
        public int fps => m_fps;

        public float animLength
        {
            get
            {
                if (m_isUsingNewFrameClass)
                {
                    return ((float)m_framesNew.Length) / m_fps;
                }
                else
                {
                    return ((float)m_frames.Length) / m_fps;
                }                
            }
        } 

        [SerializeField, Min(0)] private int m_fps = 12;
        [SerializeField, HideIf(nameof(m_isUsingNewFrameClass)), AllowNesting] private Sprite[] m_frames = new Sprite[2];
        [SerializeField, HideIf(nameof(m_isUsingNewFrameClass)), AllowNesting] private Vector2[] m_positionFrames = new Vector2[2];

        [SerializeField] private bool m_isUsingNewFrameClass = false;
        [SerializeField, ShowIf(nameof(m_isUsingNewFrameClass)), AllowNesting] private Frame[] m_framesNew = new Frame[2];

        [SerializeField] private ePlayType m_playType = ePlayType.Forward;

        
        public Frame GetFrameForTime(float startTime, float curTime)
        {
            float t_timeSinceStart = curTime - startTime;
            int t_frameIndex = Mathf.FloorToInt(t_timeSinceStart * m_fps);

            switch (m_playType)
            {
                case ePlayType.Forward:
                {
                    t_frameIndex = Mathf.Clamp(t_frameIndex, 0, m_framesNew.Length - 1);
                    break;
                }
                case ePlayType.Loop:
                {
                    t_frameIndex = t_frameIndex % m_framesNew.Length;
                    break;
                }
                case ePlayType.PingPong:
                {
                    if (t_frameIndex >= m_framesNew.Length)
                    {
                        int t_indexModule = t_frameIndex % m_framesNew.Length;
                        t_frameIndex = m_framesNew.Length - 1 - t_indexModule;
                    }
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(m_playType, nameof(ManualSpriteAnimation));
                    break;
                }
            }

            if (t_frameIndex < 0)
            {
                return null;
            }
            return m_framesNew[t_frameIndex];
        }
        public void ApplyFrameAtTime(SpriteRenderer spriteRenderer, float startTime, float curTime)
        {
            Frame t_curFrame = GetFrameForTime(startTime, curTime);
            spriteRenderer.sprite = t_curFrame.sprite;
            spriteRenderer.transform.localPosition = t_curFrame.position;
            spriteRenderer.color = t_curFrame.color;
        }

        [Obsolete("Please use GetFrameForTime instead")]
        public Sprite GetSpriteForTime(float startTime, float curTime)
        {
            float t_timeSinceStart = curTime - startTime;
            int t_frameIndex = Mathf.FloorToInt(t_timeSinceStart * m_fps);
            t_frameIndex = Mathf.Clamp(t_frameIndex, 0, m_frames.Length - 1);
            return m_frames[t_frameIndex];
        }
        [Obsolete("Please use GetFrameForTime instead")]
        public Vector2 GetPositionForTime(float startTime, float curTime)
        {
            float t_timeSinceStart = curTime - startTime;
            int t_frameIndex = Mathf.FloorToInt(t_timeSinceStart * m_fps);
            t_frameIndex = Mathf.Clamp(t_frameIndex, 0, m_positionFrames.Length - 1);
            return m_positionFrames[t_frameIndex];
        }
        [Obsolete("Please use ApplyFrameAtTime instead")]
        public void ApplySpriteAndPositionAtTime(SpriteRenderer spriteRenderer, float startTime, float curTime)
        {
            spriteRenderer.sprite = GetSpriteForTime(startTime, curTime);
            spriteRenderer.transform.localPosition = GetPositionForTime(startTime, curTime);
        }



        [Serializable]
        public sealed class Frame
        {
            public Sprite sprite => m_sprite;
            public Vector2 position => m_position;
            public Color color => m_color;

            [SerializeField] private Sprite m_sprite = null;
            [SerializeField] private Vector2 m_position = Vector2.zero;
            [SerializeField] private Color m_color = Color.white;
        }

        public enum ePlayType { Forward, Loop, PingPong }
    }
}