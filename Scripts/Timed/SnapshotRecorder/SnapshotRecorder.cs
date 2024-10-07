using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Transforms;
// Original Authors - Wyatt Senalik

namespace Timed
{
    [RequireComponent(typeof(ITimedObject))]
    public abstract class SnapshotRecorder<TSnap, TSnapData> :
        TimedComponent, ISnapshotRecorder<TSnap, TSnapData>
        where TSnap : ISnapshot<TSnapData, TSnap>
        where TSnapData : IEquatable<TSnapData>
    {
        private const bool IS_DEBUGGING = false;

        #region ISnapshotRecorder
        public IReadOnlySnapshotScrapbook<TSnap, TSnapData> scrapbook => timedVariable.scrapbook;
        #endregion ISnapshotRecorder
        // When the last snapshot was taken.
        public float lastSnapTime => timedVariable.scrapbook.GetLatestTime();

        #region Debug Fields
        [ShowNonSerializedField] private int m_amountSnapshots = 0;
        [SerializeField, ReadOnly] private List<Vector2> m_debugSnapData = new List<Vector2>();
        #endregion Debug Fields

        [SerializeField] private bool m_isDebuggingIndividual = false;

        private bool m_isFixedUpdateDirty = false;

        protected GenericTimedVariable<TSnap, TSnapData> timedVariable
        {
            get
            {
                if (m_timedVariable == null)
                {
                    InitVariable();
                }
                return m_timedVariable;
            }
        }
        private GenericTimedVariable<TSnap, TSnapData> m_timedVariable = null;
        private void InitVariable()
        {
            m_timedVariable = new GenericTimedVariable<TSnap, TSnapData>(
                (TSnapData data, float time) => CreateSnapshot(data, time), 
                GatherCurrentData(), !timedObject.shouldRecord, m_isDebuggingIndividual);
            m_timedVariable.debugName = $"{name}/{GetType().Name}";
        }


        private void Start()
        {
            if (m_timedVariable == null)
            {
                InitVariable();
            }
        }


        public virtual void TrimDataAfter(float time) { }
        public virtual void OnRecordingResume(float time) { }
        public virtual void OnRecordingStop(float time) { }
        public virtual void SetToTime(float time)
        {
            if (isRecording)
            {
                // In some cases, we don't want to set curData every frame.
                // For example, if we are storing transform data and moving via physics we only want to set curData every FixedUpdate.
                // BUT if the data is being changed BOTH via physics AND via update calls, then we want to set curData every FixedUpdate OR if the data has been changed.
                bool t_shouldSet;
                if (shouldObserveFixedUpdate)
                {
                    TSnapData t_newData = GatherCurrentData();
                    if (t_newData.Equals(timedVariable.curData))
                    {
                        // Data is equal to last one, so only want to set if fixed upate is dirty.
                        t_shouldSet = m_isFixedUpdateDirty;
                    }
                    else
                    {
                        // Data is not equal, so we want to set.
                        t_shouldSet = true;
                    }
                }
                else
                {
                    t_shouldSet = true;
                }

                if (t_shouldSet)
                {
                    timedVariable.curData = GatherCurrentData();
                }
            }
            else
            {
                ApplySnapshotData(timedVariable.curData);
            }
            m_amountSnapshots = timedVariable.scrapbook.Length;
            if (m_isDebuggingIndividual)
            {
                m_debugSnapData.Clear();
                foreach (TSnap t_snap in timedVariable.scrapbook.GetInternalData())
                {
                    if (t_snap.data is TransformData t_transData)
                    {
                        m_debugSnapData.Add(t_transData.position);
                    }
                }
            }

            m_isFixedUpdateDirty = false;
        }
        public void ClearAllRecordedData() => timedVariable.ClearAllRecordedData();

        private void FixedUpdate()
        {
            if (isRecording)
            {
                #region Logs
                //CustomDebug.LogForComponent($"[{Time.frameCount}] [{curTime}] FixedUpdate", this, m_isDebuggingIndividual);
                #endregion Logs
                m_isFixedUpdateDirty = true;
            }
        }


        /// <summary>
        /// Called when a snapshot needs to be taken of the current state.
        /// 
        /// Should create and return a new snapshot for the current time.
        /// </summary>
        protected abstract TSnapData GatherCurrentData();

        /// <summary>
        /// Should simply new up a snap with the given data and time.
        /// </summary>
        protected abstract TSnap CreateSnapshot(TSnapData data, float time);
        /// <summary>
        /// Called when not currently recording and instead is trying to replay.
        /// 
        /// Should apply the given snapshot data.
        /// </summary>
        protected abstract void ApplySnapshotData(TSnapData snapData);

        protected virtual bool shouldObserveFixedUpdate => false;
    }
}
