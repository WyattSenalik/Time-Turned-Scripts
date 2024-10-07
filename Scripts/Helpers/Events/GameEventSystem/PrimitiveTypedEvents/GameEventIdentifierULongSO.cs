using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(ulong)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/ulong")]
    public sealed class GameEventIdentifierULongSO : GameEventIdentifierSO<ulong>
    { }
}