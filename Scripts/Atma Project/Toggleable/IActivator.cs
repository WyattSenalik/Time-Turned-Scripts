using Helpers.UnityInterfaces;

namespace Atma
{
    public interface IActivator : IMonoBehaviour
    {
        bool isActive { get; }

        public void Activate();

        public void Deactivate();

    }
}