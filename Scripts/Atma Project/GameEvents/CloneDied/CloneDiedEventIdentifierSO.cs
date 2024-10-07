using UnityEngine;

using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// <see cref="GameEventIdentifierSO"/> for <see cref="ICloneDiedContext"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new CloneDiedEvent", menuName =
        GameEventIdentifierSO.MENU_FOLDER_PATH + "/Atma/CloneDied")]
    public class CloneDiedEventIdentifierSO : GameEventIdentifierSO<ICloneDiedContext>
    { }
}
