// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Context data for when a CloneDeleted event has been called.
    /// </summary>
    public interface ICloneDeletedContext
    {
        /// <summary>
        /// Reference to the time clone itself before it gets deleted.
        /// </summary>
        TimeClone timeClone { get; }
    }
}
