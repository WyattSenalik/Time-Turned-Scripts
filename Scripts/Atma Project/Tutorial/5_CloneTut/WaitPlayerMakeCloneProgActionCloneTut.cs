using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Atma.UI;
using Dialogue;
using Dialogue.ConvoActions.Branching;
using Dialogue.ConvoActions.Programmed;
using Helpers.Events.GameEventSystem;
using Timed;
using UnityEngine.InputSystem.Extension;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Clone
{
    [DisallowMultipleComponent]
    public sealed class WaitPlayerMakeCloneProgActionCloneTut : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private MovementSuspender m_playerMoveSus = null;
        [SerializeField, Required] private SwitchPressurePlateHandler m_ppHandler = null;
        [SerializeField, Required] private BranchPlayerController m_branchCont = null;
        [SerializeField, Required] private TimeManipControllerCloneTut m_timeManipCont = null;

        [SerializeField, Required] private GameObject m_standOnPPPopupPrefab = null;
        [SerializeField, Required] private GameObject m_stopTimePopupPrefab = null;
        [SerializeField, Required] private GameObject m_createClonePopupPrefab = null;
        [SerializeField, Required] private GameObject m_resumeTimePopupPrefab = null;

        [SerializeField, Required] private CloneCreatedEventIdentifierSO m_cloneCreatedID = null;
        [SerializeField, Required] private BranchConvoAction m_wasCloneMadeCorrectlyBranchAction = null;
        [SerializeField, Required] private BranchConvoAction m_isFirstTimeBranchAction = null;

        [SerializeField] private string m_movingInpMapName = "Default";
        [SerializeField] private string m_resumeMovingUniqueID = "b49745358a944a7c89ac4f36f1e3defe";
        [SerializeField] private string m_stopMovingUniqueID = "148556bd7fd948b08ea74c36ba6761c2";

        [SerializeField, Min(0.0f)] private float m_waitTimeAfterCloneCreated = 1.0f;

        [SerializeField, ReadOnly] private ePopupType m_debugPopupType = ePopupType.None;

        private ConditionManager m_condMan = null;
        private PopupController m_popupCont = null;
        private SkipButton m_skipButton = null;

        private bool m_hasBegunAndIsntFinished = false;
        private Action m_onFinished = null;
        private bool m_isPPOn = false;
        private bool m_isTimeManipActive = false;
        private bool m_wasPPOnAtTimeManipBegin = false;
        private bool m_hasCloneBeenCreated = false;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerMoveSus, nameof(m_playerMoveSus), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppHandler, nameof(m_ppHandler), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_branchCont, nameof(m_branchCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_standOnPPPopupPrefab, nameof(m_standOnPPPopupPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopTimePopupPrefab, nameof(m_stopTimePopupPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_createClonePopupPrefab, nameof(m_createClonePopupPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_resumeTimePopupPrefab, nameof(m_resumeTimePopupPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneCreatedID, nameof(m_cloneCreatedID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_wasCloneMadeCorrectlyBranchAction, nameof(m_wasCloneMadeCorrectlyBranchAction), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_isFirstTimeBranchAction, nameof(m_isFirstTimeBranchAction), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_condMan = ConditionManager.instance;
            m_popupCont = PopupController.GetInstanceSafe(this);
            m_skipButton = SkipButton.GetInstanceSafe(this);

            ToggleEventSuscriptions(true);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            ToggleEventSuscriptions(false);
        }
        private void FixedUpdate()
        {
            // Added to fix the popup for creating a clone begin active when it shouldn't while rewinding during time clone creation.
            if (m_hasCloneBeenCreated) { return; }
            if (m_timeRewinder.hasStarted)
            {
                if (m_timeRewinder.navigationDir < 0)
                {
                    // Clone currently being created.
                    m_hasCloneBeenCreated = true;
                    UpdatePopups();
                }
            }
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_onFinished = onFinished;
            m_hasBegunAndIsntFinished = true;

            // Time is currently paused (see AdvanceFromPPBlink). So unpause it.
            m_timeRewinder.CancelRewind();

            // Since the player has now heard how to make a clone, mark the branch for "is first time" as false;
            m_condMan.SetCondition(m_isFirstTimeBranchAction, false);

            // Let player move again
            m_inputMapStack.SwitchInputMap(m_movingInpMapName);
            m_playerMoveSus.CancelSuspensionWithUniqueKey(m_resumeMovingUniqueID);

            // Update the state of the controller
            m_timeManipCont.SetCurState(TimeManipControllerCloneTut.eCloneTutState.CloneCreating);

            // Update the popups
            UpdatePopups();

            // Reset skipping
            m_skipButton.ResetSkipButton();
        }
        public override bool Advance(ConvoData convoData)
        {
            return m_hasCloneBeenCreated;
        }


        private void ToggleEventSuscriptions(bool cond)
        {
            // Pressure Plate (On/Off)
            m_ppHandler?.onActivated.ToggleSubscription(OnPressurePlateActivated, cond);
            m_ppHandler?.onDeactivated.ToggleSubscription(OnPressurePlateDeactivated, cond);
            // Branch Controller (Time Manip Begin/End)
            m_branchCont?.onBeginTimeManip.ToggleSubscription(OnTimeManipBegin, cond);
            m_branchCont?.onEndTimeManip.ToggleSubscription(OnTimeManipEnd, cond);
            // Clone Created
            m_cloneCreatedID.ToggleSubscriptionToEvent(OnCloneCreated, cond);
        }
        #region Event Driven
        private void OnPressurePlateActivated()
        {
            m_isPPOn = true;
            UpdatePopups();
        }
        private void OnPressurePlateDeactivated()
        {
            m_isPPOn = false;
            UpdatePopups();
        }
        private void OnTimeManipBegin()
        {
            m_isTimeManipActive = true;
            m_wasPPOnAtTimeManipBegin = m_isPPOn;
            UpdatePopups();
        }
        private void OnTimeManipEnd()
        {
            m_isTimeManipActive = false;
            UpdatePopups();
        }
        private void OnCloneCreated(ICloneCreatedContext context)
        {
            #region Logs
            //CustomDebug.LogForComponent($"CloneCreated", this, IS_DEBUGGING);
            #endregion Logs
            m_condMan.SetCondition(m_wasCloneMadeCorrectlyBranchAction, m_wasPPOnAtTimeManipBegin);

            m_hasCloneBeenCreated = true;
            UpdatePopups();

            // Stop the player from moving for next dialogue.
            m_inputMapStack.PopInputMap(m_movingInpMapName);
            m_playerMoveSus.SuspendWithUniqueKey(m_stopMovingUniqueID);

            // Update the state of the controller
            m_timeManipCont.SetCurState(TimeManipControllerCloneTut.eCloneTutState.AfterCloneCreated);

            Invoke(nameof(Finish), m_waitTimeAfterCloneCreated);
        } 

        private void Finish()
        {
            StartCoroutine(FinishAfterShortDelayCorout());
        }
        private IEnumerator FinishAfterShortDelayCorout()
        {
            yield return new WaitForFixedUpdate();

            // Pause time until for the next set of dialogue.
            m_timeRewinder.StartRewind();

            m_hasBegunAndIsntFinished = false;
            m_onFinished?.Invoke();
            UpdatePopups();
        }
        #endregion Event Driven

        private void UpdatePopups()
        {
            
            ePopupType t_popupToShow = DetermineCurrentPopupToShow();
            DisableAllPopupsExcept(t_popupToShow);
            m_debugPopupType = t_popupToShow;
            switch (t_popupToShow)
            {
                case ePopupType.None: break;
                case ePopupType.StandOnPP: m_popupCont.ShowPopup(m_standOnPPPopupPrefab); break;
                case ePopupType.StopTime: m_popupCont.ShowPopup(m_stopTimePopupPrefab); break;
                case ePopupType.CreateClone: m_popupCont.ShowPopup(m_createClonePopupPrefab); break;
                case ePopupType.ResumeTime: m_popupCont.ShowPopup(m_resumeTimePopupPrefab); break;
                default: CustomDebug.UnhandledEnum(t_popupToShow, this); break;
            }
        }
        private void DisableAllPopupsExcept(ePopupType popupNotToDisable)
        {
            if (popupNotToDisable != ePopupType.StandOnPP)
            {
                m_popupCont.HidePopup(m_standOnPPPopupPrefab, true);
            }
            if (popupNotToDisable != ePopupType.StopTime)
            {
                m_popupCont.HidePopup(m_stopTimePopupPrefab, true);
            }
            if (popupNotToDisable != ePopupType.CreateClone)
            {
                m_popupCont.HidePopup(m_createClonePopupPrefab, true);
            }
            if (popupNotToDisable != ePopupType.ResumeTime)
            {
                m_popupCont.HidePopup(m_resumeTimePopupPrefab, true);
            }
        }
        private ePopupType DetermineCurrentPopupToShow()
        {
            if (m_hasCloneBeenCreated || !m_hasBegunAndIsntFinished)
            {
                return ePopupType.None;
            }

            if (m_isTimeManipActive)
            {
                if (m_wasPPOnAtTimeManipBegin)
                {
                    // Player has done everything correctly so far, now needs to make the time clone.
                    return ePopupType.CreateClone;
                }
                else
                {
                    // Player started pause without standing on pp.
                    return ePopupType.ResumeTime;
                }
            }
            else
            {
                if (m_isPPOn)
                {
                    // Player is standing on the pressure plate. Now needs to begin time manip.
                    return ePopupType.StopTime;
                }
                else
                {
                    // Player has yet to stand on pressure plate.
                    return ePopupType.StandOnPP;
                }
            }
        }



        public enum ePopupType { None, StandOnPP, StopTime, CreateClone, ResumeTime }
    }
}