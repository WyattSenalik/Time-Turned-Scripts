using System.Collections.Generic;
using System.Linq;
using Helpers.Singletons;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Dialogue.Internal
{
    /// <summary>
    /// Singleton for storing all the dynamic strings in.
    /// </summary>
    public sealed class DynamicStringReferenceManager : DynamicSingletonMonoBehaviourPersistant<DynamicStringReferenceManager>
    {
        private readonly Dictionary<string, string> m_dynamicStringDict = new Dictionary<string, string>();

        [ShowNativeProperty] public string playerName
        {
            get
            {
                if (m_dynamicStringDict.Count < 0)
                {
                    return "UNITIALIZED";
                }
                KeyValuePair<string, string> t_strings = m_dynamicStringDict.ToArray()[0];
                return $"{t_strings.Key}; {t_strings.Value}";
            }
        }

        public DynamicStringReferenceManager() : base() { }


        public void SetDynamicString(string id, string value)
        {
            if (!m_dynamicStringDict.ContainsKey(id))
            {
                m_dynamicStringDict.Add(id, value);
            }
            else
            {
                m_dynamicStringDict[id] = value;
            }
        }
        public void SetDynamicString(DynamicStringReference idSO, string value) => SetDynamicString(idSO.uniqueID, value);
        public bool RemoveDynamicString(string id)
        {
            return m_dynamicStringDict.Remove(id);
        }
        public bool RemoveDynamicString(DynamicStringReference idSO) => RemoveDynamicString(idSO.uniqueID);
        public string GetDynamicString(DynamicStringReferenceReadOnly idSO)
        {
            string t_id = idSO.uniqueID;
            if (m_dynamicStringDict.TryGetValue(t_id, out string t_value))
            {
                return t_value;
            }
            return idSO.defaultValue;
        }
    }
}