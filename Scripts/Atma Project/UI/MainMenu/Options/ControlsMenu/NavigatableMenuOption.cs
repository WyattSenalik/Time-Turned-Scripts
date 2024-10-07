using Helpers.Events;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public class NavigatableMenuOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public IEventPrimer onChosen => m_onChosen;
        public IEventPrimer onHighlighted => m_onHighlighted;
        public IEventPrimer onUnhighlighted => m_onUnhighlighted;

        public NavigatableMenuOption navUpOpt => m_navUpOpt;
        public NavigatableMenuOption navDownOpt => m_navDownOpt;
        public NavigatableMenuOption navLeftOpt => m_navLeftOpt;
        public NavigatableMenuOption navRightOpt => m_navRightOpt;
        public bool isActive { get => m_isActive; set => m_isActive = value; }
        public bool isHighlighted { get; private set; } = false;

        [SerializeField] private NavigatableMenuOption m_navUpOpt = null;
        [SerializeField] private NavigatableMenuOption m_navDownOpt = null;
        [SerializeField] private NavigatableMenuOption m_navLeftOpt = null;
        [SerializeField] private NavigatableMenuOption m_navRightOpt = null;

        [SerializeField, Required] private NavigatableMenu m_navigatableMenu = null;

        [SerializeField] private bool m_isActive = true;

        [SerializeField] private MixedEvent m_onChosen = new MixedEvent();
        [SerializeField] private MixedEvent m_onHighlighted = new MixedEvent();
        [SerializeField] private MixedEvent m_onUnhighlighted = new MixedEvent();

        [SerializeField] private float m_gizmoLineOffsetAm = 0.1f;


        protected virtual void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_navigatableMenu, nameof(m_navigatableMenu), this);
            #endregion Asserts
        }


        public NavigatableMenuOption GetNavigatedToOption(Vector2 navDir)
        {
            if (navDir.x > 0.05f && navRightOpt != null && navRightOpt.isActive)
            {
                return navRightOpt;
            }
            else if (navDir.x < -0.05f && navLeftOpt != null && navLeftOpt.isActive)
            {
                return navLeftOpt;
            }
            else if (navDir.y > 0.05f && navUpOpt != null && navUpOpt.isActive)
            {
                return navUpOpt;
            }
            else if (navDir.y < -0.05f && navDownOpt != null && navDownOpt.isActive)
            {
                return navDownOpt;
            }
            else
            {
                return this;
            }
        }


        public virtual void OnChosen()
        {
            isHighlighted = true;
            m_onChosen.Invoke();
        }
        public virtual void OnHighlighted()
        {
            isHighlighted = true;
            m_onHighlighted.Invoke();
        }
        public virtual void OnUnhighlighted()
        {
            isHighlighted = false;
            m_onUnhighlighted.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isActive)
            {
                m_navigatableMenu.OnPointerEnterOption(this);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isActive)
            {
                m_navigatableMenu.OnPointerExitOption(this);
            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isActive)
            {
                m_navigatableMenu.OnPointerClickOption(this);
            }
        }



        protected virtual void OnDrawGizmosSelected()
        {
            if (m_navUpOpt != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position + new Vector3(m_gizmoLineOffsetAm, m_gizmoLineOffsetAm, 0), m_navUpOpt.transform.position);
            }
            if (m_navDownOpt != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position + new Vector3(-m_gizmoLineOffsetAm, -m_gizmoLineOffsetAm, 0), m_navDownOpt.transform.position);
            }
            if (m_navRightOpt != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position + new Vector3(m_gizmoLineOffsetAm, -m_gizmoLineOffsetAm, 0), m_navRightOpt.transform.position);
            }
            if (m_navLeftOpt != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position + new Vector3(-m_gizmoLineOffsetAm, m_gizmoLineOffsetAm, 0), m_navLeftOpt.transform.position);
            }
        }
    }
}