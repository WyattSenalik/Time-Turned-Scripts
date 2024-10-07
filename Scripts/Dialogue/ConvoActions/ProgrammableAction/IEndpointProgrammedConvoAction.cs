using System;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Programmed
{
    /// <summary>
    /// Endpoint for a <see cref="ProgrammableConvoAction"/>. Does the Begin and Advance behaviour for the ProgrammableCovoAction that shares the id.
    /// </summary>
    public interface IEndpointProgrammedConvoAction
    {
        string uniqueID { get; }

        void Begin(ConvoData convoData, Action onFinished = null);
        bool Advance(ConvoData convoData);
    }
}