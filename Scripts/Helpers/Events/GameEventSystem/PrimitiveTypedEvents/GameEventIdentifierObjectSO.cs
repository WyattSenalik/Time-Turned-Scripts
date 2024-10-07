using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(object)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/object")]
    public sealed class GameEventIdentifierObjectSO : GameEventIdentifierSO<object>
    { }
}