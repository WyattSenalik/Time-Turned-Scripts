using UnityEngine;

using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// <see cref="GameEventIdentifierSO"/> for <see cref="ICloneCreatedContext"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new CloneCreatedEvent", menuName = 
        GameEventIdentifierSO.MENU_FOLDER_PATH + "/Atma/CloneCreated")]
    public sealed class CloneCreatedEventIdentifierSO : 
        GameEventIdentifierSO<ICloneCreatedContext>
    { }
}
