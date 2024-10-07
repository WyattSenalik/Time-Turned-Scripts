// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// <see cref="ITimedRecorder"/> that uses Snapshots to store data.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot type.</typeparam>
    /// <typeparam name="TSnapType">Type of the snapshot's data.</typeparam>
    public interface ISnapshotRecorder<TSnap, TSnapType> : ITimedRecorder
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        IReadOnlySnapshotScrapbook<TSnap, TSnapType> scrapbook { get; }
    }
}
