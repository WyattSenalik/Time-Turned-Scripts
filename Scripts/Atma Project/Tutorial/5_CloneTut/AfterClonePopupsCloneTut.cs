using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Atma.UI;
using Helpers.Events.GameEventSystem;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Clone
{
    /// <summary>
    /// Turns on and off popups for after the player creates a time clone.
    /// If the clone disappears, player is prompted to pause (for rewinding).
    /// Once the player pauses, prompts them to rewind.
    /// Once the player rewinds all the way, prompts them to resume.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CloneMadeSuccessCloneTut))]
    public sealed class AfterClonePopupsCloneTut : TimedObserver
    {
        [SerializeField, Required] private GameObject m_pausePopupPrefab = null;
        [SerializeField, Required] private GameObject m_rewindPopupPrefab = null;
        [SerializeField, Required] private GameObject m_resumePopupPrefab = null;
        [SerializeField, Required] private CloneCreatedEventIdentifierSO m_cloneCreatedEventIdentifierSO = null;

        private PopupController m_popupCont = null;

        private CloneMadeSuccessCloneTut m_cloneMadeSuccessProgAction = null;
        private ITimeRewinder m_timeRewinder = null;

        private SubManager<ICloneCreatedContext> m_cloneCreatedSubMan = null;
        private float m_cloneEndTime = float.NaN;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedEventIdentifierSO, nameof(m_cloneCreatedEventIdentifierSO), this);
            #endregion Asserts
            m_cloneMadeSuccessProgAction = GetComponent<CloneMadeSuccessCloneTut>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_cloneMadeSuccessProgAction, this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_player = PlayerSingleton.instance;
            m_popupCont = PopupController.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_popupCont, this);
            #endregion Asserts
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts

            m_cloneCreatedSubMan = new SubManager<ICloneCreatedContext>(m_cloneCreatedEventIdentifierSO, OnCloneCreated);
            m_cloneCreatedSubMan.Subscribe();
        }
        private void OnDestroy()
        {
            m_cloneCreatedSubMan.Unsubscribe();
        }
        private void Update()
        {
            // If the programmed action has not finished (Sr Agent is going to pause time a few times before ready to show).
            if (!m_cloneMadeSuccessProgAction.hasBegun) { return; }
            // If no clone exists (aka no clone end time), just do nothing.
            if (float.IsNaN(m_cloneEndTime)) { return; }
            UpdatePopup();
        }


        private void UpdatePopup()
        {
            ePopupToShow t_popupToShow = DeterminePopupToShow();
            
            switch (t_popupToShow)
            {
                case ePopupToShow.None:
                {
                    m_popupCont.HidePopup(m_pausePopupPrefab);
                    m_popupCont.HidePopup(m_rewindPopupPrefab);
                    m_popupCont.HidePopup(m_resumePopupPrefab);
                    break;
                }
                case ePopupToShow.Pause:
                {
                    m_popupCont.HidePopup(m_rewindPopupPrefab);
                    m_popupCont.HidePopup(m_resumePopupPrefab);

                    m_popupCont.ShowPopup(m_pausePopupPrefab);
                    break;
                }
                case ePopupToShow.Rewind:
                {
                    m_popupCont.HidePopup(m_pausePopupPrefab);
                    m_popupCont.HidePopup(m_resumePopupPrefab);

                    m_popupCont.ShowPopup(m_rewindPopupPrefab);
                    break;
                }
                case ePopupToShow.Resume:
                {
                    m_popupCont.HidePopup(m_pausePopupPrefab);
                    m_popupCont.HidePopup(m_rewindPopupPrefab);

                    m_popupCont.ShowPopup(m_resumePopupPrefab);
                    break;
                }
                default: CustomDebug.UnhandledEnum(t_popupToShow, this); break;
            }
        }
        private ePopupToShow DeterminePopupToShow()
        {
            // Time is being manipulated
            if (m_timeRewinder.hasStarted)
            {
                if (curTime > m_timeRewinder.earliestTime)
                {
                    return ePopupToShow.Rewind;
                }
                else
                {
                    return ePopupToShow.Resume;
                }
            }
            // Time is NOT being manipulated
            else
            {
                if (curTime > m_cloneEndTime)
                {
                    return ePopupToShow.Pause;
                }
                else
                {
                    return ePopupToShow.None;
                }
            }
        }


        private void OnCloneCreated(ICloneCreatedContext context)
        {
            TimeCloneInitData t_cloneData = context.timeClone.cloneData;
            m_cloneEndTime = t_cloneData.farthestTime + t_cloneData.blinkTime;
        }


        public enum ePopupToShow { None, Pause, Rewind, Resume }
    }
}