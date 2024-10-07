// Original Authors - Wyatt Senalik

namespace Helpers.Events.CatchupEvents
{
    /// <summary>
    /// Interface for <see cref="CatchupEvent"/>s to reset themselves.
    /// </summary>
    public interface ICatchupEventReset
    {
        public void Reset();
    }
}