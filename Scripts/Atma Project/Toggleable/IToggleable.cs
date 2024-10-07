using Helpers.UnityInterfaces;
// Original Authors -?
// Tweaked by Wyatt Senalik

namespace Atma
{
    public interface IToggleable : IMonoBehaviour
    {
        bool isOn { get; }

        public void Activate();
        public void Deactivate();

    }
}