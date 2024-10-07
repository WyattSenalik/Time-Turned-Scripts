using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(char)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/char")]
    public sealed class GameEventIdentifierCharSO : GameEventIdentifierSO<char>
    { }
}