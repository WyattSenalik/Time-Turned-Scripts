using Helpers.UnityInterfaces;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Element to be spawned by <see cref="LifetimeManager"/> that initializes itself to be a display for the clone's lifetime that it represents.
    /// </summary>
    public interface ILifetimeUIElement : IMonoBehaviour
    {
        TimeFrame frame { get; }
        int chargeIndex { get; }
        ITimeRewinder timeRewinder { get; }
        float startTime => frame.startTime;
        float endTime => frame.endTime;

        void Initialize(float startTime, float endTime, int chargeIndex, ITimeRewinder timeRewinder) => Initialize(new TimeFrame(startTime, endTime), chargeIndex, timeRewinder);
        void Initialize(TimeFrame frame, int chargeIndex, ITimeRewinder timeRewinder);
    }
}
