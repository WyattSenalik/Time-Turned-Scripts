using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Controls a UI element that displays what is the current state of the 
    /// time manipulation with an icon (paused, rewinding, or fastforwarding).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TimeManipIcon : MonoBehaviour
    {
        public IEventPrimer onPausedState => m_onPausedState;
        public IEventPrimer onRewindState => m_onRewindState;
        public IEventPrimer onFastForwardState => m_onFastForwardState;

        [SerializeField, Required] private GameObject m_pauseStateObj = null;
        [SerializeField, Required] private GameObject m_rewindStateObj = null;
        [SerializeField, Required] private GameObject m_fastForwardStateObj = null;
        [SerializeField] private MixedEvent m_onPausedState = new MixedEvent();
        [SerializeField] private MixedEvent m_onRewindState = new MixedEvent();
        [SerializeField] private MixedEvent m_onFastForwardState = new MixedEvent();

        private ITimeRewinder m_timeRewinder = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pauseStateObj, nameof(m_pauseStateObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rewindStateObj, nameof(m_rewindStateObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_fastForwardStateObj, nameof(m_fastForwardStateObj), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_player = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            #endregion Asserts
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts

            m_timeRewinder.onBeginReached.ToggleSubscription(OnRewinderBeginChanged, true);
            m_timeRewinder.onEndReached.ToggleSubscription(OnRewinderEndChanged, true);
            m_timeRewinder.onNavDirChanged.ToggleSubscription(OnRewinderNavDirChanged, true);
        }
        private void OnDestroy()
        {
            m_timeRewinder.onBeginReached.ToggleSubscription(OnRewinderBeginChanged, false);
            m_timeRewinder.onEndReached.ToggleSubscription(OnRewinderEndChanged, false);
            m_timeRewinder.onNavDirChanged.ToggleSubscription(OnRewinderNavDirChanged, false);
        }


        private void OnRewinderBeginChanged()
        {
            SwapToPausedState();
        }
        private void OnRewinderEndChanged()
        {
            SwapToPausedState();
        }
        private void OnRewinderNavDirChanged(float navDir)
        {
            float t_curTime = m_timeRewinder.curTime;

            if (navDir > 0 && t_curTime < m_timeRewinder.farthestTime)
            {
                SwapToFastForwardState();
            }
            else if (navDir < 0 && t_curTime > 0.0f)
            {
                SwapToRewindState();
            }
            else
            {
                SwapToPausedState();
            }
        }

        private void SwapToPausedState()
        {
            m_pauseStateObj.SetActive(true);
            m_rewindStateObj.SetActive(false);
            m_fastForwardStateObj.SetActive(false);

            m_onPausedState.Invoke();
        }
        private void SwapToRewindState()
        {
            m_pauseStateObj.SetActive(false);
            m_rewindStateObj.SetActive(true);
            m_fastForwardStateObj.SetActive(false);

            m_onRewindState.Invoke();
        }
        private void SwapToFastForwardState()
        {
            m_pauseStateObj.SetActive(false);
            m_rewindStateObj.SetActive(false);
            m_fastForwardStateObj.SetActive(true);

            m_onFastForwardState.Invoke();
        }
    }
}