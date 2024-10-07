using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Controls the enable state of the collider that interacts with pressure plate for a pickup.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PickUpStateManager))]
    public sealed class PickUpPressurePlateColliderEnableController : MonoBehaviour
    {
        private PickUpStateManager m_pickupStateMan = null;
        [SerializeField, Required] private Collider2D m_ppCollider = null;

        [SerializeField, ReadOnly] private ePickupState m_lastObservedState = ePickupState.OnGround;


        private void Awake()
        {
            m_pickupStateMan = this.GetComponentSafe<PickUpStateManager>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppCollider, nameof(m_ppCollider), this);
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
            if (m_pickupStateMan != null)
            {
                m_pickupStateMan.onStateChanged.ToggleSubscription(OnPickupStateChanged, cond);
            }
        }
        private void OnPickupStateChanged(ePickupState newState)
        {
            m_lastObservedState = newState;
            m_ppCollider.enabled = DetermineEnableStateFromPickupState(newState);
        }
        private bool DetermineEnableStateFromPickupState(ePickupState pickupState)
        {
            switch (pickupState)
            {
                case ePickupState.OnGround: return true;
                case ePickupState.Held: return false;
                case ePickupState.InAirRising: return false;
                case ePickupState.InAirFalling: return false;
                case ePickupState.FallingIntoVoid: return false;
                case ePickupState.InVoid: return false;
                default:
                {
                    CustomDebug.UnhandledEnum(pickupState, this);
                    return true;
                }
            }
        }
    }
}