using System.Collections.Generic;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Branching
{
    /// <summary>
    /// Holds the value of conditions for BranchConvoAcitons specified by an id in a dictionary.
    /// </summary>
    public sealed class ConditionManager : DynamicSingletonMonoBehaviourPersistant<ConditionManager>
    {
        private readonly Dictionary<string, bool> m_conditionDict = new Dictionary<string, bool>();


        /// <summary>
        /// Registering is optional. Will simply set the condition to false. If not registered or set and something tries to query the condition, then a warning will be thrown.
        /// </summary>
        /// <returns>False if the condition already exists. True if a new condition was created.</returns>
        public bool RegisterNewCondition(string conditionID)
        {
            // Already registered
            if (m_conditionDict.ContainsKey(conditionID))
            {
                return false;
            }
            else
            {
                // Default value should be false until set true.
                m_conditionDict.Add(conditionID, false);
                return true;
            }
        }
        public void SetCondition(BranchConvoAction branchConvoAction, bool condition) => SetCondition(branchConvoAction.conditionID, condition);
        public void SetCondition(string conditionID, bool condition)
        {
            // Already exists, so replace.
            if (m_conditionDict.ContainsKey(conditionID))
            {
                m_conditionDict[conditionID] = condition;
            }
            // Doesn't yet exist, so add new.
            else
            {
                m_conditionDict.Add(conditionID, condition);
            }
        }
        public bool GetCondition(BranchConvoAction branchConvoAction) => GetCondition(branchConvoAction.conditionID, branchConvoAction.defaultBranchValue);
        public bool GetCondition(string conditionID, bool defaultValue)
        {
            if (m_conditionDict.TryGetValue(conditionID, out bool t_condition))
            {
                return t_condition;
            }
            // If does not contain, just return defaultValue. Since it hasn't been set to anything yet.
            else
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Condition w/ ID ({conditionID}) has not been registered or set. Returning its default value ({defaultValue}). Not necessarily wrong that this is happening.", this);
                #endregion Logs
                return defaultValue;
            }
        }

        /// <summary>
        /// Clears all registered conditions from the dictionary.
        /// </summary>
        public void UnregisterAllConditions()
        {
            m_conditionDict.Clear();
        }
    }
}