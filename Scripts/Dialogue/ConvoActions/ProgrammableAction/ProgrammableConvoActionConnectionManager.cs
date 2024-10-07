using System;
using System.Collections.Generic;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Programmed
{
    /// <summary>
    /// Holds endpoints that will receive the calls for the <see cref="ProgrammableConvoAction"/>s.
    /// </summary>
    public sealed class ProgrammableConvoActionConnectionManager : DynamicSingletonMonoBehaviourPersistant<ProgrammableConvoActionConnectionManager>
    {
        private readonly Dictionary<string, IEndpointProgrammedConvoAction> m_endpointDict = new Dictionary<string, IEndpointProgrammedConvoAction>();


        public void RegisterEndpoint(string id, IEndpointProgrammedConvoAction endpoint)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(!m_endpointDict.ContainsKey(id), $"Tried to register an endpoint with id ({id}). Endpoint already exists.", this);
            #endregion Asserts
            m_endpointDict.Add(id, endpoint);
        }
        public bool UnregisterEndpoint(string id)
        {
            return m_endpointDict.Remove(id);
        }
        public IEndpointProgrammedConvoAction GetEndpoint(string id)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(m_endpointDict.ContainsKey(id), $"No endpoint found for id ({id})", this);
            #endregion Asserts
            return m_endpointDict[id];
        }
        public void TellEndpointToBegin(string id, ConvoData convoData, Action onFinished)
        {
            IEndpointProgrammedConvoAction t_action = GetEndpoint(id);
            t_action.Begin(convoData, onFinished);
        }
        public bool TellEndpointToAdvance(string id, ConvoData convoData)
        {
            IEndpointProgrammedConvoAction t_action = GetEndpoint(id);
            return t_action.Advance(convoData);
        }
    }
}