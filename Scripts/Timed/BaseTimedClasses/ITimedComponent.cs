using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Behaviour meant to be attached to a <see cref="ITimedObject"/>.
    /// Has some a reference to the <see cref="ITimedObject"/>.
    /// </summary>
    public interface ITimedComponent : IMonoBehaviour
    {
        /// <summary>
        /// Reference to the <see cref="ITimedObject"/> this is attached to.
        /// </summary>
        ITimedObject timedObject { get; }
        /// <summary>
        /// If this <see cref="ITimedObject"/> is recording (true) or replaying (false).
        /// </summary>
        bool isRecording { get; }
    }
}
