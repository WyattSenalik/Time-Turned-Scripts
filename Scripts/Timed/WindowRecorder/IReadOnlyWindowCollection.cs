using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// ReadOnly collection of <see cref="WindowData{T}"/>. Must be stored sequentially.
    /// </summary>
    public interface IReadOnlyWindowCollection<TData> : IReadOnlyCollection<WindowData<TData>>
    {
        WindowData<TData> mostRecentWindow { get; }

        int GetWindowsInFrame(TimeFrame frame, out WindowData<TData>[] windowsInFrame);
        WindowData<TData>[] GetAllWindows();
        WindowData<TData> GetWindow(float time);
        WindowData<TData> GetWindow(float time, eTimeFrameContainsOption option);
        bool TryGetWindow(float time, out WindowData<TData> windowData);
    }
}
