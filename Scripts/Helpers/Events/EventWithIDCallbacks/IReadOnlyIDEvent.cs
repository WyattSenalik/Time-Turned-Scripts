using System;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.IDEvents
{
    /// <summary>
    /// An event that ties its callbacks to ids instead of references to the callbacks themselves.
    /// ReadOnly version (cannot invoke).
    /// </summary>
    public interface IReadOnlyIDEvent
    {
        /// <summary>
        /// Get an id that is not currently being used by any callback.
        /// </summary>
        int GetUnusedID();
        /// <summary>
        /// Subscribes the given action to the event.
        /// A previously unused ID is returned that can be used to unsubscribe
        /// the action.
        /// </summary>
        int Subscribe(Action triggerAction);
        /// <summary>
        /// Subscribes the given action to the event.
        /// The id is what can be used to unsubscribe the action.
        /// </summary>
        void Subscribe(int id, Action triggerAction);
        /// <summary>
        /// Unsubscribe the action with the given id from the event.
        /// </summary>
        void Unsubscribe(int id);
    }
}
