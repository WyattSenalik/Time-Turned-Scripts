using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavigatableMenuOption))]
    public sealed class PlayUISoundsNavigateMenuOption : MonoBehaviour
    {
        [SerializeField] private bool m_shouldPlaySelectSound = true;
        [SerializeField] private bool m_shouldPlayHighlightSound = true;

        private UISoundController m_soundCont = null;
        private NavigatableMenuOption m_menuOption = null;


        private void Awake()
        {
            m_menuOption = this.GetComponentSafe<NavigatableMenuOption>();
        }
        private void Start()
        {
            m_soundCont = UISoundController.GetInstanceSafe();

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_menuOption != null)
            {
                m_menuOption.onChosen.ToggleSubscription(OnChosen, cond);
                m_menuOption.onHighlighted.ToggleSubscription(OnHighlighted, cond);
            }
        }
        private void OnChosen()
        {
            if (m_shouldPlaySelectSound)
            {
                m_soundCont.PlayMenuSelectSound();
            }
        }
        private void OnHighlighted()
        {
            if (m_shouldPlayHighlightSound)
            {
                m_soundCont.PlayMenuHighlightSound();
            }
        }
    }
}