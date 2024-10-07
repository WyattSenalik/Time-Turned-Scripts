using UnityEngine.UI;

using Helpers.Events;
using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Time Slider that knows what state of interaction the player has with the UI slider.
    /// </summary>
    public interface ITimeSlider : IMonoBehaviour
    {
        eInteractState interactState { get; }
        IEventPrimer onSliderValChanged { get; }
        Slider slider { get; }

        public enum eInteractState { None, EndHandle }
    }
}