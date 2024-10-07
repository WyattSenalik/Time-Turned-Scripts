using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.UnityTypeEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(Quaternion)",
    menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
    "/Unity/Quaternion")]
    public sealed class GameEventIdentifierQuaternionSO : GameEventIdentifierSO<Quaternion>
    { }
}