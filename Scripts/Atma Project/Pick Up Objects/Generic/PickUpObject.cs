using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Helpers;
using Helpers.Extensions;
using Timed;
// Original Authors - Bryce Cernohous-Schrader
// Tweaked by Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class PickUpObject : MonoBehaviour, IPickUpObject
    {
        private const bool IS_DEBUGGING = false;

        public IEventPrimer onPickup => m_onPickup;
        public IEventPrimer onReleased => m_onRelease;

        public bool shouldChangeCursorIcon => m_shouldChangeCursorIcon;
        public float holdingRange => m_holdingRange;
        public bool hasAction => m_hasAction;
        public IPickUpObject.eReleaseType releaseType => m_releaseType;
        public Vector2 direction { get; set; } = Vector2.zero;
        public Vector2 aimDirection => m_overrideDir.HasValue ? m_overrideDir.Value : direction;
        public Rigidbody2D rb2D { get; private set; } = null;
        // Set to true OnPickUp and false OnRelease.
        public bool isHeld => holder != null;
        public bool canBePickedUp { get; private set; } = true;
        public IPickUpHolder holder { get; private set; }
        public Vector2 handPlacementPosition => m_handPlacementPosTrans.position;


        [SerializeField] private bool m_shouldChangeCursorIcon = true;
        [SerializeField] private float m_holdingRange = 0.5f;
        [SerializeField] private bool m_hasAction = false;
        [SerializeField] private IPickUpObject.eReleaseType m_releaseType = IPickUpObject.eReleaseType.Throw;
        [SerializeField, Tag] private string m_wallTag = "Wall";
        [SerializeField] private Transform m_handPlacementPosTrans = null;
        [SerializeField, Required] private BoxCollider2D m_pickupCollider = null;
        [SerializeField] private ContactFilter2D m_insideWallCheckContactFilter = new ContactFilter2D();
        [SerializeField, Min(1)] private int m_binarySearchChecks = 32;
        [SerializeField] private MixedEvent m_onPickup = new MixedEvent();
        [SerializeField] private MixedEvent m_onRelease = new MixedEvent();

        [SerializeField, BoxGroup("Sound"), Required] private SoundRecorder m_pickupSoundRecorder = null;

        private IPickUpBehavior[] m_behaviors = null;
        private readonly IDLibrary m_idLibrary = new IDLibrary();
        private readonly Collider2D[] m_overlapColliders = new Collider2D[4];

        private Vector2? m_overrideDir = Vector2.right;


        private void Awake()
        {
            m_behaviors = GetComponentsInChildren<IPickUpBehavior>();
            rb2D = GetComponent<Rigidbody2D>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(rb2D, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_handPlacementPosTrans, nameof(m_handPlacementPosTrans), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCollider, nameof(m_pickupCollider), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupSoundRecorder, nameof(m_pickupSoundRecorder), this);
            #endregion Asserts
        }


        /// <summary>
        /// Calls each take action in all pickup behaviors. I.E. Gun will have two behaviors to shoot and throw.
        /// </summary>
        /// <param name="onPressedOrReleased"></param>
        public void TakeAction(bool onPressedOrReleased)
        {
            foreach (IPickUpBehavior t_behavior in m_behaviors)
            {
                t_behavior.TakeAction(onPressedOrReleased);
            }
        }
        public void ThrowMe()
        {
            foreach (IPickUpBehavior t_behavior in m_behaviors)
            {
                t_behavior.ThrowMe();
            }
        }
        public void OnPickUp(IPickUpHolder holder)
        {
            rb2D.velocity = Vector2.zero;
            this.holder = holder;

            if (GlobalTimeManager.instance.shouldRecord)
            {
                m_pickupSoundRecorder.Play();
            }

            foreach (IPickUpBehavior t_behavior in m_behaviors)
            {
                t_behavior.OnPickUp();
            }
            m_onPickup.Invoke();
        }
        public void OnReleased()
        {
            this.holder = null;
            foreach (IPickUpBehavior t_behavior in m_behaviors)
            {
                t_behavior.OnReleased();
            }
            m_onRelease.Invoke();
        }
        public int RequestDisallowPickup()
        {
            int t_id = m_idLibrary.CheckoutID();
            canBePickedUp = false;
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(RequestDisallowPickup)}. {nameof(t_id)}={t_id}", this, IS_DEBUGGING);
            #endregion Logs
            return t_id;
        }
        public void CancelDisallowPickupRequest(int requestID)
        {
            m_idLibrary.ReturnID(requestID);
            canBePickedUp = m_idLibrary.AreAllIDsReturned();
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(CancelDisallowPickupRequest)}. {nameof(requestID)}={requestID}. {nameof(canBePickedUp)}={canBePickedUp}", this, IS_DEBUGGING);
            #endregion Logs
        }

        public void UpdateCarryObjPosition(Vector2 parentPos, float parentSize)
        {
            // Don't update position if direction is zero.
            if (direction == Vector2.zero) { return; }

            Vector2 t_correctedPos = HandleWallCollisionWhenCarryingObject(parentPos, parentSize, out bool t_wasCorrected);
            transform.position = t_correctedPos;
            transform.localScale = new Vector3(parentSize, parentSize, parentSize);

            if (t_wasCorrected)
            {
                Vector2 t_correctedDir = (t_correctedPos - parentPos).normalized;
                m_overrideDir = t_correctedDir;
            }
            else
            {
                m_overrideDir = null;
            }
        }


        /// <summary>
        /// Checks if there is a wall between the desired hold position and the parent position. Restricts the hold position to not be in the wall or go through it.
        /// </summary>
        private Vector2 HandleWallCollisionWhenCarryingObject(Vector2 parentPos, float parentSize, out bool wasCorrected)
        {
            Vector2 t_normalSizeOffset = direction * holdingRange;
            Vector2 t_scaledSizeOffset = t_normalSizeOffset * parentSize;
            Vector2 t_desiredPos = parentPos + t_scaledSizeOffset;
            
            if (IsValidHoldPosition(t_desiredPos, parentSize))
            {
                wasCorrected = false;
                return t_desiredPos;
            }
            else
            {
                float[] t_checkAngles = new float[] { 45, -45, 90, -90, 135, -135, 180 };

                // Result position will be used and updated for when a valid check angle is found.
                Vector2 t_resultPos = Vector2.negativeInfinity;
                int? t_validIndex = null;
                for (int i = 0; i < t_checkAngles.Length; ++i)
                {
                    Vector2 t_rotatedOffset = Quaternion.Euler(0, 0, t_checkAngles[i]) * t_scaledSizeOffset;
                    Vector2 t_adjustedDesiredPos = parentPos + t_rotatedOffset;

                    if (IsValidHoldPosition(t_adjustedDesiredPos, parentSize))
                    {
                        t_validIndex = i;
                        t_resultPos = t_adjustedDesiredPos;
                        break;
                    }
                }

                if (!t_validIndex.HasValue)
                {
                    #region Logs
                    //CustomDebug.LogWarningForComponent($"There was no valid place to hold the object ({name})", this);
                    #endregion Logs
                    // Hopefully won't happen.
                    wasCorrected = false;
                    return t_desiredPos;
                }
                else
                {
                    // Alternate between getting closer to the invalid angle and the valid angle to find the valid angle most on the border between invalid and valid (closest to the player's desired hold position).
                    float t_previousInvalidAngle = 0.0f;
                    float t_previousValidAngle = t_checkAngles[t_validIndex.Value];
                    float t_curCheckAngle = (t_previousInvalidAngle + t_previousValidAngle) * 0.5f;
                    for (int i = 0; i < m_binarySearchChecks; ++i)
                    {
                        Vector2 t_rotatedOffset = Quaternion.Euler(0, 0, t_curCheckAngle) * t_scaledSizeOffset;
                        Vector2 t_adjustedDesiredPos = parentPos + t_rotatedOffset;

                        float t_nextCheckAngle;
                        if (IsValidHoldPosition(t_adjustedDesiredPos, parentSize))
                        {
                            // The position was valid, so go towards the invalid one next.
                            t_nextCheckAngle = (t_curCheckAngle + t_previousInvalidAngle) * 0.5f;
                            t_previousValidAngle = t_curCheckAngle;
                            // Also since this was valid, store it as the result for now.
                            t_resultPos = t_adjustedDesiredPos;
                        }
                        else
                        {
                            // Position was invalid, go towards the valid one next.
                            t_nextCheckAngle = (t_curCheckAngle + t_previousValidAngle) * 0.5f;
                            t_previousInvalidAngle = t_curCheckAngle;
                        }
                        t_curCheckAngle = t_nextCheckAngle;
                    }
                    wasCorrected = true;
                    return t_resultPos;
                }
            }
        }
        private bool IsValidHoldPosition(Vector2 desiredHoldPos, float parentSize)
        {
            Vector2 t_colliderScaledSize = m_pickupCollider.size * parentSize;

            int t_overlappedColliders = Physics2D.OverlapBox(desiredHoldPos, t_colliderScaledSize, 0.0f, m_insideWallCheckContactFilter, m_overlapColliders);
            if (t_overlappedColliders > m_overlapColliders.Length)
            {
                t_overlappedColliders = m_overlapColliders.Length;
            }
            bool t_hitAWallCollider = false;
            for (int i = 0; i < t_overlappedColliders; ++i)
            {
                Collider2D t_curCol = m_overlapColliders[i];
                if (t_curCol != m_pickupCollider)
                {
                    t_hitAWallCollider = true;
                    break;
                }
            }

            return !t_hitAWallCollider;
        }
    }
}