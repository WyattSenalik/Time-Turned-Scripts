using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.UnityTypeEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(Vector2)",
    menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
    "/Unity/Vector2")]
    public sealed class GameEventIdentifierVector2SO : GameEventIdentifierSO<Vector2>
    { }
}