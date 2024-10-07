// Original Authors - Wyatt Senalik

namespace Timed
{
    public interface IWindowCollection<TData> : IReadOnlyWindowCollection<TData>
    {
        void Add(WindowData<TData> windowData);
        int RemoveAfterTime(float time);
    }
}
