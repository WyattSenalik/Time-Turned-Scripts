using Helpers.Events;
using Helpers.UnityInterfaces;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Interface for controlling time manipulation.
    /// </summary>
    public interface ITimeManipController : IMonoBehaviour
    {
        IEventPrimer onSkipToBegin { get; }
        ITimeRewinder rewinder { get; }

        void BeginTimeManipulation(bool isFirstPause = false);
        void CreateTimeClone();
        void SkipToBeginning();
        void Rewind();
        void Play();
        void Pause();
        void FastForward();
        void SkipToEnd();
    }
}