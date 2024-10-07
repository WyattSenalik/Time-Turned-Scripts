using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(short)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/short")]
    public sealed class GameEventIdentifierShortSO : GameEventIdentifierSO<short>
    { }
}