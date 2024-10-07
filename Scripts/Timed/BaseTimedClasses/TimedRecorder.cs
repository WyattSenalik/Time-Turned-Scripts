// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Default implementation of <see cref="ITimedRecorder"/>.
    /// Can optionally override ITimedRecorder functions as desired.
    /// </summary>
    public abstract class TimedRecorder : TimedComponent, ITimedRecorder
    {
        #region TimedRecorder
        public virtual void TrimDataAfter(float time) { }
        public virtual void OnRecordingResume(float time) { }
        public virtual void OnRecordingStop(float time) { }
        public virtual void SetToTime(float time) { }
        #endregion ISnapshotRecorder
    }
}
