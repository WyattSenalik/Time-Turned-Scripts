using System.Collections;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Collection of <see cref="WindowData{T}"/> that is stored sequentially.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public sealed class WindowCollection<TData> : IWindowCollection<TData>
    {
        private readonly List<WindowData<TData>> m_windowDataList = new List<WindowData<TData>>();

        #region IWindowCollection
        public void Add(WindowData<TData> windowData)
        {
            #region Asserts
            if (mostRecentWindow != null)
            {
                //CustomDebug.AssertIsTrueForObj(mostRecentWindow.window.endTime <= windowData.window.startTime, $"to only add windows that start ({windowData.window.startTime}) after the previous window ends ({mostRecentWindow.window.endTime})", this);
            }
            #endregion Asserts
            m_windowDataList.Add(windowData);
        }
        public int RemoveAfterTime(float time)
        {
            int t_amRemoved = 0;
            int t_index = 0;
            while (t_index < m_windowDataList.Count)
            {
                WindowData<TData> curWindData = m_windowDataList[t_index];
                // Time is before the window even starts, remove the window entirely
                if (time < curWindData.window.startTime)
                {
                    m_windowDataList.RemoveAt(t_index);
                    ++t_amRemoved;
                }
                // Time is during the window, reset the window's end time
                else if (time < curWindData.window.endTime)
                {
                    curWindData.ResetWindowEndTime();
                    ++t_index;
                }
                // TODO: If the time is exactly when the window ends?
                // Time is after the window, simply move to the next one
                else
                {
                    ++t_index;
                }
            }
            return t_amRemoved;
        }
        #endregion IWindowCollection

        #region IReadOnlyWindowCollection
        public WindowData<TData> mostRecentWindow
        {
            get
            {
                if (m_windowDataList.Count > 0)
                {
                    return m_windowDataList[m_windowDataList.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public WindowData<TData>[] GetAllWindows() => m_windowDataList.ToArray();
        public int GetWindowsInFrame(TimeFrame frame, out WindowData<TData>[] windowsInFrame)
        {
            List<WindowData<TData>> t_listWindsInFrame = new List<WindowData<TData>>();
            foreach (WindowData<TData> window in m_windowDataList)
            {
                if (window.HasOverlap(frame))
                {
                    t_listWindsInFrame.Add(window);
                }
            }
            windowsInFrame = t_listWindsInFrame.ToArray();
            return t_listWindsInFrame.Count;
        }
        public WindowData<TData> GetWindow(float time) => GetWindow(time, eTimeFrameContainsOption.Inclusive);
        public WindowData<TData> GetWindow(float time, eTimeFrameContainsOption option)
        {
            foreach (WindowData<TData> window in m_windowDataList)
            {
                if (window.ContainsTime(time, option))
                {
                    return window;
                }
            }
            return null;
        }
        public bool TryGetWindow(float time, out WindowData<TData> windowData)
        {
            foreach (WindowData<TData> window in m_windowDataList)
            {
                if (window.ContainsTime(time))
                {
                    windowData = window;
                    return true;
                }
            }
            windowData = null;
            return false;
        }
        #endregion IReadOnlyWindowCollection

        #region IReadOnlyCollection
        public int Count => m_windowDataList.Count;
        public IEnumerator<WindowData<TData>> GetEnumerator() => m_windowDataList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_windowDataList.GetEnumerator();
        #endregion IReadOnlyCollection
    }
}