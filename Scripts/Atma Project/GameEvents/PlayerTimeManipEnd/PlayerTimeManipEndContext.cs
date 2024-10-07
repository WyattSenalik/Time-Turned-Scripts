// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// Parameter to pass in the PlayerTimeManipEnd event
    /// </summary>
    public sealed class PlayerTimeManipEndContext : IPlayerTimeManipEndContext
    {
        public float timeManipEnded { get; private set; }


        public PlayerTimeManipEndContext(float timeEnded)
        {
            timeManipEnded = timeEnded;
        }
    }
}