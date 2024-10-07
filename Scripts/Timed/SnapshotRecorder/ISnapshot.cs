// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Snapshot that stores data and the time the data was stored.
    /// </summary>
    /// <typeparam name="TData">Data that is being stored in the snapshot.</typeparam>
    /// <typeparam name="TSelf">Type of the snapshot.</typeparam>
    public interface ISnapshot<TData, TSelf> where TSelf : ISnapshot<TData, TSelf>
    {
        /// <summary>
        /// Timestamp the snapshot was taken.
        /// </summary>
        float time { get; }
        /// <summary>
        /// Data stored in the snapshot.
        /// </summary>
        TData data { get; }
        /// <summary>
        /// How to interpolate if interpolating from the right of this snapshot.
        /// </summary>
        eInterpolationOption interpolationOpt { get; set; }

        /// <summary>
        /// Interpolates two snapshots and returns the result of the
        /// interpolation.
        /// </summary>
        /// <param name="other">The other snapshot to interpolate for.</param>
        /// <param name="targetTime">Target time to interpolate to.</param>
        /// <returns></returns>
        TSelf Interpolate(TSelf other, float targetTime);
        /// <summary>
        /// Returns an exact clone of this snapshot.
        /// </summary>
        TSelf Clone();
        /// <summary>
        /// Returns an exact clone of this snapshot but at the given time.
        /// </summary>
        TSelf Clone(float timeToUseForClone);
    }
}
