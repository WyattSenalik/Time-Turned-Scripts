using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation;
using Helpers.Animation.BetterCurve;
// Original Authors - Wyatt Senalik

namespace Timed.Animation.BetterCurve
{
    /// <summary>
    /// <see cref="BetterCurveAnimation"/> that knows about the timed system.
    /// </summary>
    public abstract class TimedBetterCurveAnimation : TimedRecorder, IScriptedAnimation
    {
        public event Action onEnd;

        public bool isPlaying => CheckIsPlaying();
        public float duration => curve.timeDuration;
        public IBetterCurve curve => m_curve;

        protected float deltaTime { get; private set; }

        [SerializeField, Required, Expandable] private BetterCurveSO m_curve = null;
        [SerializeField] private bool m_playOnStart = false;
        [SerializeField] private bool m_repeat = false;

        private float m_prevTime = 0.0f;

        [SerializeField, ReadOnly] private List<TimeFrame> m_playFrames = new List<TimeFrame>();
        [SerializeField] private bool m_isDebugging = false;
        private bool m_wasPlaying = false;


        protected virtual void Start()
        {
            if (m_playOnStart)
            {
                StartPlaying();
            }
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (isRecording)
            {
                IsRecordingUpdate();
            }
        }
        private void IsRecordingUpdate()
        {
            // Update the deltaTime.
            if (curTime < m_prevTime)
            {
                deltaTime = 0.0f;
            }
            else
            {
                deltaTime = curTime - m_prevTime;
            }
            m_prevTime = curTime;

            bool t_isPlaying = CheckIsPlaying();
            if (t_isPlaying)
            {
                TakeCurveActionAtCurTime();
            }
            // If we were playing (during game time) last frame
            else if (wasRecording && m_wasPlaying)
            {
                StopPlaying();
            }
            m_wasPlaying = t_isPlaying; 
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            //CustomDebug.LogForComponentFullPath($"[{Time.frameCount}] [{curTime}] OnRecordingResume.", this, m_isDebugging);

            while (m_playFrames.Count > 0)
            {
                TimeFrame t_latestFrame = m_playFrames[^1];

                if (t_latestFrame.ContainsTime(curTime, eTimeFrameContainsOption.EndExclusive))
                {
                    //CustomDebug.LogForComponentFullPath($"[{Time.frameCount}] [{curTime}] Latest Frame ({t_latestFrame}) contains curTime ({curTime}).", this, m_isDebugging);
                    // Resumed during a frame.
                    TakeCurveActionAtCurTime();
                    break;
                }
                else if (curTime >= t_latestFrame.endTime)
                {
                    //CustomDebug.LogForComponentFullPath($"[{Time.frameCount}] [{curTime}] Breaking early cause all frames left are after. Latest Frame ({t_latestFrame})", this, m_isDebugging);
                    // This is fine, the time is after the latest one ends, so just break.
                    break;
                }
                else if (curTime < t_latestFrame.startTime)
                {
                    //CustomDebug.LogForComponentFullPath($"[{Time.frameCount}] [{curTime}] Cur time ({curTime}) is earlier than the latest frame ({t_latestFrame}).", this, m_isDebugging);
                    // The time is before this one, so we must get rid of the latest one.
                    m_playFrames.RemoveAt(m_playFrames.Count - 1);
                }
                else
                {
                    #region Logs
                    //CustomDebug.LogErrorForComponent($"Unhandled case. Fell into an else that was thought to be impossible.", this);
                    #endregion Logs
                    break;
                }
            }
        }


        public void Play(bool shouldInterrupt = false)
        {
            // Already playing.
            if (CheckIsPlaying())
            {
                if (shouldInterrupt)
                {
                    Restart();
                }
            }
            // Not playing yet.
            else
            {
                StartPlaying();
            }
        }
        public void Stop()
        {
            for (int i = 0; i < m_playFrames.Count; ++i)
            {
                TimeFrame t_frame = m_playFrames[i];
                if (t_frame.ContainsTime(curTime, eTimeFrameContainsOption.EndExclusive))
                {
                    m_playFrames[i] = new TimeFrame(t_frame.startTime, curTime);
                }
            }

            StopPlaying();
        }

        public void StopAndSkipToEnd()
        {
            Stop();
            TakeCurveAction(m_curve.Evaluate(m_curve.GetEndTime()));
        }


        protected abstract void TakeCurveAction(float curveValue);
        protected virtual void OnEnd() { }


        private void StartPlaying()
        {
            float t_beginTime = Mathf.Max(curTime, 0.0f);
            #region Asserts
            TimeFrame t_latestPlayFrame = GetLatestPlayFrame();
            if (!TimeFrame.IsNaN(t_latestPlayFrame))
            {
                //CustomDebug.AssertIsTrueForComponent(t_beginTime > t_latestPlayFrame.startTime, $"to not start playing animation if there is play time after now. CurTime ({curTime}). LatestPlayFrame ({t_latestPlayFrame}).", this);
            }
            #endregion Asserts
            m_playFrames.Add(new TimeFrame(t_beginTime, t_beginTime + m_curve.GetEndTime()));
        }
        private void StopPlaying()
        {
            //#region Logs
            ////CustomDebug.LogForComponent("StopPlaying", this, IS_DEBUGGING);
            //#endregion Logs
            //TimeFrame t_latestPlayFrame = GetLatestPlayFrame();
            //// If its already stopped, no need to stop it.
            //if (t_latestPlayFrame.endTime < curTime)
            //{
            //    m_playFrames[^1] = new TimeFrame(t_latestPlayFrame.startTime, curTime);
            //}

            OnEnd();
            onEnd?.Invoke();
        }
        private void Restart()
        {
            // Stop playing w/o calling the onEnd event
            //TimeFrame t_latestPlayFrame = GetLatestPlayFrame();
            //m_playFrames[^1] = new TimeFrame(t_latestPlayFrame.startTime, curTime);
            // Start playing again
            StartPlaying();
        }
        private bool CheckIsPlaying()
        {
            TimeFrame t_latestPlayFrame = GetLatestPlayFrame();
            if (TimeFrame.IsNaN(t_latestPlayFrame))
            {
                return false;
            }
            // If the current time is within the interval.
            if (t_latestPlayFrame.ContainsTime(curTime))
            {
                return true;
            }
            return false;
        }
        private void TakeCurveActionAtCurTime()
        {
            TimeFrame t_latestFrame = GetLatestPlayFrame();
            if (TimeFrame.IsNaN(t_latestFrame))
            {
                // There are no frames.
                return;
            }
            float t_curEvalTime = curTime - t_latestFrame.startTime;
            t_curEvalTime = Mathf.Clamp(t_curEvalTime, 0.0f, m_curve.GetEndTime());
            float t_curveVal = m_curve.Evaluate(t_curEvalTime);
            TakeCurveAction(t_curveVal);
        }
        private TimeFrame GetLatestPlayFrame()
        {
            if (m_playFrames.Count > 0)
            {
                // Assumes they are held in chronological order
                return m_playFrames[^1];
            }
            return TimeFrame.NaN;
        }
    }
}