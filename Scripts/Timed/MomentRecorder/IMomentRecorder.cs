// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Recorder that stores data in <see cref="IMoment{TData, TSelf}"/>s.
    /// </summary>
    public interface IMomentRecorder<TMomentSelf> : ITimedRecorder
        where TMomentSelf : IMoment<TMomentSelf>
    {
        /// <summary>
        /// Album the <see cref="IMoment{TSelf}"/>s are stored in.
        /// </summary>
        IReadOnlyMomentAlbum<TMomentSelf> album { get; }
    }
}