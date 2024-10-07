using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    [RequireComponent(typeof(ITimedObject))]
    public abstract class MomentRecorder<TMomentSelf> : TimedComponent,
        IMomentRecorder<TMomentSelf> where TMomentSelf : IMoment<TMomentSelf>
    {
        private const bool IS_DEBUGGING = false;

        public IReadOnlyMomentAlbum<TMomentSelf> album => m_album;
    
        private readonly IMomentAlbum<TMomentSelf> m_album = new MomentAlbum<TMomentSelf>();

        private float m_prevTime = -1.0f;


        public virtual void TrimDataAfter(float time)
        {
            m_album.RemoveMomentsAfter(time);
        }
        public virtual void OnRecordingResume(float time) { }
        public virtual void OnRecordingStop(float time) { }
        public virtual void SetToTime(float time)
        {
            // If we are not recording (are instead replaying),
            // see if there was any previous stuff.
            if (!isRecording)
            {
                // Advancing forward in time (Do moments).
                if (time > m_prevTime)
                {
                    TMomentSelf[] t_occuredMoments = m_album.Get(m_prevTime, time); 
                    if (t_occuredMoments.Length > 0)
                    {
                        #region Logs
                        //CustomDebug.LogForComponent($"SetToTime with lower={m_prevTime} and upper={time}. Found {t_occuredMoments.Length} moments", this, IS_DEBUGGING);
                        #endregion Logs
                    }
                    foreach (TMomentSelf t_mom in t_occuredMoments)
                    {
                        #region Asserts
                        //CustomDebug.AssertIsTrueForObj(t_mom.time >= m_prevTime && t_mom.time <= time, $"the moment at t={t_mom.time} to be in range [{m_prevTime}, {time}]", this);
                        #endregion Asserts
                        t_mom.Do();
                    }
                }
                // Going backwards in time (Undo moments).
                else if (time < m_prevTime)
                {
                    TMomentSelf[] t_occuredMoments = m_album.Get(time, m_prevTime);
                    if (t_occuredMoments.Length > 0)
                    {
                        #region Logs
                        //CustomDebug.LogForComponent($"SetToTime with time={time} and prevTime={m_prevTime}. Found {t_occuredMoments.Length} moments", this, IS_DEBUGGING);
                        #endregion Logs
                    }
                    foreach (TMomentSelf t_mom in t_occuredMoments)
                    {
                        #region Asserts
                        //CustomDebug.AssertIsTrueForObj(t_mom.time >= time && t_mom.time <= m_prevTime, $"the moment at t={t_mom.time} to be in range [{time}, {m_prevTime}]", this);
                        #endregion Asserts
                        t_mom.Undo();
                    }
                }

                m_prevTime = time;
            }
        }


        /// <summary>
        /// Adds the given moment to the album and calls Do on it right away.
        /// </summary>
        protected void AddMoment(TMomentSelf moment, bool callDoRightAway = true)
        {
            m_album.AddMoment(moment);
            if (callDoRightAway)
            {
                moment.Do();
            }
        }
    }
}
