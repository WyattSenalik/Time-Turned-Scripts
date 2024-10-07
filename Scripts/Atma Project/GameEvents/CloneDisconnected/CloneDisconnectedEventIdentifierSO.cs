using UnityEngine;

using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// <see cref="GameEventIdentifierSO"/> for <see cref="ICloneDisconnectedContext"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new CloneDisconnectedEvent", menuName =
        GameEventIdentifierSO.MENU_FOLDER_PATH + "/Atma/CloneDisconnected")]
    public sealed class CloneDisconnectedEventIdentifierSO : GameEventIdentifierSO<ICloneDisconnectedContext>
    { }
}