// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Collection of snapshots.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot type.</typeparam>
    /// <typeparam name="TSnapType">Type of the snapshot's data.</typeparam>
    public interface ISnapshotScrapbook<TSnap, TSnapType> : 
        IReadOnlySnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        /// <summary>
        /// Adds the given snapshot to the scrapbook.
        /// </summary>
        void AddSnapshot(TSnap snapshot);
        /// <summary>
        /// Removes all snapshots after (and including) the given time.
        /// </summary>
        /// <returns>The amount of snapshots removed from the scrapbook.</returns>
        int RemoveSnapshotsAfter(float time, bool includeSnapAtTime = true);

        /// <summary>
        /// Removes the snapshot at the given index.
        /// BE CAREFUL WHEN USING.
        /// </summary>
        void RemoveSnapshotAtIndex(int index);
        /// <summary>
        /// Inserts the snapshot at the given index into the internal data.
        /// BE CAREFUL WHEN USING.
        /// </summary>
        void InsertSnapshotAtIndex(int index, TSnap snap);
    }
}
