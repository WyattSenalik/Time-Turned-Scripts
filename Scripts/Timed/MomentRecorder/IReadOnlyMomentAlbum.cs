// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// ReadOnly Collection of moments.
    /// </summary>
    /// <typeparam name="TMomentSelf">Type of the moment.</typeparam>
    public interface IReadOnlyMomentAlbum<TMomentSelf>
        where TMomentSelf : IMoment<TMomentSelf>
    {
        int Count { get; }
        int Length => Count;
        int Size => Count;

        /// <summary>
        /// Gets all moments that occured between the two given times.
        /// </summary>
        /// <param name="lowerTime">Time before the moments (exclusive).</param>
        /// <param name="upperTime">Time after the moments (inclusive).</param>
        TMomentSelf[] Get(float lowerTime, float upperTime);
        /// <summary>
        /// Gets the time of the latest (in time) moment added. Returns -1.0f if 
        /// there are no moments in the album.
        /// </summary>
        float GetLatestTime();

        /// <summary>
        /// Returns all the moments. ONLY USE IF YOU KNOW WHAT YOU ARE DOING.
        /// </summary>
        TMomentSelf[] GetInternalData();
        /// <summary>
        /// Returns the moment at the latest time.
        /// </summary>
        TMomentSelf GetLatestMoment();
        /// <summary>
        /// Returns the latest moment that takes place before the given time.
        /// </summary>
        TMomentSelf GetMomentBeforeOrAtTime(float time);
    }
}
