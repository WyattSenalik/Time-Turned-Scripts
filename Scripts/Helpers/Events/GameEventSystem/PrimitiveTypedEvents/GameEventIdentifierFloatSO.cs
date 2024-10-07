using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(float)",
       menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
       "/Primitives/float")]
    public sealed class GameEventIdentifierFloatSO : GameEventIdentifierSO<float>
    { }
}