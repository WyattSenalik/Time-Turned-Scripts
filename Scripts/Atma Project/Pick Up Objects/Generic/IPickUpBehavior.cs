// Original Authors - Bryce Cernohous-Schrader
// Tweaked by Wyatt Senalik

namespace Atma
{
    public interface IPickUpBehavior
    {
        void ThrowMe();
        void TakeAction(bool onPressedOrReleased);
        void OnPickUp();
        void OnReleased();
    }
}