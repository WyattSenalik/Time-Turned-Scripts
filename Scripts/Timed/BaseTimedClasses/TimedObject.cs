using UnityEngine;

using NaughtyAttributes;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Implementation of <see cref="ITimedObject"/> that allows
    /// <see cref="ITimedComponent"/>s, <see cref="ITimedObserver"/>s, and 
    /// <see cref="ITimedRecorder"/>s to be attached to this gameobject.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimedObject : MonoBehaviour, ITimedObject
    {
        public bool isRecording { get; private set; } = true;
        public bool wasRecording { get; private set; } = true;
        public bool shouldRecord { get => m_shouldRecord; set => m_shouldRecord = value; }
        public bool shouldRecordWithRequests => shouldRecord && !m_stopRecordingRequested;
        public float curTime => m_timeMan != null ? m_timeMan.curTime : -1.0f;
        public float spawnTime => m_spawnTime;
        public float farthestTime => m_farthestTime != float.NegativeInfinity ?
            m_farthestTime : 0.0f;
        public float timeScale => m_timeMan != null ? m_timeMan.timeScale : 0.0f;
        public float deltaTime => m_timeMan != null ? m_timeMan.deltaTime : 0.0f;

        public long setToTimeFuncTicks => m_setToTimeFuncTicks;
        public long requestSuspendRecordingFuncTicks => m_requestSuspendRecordingFuncTicks;
        public long cancelSuspendRecordingRequestFuncTicks => m_cancelSuspendRecordingRequestFuncTicks;
        public long forceSetTimeBoundsFuncTicks => m_forceSetTimeBoundsFuncTicks;
        public long trimDataAfterFuncTicks => m_trimDataAfterFuncTicks;

        [SerializeField] private bool m_isDebugging = false;
        [SerializeField] private bool m_shouldRecord = true;
        [Tooltip("If the timed object should always record when shouldRecord is " +
            "true. Will cause data to be overwritten.")]
        [SerializeField] private bool m_alwaysRecordWhenShould = true;
        [SerializeField] private bool m_ignoreDataTrimming = false;

        [SerializeField, BoxGroup("Time Testing")] private long m_setToTimeFuncTicks = 0;
        [SerializeField, BoxGroup("Time Testing")] private long m_requestSuspendRecordingFuncTicks = 0;
        [SerializeField, BoxGroup("Time Testing")] private long m_cancelSuspendRecordingRequestFuncTicks = 0;
        [SerializeField, BoxGroup("Time Testing")] private long m_forceSetTimeBoundsFuncTicks = 0;
        [SerializeField, BoxGroup("Time Testing")] private long m_trimDataAfterFuncTicks = 0;

        //[SerializeField, ReadOnly] private List<string> m_debugSubscribedRecorders = new List<string>();

        private GlobalTimeManager m_timeMan = null;
        private ITimedObserver[] m_timedObservers = null;
        private ITimedRecorder[] m_timedRecorders = null;
        // The spawn time of this object
        private float m_spawnTime = float.PositiveInfinity;
        // Farthest time we had gotten in the recording.
        private float m_farthestTime = float.NegativeInfinity;

        private readonly IDLibrary m_recordingIDs = new IDLibrary();
        private bool m_stopRecordingRequested = false;

        private System.Diagnostics.Stopwatch m_diagnosticsWatch = null;


        // Domestic Initialization
        private void Awake()
        {
            m_timedObservers = GetComponents<ITimedObserver>();
            m_timedRecorders = GetComponents<ITimedRecorder>();

            m_diagnosticsWatch = System.Diagnostics.Stopwatch.StartNew();
            m_setToTimeFuncTicks = 0;
            m_requestSuspendRecordingFuncTicks = 0;
            m_cancelSuspendRecordingRequestFuncTicks = 0;
            m_forceSetTimeBoundsFuncTicks = 0;
            m_trimDataAfterFuncTicks = 0;
        }
        // Foreign Initialization
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
            #endregion Asserts

            // Only set the spawn time and farthest time if they haven't been set using ForceSetTimeBounds (aka are their default values of +inf and -inf).
            if (float.IsPositiveInfinity(m_spawnTime))
            {
                m_spawnTime = m_timeMan.curTime;
            }
            if (float.IsNegativeInfinity(m_farthestTime))
            {
                m_farthestTime = m_spawnTime;
            }

            m_timeMan.AddTimeObject(this);
        }
        private void OnDestroy()
        {
            // Remove this from its global time manager when it is destroyed.
            if (m_timeMan != null)
            {
                m_timeMan.RemoveTimeObject(this);
            }
        }


        public void SetToTime(float time, float deltaTime)
        {
            m_diagnosticsWatch.Restart();

            // We want to disable the gameobject and not call updateToTime
            // if either we are before it should spawn or we are after the
            // furthest time and we shouldn't be recording.
            bool t_shouldBeActive = DetermineShouldBeActive(time);
            gameObject.SetActive(t_shouldBeActive);
            // Do nothing if inactive.
            if (!gameObject.activeSelf)
            {
                m_diagnosticsWatch.Stop();
                m_setToTimeFuncTicks += m_diagnosticsWatch.ElapsedTicks;
                return;
            }

            // Update is recording.
            wasRecording = isRecording;
            isRecording = DetermineIfIsRecording(time);

            // Update recorders and observers.
            UpdateObservers(time);
            UpdateRecorders(time);

            //CustomDebug.RunDebugFunction(() =>
            //{
            //    m_debugSubscribedRecorders.Clear();
            //    foreach (ITimedRecorder t_recorder in m_timedRecorders)
            //    {
            //        if (t_recorder is MonoBehaviour t_mono)
            //        {
            //            m_debugSubscribedRecorders.Add($"{t_mono.gameObject.GetFullName()} ({t_mono.GetType().Name})");
            //        }
            //        else
            //        {
            //            m_debugSubscribedRecorders.Add($"{t_recorder.GetType().FullName}");
            //        }
            //    }
            //}, m_isDebugging);

            m_diagnosticsWatch.Stop();
            m_setToTimeFuncTicks += m_diagnosticsWatch.ElapsedTicks;
        }
        public int RequestSuspendRecording()
        {
            m_diagnosticsWatch.Restart();

            m_stopRecordingRequested = true;
            int t_idToReturn = m_recordingIDs.CheckoutID();
            gameObject.SetActive(DetermineShouldBeActive(curTime));

            m_diagnosticsWatch.Stop();
            m_requestSuspendRecordingFuncTicks += m_diagnosticsWatch.ElapsedTicks;

            return t_idToReturn;
        }
        public void CancelSuspendRecordingRequest(int requestId)
        {
            m_diagnosticsWatch.Restart();

            m_recordingIDs.ReturnID(requestId);
            // Stop recording request is false when all IDs are returned
            m_stopRecordingRequested = !m_recordingIDs.AreAllIDsReturned();
            gameObject.SetActive(DetermineShouldBeActive(curTime));

            m_diagnosticsWatch.Stop();
            m_cancelSuspendRecordingRequestFuncTicks += m_diagnosticsWatch.ElapsedTicks;
        }
        public void ForceSetTimeBounds(float newSpawnTime, float newFarthestTime)
        {
            m_diagnosticsWatch.Restart();

            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(newSpawnTime <= newFarthestTime, $"{nameof(newSpawnTime)}({newSpawnTime}) <= {nameof(newFarthestTime)}({newFarthestTime})", this);
            #endregion Asserts
            newSpawnTime = Mathf.Min(newSpawnTime, newFarthestTime);
            newFarthestTime = Mathf.Max(newSpawnTime, newFarthestTime);

            m_spawnTime = newSpawnTime;
            m_farthestTime = newFarthestTime;

            m_diagnosticsWatch.Stop();
            m_forceSetTimeBoundsFuncTicks += m_diagnosticsWatch.ElapsedTicks;
        }
        public void TrimDataAfter(float time)
        {
            m_diagnosticsWatch.Restart();

            if (m_ignoreDataTrimming)
            {
                m_diagnosticsWatch.Stop();
                m_trimDataAfterFuncTicks += m_diagnosticsWatch.ElapsedTicks;
                return;
            }

            foreach (ITimedRecorder t_recorder in m_timedRecorders)
            {
                t_recorder.TrimDataAfter(time);
            }
            m_farthestTime = time;
            #region Logs
            //CustomDebug.LogForComponent($"Data trimmed at {time}. New furthest time = {m_farthestTime}", this, m_isDebugging);
            #endregion Logs

            m_diagnosticsWatch.Stop();
            m_trimDataAfterFuncTicks += m_diagnosticsWatch.ElapsedTicks;
        }


        /// <summary>
        /// Calls functions of <see cref="ITimedRecorder"/> when appropriate.
        /// </summary>
        /// <param name="time">Current global time.</param>
        private void UpdateRecorders(float time)
        {
            // Which functions of ITimedBehaviour should be called
            bool t_doCallTrimDataAfter = false;
            bool t_doCallOnRecordingResume = false;
            bool t_doCallOnRecordingStop = false;

            // We record data if shouldRecord is true and either:
            // 1. m_alwaysRecordWhenShould is true OR
            // 2. time > m_farthestTime
            if (isRecording)
            {
                // If the time is before the farthest time, trim the data
                if (time <= m_farthestTime)
                {
                    // Assumed that if this is true, so is m_alwaysRecordWhenShould.
                    #region Asserts
                    //CustomDebug.AssertIsTrueForComponent(m_alwaysRecordWhenShould, $"Record time should only ever be before the furthest time if {nameof(m_alwaysRecordWhenShould)} is true but it is false.", this);
                    #endregion Asserts
                    t_doCallTrimDataAfter = true;
                }
                m_farthestTime = time;

                // If we resumed the recording.
                t_doCallOnRecordingResume = !wasRecording;
            }
            // If we stopped recording
            else if (wasRecording)
            {
                t_doCallOnRecordingStop = true;
            }

            // Call the ITimedRecorder functions
            if (t_doCallTrimDataAfter)
            {
                foreach (ITimedRecorder t_behav in m_timedRecorders)
                {
                    t_behav.TrimDataAfter(time);
                }
            }
            if (t_doCallOnRecordingResume)
            {
                foreach (ITimedRecorder t_behav in m_timedRecorders)
                {
                    t_behav.OnRecordingResume(time);
                }
            }
            if (t_doCallOnRecordingStop)
            {
                foreach (ITimedRecorder t_behav in m_timedRecorders)
                {
                    t_behav.OnRecordingStop(time);
                }
            }
            // Always call set to time (unless inactive)
            foreach (ITimedRecorder t_behav in m_timedRecorders)
            {
                t_behav.SetToTime(time);
            }

            wasRecording = isRecording;
        }
        /// <summary>
        /// Calls timed update on all observers if recording.
        /// </summary>
        private void UpdateObservers(float time)
        {
            if (isRecording)
            {
                float t_deltaTime = time - m_timeMan.prevTime;
                foreach (ITimedObserver t_obs in m_timedObservers)
                {
                    t_obs.TimedUpdate(t_deltaTime);
                }
            }
        }
        /// <summary>
        /// We record data if shouldRecord is true and either:
        /// 1. m_alwaysRecordWhenShould is true OR
        /// 2. time > m_farthestTime
        /// </summary>
        private bool DetermineIfIsRecording(float time)
        {
            // If global time manager isn't recording, we shouldn't either
            if (!m_timeMan.shouldRecord) { return false; }
            // We have requested to stop the recording.
            if (m_stopRecordingRequested) { return false; }
            // Should always record when we should.
            if (m_alwaysRecordWhenShould) { return shouldRecord; }
            // We only record data for new times that are
            // farther than the any previous time.
            return time > m_farthestTime && shouldRecord;
        }
        /// <summary>
        /// We want to be inactive if either we are before it should spawn or we 
        /// are after the furthest time and we shouldn't be recording.
        /// </summary>
        private bool DetermineShouldBeActive(float time)
        {
            bool t_isTimeBeforeSpawn = time < m_spawnTime;
            bool t_isTimeAfterRecording = time >= m_farthestTime;
            bool t_shouldBeInactive = t_isTimeBeforeSpawn ||
                (t_isTimeAfterRecording && !shouldRecordWithRequests);
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(t_isTimeBeforeSpawn)}={t_isTimeBeforeSpawn}; {nameof(t_isTimeAfterRecording)}={t_isTimeAfterRecording}; {nameof(t_shouldBeInactive)}={t_shouldBeInactive}", this, m_isDebugging);
            #endregion Logs
            return !t_shouldBeInactive;
        }
    }
}