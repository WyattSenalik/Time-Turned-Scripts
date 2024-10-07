using Helpers.Events;
using Helpers.UnityInterfaces;
using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public interface ILeaper : IMonoBehaviour
    {
        IEventPrimer onLeapBegin { get; }
        IEventPrimer onLeapEnd { get; }

        public void Leap(eEightDirection leapDir, ILeapObject objLeaptFrom);
    }
}