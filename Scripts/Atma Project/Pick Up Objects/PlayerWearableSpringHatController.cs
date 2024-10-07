using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Enables and disables the spring hat's visual on the player when the player picks up a spring hat. Relys on 
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PickupController))]
    public sealed class PlayerWearableSpringHatController : MonoBehaviour
    {
        [SerializeField, Required] private SpriteRenderer m_wearableHatSprRend = null;

        private PickupController m_pickupCont = null;
        private bool m_isSubbed = false;


        private void Awake()
        {
            m_pickupCont = GetComponent<PickupController>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_wearableHatSprRend, nameof(m_wearableHatSprRend), this);

            //CustomDebug.AssertComponentIsNotNull(m_pickupCont, this);
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
            if (m_isSubbed == cond) { return; }
            m_isSubbed = cond;

            m_pickupCont?.onPickUp.ToggleSubscription(UpdateWearableSpringHatVisual, cond);
            m_pickupCont?.onRelease.ToggleSubscription(UpdateWearableSpringHatVisual, cond);
        }

        private void UpdateWearableSpringHatVisual(IPickUpObject pickUpObj)
        {
            if (!m_pickupCont.isCarrying)
            {
                // Not carrying anything.
                m_wearableHatSprRend.enabled = false;
            }
            else if (!pickUpObj.TryGetComponent(out SpringHatBehaviour _))
            {
                // Not carrying a spring hat.
                m_wearableHatSprRend.enabled = false;
            }
            else
            {
                // Is carrying a spring hat.
                m_wearableHatSprRend.enabled = true;
            }
        }
    }
}