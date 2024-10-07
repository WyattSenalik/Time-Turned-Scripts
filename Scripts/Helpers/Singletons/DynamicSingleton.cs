// Original Authors - Wyatt Senalik

namespace Helpers.Singletons
{
    /// <summary>
    /// Base singleton class.
    /// Will dynamically create itself if there an instance
    /// does not already exist.
    /// </summary>
    public class DynamicSingleton<T> where T : DynamicSingleton<T>, new()
    {
        /// <summary>
        /// Reference to the current instance of this class.
        /// </summary>
        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                }
                return m_instance;
            }
        }
        private static T m_instance = null;
    }
}
