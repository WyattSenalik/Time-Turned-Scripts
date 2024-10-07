using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(byte)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/byte")]
    public sealed class GameEventIdentifierByteSO : GameEventIdentifierSO<byte>
    { }
}