// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class WindowData<T>
    {
        public const float UNINIT_END_TIME = float.PositiveInfinity;

        public TimeFrame window { get; private set; }
        public T data { get; private set; }


        public WindowData(TimeFrame window, T data)
        {
            this.window = window;
            this.data = data;
        }
        public WindowData(float startTime, T data)
        {
            this.window = new TimeFrame(startTime, UNINIT_END_TIME);
            this.data = data;
        }


        public bool ContainsTime(float time) => window.ContainsTime(time);
        public bool ContainsTime(float time, eTimeFrameContainsOption containsOpt) => window.ContainsTime(time, containsOpt);
        public bool HasOverlap(TimeFrame other) => window.HasOverlap(other);
        public void SetWindowEndTime(float newEndTime) => window = new TimeFrame(window.startTime, newEndTime);
        public void ResetWindowEndTime() => SetWindowEndTime(UNINIT_END_TIME);
    }
}