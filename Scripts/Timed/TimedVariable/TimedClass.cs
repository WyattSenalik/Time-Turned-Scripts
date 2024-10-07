using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public abstract class TimedClass
    {
        public GlobalTimeManager timeMan { get; private set; } = null;
        public float startTime { get; private set; } = float.NaN;
        public float farthestTime { get; private set; } = float.NaN;
        public bool isRecording { get; private set; } = true;
        public bool wasRecording { get; private set; } = true;
        public float curTime => timeMan.curTime;
        public float timeScale => timeMan.timeScale;
        public float deltaTime => timeMan.deltaTime;

        private readonly IDLibrary m_recordingIDs = new IDLibrary();
        private bool m_doesLibraryHaveAllIDs = true;


        public TimedClass(bool shouldNeverRecord = false)
        {
            if (shouldNeverRecord)
            {
                RequestSuspendRecording();
            }

            timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(timeMan, this);
            #endregion Asserts

            startTime = curTime;
            farthestTime = startTime;

            timeMan.AddTimedVariable(this);
            Start();
        }
        ~TimedClass()
        {
            if (timeMan != null)
            {
                timeMan.RemoveTimedVariable(this);
            }
        }


        /// <summary>
        /// Called by <see cref="GlobalTimeManager"/> every frame.
        /// </summary>
        public void Update()
        {
            float t_curTime = curTime;
            if (t_curTime > farthestTime)
            {
                farthestTime = t_curTime;
            }

            // Update is recording.
            wasRecording = isRecording;
            isRecording = DetermineIfIsRecording();

            if (isRecording)
            {
                if (!wasRecording)
                {
                    // We ARE recording now, but were NOT previously.
                    OnRecordingResume();
                }
            }
            // If we stopped recording
            else if (wasRecording)
            {
                // We are NOT recording now, but WERE previously.
                OnRecordingStop();
            }

            TimedUpdate();
        }

        /// <summary>
        /// Called once when the <see cref="TimedClass"/> is instantiated.
        /// </summary>
        protected virtual void Start() { }
        /// <summary>
        /// Called every frame whether recording or not.
        /// </summary>
        protected virtual void TimedUpdate() { }
        /// <summary>
        /// Called the frame the variable stops recording but was previously just recording.
        /// </summary>
        protected virtual void OnRecordingStop() { }
        /// <summary>
        /// Called the frame the variable starts recording again after it had been stopped.
        /// </summary>
        protected virtual void OnRecordingResume() { }


        public int RequestSuspendRecording()
        {
            m_doesLibraryHaveAllIDs = false;
            return m_recordingIDs.CheckoutID();
        }
        public void CancelSuspendRecordingRequest(int requestId)
        {
            m_recordingIDs.ReturnID(requestId);
            m_doesLibraryHaveAllIDs = m_recordingIDs.AreAllIDsReturned();
        }
        public void ForceSetTimeBounds(float newStartTime, float newFarthestTime)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(newStartTime <= newFarthestTime, $"{nameof(newStartTime)}({newStartTime}) <= {nameof(newFarthestTime)}({newFarthestTime})", this);
            #endregion Asserts
            newStartTime = Mathf.Min(newStartTime, newFarthestTime);
            newFarthestTime = Mathf.Max(newStartTime, newFarthestTime);

            startTime = newStartTime;
            farthestTime = newFarthestTime;
        }


        /// <summary>
        /// We record data if shouldRecord is true and either:
        /// 1. m_alwaysRecordWhenShould is true OR
        /// 2. time > m_farthestTime
        /// </summary>
        private bool DetermineIfIsRecording()
        {
            return timeMan.shouldRecord && m_doesLibraryHaveAllIDs;
            //// If global time manager isn't recording, we shouldn't either
            //if (!timeMan.shouldRecord) { return false; }
            //// We have requested to stop the recording.
            //if (!m_recordingIDs.AreAllIDsReturned()) { return false; }
            //// Should always record when we should.
            //return true;
        }
    }
}