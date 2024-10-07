using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Base class for <see cref="PlayerHandsEnableController"/> and <see cref="TimeCloneHandsEnableController"/> to use.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class HandsEnableController : MonoBehaviour
    {
        public GameObject leftHandParentObj => m_leftHandParentObj;
        public GameObject rightHandParentObj => m_rightHandParentObj;
        public SpriteRenderer holdHandSprRend => m_holdHandSprRend;

        public SpriteRenderer sprRenderer { get; private set; }

        [SerializeField, Required] private GameObject m_leftHandParentObj = null;
        [SerializeField, Required] private GameObject m_rightHandParentObj = null;
        [SerializeField, Required] private SpriteRenderer m_holdHandSprRend = null;


        protected virtual void Awake()
        {
            sprRenderer = GetComponent<SpriteRenderer>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(sprRenderer, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_leftHandParentObj, nameof(m_leftHandParentObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rightHandParentObj, nameof(m_rightHandParentObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_holdHandSprRend, nameof(m_holdHandSprRend), this);
            #endregion Asserts
        }
        protected virtual void Update()
        {
            UpdateHandVisuals();
        }

        protected abstract bool IsCarryingPickup();
        protected abstract bool IsPlayingWindupAnimation();
        protected abstract bool IsPlayerPushingBox();
        protected abstract Vector2 GetMostRecentNonZeroMoveDirectionBeforeCurTime();
        protected abstract IPickUpObject GetCurrentCarriedObject();
        protected abstract IPickUpObject GetWindupObject();
        protected abstract bool IsLoadingIntoLevel();

        private void UpdateHandVisuals()
        {
            if (IsLoadingIntoLevel())
            {
                m_holdHandSprRend.enabled = false;
                m_leftHandParentObj.SetActive(false);
                m_rightHandParentObj.SetActive(false);
                return;
            }

            // Hold Hand
            m_holdHandSprRend.enabled = IsCarryingPickup() || IsPlayingWindupAnimation();
            // Idle Hands
            UpdateIdleHandsEnabledState();
        }
        private void UpdateIdleHandsEnabledState()
        {
            // If pushing box, both idle hands should be turned off.
            if (IsPlayerPushingBox())
            {
                m_leftHandParentObj.SetActive(false);
                m_rightHandParentObj.SetActive(false);
            }
            // If we are carrying, figure out whether its the left or right hand to turn off.
            else if (IsCarryingPickup() || IsPlayingWindupAnimation())
            {
                // Not pushing box, so do normal hand things.
                // Check if facing left or right. If facing like that, we only care about right hand
                if (IsPlayerMovingStraightLeftOrRight())
                {
                    m_leftHandParentObj.SetActive(false);
                    if (IsHoldingObjBelow())
                    {
                        // Hold direction is downward (hide idle hand).
                        m_rightHandParentObj.SetActive(false);
                    }
                    else
                    {
                        // Hold direction is upward (show idle hand).
                        m_rightHandParentObj.SetActive(true);
                    }
                }
                // Facing any other direction than left or right.
                else
                {
                    if (IsHoldingObjOnLeft())
                    {
                        // Hold direction is leftward.
                        // If not reflected, then left hand = off, right hand = on.
                        // If reflected, then left hand = on, right hand = off.
                        m_leftHandParentObj.SetActive(sprRenderer.flipX);
                        m_rightHandParentObj.SetActive(!sprRenderer.flipX);
                    }
                    else
                    {
                        // Hold direction is rightward.
                        // If not reflected, then left hand = on, right hand = off.
                        // If reflected, then left hand = off, right hand = on.
                        m_leftHandParentObj.SetActive(!sprRenderer.flipX);
                        m_rightHandParentObj.SetActive(sprRenderer.flipX);
                    }
                }
            }
            // If not carrying or pushing, then turn on both idle hands (unless moving to the side, then only the right hand)
            else
            {
                m_leftHandParentObj.SetActive(!IsPlayerMovingStraightLeftOrRight());
                m_rightHandParentObj.SetActive(true);
            }
        }
        private bool IsPlayerMovingStraightLeftOrRight()
        {
            Vector2 t_mostRecentDir = GetMostRecentNonZeroMoveDirectionBeforeCurTime();
            float t_movementDot = Vector2.Dot(t_mostRecentDir, Vector2.right);
            return t_movementDot > 0.9f || t_movementDot < -0.9f;
        }
        private bool IsHoldingObjBelow()
        {
            IPickUpObject t_holdingObj;
            if (IsCarryingPickup())
            {
                t_holdingObj = GetCurrentCarriedObject();
            }
            else
            {
                t_holdingObj = GetWindupObject();
            }
            float t_carryDirDot = Vector2.Dot(t_holdingObj.aimDirection, Vector2.up);
            return t_carryDirDot < 0;
        }
        private bool IsHoldingObjOnLeft()
        {
            IPickUpObject t_holdingObj;
            if (IsCarryingPickup())
            {
                t_holdingObj = GetCurrentCarriedObject();
            }
            else
            {
                t_holdingObj = GetWindupObject();
            }
            float t_carryDirDot = Vector2.Dot(t_holdingObj.aimDirection, Vector2.right);
            return t_carryDirDot < 0;
        }
    }
}