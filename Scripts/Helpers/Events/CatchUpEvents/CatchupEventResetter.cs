using System.Collections.Generic;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.CatchupEvents
{
    /// <summary>
    /// Classes using <see cref="CatchupEvent"/>s can use this to reset their
    /// catchup events if needed.
    /// </summary>
    public class CatchupEventResetter : SingletonMonoBehaviour<CatchupEventResetter>
    {
        private const bool IS_DEBUGGING = false;

        private List<ICatchupEventReset> m_eventsToReset
            = new List<ICatchupEventReset>();


        public void AddCatchupEventForReset(ICatchupEventReset eventToReset)
        {
            if (m_eventsToReset.Contains(eventToReset)) { return; }

            m_eventsToReset.Add(eventToReset);
        }
        public void ResetAllCatchupEvents()
        {
            #region Logs
            //CustomDebug.Log(nameof(ResetAllCatchupEvents), IS_DEBUGGING);
            #endregion Logs

            for (int i = 0; i < m_eventsToReset.Count; ++i)
            {
                ICatchupEventReset temp_curEvent = m_eventsToReset[i];
                // If the current event is null, its controller was probably
                // deleted, so it doesn't need to be reset anyway.
                if (temp_curEvent == null)
                {
                    m_eventsToReset.RemoveAt(i);
                    --i;
                    continue;
                }
                // Reset the event
                m_eventsToReset[i].Reset();
            }
        }
    }
}
