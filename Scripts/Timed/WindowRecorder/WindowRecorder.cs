using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// <see cref="TimedRecorder"/> that stores sequential <see cref="WindowData{T}"/>.
    /// </summary>
    public abstract class WindowRecorder<TData> : TimedRecorder, IWindowRecorder<TData>
    {
        public WindowData<TData> mostRecentWindow => m_windowCollection.mostRecentWindow;
        public IReadOnlyWindowCollection<TData> windowCollection => m_windowCollection;
        public WindowData<TData> timeAwareMostRecentWindow
        {
            get
            {
                if (isRecording)
                {
                    return mostRecentWindow;
                }
                windowCollection.TryGetWindow(curTime, out WindowData<TData> t_window);
                return t_window;
            }
        }

        [ShowNativeProperty] public int windowCount => m_windowCollection.Count;

        private readonly IWindowCollection<TData> m_windowCollection = new WindowCollection<TData>();


        protected void StartNewWindow(TData data)
        {
            m_windowCollection.Add(new WindowData<TData>(curTime, data));
        }
        protected void EndCurrentWindow()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(mostRecentWindow != null, $"most recent window to be non-null", this);
            //CustomDebug.AssertIsTrueForComponent(mostRecentWindow.window.endTime == float.PositiveInfinity, $"end time for most recent window to be uninitialized. Instead it was {mostRecentWindow.window.endTime}", this);
            #endregion Asserts
            mostRecentWindow.SetWindowEndTime(curTime);
        }
        protected bool HasCurrentWindow()
        {
            // We have a current window if there is a most recent window that has not been initialized.
            return mostRecentWindow != null && mostRecentWindow.window.endTime == float.PositiveInfinity;
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // If rewinding
            if (!isRecording)
            {
                if (windowCollection.TryGetWindow(time, out WindowData<TData> t_curWindow))
                {
                    SetToTimeRewindingDuringWindow(t_curWindow);
                }
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            m_windowCollection.RemoveAfterTime(time);
            if (HasCurrentWindow() && windowCollection.mostRecentWindow.ContainsTime(time))
            {
                OnRecordingResumeDuringWindow(windowCollection.mostRecentWindow);
            }
        }


        protected virtual void OnRecordingResumeDuringWindow(WindowData<TData> windowResumedDuring) { }
        protected virtual void SetToTimeRewindingDuringWindow(WindowData<TData> windowData) { }
    }
}
