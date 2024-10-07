using UnityEngine;

using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public class PauseMenuOption : MonoBehaviour
    {
        public IEventPrimer onHighlighted => m_onHighlighted;
        public IEventPrimer onUnhighlighted => m_onUnhighlighted;
        public IEventPrimer onChosen => m_onChosen;
        public IEventPrimer<float> onHoriInput => m_onHoriInput;

        private UISoundController soundMan
        {
            get
            {
                if (m_soundMan == null)
                {
                    m_soundMan = UISoundController.GetInstanceSafe(this);
                }
                return m_soundMan;
            }
        }

        [SerializeField] private MixedEvent m_onHighlighted = new MixedEvent();
        [SerializeField] private MixedEvent m_onUnhighlighted = new MixedEvent();
        [SerializeField] private MixedEvent m_onChosen = new MixedEvent();
        [SerializeField] private MixedEvent<float> m_onHoriInput = new MixedEvent<float>();
        [SerializeField] private MixedEvent m_onHoriInputZeroed = new MixedEvent();

        private UISoundController m_soundMan = null;


        private void Start()
        {
            if (m_soundMan == null)
            {
                m_soundMan = UISoundController.GetInstanceSafe(this);
            }
        }


        public virtual void OnHighlighted()
        {
            soundMan.PlayMenuHighlightSound();
            m_onHighlighted.Invoke();
        }
        public virtual void OnUnhighlighted()
        {
            m_onUnhighlighted.Invoke();
        }
        public virtual void OnChosen()
        {
            soundMan.PlayMenuSelectSound();
            m_onChosen.Invoke();
        }
        public virtual void OnHorizontalInput(float inputValue)
        {
            m_onHoriInput.Invoke(inputValue);
        }
        public virtual void OnHorizontalInputZeroed()
        {
            m_onHoriInputZeroed.Invoke();
        }
    }
}