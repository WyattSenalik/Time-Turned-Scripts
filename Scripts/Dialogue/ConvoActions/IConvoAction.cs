using System;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    public interface IConvoAction
    {
        void Begin(ConvoData convoData, Action onFinished = null);
        /// <summary>
        /// Meant to be from input. When the player request to move onto the next action.
        /// </summary>
        /// <returns>If should proceed to next action.</returns>
        bool Advance(ConvoData convoData);
    }
}