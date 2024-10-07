using UnityEngine;
using UnityEngine.InputSystem;

using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class TVInteractInput : MonoBehaviour
    {
        public IEventPrimer onTriedToInteractWithTV => m_onTriedToInteractWithTV;

        [SerializeField] private MixedEvent m_onTriedToInteractWithTV = new MixedEvent();


        private void OnInteractWithTV(InputValue value)
        {
            if (value.isPressed)
            {
                m_onTriedToInteractWithTV.Invoke();
            }
        }
    }
}