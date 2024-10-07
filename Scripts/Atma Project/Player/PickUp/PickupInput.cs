using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Input that feeds into <see cref="PickupController"/>.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PickupController))]
    public class PickupInput : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private IPickUpObject curCarriedObj => m_pickupCont.curCarriedObj;
        private bool isCarrying => m_pickupCont.isCarrying;

        [SerializeField, Required] private PickupCollider m_pickupCollider = null;
        [SerializeField, Tag] private string m_pickUpTag = "PickUp";

        private PlayerInput m_playerInput = null;
        private PickupController m_pickupCont = null;

        private Vector2 m_aimVector = Vector2.right;

        // For avoiding against throwing right after picking up
        private int m_pickUpFrame = -1;


        private void Awake()
        {
            m_playerInput = GetComponent<PlayerInput>();
            m_pickupCont = GetComponent<PickupController>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCollider, nameof(m_pickupCollider), this);
            //CustomDebug.AssertComponentIsNotNull(m_playerInput, this);
            //CustomDebug.AssertComponentIsNotNull(m_pickupCont, this);
            #endregion Asserts
        }
        private void FixedUpdate()
        {
            if (isCarrying)
            {
                if (m_playerInput.IsCurrentControlSchemeKeyboard())
                {
                    UpdateAimVectorWithMouse();
                }
                AimWithVector(m_aimVector);
            }
        }


        #region InputMessageDriven
        private void OnPickUp(InputValue value)
        {
            // Released/canceled not pressed.
            if (!value.isPressed) { return; }
            // Already carrying something, don't try to pick something else up.
            if (m_pickupCont.isCarrying) { return; }

            GameObject t_focusedObj = m_pickupCollider.focusedPickupObj;
            // No object that we are focusing.
            if (t_focusedObj == null) { return; }

            IPickUpObject t_pickUpObj = t_focusedObj.GetComponent<IPickUpObject>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(t_pickUpObj, t_focusedObj, this);
            #endregion Asserts
            // Don't pick it up if it is already being carried
            if (t_pickUpObj.isHeld) { return; }
            // Don't pick it up if its not allowed to be picked up right now.
            if (!t_pickUpObj.canBePickedUp) { return; }
            // Don't pick it up if its fallen into the void (zero scale)
            if (t_pickUpObj.transform.localScale.x <= 0.01f) { return; }

            // Set it as the carried object.
            m_pickupCont.SetPickup(t_pickUpObj);

            // Initialize its position
            MoveObjectInitial();
            m_pickUpFrame = Time.frameCount;
        }
        private void OnThrow(InputValue value)
        {
            // Didn't press.
            if (!value.isPressed) { return; }
            // Can't throw if we aren't carrying anything
            if (!m_pickupCont.isCarrying) { return; }
            // Don't throw on the same frame we picked it up.
            if (m_pickUpFrame == Time.frameCount) { return; }

            // Reset cursor texture
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

            // Throw it
            m_pickupCont.ThrowPickup();
        }
        private void OnAiming(InputValue value)
        {
            // Can't aim if we aren't carrying anything.
            if (!isCarrying) { return; }

            switch (m_playerInput.GetCurrentControlScheme())
            {
                case eControlScheme.GamePad:
                    UpdateAimVectorWithGamepad(value);
                    break;
                case eControlScheme.MouseKeyboard:
                    UpdateAimVectorWithMouse();
                    break;
                default:
                    CustomDebug.UnhandledValue(m_playerInput.currentControlScheme, this);
                    break;
            }
        }
        private void OnActivateItem(InputValue value)
        {
            // Start taking action if pressed. Stop if released.
            m_pickupCont.TakeAction(value.isPressed);
        }
        #endregion InputMessageDriven


        /// <summary>
        /// Move the object after it has just been picked up and no aiming has been done yet.
        /// </summary>
        private void MoveObjectInitial()
        {
            switch (m_playerInput.GetCurrentControlScheme())
            {
                case eControlScheme.GamePad:
                    Vector2 t_carriedPos = curCarriedObj.transform.position;
                    Vector2 t_playerPos = transform.position;
                    Vector2 t_initDiff = t_carriedPos - t_playerPos;
                    m_aimVector = t_initDiff.normalized;
                    break;
                case eControlScheme.MouseKeyboard:
                    UpdateAimVectorWithMouse();
                    break;
                default:
                    CustomDebug.UnhandledValue(m_playerInput.currentControlScheme, this);
                    break;
            }
            AimWithVector(m_aimVector);
        }
        private void UpdateAimVectorWithMouse()
        {
            Vector2 t_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 t_playerPos = m_pickupCont.holdPosCenterWorld;
            Vector2 t_mouseDir = (t_mousePos - t_playerPos).normalized;
            m_aimVector = t_mouseDir;
        }
        private void UpdateAimVectorWithGamepad(InputValue value)
        {
            Vector2 t_rawVector = value.Get<Vector2>();
            if (t_rawVector == Vector2.zero) { return; }
            Vector2 t_controllerDir = t_rawVector.normalized;
            m_aimVector = t_controllerDir;
        }
        private void AimWithVector(Vector2 dir)
        {
            m_pickupCont.SetCarryDirection(dir);
        }
        
    }
}