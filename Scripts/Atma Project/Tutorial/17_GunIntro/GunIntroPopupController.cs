using UnityEngine;

using NaughtyAttributes;

using Timed;
using Atma.UI;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    /// <summary>
    /// Controller for showing popups when the player is holding something (and not rewinding).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GunIntroPopupController : TimedRecorder
    {
        [SerializeField] private GameObject[] m_popupPrefabs = new GameObject[0];
        [SerializeField, Required] private PickupController m_pickupCont = null;

        private PopupController m_popupCont = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCont, nameof(m_pickupCont), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_popupCont = PopupController.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_popupCont, this);
            #endregion Asserts

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            UpdatePopups();
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            UpdatePopups();
        }


        private void ToggleSubscriptions(bool cond)
        {
            m_pickupCont.onPickUp.ToggleSubscription(UpdatePopups, cond);
            m_pickupCont.onThrow.ToggleSubscription(UpdatePopups, cond);
            m_pickupCont.onRelease.ToggleSubscription(UpdatePopups, cond);
        }

        private void UpdatePopups(IPickUpObject obj) => UpdatePopups();
        private void UpdatePopups()
        {
            bool t_show = ShouldShowPopups();
            foreach (GameObject t_popupPref in m_popupPrefabs)
            {
                if (t_show)
                {
                    m_popupCont.ShowPopup(t_popupPref);
                }
                else
                {
                    m_popupCont.HidePopup(t_popupPref);
                }
            }
        }
        private bool ShouldShowPopups()
        {
            // Manipulating time.
            if (!isRecording)
            {
                return false;
            }

            // Player is carrying something (assumed to be a gun).
            if (m_pickupCont.curCarriedObj != null)
            {
                return true;
            }

            // No guns held.
            return false;
        }
    }
}