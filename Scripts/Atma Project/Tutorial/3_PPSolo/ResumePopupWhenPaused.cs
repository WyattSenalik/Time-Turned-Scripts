using UnityEngine;

using NaughtyAttributes;

using Atma.UI;
using Timed;
using UnityEngine.InputSystem.Extension;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.PPSolo
{
    /// <summary>
    /// Shows a popup when time is paused.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ResumePopupWhenPaused : TimedRecorder
    {
        [SerializeField, Required] private GameObject m_resumePopupPrefab = null;
        [SerializeField] private string m_dialogueMapName = "Dialogue";

        private InputMapStack m_inpMapStack = null;
        private PopupController m_popupCont = null;
        private string m_activePopupKey = "";


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_resumePopupPrefab, nameof(m_resumePopupPrefab), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_inpMapStack = t_playerSingleton.GetComponentSafe<InputMapStack>();
            m_popupCont = PopupController.GetInstanceSafe();
        }


        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            if (!m_inpMapStack.activeMap.Equals(m_dialogueMapName))
            {
                m_activePopupKey = m_popupCont.ShowPopup(m_resumePopupPrefab);
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            if (m_activePopupKey != "")
            {
                m_popupCont.HidePopup(m_activePopupKey, false);
            }
        }
    }
}