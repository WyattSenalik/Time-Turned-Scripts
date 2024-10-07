using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(long)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/long")]
    public sealed class GameEventIdentifierLongSO : GameEventIdentifierSO<long>
    { }
}