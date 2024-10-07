using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(sbyte)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/sbyte")]
    public sealed class GameEventIdentifierSByteSO : GameEventIdentifierSO<sbyte>
    { }
}