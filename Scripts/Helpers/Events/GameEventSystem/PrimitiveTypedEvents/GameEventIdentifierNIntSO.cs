using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(nint)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/nint")]
    public sealed class GameEventIdentifierNIntSO : GameEventIdentifierSO<nint>
    { }
}