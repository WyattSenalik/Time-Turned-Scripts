using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.UnityTypeEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(GameObject)",
    menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
    "/Unity/GameObject")]
    public sealed class GameEventIdentifierGameObjectSO : GameEventIdentifierSO<GameObject>
    { }
}