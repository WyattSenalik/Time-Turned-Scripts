// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Context data for when a PlayerTimeManipEnd event has been called.
    /// </summary>
    public interface IPlayerTimeManipEndContext
    {
        /// <summary>
        /// Time the player ended their time manipulation.
        /// </summary>
        float timeManipEnded { get; }
    }
}