using System;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// An easier way to track data without needing to use <see cref="SnapshotRecorder{TSnap, TSnapData}"/>.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot type.</typeparam>
    /// <typeparam name="TSnapData">Type of the snapshot's data.</typeparam>
    public abstract class TimedVariable<TSnap, TSnapData> : TimedClass
        where TSnap : ISnapshot<TSnapData, TSnap>
        where TSnapData : IEquatable<TSnapData>
    {
        public float lastSnapTime => m_scrapbook.GetLatestTime();
        public IReadOnlySnapshotScrapbook<TSnap, TSnapData> scrapbook => m_scrapbook;
        public string debugName { get; set; } = "";

        protected bool m_isDebugging = false;


        protected readonly SnapshotScrapbook<TSnap, TSnapData> m_scrapbook = new SnapshotScrapbook<TSnap, TSnapData>();
        private readonly bool m_shouldRecordEverySnap = false;


        public TimedVariable(TSnapData initialData, bool shouldNeverRecord = false) : base(shouldNeverRecord)
        {
            float t_curTime = curTime;
            m_scrapbook.AddSnapshot(CreateSnapshot(initialData, t_curTime));
        }
        public TimedVariable(bool shouldNeverRecord = false, bool shouldRecordEverySnap = false, bool isDebugging = false) : base(shouldNeverRecord)
        {
            m_isDebugging = isDebugging;
            m_shouldRecordEverySnap = shouldRecordEverySnap;
        }
        public TimedVariable(IReadOnlySnapshotScrapbook<TSnap, TSnapData> scrapbookToCopy, bool shouldNeverRecord = false) : base(shouldNeverRecord)
        {
            foreach (TSnap t_snap in scrapbookToCopy.GetInternalData())
            {
                m_scrapbook.AddSnapshot(t_snap);
            }
            TSnap t_latestSnap = m_scrapbook.GetLatestSnapshot();
            if (shouldNeverRecord)
            {
                t_latestSnap.interpolationOpt = eInterpolationOption.Step;
            }
        }
        public TimedVariable(TimedVariable<TSnap, TSnapData> varToCopy, bool shouldNeverRecord = false) : this(varToCopy.scrapbook, shouldNeverRecord) { }


        public TSnapData curData
        {
            get
            {
                if (m_scrapbook.Count <= 0) { return default; }
                float t_curTime = curTime;
                if (lastSnapTime == t_curTime)
                {
                    return m_scrapbook.GetLatestSnapshot().data;
                }
                return m_scrapbook.GetSnapshot(t_curTime).data;
            }
            set
            {
                // Can't set when not recording.
                if (!isRecording) { return; }
                // Don't add a new snapshot if the current time already has a snapshot.
                float t_curTime = curTime;
                if (t_curTime == lastSnapTime) { return; }
                // Record every snap.
                TSnap t_newSnap = CreateSnapshot(value, t_curTime);

                if (m_shouldRecordEverySnap)
                {
                    m_scrapbook.AddSnapshot(t_newSnap);
                    return;
                }

                bool t_shouldAdd = true;
                int t_snapCount = m_scrapbook.Count;
                if (t_snapCount >= 1)
                {
                    int t_prevSnapIndex = t_snapCount - 1;
                    TSnap t_prevSnap = m_scrapbook.GetSnapshotAtIndex(t_prevSnapIndex);
                    if (t_newSnap.interpolationOpt == eInterpolationOption.Step && t_prevSnap.interpolationOpt == eInterpolationOption.Step)
                    {
                        // If the previous snap was step and this snap is also step, there are only 2 outcomes:
                        // 1. They are the same, and we do NOT need to add the new snapshot.
                        // 2. They are different and we DO need to add the new shapshot
                        if (t_newSnap.data.Equals(t_prevSnap.data))
                        {
                            t_shouldAdd = false;
                        }
                    }
                    else if (t_snapCount >= 2)
                    {
                        int t_prevPrevSnapIndex = t_prevSnapIndex - 1;
                        TSnap t_prevPrevSnap = m_scrapbook.GetSnapshotAtIndex(t_prevPrevSnapIndex);
                        TSnap t_interpolatedSnapAtPrevSnapPos = t_prevPrevSnap.Interpolate(t_newSnap, t_prevSnap.time);
                        if (t_interpolatedSnapAtPrevSnapPos.data.Equals(t_prevSnap.data))
                        {
                            // They are equal, safe to remove the prev snap data.
                            m_scrapbook.RemoveSnapshotAtIndex(t_prevSnapIndex);
                        }
                    }
                }
                if (t_shouldAdd)
                {
                    m_scrapbook.AddSnapshot(t_newSnap);
                }
            }
        }

        public void ReplaceDataBefore(float time) => m_scrapbook.ReplaceDataBefore(time);
        public void ClearAllRecordedData() => m_scrapbook.Clear();


        protected override void OnRecordingResume()
        {
            base.OnRecordingResume();

            float t_curTime = curTime;
            // Take a snapshot at the current time
            TSnap t_snapAtResumeTime = m_scrapbook.GetSnapshot(t_curTime);
            // Remove the snapshots at and after the current time.
            m_scrapbook.RemoveSnapshotsAfter(t_curTime);
            // Add the snapshot for the resume time
            if (t_snapAtResumeTime != null)
            {
                m_scrapbook.AddSnapshot(t_snapAtResumeTime);
            }
        }
        protected override void OnRecordingStop()
        {
            base.OnRecordingStop();

            if (m_scrapbook.Count <= 0) { return; }

            // Add a snapshot on stop (unless the unlikely scenario occurs where the most recent snapshot was taken on this exact frame).
            float t_curTime = curTime;
            if (m_scrapbook.GetLatestTime() != t_curTime)
            {
                TSnap t_actingSnap = CreateSnapshot(curData, t_curTime);
                TSnap t_snapAtStop = m_scrapbook.GetSnapshot(t_curTime);
                if (!t_actingSnap.data.Equals(t_snapAtStop.data))
                {
                    m_scrapbook.AddSnapshot(t_actingSnap);
                }
            }
        }

        /// <summary>
        /// Creates a snapshot with the given data and at time.
        /// </summary>
        protected abstract TSnap CreateSnapshot(TSnapData data, float time);


        public override string ToString()
        {
            return $"CurTime: {curTime}; CurData: {curData}; Scrapbook:\n{m_scrapbook}";
        }
    }



    public sealed class GenericTimedVariable<TSnap, TSnapData> : TimedVariable<TSnap, TSnapData>
        where TSnap : ISnapshot<TSnapData, TSnap>
        where TSnapData : IEquatable<TSnapData>
    {
        private readonly Func<TSnapData, float, TSnap> m_createSnapshotFunc = null;

        public GenericTimedVariable(Func<TSnapData, float, TSnap> createSnapshotFunc, TSnapData initialData, bool shouldNeverRecord = false, bool isDebugging = false) : base(shouldNeverRecord, isDebugging)
        {
            m_createSnapshotFunc = createSnapshotFunc;
            m_scrapbook.AddSnapshot(CreateSnapshot(initialData, curTime));
        }


        protected override TSnap CreateSnapshot(TSnapData data, float time)
        {
            return m_createSnapshotFunc.Invoke(data, time);
        }
    }
}