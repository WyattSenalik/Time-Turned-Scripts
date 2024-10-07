// Original Authors - Wyatt Senalik

namespace Helpers
{
    /// <summary>
    /// Simple helper to pause time.
    /// </summary>
    public sealed class GamePauser
    {
        private int m_requestId = -1;


        public void RequestToPause()
        {
            if (m_requestId < 0)
            {
                m_requestId = GamePauseController.instance.RequestPauseTime();
            }
        }
        public void CancelRequestToPause()
        {
            if (m_requestId >= 0)
            {
                GamePauseController.instance.CancelPauseTimeRequest(m_requestId);
                m_requestId = -1;
            }
        }
    }
}