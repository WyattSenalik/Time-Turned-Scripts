using System.Collections.Generic;
using UnityEngine;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SoundRecorder))]
    public sealed class FootstepController : TimedRecorder
    {
        [SerializeField, Min(0.0f)] private float m_footstepWaitTime = 0.4f;
        private SoundRecorder m_soundRecorder = null;

        private readonly List<float> m_footstepsPlayTimes = new List<float>();


        protected override void Awake()
        {
            base.Awake();

            m_soundRecorder = GetComponent<SoundRecorder>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_soundRecorder, this);
            #endregion Asserts
        }


        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            int t_infGuard = 0;
            while (m_footstepsPlayTimes.Count > 0 && m_footstepsPlayTimes[^1] > time)
            {
                m_footstepsPlayTimes.RemoveAt(m_footstepsPlayTimes.Count - 1);
                if (++t_infGuard > 1000)
                {
                    //CustomDebug.LogErrorForComponent($"InfiniteLoop detected", this);
                    return;
                }
            }
        }

        public void RequestFootstep()
        {
            if (m_footstepsPlayTimes.Count <= 0 || curTime >= m_footstepsPlayTimes[^1] + m_footstepWaitTime)
            {
                m_footstepsPlayTimes.Add(curTime);
                m_soundRecorder.Play();
            }
        }
        //public void RequestEndFootstepEarly(GameObject source)
        //{
        //    if (m_footstepsPlayTimes.TryGetValue(source, out List<float> t_playTimesForSource))
        //    {
        //        for (int i = t_playTimesForSource.Count - 1; i >= 0; --i)
        //        {
        //            float t_startTime = t_playTimesForSource[i];
        //            TimeFrame t_frame = new TimeFrame(t_startTime, t_startTime + m_soundRecorder.clipLength);
        //            if (t_frame.ContainsTime(curTime))
        //            {
        //                m_soundRecorder.Stop();
        //            }
        //        }
        //    }
        //}
    }
}