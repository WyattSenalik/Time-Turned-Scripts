// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Parameter to pass in the CloneCreated event.
    /// </summary>
    public sealed class CloneCreatedContext : ICloneCreatedContext
    {
        public float timeCreated { get; private set; }
        public TimeClone timeClone { get; private set; }


        public CloneCreatedContext(float timeCreated, TimeClone timeClone)
        {
            this.timeCreated = timeCreated;
            this.timeClone = timeClone;
        }
    }
}