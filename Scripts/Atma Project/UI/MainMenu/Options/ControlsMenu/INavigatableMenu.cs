using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    public interface INavigatableMenu : IMonoBehaviour
    {
        void OnPointerEnterOption(NavigatableMenuOption option);
        void OnPointerExitOption(NavigatableMenuOption option);
        void OnPointerClickOption(NavigatableMenuOption option);
    }
}