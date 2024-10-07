using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class RestrictPickupBasedOnActivator : MonoBehaviour
    {
        [SerializeField, Required] private GameObject m_activatorObj = null;
        [SerializeField, Required] private PickUpObject m_pickup = null;

        private IActivator m_activator = null;
        private bool m_hasRequested = false;
        private int m_requestID = -1;


        private void Awake()
        {
            m_activator = m_activatorObj.GetIComponentSafe<IActivator>();
        }
        private void FixedUpdate()
        {
            if (m_activator.isActive)
            {
                if (m_hasRequested)
                {
                    m_pickup.CancelDisallowPickupRequest(m_requestID);
                    m_hasRequested = false;
                }
            }
            else
            {
                if (!m_hasRequested)
                {
                    m_requestID = m_pickup.RequestDisallowPickup();
                    m_hasRequested = true;
                }
            }
        }
    }
}