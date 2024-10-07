using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(decimal)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/decimal")]
    public sealed class GameEventIdentifierDecimalSO : GameEventIdentifierSO<decimal>
    { }
}