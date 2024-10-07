using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class VideoControlsButtons : MonoBehaviour
    {
        [SerializeField, Required] private PlayerInput m_playerInp = null;
        [SerializeField] private string m_timeManipInputMapName = "TimeManipulation";
        [SerializeField, Required] private GameObject m_playButtonObj = null;
        [SerializeField, Required] private GameObject m_pauseButtonObj = null;

        private GlobalTimeManager m_timeMan = null;
        private LevelOptions m_levelOpts = null;

        private ITimeManipController m_timeManipCont = null;
        private ITimeRewinder m_timeRewinder = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerInp, nameof(m_playerInp), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playButtonObj, nameof(m_playButtonObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pauseButtonObj, nameof(m_pauseButtonObj), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            m_levelOpts = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
            //CustomDebug.AssertSingletonIsNotNull(m_levelOpts, this);
            #endregion Asserts
            // Get time rewinder from player.
            PlayerSingleton t_player = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            #endregion Asserts
            m_timeManipCont = t_player.GetComponent<ITimeManipController>();
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeManipCont, t_player.gameObject, this);
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts

            m_timeRewinder.onBeginReached.ToggleSubscription(OnRewinderBeginReached, true);
            m_timeRewinder.onEndReached.ToggleSubscription(OnRewinderEndReached, true);
            m_timeRewinder.onNavDirChanged.ToggleSubscription(OnNavDirChanged, true);
        }
        private void OnDestroy()
        {
            if (m_timeRewinder != null)
            {
                m_timeRewinder.onBeginReached.ToggleSubscription(OnRewinderBeginReached, false);
                m_timeRewinder.onEndReached.ToggleSubscription(OnRewinderEndReached, false);
                m_timeRewinder.onNavDirChanged.ToggleSubscription(OnNavDirChanged, false);
            }
        }


        public void OnSkipToBeginButton()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.SkipToBeginning();
        }
        public void OnRewindButtonDown()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.Rewind();
            ShowPauseButton();
        }
        public void OnRewindButtonUp()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.Pause();
            ShowPlayButton();
        }
        public void OnPlayButton()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.Play();
        }
        public void OnPauseButton()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.Pause();
            ShowPlayButton();
        }
        public void OnFastForwardButtonDown()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.FastForward();
            ShowPauseButton();
        }
        public void OnFastForwardButtonUp()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.Pause();
            ShowPlayButton();
        }
        public void OnSkipToEndButton()
        {
            if (!CanUseButtons()) { return; }
            m_timeManipCont.SkipToEnd();
        }


        #region Event Driven
        private void OnRewinderBeginReached()
        {
            ShowPlayButton();
        }
        private void OnRewinderEndReached()
        {
            ShowPlayButton();
        }
        private void OnNavDirChanged(float navDir)
        {
            if (navDir != 0)
            {
                ShowPauseButton();
            }
            else
            {
                ShowPlayButton();
            }
        }
        #endregion Event Driven

        private void ShowPlayButton()
        {
            m_playButtonObj.SetActive(true);
            m_pauseButtonObj.SetActive(false);
        }
        private void ShowPauseButton()
        {
            m_playButtonObj.SetActive(false);
            m_pauseButtonObj.SetActive(true);
        }

        private bool CanUseButtons()
        {
            return m_playerInp.currentActionMap.name == m_timeManipInputMapName;
        }
    }
}