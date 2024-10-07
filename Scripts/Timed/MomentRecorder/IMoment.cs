using System;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Moment that stores data and the time the data was stored.
    /// </summary>
    /// <typeparam name="TData">Data that is being stored in the moment.</typeparam>
    /// <typeparam name="TSelf">Type of the moment.</typeparam>
    public interface IMoment<TSelf> where TSelf : IMoment<TSelf>
    {
        /// <summary>
        /// Timestamp the moment was taken.
        /// </summary>
        float time { get; }

        /// <summary>
        /// Causes the moment to take some action.
        /// </summary>
        void Do();
        /// <summary>
        /// Reverses whatever action was done in <see cref="Do"/>.
        /// </summary>
        void Undo();
        /// <summary>
        /// Gets rid of the moment and cleans up anything that needs to be
        /// cleaned up or released.
        /// </summary>
        void Destroy(float destroyTime);
        /// <summary>
        /// Returns an exact clone of this moment.
        /// </summary>
        TSelf Clone();
    }
}
