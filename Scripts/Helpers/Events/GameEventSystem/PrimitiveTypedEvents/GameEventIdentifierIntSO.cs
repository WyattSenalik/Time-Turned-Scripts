using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(int)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/int")]
    public sealed class GameEventIdentifierIntSO : GameEventIdentifierSO<int>
    { }
}