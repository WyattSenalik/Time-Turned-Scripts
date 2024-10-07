using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Holds if the player movement is currently suspended or not. Movement can be
    /// suspended for an amount of time, or suspended indefinitely until told to
    /// unsuspend (using ids).
    /// </summary>
    public interface IMovementSuspender : IMonoBehaviour
    {
        bool isMovementSuspended { get; }
        bool shouldClearMovementMemory { get; }


        public void SuspendForTime(float timeToSusFor, bool clearMovementMemory = true);
        public int RequestSuspension();
        public void CancelSuspension(int suspensionID);
    }
}