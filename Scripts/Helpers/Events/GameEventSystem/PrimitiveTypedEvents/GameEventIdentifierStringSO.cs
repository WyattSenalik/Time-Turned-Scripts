using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(string)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/string")]
    public sealed class GameEventIdentifierStringSO : GameEventIdentifierSO<string>
    { }
}