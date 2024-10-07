using System;
using UnityEngine;

using NaughtyAttributes;

using Atma.UI;
using Dialogue;
using Dialogue.ConvoActions.Programmed;
using Timed;
using UnityEngine.InputSystem.Extension;
using System.Collections;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.TwoClone
{
    /// <summary>
    /// Handles Sr Agent leaving, followed by the player (and handling some edge cases). Meant to be attached to SrAgent.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LeaveAfterSuccess_TwoClone : MonoBehavEndpointProgrammedConvoAction
    {
        [SerializeField, Required] private MovementSuspender m_moveSus = null;
        [SerializeField, Required] private InputMapStack m_inpMapStack = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;

        [SerializeField, Required] private Door m_exitDoor = null;

        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;

        [SerializeField, Required] private GameObject m_pauseTimePopupPrefab = null;
        [SerializeField, Required] private GameObject m_rewindTimePopupPrefab = null;
        [SerializeField, Required] private GameObject m_resumeTimePopupPrefab = null;

        [SerializeField] private string m_moveInpMap = "Default";
        [SerializeField] private string m_resumeMoveUniqueKey = "c3014f79a35e47949a27ab1c7d90627d";

        private PopupController m_popupCont = null;
        private ConversationSkipper m_convoSkipper = null;

        private bool m_isLeaveAnimFin = false;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_moveSus, nameof(m_moveSus), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inpMapStack, nameof(m_inpMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_exitDoor, nameof(m_exitDoor), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pauseTimePopupPrefab, nameof(m_pauseTimePopupPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rewindTimePopupPrefab, nameof(m_rewindTimePopupPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_resumeTimePopupPrefab, nameof(m_resumeTimePopupPrefab), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_popupCont = PopupController.GetInstanceSafe(this);
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
        }
        private void Update()
        {
            if (!m_isLeaveAnimFin) { return; }

            ePopupToDisplay t_popupToDisplay = DetermineWhichPopupToDisplay();
            // Turn on the (potential) one popup to display.
            switch (t_popupToDisplay)
            {
                case ePopupToDisplay.None:
                {
                    m_popupCont.HidePopup(m_pauseTimePopupPrefab);
                    m_popupCont.HidePopup(m_rewindTimePopupPrefab);
                    m_popupCont.HidePopup(m_resumeTimePopupPrefab);
                    break;
                }
                case ePopupToDisplay.Pause:
                {
                    m_popupCont.HidePopup(m_rewindTimePopupPrefab);
                    m_popupCont.HidePopup(m_resumeTimePopupPrefab);

                    m_popupCont.ShowPopup(m_pauseTimePopupPrefab);
                    break;
                }
                case ePopupToDisplay.Rewind:
                {
                    m_popupCont.HidePopup(m_pauseTimePopupPrefab);
                    m_popupCont.HidePopup(m_resumeTimePopupPrefab);

                    m_popupCont.ShowPopup(m_rewindTimePopupPrefab);
                    break;
                }
                case ePopupToDisplay.Resume:
                {
                    m_popupCont.HidePopup(m_pauseTimePopupPrefab);
                    m_popupCont.HidePopup(m_rewindTimePopupPrefab);

                    m_popupCont.ShowPopup(m_resumeTimePopupPrefab);
                    break;
                }
                default: CustomDebug.UnhandledEnum(t_popupToDisplay, this); break;
            }
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            // Last action of the scene, no need to hold onFinished

            // Resume time
            m_timeRewinder.CancelRewind();
            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                // Have the sr agent move then allow the player to move after the anim.
                m_srAgentMover.PlayMoveAnimation(1);
            }
            else
            {
                // Can just hide the agent if skipping instead of playing the leave anim.
                m_srAgentMover.gameObject.SetActive(false);
                // Also restore control because the leave animation can't since we aren't playing it.
                RestorePlayerControl();
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            // Doesn't matter, its the last action.
            return false;
        }


        /// <summary>
        /// To be called by <see cref="m_srAgentMover"/>'s 2nd (1-index) move anim when finished.
        /// </summary>
        public void OnLeaveAnimEnd()
        {
            StartCoroutine(OnLeaveAnimEndAfterDelay());
        }
        private IEnumerator OnLeaveAnimEndAfterDelay()
        {
            yield return null;

            RestorePlayerControl();
        }
        private void RestorePlayerControl()
        {
            // Set now as earliest rewind time
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);

            // Let the player move again
            m_inpMapStack.SwitchInputMap(m_moveInpMap);
            m_moveSus.CancelSuspensionWithUniqueKey(m_resumeMoveUniqueKey);

            m_isLeaveAnimFin = true;
        }


        private ePopupToDisplay DetermineWhichPopupToDisplay()
        {
            if (m_timeRewinder.hasStarted)
            {
                if (m_exitDoor.isOn || m_timeRewinder.curTime <= m_timeRewinder.earliestTime)
                {
                    return ePopupToDisplay.Resume;
                }
                else
                {
                    return ePopupToDisplay.Rewind;
                }
            }
            else
            {
                if (m_exitDoor.isOn)
                {
                    return ePopupToDisplay.None;
                }
                else
                {
                    return ePopupToDisplay.Pause;
                }
            }
        }

        public enum ePopupToDisplay { None, Pause, Rewind, Resume }
    }
}