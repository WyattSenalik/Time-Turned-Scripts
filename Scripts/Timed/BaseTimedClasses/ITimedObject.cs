using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Object that <see cref="ITimedComponent"/>s, <see cref="ITimedObserver"/>s,
    /// and <see cref="ITimedRecorder"/>s can be attached to.
    /// </summary>
    public interface ITimedObject : IMonoBehaviour
    {
        /// <summary>
        /// If time is advancing past this time object has seen yet.
        /// Indicates when a script can change data freely (true) and when
        /// it instead needs to look at past stored data (false) and update itself
        /// using that past data.
        /// </summary>
        public bool isRecording { get; }
        /// <summary>
        /// <see cref="isRecording"/> from the previous UpdateToTime call.
        /// </summary>
        public bool wasRecording { get; }
        /// <summary>
        /// If this <see cref="ITimedObject"/> should be recording when possible.
        /// </summary>
        public bool shouldRecord { get; set; }
        /// <summary>
        /// Current time.
        /// </summary>
        public float curTime { get; }
        /// <summary>
        /// Time this <see cref="ITimedObject"/> was created.
        /// </summary>
        public float spawnTime { get; }
        /// <summary>
        /// Furthest time this timed object has seen.
        /// </summary>
        public float farthestTime { get; }
        /// <summary>
        /// Time scale that the timed system is operating at.
        /// </summary>
        public float timeScale { get; }
        public float deltaTime { get; }
        /// <summary>[SpawnTime, FarthestTime]</summary>
        public TimeFrame lifeTimeFrame => new TimeFrame(spawnTime, farthestTime);

        /// <summary>
        /// Updates all <see cref="ITimedObserver"/>s and
        /// <see cref="ITimedRecorder"/>s attached to this 
        /// <see cref="ITimedObject"/> to the given time.
        /// </summary>
        public void SetToTime(float time, float deltaTime);
        /// <summary>
        /// If this object can record, turns off recording for this object
        /// until it is turned back on by the same requester. If there is at
        /// least 1 requester, recording will be suspended.
        /// </summary>
        /// <returns>An id that should be used to cancel the request.</returns>
        public int RequestSuspendRecording();
        /// <summary>
        /// Cancels a previous request to suspend recording.
        /// </summary>
        /// <param name="requestId">The id given by 
        /// <see cref="RequestSuspendRecording"/> when the recording was
        /// suspended.</param>
        public void CancelSuspendRecordingRequest(int requestId);
        /// <summary>
        /// Forcefully changes <see cref="spawnTime"/> and <see cref="farthestTime"/>.
        /// Don't use unless you know what you are doing.
        /// </summary>
        public void ForceSetTimeBounds(float newSpawnTime, float newFarthestTime);
        /// <summary>
        /// Forcefully calls <see cref="ITimedRecorder.TrimDataAfter(float)"/> 
        /// for all <see cref="ITimedRecorder"/>s attached to this object.
        /// Also forcefully changes <see cref="farthestTime"/> to the given time.
        /// </summary>
        void TrimDataAfter(float time);
    }
}
