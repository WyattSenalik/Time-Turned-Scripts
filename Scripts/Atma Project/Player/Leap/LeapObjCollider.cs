using UnityEngine;

using NaughtyAttributes;

using Helpers.UnityEnums;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Collides with LeapObjects and holds them.
    /// </summary>
    public sealed class LeapObjCollider : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public ILeapObject targetedLeapObj { get; private set; } = null;
        public ILeapObject curLeapingObj { get; set; } = null;
        public eEightDirection highlightDir { get; private set; } = eEightDirection.Up;

        [SerializeField, Required] private PickupCollider m_pickupCol = null;
        [SerializeField, Required] private PickupController m_pickupCont = null;
        [SerializeField] private LayerMask m_layerMask = 0;
        [SerializeField, Min(0.0f)] private float m_radius = 1.0f;
        [SerializeField, Min(0.0f)] private float m_tooCloseDist = 0.1f;
        [SerializeField, Tag] private string m_leapObjTag = "LeapObject";

        private GlobalTimeManager m_timeMan = null;

        private readonly Collider2D[] m_overlapResults = new Collider2D[8];


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCol, nameof(m_pickupCol), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupCont, nameof(m_pickupCont), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
            #endregion Asserts
        }
        private void FixedUpdate()
        {
            // Check if rewinding
            if (!m_timeMan.shouldRecord)
            {
                targetedLeapObj?.OnLeapHighlightEnd();
                targetedLeapObj = null;
                return;
            }

            int t_overlapAmount = Physics2D.OverlapCircleNonAlloc(transform.position, m_radius, m_overlapResults, m_layerMask);

            // If nothing is overlapping, stop highlighting (if not null) and do nothing else.
            if (t_overlapAmount == 0)
            {
                targetedLeapObj?.OnLeapHighlightEnd();
                targetedLeapObj = null;
                return;
            }

            // Find the closest highlighted object (or the previously highlighted object)
            ILeapObject t_closestHighObj = null;
            float t_closestDist = float.PositiveInfinity;
            Vector2 t_closestDiff = Vector2.zero;
            Vector2 t_pos2D = transform.position;
            for (int i = 0; i < t_overlapAmount; ++i)
            {
                //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.blue, IS_DEBUGGING);
                Collider2D t_col = m_overlapResults[i];
                if (!HasLeapTag(t_col.gameObject)) { continue; }
                //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.cyan, IS_DEBUGGING);
                ILeapObject t_leapObj = t_col.GetComponent<ILeapObject>();
                #region Asserts
                //CustomDebug.AssertIComponentOnOtherIsNotNull(t_leapObj, t_col.gameObject, this);
                #endregion Asserts
                // Not available to use, so continue
                if (!t_leapObj.availableToUse) { continue; }
                //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.magenta, IS_DEBUGGING);
                // Thats the object we're currently leaping off of, so not allowed.
                if (t_leapObj == curLeapingObj) { continue; }
                //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.red, IS_DEBUGGING);
                // If the object is a player held spring hat, not allowed to use it.
                if (IsPlayerHeldSpringHat(t_leapObj)) { continue; }
                //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.yellow, IS_DEBUGGING);

                Vector2 t_leapObjPos2D = t_leapObj.transform.position;
                Vector2 t_diff = t_leapObjPos2D - t_pos2D;
                float t_dist = t_diff.magnitude;

                // This one is too close, so ignore it.
                if (t_dist < m_tooCloseDist) { continue; }
                //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.green, IS_DEBUGGING);

                if (t_dist < t_closestDist)
                {
                    t_closestHighObj = t_leapObj;
                    t_closestDiff = t_diff;
                    t_closestDist = t_dist;
                    //CustomDebug.DrawCircle(m_overlapResults[i].transform.position, 0.1f, 8, Color.white, IS_DEBUGGING);
                }
            }

            // If the focused object is closer, don't highlight a leap object.
            if (m_pickupCol.focusedPickupObj != null)
            {
                float t_distToFocusedPickup = Vector2.Distance(transform.position, m_pickupCol.focusedPickupObj.transform.position);
                if (t_distToFocusedPickup < t_closestDist)
                {
                    //CustomDebug.Log($"DistToFocused:{t_distToFocusedPickup} is closer than closestDist:{t_closestDist}", IS_DEBUGGING);
                    targetedLeapObj?.OnLeapHighlightEnd();
                    targetedLeapObj = null;
                    return;
                }
            }

            // Previously highlighted object still the closest, keep that one highlighted.
            if (t_closestHighObj == targetedLeapObj)
            {
                // Update the highlight.
                highlightDir = t_closestDiff.ToClosestEightDirection();
                targetedLeapObj?.OnLeapHighlight(highlightDir);
            }
            // New object highlighted
            else
            {
                // Highlight the object (and dehighlight the prev highlighted (if there is one)).
                targetedLeapObj?.OnLeapHighlightEnd();
                highlightDir = t_closestDiff.ToClosestEightDirection();
                t_closestHighObj?.OnLeapHighlight(highlightDir);
                targetedLeapObj = t_closestHighObj;
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, m_radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_tooCloseDist);
        }


        private bool HasLeapTag(GameObject objToCheck)
        {
            return objToCheck.CompareTag(m_leapObjTag);
        }
        private bool IsPlayerHeldSpringHat(ILeapObject leapObjToCheck)
        {
            IPickUpObject t_pickupObj = leapObjToCheck.GetComponentInParent<IPickUpObject>();
            // Not a spring hat (not a pickup)
            if (t_pickupObj == null) { return false; }
            // If the carried object is this one (this one has to be a spring hat, or it wouldn't be here), then the player is currently carrying this.
            return m_pickupCont.curCarriedObj == t_pickupObj;
        }
    }
}
