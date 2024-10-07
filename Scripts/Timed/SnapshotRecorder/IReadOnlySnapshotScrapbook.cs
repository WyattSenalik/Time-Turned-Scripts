using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// ReadOnly Collection of snapshots.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot type.</typeparam>
    /// <typeparam name="TSnapType">Type of the snapshot's data.</typeparam>
    public interface IReadOnlySnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        int Count { get; }
        int Length => Count;
        int Size => Count;

        /// <summary>
        /// Gets an interpolated snapshot at the given time.
        /// Uses other snapshot data to determine what this time should be.
        /// </summary>
        /// <param name="time">Time to get a snapshot for.</param>
        TSnap GetSnapshot(float time);
        /// <summary>
        /// Gets the time of the latest (in time) snapshot added.
        /// </summary>
        float GetLatestTime();
        /// <summary>
        /// Returns all saved snapshots after the given time.
        /// </summary>
        /// <param name="includeInterpolatedAtGivenTime">If an interpolated snapshot
        /// at the given time should also be included as the first element in the
        /// returned array.</param>
        TSnap[] GetAllSnapshotsAfter(float time,
            bool includeInterpolatedAtGivenTime);
        /// <summary>
        /// Returns the snapshot before the given time and after the given time.
        /// </summary>
        /// <param name="hasBeforeSnap">If there exists a snap before the time.</param>
        /// <param name="hasAfterSnap">If there exists a snap after the time.</param>
        /// <param name="beforeSnap">Snap with a time less than the given time.</param>
        /// <param name="afterSnap">Snap with a time greater than or equal to the given time.</param>
        void GetSnapshotsBeforeAndAfter(float time, out bool hasBeforeSnap, 
            out bool hasAfterSnap, out TSnap beforeSnap, out TSnap afterSnap);
        /// <summary>
        /// Gets the latests stored (non-interpolated) snapshot.
        /// </summary>
        TSnap GetLatestSnapshot();
        /// <summary>
        /// Gets the non-interpolated snapshot held in internal data at the given index.
        /// </summary>
        TSnap GetSnapshotAtIndex(int index);


        ICollection<TSnap> GetInternalData();
    }
}
