using UnityEngine;

using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Calls event on fall.
    /// </summary>
    public sealed class EventPitfallHandler : MonoBehaviour, IPitfallHandler
    {
        public IEventPrimer onEnclosedInPitStart => m_onEnclosedInPitStart;
        public IEventPrimer onEnclosedInPitEnd => m_onEnclosedInPitEnd;
        public IEventPrimer onFall => m_onFall;
        public IEventPrimer onFallStay => m_onFallStay;

        [SerializeField] private MixedEvent m_onEnclosedInPitStart = new MixedEvent();
        [SerializeField] private MixedEvent m_onEnclosedInPitEnd = new MixedEvent();
        [SerializeField] private MixedEvent m_onFall = new MixedEvent();
        [SerializeField] private MixedEvent m_onFallStay = new MixedEvent();

        public void OnEnclosedInPitStart() => m_onEnclosedInPitStart.Invoke();
        public void OnEnclosedInPitEnd() => m_onEnclosedInPitEnd.Invoke();
        public void Fall(GameObject pitVisualsParentObj) => m_onFall.Invoke();
        public void FallStay(GameObject pitVisualsParentObj) => m_onFallStay.Invoke();
    }
}