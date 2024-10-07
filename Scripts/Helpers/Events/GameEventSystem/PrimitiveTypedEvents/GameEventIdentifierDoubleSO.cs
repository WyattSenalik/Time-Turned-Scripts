using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.ParameterEvents
{
    [CreateAssetMenu(fileName = "new GameEventIdentifier(double)",
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH +
        "/Primitives/double")]
    public sealed class GameEventIdentifierDoubleSO : GameEventIdentifierSO<double>
    { }
}