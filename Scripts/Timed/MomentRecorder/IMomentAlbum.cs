// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Collection of moments.
    /// </summary>
    /// <typeparam name="TMomentSelf">Type of the moment.</typeparam>
    public interface IMomentAlbum<TMomentSelf> : IReadOnlyMomentAlbum<TMomentSelf>
        where TMomentSelf : IMoment<TMomentSelf>
    {
        /// <summary>
        /// Adds the given moment to the scrapbook.
        /// </summary>
        void AddMoment(TMomentSelf moment);
        /// <summary>
        /// Removes all moments after (and including) the given time.
        /// </summary>
        /// <returns>The amount of moments removed from the album.</returns>
        int RemoveMomentsAfter(float time);
    }
}
