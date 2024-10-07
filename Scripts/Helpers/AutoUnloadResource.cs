using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    /// <summary>
    /// Resource that get auto deloaded if not accessed for an amount of time. Default amount of time is 10 seconds.
    /// </summary>
    public sealed class AutoUnloadResource<T> where T : Object
    {
        public T resource
        {
            get
            {
                if (!m_isLoaded || !Application.isPlaying)
                {
                    m_loadedResource = Resources.Load<T>(m_path);
                    m_isLoaded = true;
                }
                m_lastTimeReferenced = Time.time;
                return m_loadedResource;
            }
        }

        private T m_loadedResource = null;
        private bool m_isLoaded = false;
        private float m_lastTimeReferenced = 0.0f;
        private readonly string m_path = "";
        private readonly float m_timeUntilUnload = 0.0f;


        public AutoUnloadResource(string path, float timeUntilUnload = 10.0f)
        {
            m_path = path;
            m_timeUntilUnload = timeUntilUnload;

            m_loadedResource = Resources.Load<T>(m_path);
            m_isLoaded = true;
            m_lastTimeReferenced = Time.time;
        }

        /// <summary>
        /// Meant to be called regularly (every update).
        /// </summary>
        public void Update()
        {
            if (m_isLoaded)
            {
                if (Time.time - m_lastTimeReferenced >= m_timeUntilUnload)
                {
                    Resources.UnloadAsset(m_loadedResource);
                    m_isLoaded = false;
                }
            }
        }
    }
}