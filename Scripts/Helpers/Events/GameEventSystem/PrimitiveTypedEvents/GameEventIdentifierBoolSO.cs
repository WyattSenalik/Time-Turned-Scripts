using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(bool)",
    menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
    "/Primitives/bool")]
    public sealed class GameEventIdentifierBoolSO : GameEventIdentifierSO<bool>
    { }
}