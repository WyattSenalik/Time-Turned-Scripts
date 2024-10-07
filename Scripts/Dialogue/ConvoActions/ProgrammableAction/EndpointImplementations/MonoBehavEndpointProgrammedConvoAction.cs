using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Programmed
{
    /// <summary>
    /// Abstract class for a MonoBehaviour variant Endpoint for a ProgrammedConvoAction.
    /// </summary>
    public abstract class MonoBehavEndpointProgrammedConvoAction : MonoBehaviour, IEndpointProgrammedConvoAction
    {
        public string uniqueID => m_linkedConvoAction.uniqueID;
        public ProgrammableConvoAction linkedConvoAction => m_linkedConvoAction;

        [SerializeField, Required]
        private ProgrammableConvoAction m_linkedConvoAction = null;

        private ProgrammableConvoActionConnectionManager m_conMan = null;


        protected virtual void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_linkedConvoAction, nameof(m_linkedConvoAction), this);
            #endregion Asserts

            // Register itself as the endpoint.
            m_conMan = ProgrammableConvoActionConnectionManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_conMan, this);
            #endregion Asserts
            m_conMan.RegisterEndpoint(m_linkedConvoAction.uniqueID, this);
        }
        protected virtual void OnDestroy()
        {
            m_conMan?.UnregisterEndpoint(m_linkedConvoAction.uniqueID);
        }


        public abstract void Begin(ConvoData convoData, Action onFinished = null);
        public abstract bool Advance(ConvoData convoData);
    }
}