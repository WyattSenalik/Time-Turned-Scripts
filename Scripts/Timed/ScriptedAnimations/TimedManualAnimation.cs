using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation;

namespace Timed.Animation
{
    public sealed class TimedManualAnimation : TimedRecorder
    {
        [SerializeField, Required] private SpriteRenderer m_spriteRenderer = null;
        [SerializeField] private ManualSpriteAnimation m_spriteAnim = new ManualSpriteAnimation();

        private readonly List<float> m_playTimes = new List<float>();


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            float t_startTime = GetMostRecentPlayTime();
            if (!float.IsPositiveInfinity(t_startTime))
            {
                ManualSpriteAnimation.Frame t_frame = m_spriteAnim.GetFrameForTime(t_startTime, curTime);
                m_spriteRenderer.sprite = t_frame.sprite;
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            int t_index = m_playTimes.Count - 1;
            while (t_index >= 0)
            {
                if (m_playTimes[t_index] >= curTime)
                {
                    m_playTimes.RemoveAt(t_index);
                    --t_index;
                }
                else
                {
                    break;
                }
            }
        }

        public void Play()
        {
            m_playTimes.Add(curTime);
        }
        public void ClearAllPlayTimes()
        {
            m_playTimes.Clear();
        }


        private float GetMostRecentPlayTime()
        {
            int t_index = m_playTimes.Count - 1;
            while (t_index >= 0)
            {
                if (m_playTimes[t_index] <= curTime)
                {
                    return m_playTimes[t_index];
                }
                --t_index;
            }

            return float.PositiveInfinity;
        }
    }
}