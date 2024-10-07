// Original Authors - Wyatt Senalik

namespace Timed
{
    public interface ITimedRecorder : ITimedComponent
    {
        /// <summary>
        /// Updates the <see cref="ITimedRecorder"/> to be at the given time.
        /// </summary>
        void SetToTime(float time);
        /// <summary>
        /// Called when the recording stops.
        /// Recording stops when a jump to a previous time occurs or when
        /// time starts moving in reverse.
        /// </summary>
        /// <param name="time">Time when the recording stopped.</param>
        void OnRecordingStop(float time);
        /// <summary>
        /// Called when the recording was stopped but has begun again.
        /// </summary>
        /// <param name="time">Time when the recording was resumed.</param>
        void OnRecordingResume(float time);
        /// <summary>
        /// Called when the data this behaviour is storing should
        /// have all data after (and including) the given time thrown away. 
        /// </summary>
        void TrimDataAfter(float time);
    }
}
