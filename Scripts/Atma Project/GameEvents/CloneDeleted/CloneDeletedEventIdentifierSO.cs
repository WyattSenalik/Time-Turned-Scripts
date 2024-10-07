using UnityEngine;

using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// <see cref="GameEventIdentifierSO"/> for <see cref="ICloneDeletedContext"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new CloneDeletedEvent", menuName =
        GameEventIdentifierSO.MENU_FOLDER_PATH + "/Atma/CloneDeleted")]
    public sealed class CloneDeletedEventIdentifierSO : GameEventIdentifierSO<ICloneDeletedContext>
    { }
}
