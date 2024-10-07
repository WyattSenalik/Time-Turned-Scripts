using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem
{
    /// <summary>
    /// <see cref="IGameEventIdentifier"/> as a <see cref="ScriptableObject"/>.
    /// Uses the name of the <see cref="GameEventIdentifierSO"/> as the eventID,
    /// so make sure all the names of the <see cref="GameEventIdentifierSO"/>
    /// are unique.
    /// </summary>
    [CreateAssetMenu(fileName = "new GameEventIdentifier",
        menuName = MENU_FOLDER_PATH + "/NoParam", order = -1)]
    public sealed class GameEventIdentifierSO : ScriptableObject, IGameEventIdentifier
    {
        public const string MENU_FOLDER_PATH = "ScriptableObjects/GameEventSystem/Identifiers";

        public string eventID => name;
    }

    /// <summary>
    /// Must be extended per type.
    /// 
    /// <see cref="IGameEventIdentifier{T}"/> as a <see cref="ScriptableObject"/>.
    /// Uses the name of the <see cref="GameEventIdentifierSO{T}"/> as the eventID,
    /// so make sure all the names of the <see cref="GameEventIdentifierSO{T}"/>
    /// are unique. Inlcuding any overlap with the parameter-less 
    /// <see cref="GameEventIdentifierSO"/>.
    /// </summary>
    public abstract class GameEventIdentifierSO<T> : ScriptableObject, IGameEventIdentifier<T>
    {
        public string eventID => name;
    }
}