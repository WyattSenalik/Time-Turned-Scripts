using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Represents a single action for a Conversation to execute.
    /// </summary>
    public abstract class ConvoActionObject : ScriptableObject, IConvoAction
    {
        public abstract bool autoAdvance { get; }

        public abstract void Begin(ConvoData convoData, Action onFinished = null);
        public abstract bool Advance(ConvoData convoData);
    }
}
