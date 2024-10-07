using UnityEngine;

using Helpers.Events;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    public sealed class GamePauseController : DynamicSingletonMonoBehaviourPersistant<GamePauseController>
    {
        public IEventPrimer onPause => m_onPause;
        public IEventPrimer onResume => m_onResume;
        public bool isPaused { get; private set; } = false;

        [SerializeField] private MixedEvent m_onPause = new MixedEvent();
        [SerializeField] private MixedEvent m_onResume = new MixedEvent();

        private readonly IDLibrary m_library = new IDLibrary();


        public int RequestPauseTime()
        {
            int t_id = m_library.CheckoutID();
            UpdateTimeScale();
            return t_id;
        }
        public void CancelPauseTimeRequest(int checkoutID)
        {
            m_library.ReturnID(checkoutID);
            UpdateTimeScale();
        }



        private void UpdateTimeScale()
        {
            if (m_library.AreAllIDsReturned())
            {
                if (isPaused)
                {
                    // Nothing wants time to be paused.
                    isPaused = false;
                    Time.timeScale = 1.0f;
                    m_onResume.Invoke();
                }
            }
            else
            {
                if (!isPaused)
                {
                    // Some ids are checked out some some things want the game to be paused.
                    isPaused = true;
                    Time.timeScale = 0.0f;
                    m_onPause.Invoke();
                }
            }
        }
    }
}