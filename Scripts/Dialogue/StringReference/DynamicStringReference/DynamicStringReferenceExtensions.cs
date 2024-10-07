using Dialogue.Internal;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Extensions that make it easier to interact with DynamicStringReferences without having to directly access the manager singleton.
    /// </summary>
    public static class DynamicStringReferenceExtensions
    {
        private static DynamicStringReferenceManager dynamicStrRefMan
        {
            get
            {
                if (s_dynamicStrRefMan == null)
                {
                    s_dynamicStrRefMan = DynamicStringReferenceManager.instance;
                    #region Asserts
                    //CustomDebug.AssertSingletonIsNotNull(s_dynamicStrRefMan, nameof(DynamicStringReferenceExtensions));
                    #endregion Asserts
                }
                return s_dynamicStrRefMan;
            }
        }
        private static DynamicStringReferenceManager s_dynamicStrRefMan = null;


        public static string GetCurrentDynamicStringValue(this DynamicStringReferenceReadOnly dynamicStrRefID)
        {
            return dynamicStrRefMan.GetDynamicString(dynamicStrRefID);
        }
        public static void SetDynamicStringValue(this DynamicStringReference dynamicStrRefID, string newValue)
        {
            dynamicStrRefMan.SetDynamicString(dynamicStrRefID, newValue);
        }
        public static bool RemoveDynamicString(this DynamicStringReference dynamicStrRefID)
        {
            return dynamicStrRefMan.RemoveDynamicString(dynamicStrRefID);
        }
    }
}