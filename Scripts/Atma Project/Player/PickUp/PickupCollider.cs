using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class PickupCollider : MonoBehaviour
    {
        public GameObject focusedPickupObj => m_focusedPickup != null ? m_focusedPickup.gameObject : null;

        [SerializeField, Required] private PickupController m_pickupCont = null;
        [SerializeField, Required] private LeapObjCollider m_leapObjCol = null;
        [SerializeField, Tag] private string m_pickupTag = "PickUp";

        [SerializeField, ReadOnly] private string m_debugFocusedPickupObj = "";

        private PickupIndicatorBehavior m_focusedPickup = null;
        private readonly List<GameObject> m_inRangePickups = new List<GameObject>();

        private int? m_checkedOutID = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCont, nameof(m_pickupCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_leapObjCol, nameof(m_leapObjCol), this);
            #endregion Asserts
        }
        private void FixedUpdate()
        {
            if (focusedPickupObj != null)
            {
                m_debugFocusedPickupObj = focusedPickupObj.GetFullName();
            }
            else
            {
                m_debugFocusedPickupObj = "null";
            }

            if (m_pickupCont.isCarrying)
            {
                if (focusedPickupObj != null)
                {
                    ClearFocusedPickup();
                }
                return;
            }

            // No need to try and change the focused pickup if there is only one or none.
            //if (m_inRangePickups.Count <= 1) { return; }
            UpdateFocusedFromInRange();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Wrong tag.
            if (!other.gameObject.CompareTag(m_pickupTag)) { return; }
            // If in range list doesn't already contain this gameobject, add it.
            if (!m_inRangePickups.Contains(other.gameObject))
            {
                m_inRangePickups.Add(other.gameObject);
            }

            // If there is no focused pickup, this is the new focused.
            if (m_focusedPickup == null)
            {
                SetNewFocusedPickup(other.gameObject);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            // Wrong tag
            if (!other.gameObject.CompareTag(m_pickupTag)) { return; }

            m_inRangePickups.Remove(other.gameObject);

            if (m_focusedPickup != null && m_focusedPickup.gameObject == other.gameObject)
            {
                ClearFocusedPickup();
                if (m_inRangePickups.Count > 0)
                {
                    UpdateFocusedFromInRange();
                }
            }
        }


        private void SetNewFocusedPickup(GameObject focusObj)
        {
            if (m_pickupCont.isCarrying) { return; }

            if (m_leapObjCol.targetedLeapObj != null && focusObj != null)
            {
                float t_distToLeapObj = Vector2.Distance(transform.position, m_leapObjCol.targetedLeapObj.transform.position);
                float t_distToPickupObj = Vector2.Distance(transform.position, focusObj.transform.position);
                if (t_distToLeapObj < t_distToPickupObj)
                {
                    // If the leap object is closer, make it so no focused object is selected.
                    if (m_focusedPickup != null)
                    {
                        ClearFocusedPickup();
                    }
                    return;
                }
            }

            PickUpObject t_pickUpObj = focusObj.GetComponentSafe<PickUpObject>();
            if (t_pickUpObj.isHeld)
            {
                // Its already being held.
                return;
            }

            // If there was a previous focus that was not the given focus.
            if (m_focusedPickup != null && m_focusedPickup.gameObject != focusObj)
            {
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(m_checkedOutID.HasValue, $"A checked out ID to exist, but none does.", this);
                #endregion Asserts
                m_focusedPickup.CancelEnableIndicatorRequest(m_checkedOutID.Value);
                m_checkedOutID = null;
            }

            PickupIndicatorBehavior t_indicator = focusObj.GetComponent<PickupIndicatorBehavior>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(t_indicator, focusObj, this);
            //CustomDebug.AssertIsTrueForComponent(!m_checkedOutID.HasValue, $"checked out id to not have a value", this);
            #endregion Asserts
            m_focusedPickup = t_indicator;
            m_checkedOutID = m_focusedPickup.RequestEnableIndicator();
        }
        private void ClearFocusedPickup()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_checkedOutID.HasValue, $"A checked out ID to exist, but none does.", this);
            #endregion Asserts
            m_focusedPickup.CancelEnableIndicatorRequest(m_checkedOutID.Value);
            m_checkedOutID = null;
            m_focusedPickup = null;
        }
        private void UpdateFocusedFromInRange()
        {
            if (m_pickupCont.isCarrying) { return; }

            GameObject t_origFocused = m_focusedPickup != null ? m_focusedPickup.gameObject : null;
            foreach (GameObject t_pickupObj in m_inRangePickups)
            {
                // Don't compare the focused pickup to itself.
                if (t_pickupObj == t_origFocused) { continue; }

                if (t_pickupObj.TryGetComponent(out PickUpObject t_pickupObjScript))
                {
                    if (t_pickupObjScript.isHeld)
                    {
                        // Already being held.
                        continue;
                    }
                }
                else
                {
                    // No pickupObj?
                    continue;
                }

                Vector2 t_distToFocused = m_focusedPickup != null ? m_focusedPickup.transform.position - transform.position : Vector2.positiveInfinity;
                Vector2 t_distToNew = t_pickupObj.transform.position - transform.position;

                if (t_distToNew.sqrMagnitude < t_distToFocused.sqrMagnitude)
                {
                    SetNewFocusedPickup(t_pickupObj);
                }
            }
        }
    }
}