// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Parameter to pass in the CloneDeleted event.
    /// </summary>
    public sealed class CloneDeletedContext : ICloneDeletedContext
    {
        public TimeClone timeClone { get; private set; }

        public CloneDeletedContext(TimeClone timeClone)
        {
            this.timeClone = timeClone;
        }
    }
}
