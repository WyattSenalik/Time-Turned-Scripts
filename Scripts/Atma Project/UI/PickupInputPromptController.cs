using UnityEngine;

using NaughtyAttributes;

using Timed;
using TMPro;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Shows and hides input prompts for what to do with held items.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PickupInputPromptController : MonoBehaviour
    {
        [SerializeField] private bool m_disable = false;
        [SerializeField, Required] private GameObject m_aimThrowPromptGameObject = null;
        [SerializeField, Required] private GameObject m_useItemPromptGameObject = null;
        [SerializeField, Required] private TextMeshProUGUI m_throwTextMesh = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private PickupController m_pickupCont = null;

        [SerializeField] private string m_throwText = "Throw";
        [SerializeField] private string m_dropText = "Drop";


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_aimThrowPromptGameObject, nameof(m_aimThrowPromptGameObject), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_useItemPromptGameObject, nameof(m_useItemPromptGameObject), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_throwTextMesh, nameof(m_throwTextMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCont, nameof(m_pickupCont), this);
            #endregion Asserts
        }
        private void Start()
        {
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            m_timeRewinder?.onRewindBegin.ToggleSubscription(UpdateShownState, cond);
            m_timeRewinder?.onRewindEnded.ToggleSubscription(UpdateShownState, cond);

            m_pickupCont?.onPickUp.ToggleSubscription(UpdateShownState, cond);
            m_pickupCont?.onRelease.ToggleSubscription(UpdateShownState, cond);
            m_pickupCont?.onThrow.ToggleSubscription(UpdateShownState, cond);
        }


        private void UpdateShownState(IPickUpObject obj) => UpdateShownState();
        private void UpdateShownState()
        {
            if (m_disable) { return; }

            if (ShouldShow())
            {
                m_throwTextMesh.text = DetermineReleaseText();

                m_aimThrowPromptGameObject.SetActive(true);
                m_useItemPromptGameObject.SetActive(m_pickupCont.curCarriedObj.hasAction);
            }
            else
            {
                m_aimThrowPromptGameObject.SetActive(false);
                m_useItemPromptGameObject.SetActive(false);
            }
        }
        private bool ShouldShow()
        {
            // Hide if time is being rewound.
            if (m_timeRewinder.hasStarted)
            {
                return false;
            }
            // Hide if nothing is being carried.
            if (!m_pickupCont.isCarrying)
            {
                return false;
            }
            // Time isn't being rewound and something is being carried -> show
            return true;
        }
        private string DetermineReleaseText()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_pickupCont.isCarrying, $"Pickup Controller to be carrying something.", this);
            #endregion Asserts
            switch (m_pickupCont.curCarriedObj.releaseType)
            {
                case IPickUpObject.eReleaseType.Throw:
                    return m_throwText;
                case IPickUpObject.eReleaseType.Drop:
                    return m_dropText;
                default:
                    CustomDebug.UnhandledEnum(m_pickupCont.curCarriedObj.releaseType, this);
                    return m_throwText;
            }
        }
    }
}