using UnityEngine;

using Helpers.Events.GameEventSystem;
// Original Authors - Wyatt Senalik

namespace Atma.Events
{
    /// <summary>
    /// <see cref="GameEventIdentifierSO"/> for <see cref="IIneptusDiedContext"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new IneptusDiedEvent", menuName =
        GameEventIdentifierSO.MENU_FOLDER_PATH + "/Atma/IneptusDied")]
    public sealed class IneptusDiedEventIdentifierSO : GameEventIdentifierSO<IIneptusDiedContext>
    { }
}