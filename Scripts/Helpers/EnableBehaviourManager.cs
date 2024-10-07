using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    /// <summary>
    /// Controls if the specified behaviour is enabled or not. Allows multiple sources to request that the behaviour be disabled to avoid if one of them stops wanting to to be disabled (enabling it) while another source wants it to stay disabled.
    /// </summary>
    public sealed class EnableBehaviourManager : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private Behaviour m_controlledBehaviour = null;

        private readonly IDLibrary m_idLibrary = new IDLibrary();
        private readonly HashSet<string> m_uniqueIDs = new HashSet<string>();


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_controlledBehaviour, nameof(m_controlledBehaviour), this);
            #endregion Asserts
        }
        

        public int RequestDisableBehaviour()
        {
            #region Logs
            //CustomDebug.LogForComponent(nameof(RequestDisableBehaviour), this, IS_DEBUGGING);
            #endregion Logs
            m_controlledBehaviour.enabled = false;

            int t_id = m_idLibrary.CheckoutID();
            return t_id;
        }
        public void CancelDisableRequest(int requestID)
        {
            m_idLibrary.ReturnID(requestID);
            UpdateBehaviourEnabledState();
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(CancelDisableRequest)}. enabled={m_controlledBehaviour.enabled}", this, IS_DEBUGGING);
            #endregion Logs
        }
        public void RequestDisableBehaviourWithUniqueID(string uniqueID)
        {
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(RequestDisableBehaviourWithUniqueID)}. uniqueID={uniqueID}", this, IS_DEBUGGING);
            #endregion Logs
            m_controlledBehaviour.enabled = false;

            m_uniqueIDs.Add(uniqueID);
        }
        public void CancelDisableRequestWithUniqueID(string uniqueID)
        {
            m_uniqueIDs.Remove(uniqueID);
            UpdateBehaviourEnabledState();
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(CancelDisableRequestWithUniqueID)}. uniqueID={uniqueID}. enabled={m_controlledBehaviour.enabled}", this, IS_DEBUGGING);
            #endregion Logs
        }


        private void UpdateBehaviourEnabledState()
        {
            // Should be enabled if all ids are returned and there are no unique ids still in the hash set.
            m_controlledBehaviour.enabled = m_idLibrary.AreAllIDsReturned() && m_uniqueIDs.Count == 0;
        }
    }
}