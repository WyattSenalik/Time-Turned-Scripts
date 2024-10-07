// Original Authors - Wyatt Senalik

namespace Helpers.Events.IDEvents
{
    /// <summary>
    /// An event that ties its callbacks to ids instead of references to the callbacks themselves.
    /// </summary>
    public interface IIDEvent : IReadOnlyIDEvent
    {
        /// <summary>
        /// Invokes the event to call all the callbacks.
        /// </summary>
        void Invoke();
    }
}
