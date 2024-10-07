using System.Collections.Generic;
using UnityEngine;

using Helpers.Events;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Helpers.Physics.EnclosedTrigger
{
    /// <summary>
    /// Enter, Stay, and Exit events for when a collider becomes fully
    /// enclosed by this collider. Sets the attached Collider2D to trigger on Awake.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class EnclosedTriggerCollider : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public IEventPrimer<Collider2D> onFullyEnclosedEnter => m_onFullyEnclosedEnter;
        public IEventPrimer<Collider2D> onFullyEnclosedStay => m_onFullyEnclosedStay;
        public IEventPrimer<Collider2D> onFullyEnclosedExit => m_onFullyEnclosedExit;

        [SerializeField] private MixedEvent<Collider2D> m_onFullyEnclosedEnter = new MixedEvent<Collider2D>();
        [SerializeField] private MixedEvent<Collider2D> m_onFullyEnclosedStay = new MixedEvent<Collider2D>();
        [SerializeField] private MixedEvent<Collider2D> m_onFullyEnclosedExit = new MixedEvent<Collider2D>();

        private Collider2D m_collider = null;

        // Object that have already been fully enclosed.
        private readonly HashSet<int> m_enclosedObjIDs = new HashSet<int>(4);


        private void Awake()
        {
            // Only support a single collider.
            Collider2D[] t_cols = GetComponents<Collider2D>();
            if (t_cols.Length == 1)
            {
               m_collider = t_cols[0];
            }
            else if (t_cols.Length > 1)
            {
                m_collider = t_cols[0];
                #region Logs
                //CustomDebug.LogWarningForComponent($"Only supports having a single Collider2D attached to the same object. Colliders besides a single {m_collider.GetType().Name} will be ignored.", this);
                #endregion Logs
            }
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_collider, this);
            #endregion Asserts
            m_collider.isTrigger = true;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if they are completely enclosed by this collider's collider
            // and mark it as enclosed if it is. If just enclosed, call enter.
            // If was previously eclosed, call stay.
            AddIfEnclosed(collision);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            // Check if they are completely enclosed by this collider's collider
            // and mark it as enclosed if it is. If just enclosed, call enter.
            // If was previously eclosed, call stay.
            if (!AddIfEnclosed(collision))
            {
                // Is not completely enclosed, try to remove it. If it was previously
                // enclosed, call exit.
                TryRemoveEnclosedObject(collision);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            TryRemoveEnclosedObject(collision);
        }


        /// <returns>True if enclosed, false if not enclosed.</returns>
        private bool AddIfEnclosed(Collider2D collision)
        {
            // Check if they are completely enclosed by this collider's collider.
            if (m_collider.bounds.Contains(collision.bounds))
            {
                int t_instanceID = collision.gameObject.GetInstanceID();
                // Already an enclosed object, call stay
                if (m_enclosedObjIDs.Contains(t_instanceID))
                {
                    m_onFullyEnclosedStay.Invoke(collision);
                }
                // Object is a new enclosed object, remember it as one and call enter.
                else
                {
                    m_enclosedObjIDs.Add(t_instanceID);

                    m_onFullyEnclosedEnter.Invoke(collision);
                }
                #region Logs
                //CustomDebug.LogForComponent($"Collider ({collision}) is fully enclosed in this collider ({name}).", this, IS_DEBUGGING);
                #endregion Logs
                return true;
            }
            return false;
        }
        /// <returns>If was removed.</returns>
        private bool TryRemoveEnclosedObject(Collider2D collision)
        {
            int t_instanceID = collision.gameObject.GetInstanceID();
            // Try to remove it from the list.
            if (m_enclosedObjIDs.Remove(t_instanceID))
            {
                #region Logs
                //CustomDebug.LogForComponent($"Collider ({collision}) is no longer enclosed in this collider ({name}).", this, IS_DEBUGGING);
                #endregion Logs
                // If it was removed, then call exit
                m_onFullyEnclosedExit.Invoke(collision);
                return true;
            }
            return false;
        }
    }
}