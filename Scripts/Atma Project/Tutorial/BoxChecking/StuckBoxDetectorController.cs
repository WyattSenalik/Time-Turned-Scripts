using System.Linq;
using UnityEngine;

using NaughtyAttributes;

using Atma.UI;
using Timed;
using UnityEngine.InputSystem.Extension;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class StuckBoxDetectorController : TimedRecorder
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private GameObject m_manualPauseTipPrefab = null;
        [SerializeField, Required] private GameObject m_rewindTipPrefab = null;
        [SerializeField, Required] private GameObject m_resumeTipPrefab = null;

        [SerializeField] private string[] m_activeInputMapNames = new string[2] { "Default", "TimeManipulation" };

        private PopupController m_popupCont = null;
        private InputMapStack m_inputMapStack = null;
        private StuckBoxDetector[] m_stuckBoxDetectors = null;

        private bool m_isSubbed = false;


        protected override void Awake()
        {
            base.Awake();

            m_stuckBoxDetectors = GetComponentsInChildren<StuckBoxDetector>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_manualPauseTipPrefab, nameof(m_manualPauseTipPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rewindTipPrefab, nameof(m_rewindTipPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_resumeTipPrefab, nameof(m_resumeTipPrefab), this);

            //CustomDebug.AssertIsTrueForComponent(m_stuckBoxDetectors.Length > 0, $"to have at least 1 stuck box detector as a child", this);
            #endregion Asserts
        }
        private void Start()
        {
            m_popupCont = PopupController.instance;
            PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_popupCont, this);
            //CustomDebug.AssertSingletonIsNotNull(t_playerSingleton, this);
            #endregion Asserts
            m_inputMapStack = t_playerSingleton.GetComponent<InputMapStack>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_inputMapStack, t_playerSingleton.gameObject, this);
            #endregion Asserts

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Update when rewinding constantly
            if (!isRecording) { UpdateShownTip(); }
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            UpdateShownTip();
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            UpdateShownTip();
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (cond == m_isSubbed) { return; }
            if (cond)
            {
                foreach (StuckBoxDetector t_stuckBoxDetector in m_stuckBoxDetectors)
                {
                    t_stuckBoxDetector?.onAmountBoxesContainedChanged.ToggleSubscription(UpdateShownTip, cond);
                }
            }
            else
            {
                foreach (StuckBoxDetector t_stuckBoxDetector in m_stuckBoxDetectors)
                {
                    t_stuckBoxDetector?.onAmountBoxesContainedChanged.ToggleSubscription(UpdateShownTip, cond);
                }
            }
            m_isSubbed = cond;
        }
        private void UpdateShownTip()
        {
            eTipToShow t_tipToShow = DetermineTipToShow();
            #region Logs
            //CustomDebug.LogForComponent($"Updating to show tip {t_tipToShow}.", this, IS_DEBUGGING);
            #endregion Logs
            switch (t_tipToShow)
            {
                case eTipToShow.None:
                {
                    m_popupCont.HidePopup(m_manualPauseTipPrefab, true);
                    m_popupCont.HidePopup(m_rewindTipPrefab, true);
                    m_popupCont.HidePopup(m_resumeTipPrefab, true);
                    break;
                }
                case eTipToShow.Pause:
                {
                    m_popupCont.ShowPopup(m_manualPauseTipPrefab);
                    m_popupCont.HidePopup(m_rewindTipPrefab, true);
                    m_popupCont.HidePopup(m_resumeTipPrefab, true);
                    break;
                }
                case eTipToShow.Rewind:
                {
                    m_popupCont.HidePopup(m_manualPauseTipPrefab, true);
                    m_popupCont.ShowPopup(m_rewindTipPrefab);
                    m_popupCont.HidePopup(m_resumeTipPrefab, true);
                    break;
                }
                case eTipToShow.Resume:
                {
                    m_popupCont.HidePopup(m_manualPauseTipPrefab, true);
                    m_popupCont.HidePopup(m_rewindTipPrefab, true);
                    m_popupCont.ShowPopup(m_resumeTipPrefab);
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(t_tipToShow, this);
                    break;
                }
            }
        }
        private eTipToShow DetermineTipToShow()
        {
            // Quick check to make sure the player has one of the valid input maps.
            if (!PlayerHasValidInputMap()) { return eTipToShow.None; }

            if (!AreAnyBoxesStuck())
            {
                if (isRecording)
                {
                    // No boxes in bad spots and recording. All good.
                    return eTipToShow.None;
                }
                else
                {
                    // No boxes in bad spots, but still rewinding, so resume.
                    return eTipToShow.Resume;
                }
            }
            else
            {
                if (isRecording)
                {
                    // Box in bad spot and recording, tell them to fix it.
                    return eTipToShow.Pause;
                }
                else
                {
                    // Box in bad stop and rewinding, keep rewinding.
                    return eTipToShow.Rewind;
                }
            }
        }
        private bool PlayerHasValidInputMap()
        {
            return m_activeInputMapNames.Contains(m_inputMapStack.activeMap);
        }
        private bool AreAnyBoxesStuck()
        {
            foreach (StuckBoxDetector t_stuckBoxDetector in m_stuckBoxDetectors)
            {
                if (t_stuckBoxDetector.DoesDetectStuckBox())
                {
                    return true;
                }
            }
            return false;
        }


        public enum eTipToShow { None, Pause, Rewind, Resume }
    }
}