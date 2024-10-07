using System.Collections.Generic;
using UnityEngine;

using Helpers;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Holds if the player movement is currently suspended or not. Movement can be
    /// suspended for an amount of time, or suspended indefinitely until told to
    /// unsuspend (using ids). Amount of time respects the Timed system.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MovementSuspender : TimedRecorder, IMovementSuspender
    {
        public bool isMovementSuspended { get; private set; }
        public bool shouldClearMovementMemory { get; private set; }
        public TimeFrame furthestTimeFrame => m_timesWhenMovementIsSuspended.Count > 0 ? m_timesWhenMovementIsSuspended[^1].frame : TimeFrame.NaN;
        public (TimeFrame frame, int requestID) furthestTimeFramePair => m_timesWhenMovementIsSuspended.Count > 0 ? m_timesWhenMovementIsSuspended[^1] : (TimeFrame.NaN, -1);

        private readonly IDLibrary m_suspendLibrary = new IDLibrary();
        private readonly List<string> m_suspensionKeys = new List<string>();

        private readonly List<(TimeFrame frame, int requestID)> m_timesWhenMovementIsSuspended = new List<(TimeFrame, int)>();


        public void SuspendForTime(float timeToSusFor, bool clearMovementMemory = true)
        {
            if (!isRecording)
            {
                //CustomDebug.LogErrorForComponent($"Can't suspend when not recording.", this);
                return;
            }

            float t_startTime = curTime;
            float t_endTime = t_startTime + timeToSusFor;
            AddNewTimeFrame(t_startTime, t_endTime);

            isMovementSuspended = true;
            shouldClearMovementMemory = clearMovementMemory;
        }
        public int RequestSuspension()
        {
            if (!isRecording)
            {
                //CustomDebug.LogErrorForComponent($"Can't suspend when not recording.", this);
                return -1;
            }

            isMovementSuspended = true;
            shouldClearMovementMemory = true;

            int t_id = m_suspendLibrary.CheckoutID();

            AddNewTimeFrame(curTime, t_id);

            return t_id;
        }
        public void CancelSuspension(int suspensionID)
        {
            if (!isRecording)
            {
                //CustomDebug.LogErrorForComponent($"Can't cancel suspension when not recording.", this);
                return;
            }

            m_suspendLibrary.ReturnID(suspensionID);

            int t_index = FindPairWithID(suspensionID);
            if (t_index == -1)
            {
                //CustomDebug.LogErrorForComponent($"ID {suspensionID} not found", this);
                return;
            }
            TimeFrame t_prevFrame = m_timesWhenMovementIsSuspended[t_index].frame;
            m_timesWhenMovementIsSuspended[t_index] = (new TimeFrame(t_prevFrame.startTime, curTime), -1);

            isMovementSuspended = ShouldMovementBeSuspended();
        }
        public void SuspendWithUniqueKey(string suspendKey)
        {
            // SuspendWithUniqueKey doesn't need to actually use time stuff because its only used for tutorial levels when the player isn't allowed to do anything.

            isMovementSuspended = true;
            shouldClearMovementMemory = true;

            m_suspensionKeys.Add(suspendKey);
        }
        public void CancelSuspensionWithUniqueKey(string suspendKey)
        {
            if (!m_suspensionKeys.Remove(suspendKey))
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Suspension key {suspendKey} was not released.", this);
                #endregion Logs
            }

            isMovementSuspended = ShouldMovementBeSuspended();
        }

        public override void TrimDataAfter(float time)
        {
            base.TrimDataAfter(time);

            for (int i = m_timesWhenMovementIsSuspended.Count - 1; i >= 0; --i)
            {
                (TimeFrame t_frame, int _) = m_timesWhenMovementIsSuspended[i];
                if (time < t_frame.startTime)
                {
                    m_timesWhenMovementIsSuspended.RemoveAt(i);
                }
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            isMovementSuspended = ShouldMovementBeSuspended();
        }
        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            isMovementSuspended = ShouldMovementBeSuspended();
        }


        private bool ShouldMovementBeSuspended()
        {
            foreach ((TimeFrame t_frame, int _) in m_timesWhenMovementIsSuspended)
            {
                if (t_frame.ContainsTime(curTime))
                {
                    return true;
                }
            }

            // Unsuspend time if greater than current time
            // or there are active suspend requests.
            return !m_suspendLibrary.AreAllIDsReturned() ||
                m_suspensionKeys.Count > 0;
        }
        private void AddNewTimeFrame(float startTime, int requestID) => AddNewTimeFrame(startTime, float.PositiveInfinity, requestID);
        private void AddNewTimeFrame(float startTime, float endTime) => AddNewTimeFrame(startTime, endTime, -1);
        private void AddNewTimeFrame(float startTime, float endTime, int requestID)
        {
            if (m_timesWhenMovementIsSuspended.Count > 0)
            {
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(furthestTimeFrame.startTime < startTime, $"the new suspension time ({startTime}) to be later than the previous suspension time {furthestTimeFrame.startTime}.", this);
                #endregion Asserts
            }
            m_timesWhenMovementIsSuspended.Add((new TimeFrame(startTime, endTime), requestID));
        }
        private int FindPairWithID(int id)
        {
            for (int i = m_timesWhenMovementIsSuspended.Count - 1; i >= 0; --i)
            {
                (TimeFrame _, int t_curID) = m_timesWhenMovementIsSuspended[i];
                if (t_curID == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}