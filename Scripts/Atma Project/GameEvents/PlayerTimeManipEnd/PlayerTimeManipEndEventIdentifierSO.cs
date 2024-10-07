using UnityEngine;

using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// <see cref="GameEventIdentifierSO"/> for <see cref="IPlayerTimeManipEndContext"/>
    /// </summary>
    [CreateAssetMenu(
        fileName = "new PlayerTimeManipEndEventIDSO", 
        menuName = GameEventIdentifierSO.MENU_FOLDER_PATH + "/Atma/PlayerTimeManipEnd"
        )]
    public sealed class PlayerTimeManipEndEventIdentifierSO : 
        GameEventIdentifierSO<IPlayerTimeManipEndContext>
    { }
}