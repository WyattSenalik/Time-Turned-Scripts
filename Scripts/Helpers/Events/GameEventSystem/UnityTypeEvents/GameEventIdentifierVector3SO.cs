using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.UnityTypeEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(Vector3)",
    menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
    "/Unity/Vector3")]
    public sealed class GameEventIdentifierVector3SO : GameEventIdentifierSO<Vector3>
    { }
}