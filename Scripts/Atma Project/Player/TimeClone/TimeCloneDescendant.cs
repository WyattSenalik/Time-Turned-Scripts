using UnityEngine;

using Timed;
using NaughtyAttributes;
using System.Collections;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// For when we want TimedObjects as children of the time clone.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ITimedObject))]
    public sealed class TimeCloneDescendant : MonoBehaviour
    {
        public TimeCloneInitData cloneData => clone.cloneData;

        public TimeClone clone { get; private set; }
        public ITimedObject timedObject { get; private set; }

        [SerializeField, ReadOnly, BoxGroup("Debugging")] private float m_spawnTime = -1.0f;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private float m_farthestTime = -1.0f;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private float m_blinkTime = -1.0f;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private int m_chargeIndex = -1;


        private void Awake()
        {
            clone = GetComponentInParent<TimeClone>();
            timedObject = this.GetIComponentSafe<ITimedObject>();
            #region Asserts
            //CustomDebug.AssertComponentInParentIsNotNull(clone, this);
            #endregion Asserts

            clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (clone != null)
            {
                clone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }

        private void Initialize()
        {
            m_spawnTime = cloneData.spawnTime;
            m_farthestTime = cloneData.farthestTime;
            m_blinkTime = cloneData.blinkTime;
            m_chargeIndex = cloneData.occupyingCharge;
            timedObject.ForceSetTimeBounds(cloneData.spawnTime, cloneData.farthestTime + cloneData.blinkTime);
        }


        public bool HasEarlyDisappearanceBeforeOrAtTime(float time) => clone.HasEarlyDisappearanceBeforeOrAtTime(time);
    }
}