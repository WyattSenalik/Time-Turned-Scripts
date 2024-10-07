// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Parameter to pass in the CloneDisconnected event.
    /// </summary>
    public sealed class CloneDisconnectedContext : ICloneDisconnectedContext
    {
        public TimeClone timeClone { get; private set; }

        public CloneDisconnectedContext(TimeClone timeClone)
        {
            this.timeClone = timeClone;
        }
    }
}