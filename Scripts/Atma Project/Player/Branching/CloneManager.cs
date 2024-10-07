using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Atma.Events;
using Helpers.Events.GameEventSystem;
using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Keeps track of the player's clones and handles working with them.
    /// Assumes it is attached to the player.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CloneManager : TimedComponent
    {
        public float blinkTimeAm => m_blinkTimeAm;
        public IReadOnlyList<TimeClone> existingClones => m_existingClones;
        public IReadOnlyList<TimeClone> disconnectedClones => m_disconnectedClones;

        [SerializeField, Min(0)] private int m_maxCharges = 3;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneDiedEventIdentifierSO m_cloneDiedEventID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneDeletedEventIdentifierSO m_cloneDeletedEventID = null;
        [SerializeField, Required, BoxGroup("Event IDs")]
        private CloneDisconnectedEventIdentifierSO m_cloneDisconnectedEventID = null;

        [SerializeField, Required] private GameObject m_timeClonePrefab = null;

        [SerializeField, Min(0.0f)] private float m_blinkTimeAm = 1.0f;

        private IGameEvent<ICloneDiedContext> m_cloneDiedEvent = null;
        private IGameEvent<ICloneDeletedContext> m_cloneDeletedEvent = null;
        private IGameEvent<ICloneDisconnectedContext> m_cloneDisconnectedEvent = null;

        private readonly List<TimeClone> m_existingClones = new List<TimeClone>();
        private readonly List<TimeClone> m_disconnectedClones = new List<TimeClone>();
        private readonly List<int> m_availableCharges = new List<int>();


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDiedEventID, nameof(m_cloneDiedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDeletedEventID, nameof(m_cloneDeletedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDisconnectedEventID, nameof(m_cloneDisconnectedEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeClonePrefab, nameof(m_timeClonePrefab), this);
            #endregion Asserts

            // Fill the available charges
            for (int i = 0; i < m_maxCharges; ++i)
            {
                m_availableCharges.Add(i);
            }

            m_cloneDiedEvent = m_cloneDiedEventID.CreateEvent();
            m_cloneDeletedEvent = m_cloneDeletedEventID.CreateEvent();
            m_cloneDisconnectedEvent = m_cloneDisconnectedEventID.CreateEvent();
        }
        private void OnDestroy()
        {
            m_cloneDiedEvent.DeleteEvent();
            m_cloneDeletedEvent.DeleteEvent();
            m_cloneDisconnectedEvent.DeleteEvent();

        }


        /// <summary>
        /// Creates a clone that is exists from the given start time to the given end time.
        /// </summary>
        public TimeClone SpawnClone(float start, float end)
        {
            LevelOptions t_levelOpt = LevelOptions.GetInstanceSafe();

            // Create the object.
            GameObject t_cloneObj = Instantiate(m_timeClonePrefab, transform.position, transform.rotation);
            // Grab references.
            TimeClone t_timeClone = t_cloneObj.GetComponentSafe<TimeClone>();
            ITimedObject t_timeCloneObj = t_cloneObj.GetIComponentSafe<ITimedObject>();
            // Clone index is which charge is being used, so is determined by 
            int t_chargeUsed = TakeNextAvailableCharge();
            // Clone blink time (amount of time the clone will blink for after its end.
            float t_blinkTime = t_levelOpt.noTimeLimit ? float.PositiveInfinity : (t_levelOpt.time - end);
            // Initialize.
            t_timeClone.Initialize(new TimeCloneInitData(gameObject, this, start, end, t_chargeUsed, t_blinkTime));
            // Force set the bounds.
            t_timeCloneObj.ForceSetTimeBounds(start, end + t_blinkTime);
            // Add to list.
            m_existingClones.Add(t_timeClone);

            return t_timeClone;
        }
        public TimeClone SpawnCloneDisconnected(float start, float end)
        {
            LevelOptions t_levelOpt = LevelOptions.GetInstanceSafe();

            // Create the object.
            GameObject t_cloneObj = Instantiate(m_timeClonePrefab, transform.position, transform.rotation);
            // Grab references.
            TimeClone t_timeClone = t_cloneObj.GetComponentSafe<TimeClone>();
            ITimedObject t_timeCloneObj = t_cloneObj.GetIComponentSafe<ITimedObject>();
            // Disconnected clone doesn't take up a charge.
            int t_chargeUsed = -1;
            // Clone blink time (amount of time the clone will blink for after its end.
            float t_blinkTime = t_levelOpt.noTimeLimit ? float.PositiveInfinity : (t_levelOpt.time - end);
            // Force set the bounds.
            t_timeCloneObj.ForceSetTimeBounds(start, end + t_blinkTime);
            // Initialize.
            t_timeClone.Initialize(new TimeCloneInitData(gameObject, this, start, end, t_chargeUsed, t_blinkTime));
            t_timeClone.isDisconnected = true;
            // We add it to the disconnected list.
            m_disconnectedClones.Add(t_timeClone);

            return t_timeClone;
        }
        /// <summary>
        /// Destroys all the clones that spawned after the given time.
        /// </summary>
        public void DestroyClonesAfterTime(float time)
        {
            for (int i = 0; i < m_existingClones.Count; ++i)
            {
                TimeClone t_singleClone = m_existingClones[i];
                ITimedObject t_cloneTimedObj = t_singleClone.timedObject;

                if (t_cloneTimedObj.spawnTime > time)
                {
                    // Invoke the deleted event before actually deleting the clone.
                    m_cloneDeletedEvent.Invoke(new CloneDeletedContext(t_singleClone));
                    // Return the clones charge before destroying it.
                    ReturnCharge(t_singleClone.cloneData.occupyingCharge);
                    // Destroy it.
                    Destroy(t_singleClone.gameObject);
                    m_existingClones.RemoveAt(i);
                    --i;
                }
            }
        }
        public void DeleteAllConnectedClones()
        {
            for (int i = 0; i < m_existingClones.Count; ++i)
            {
                TimeClone t_singleClone = m_existingClones[i];
                // Invoke the deleted event before actually deleting the clone.
                m_cloneDeletedEvent.Invoke(new CloneDeletedContext(t_singleClone));
                // Return the clones charge before destroying it.
                ReturnCharge(t_singleClone.cloneData.occupyingCharge);
                // Destroy it.
                Destroy(t_singleClone.gameObject);
            }
            m_existingClones.Clear();
        }
        public void DeleteAllDisonnectedClones()
        {
            for (int i = 0; i < m_disconnectedClones.Count; ++i)
            {
                TimeClone t_singleClone = m_disconnectedClones[i];
                // Invoke the deleted event before actually deleting the clone.
                m_cloneDeletedEvent.Invoke(new CloneDeletedContext(t_singleClone));
                // No returning of hte charge because its already been returned (since this clone is disconnected).
                // Destroy it.
                Destroy(t_singleClone.gameObject);
            }
            m_disconnectedClones.Clear();
        }
        /// <summary>
        /// Deletes both connected and disconnected clones.
        /// </summary>
        public void DeleteAllClones()
        {
            DeleteAllConnectedClones();
            DeleteAllDisonnectedClones();
        }
        /// <summary>
        /// Meant to ONLY be called from <see cref="TimeCloneHealth"/> when a clone dies.
        /// </summary>
        public void InvokeCloneDiedEvent(ICloneDiedContext context)
        {
            m_cloneDiedEvent.Invoke(context);
        }
        /// <summary>
        /// Keeps the clones alive, but makes it so they are not being tracked by the CloneManager anymore
        /// </summary>
        public void DisconnectExistingClones()
        {
            for (int i = 0; i < m_existingClones.Count; ++i)
            {
                TimeClone t_clone = m_existingClones[i];
                m_availableCharges.Add(t_clone.cloneData.occupyingCharge);
                m_disconnectedClones.Add(t_clone);
                t_clone.isDisconnected = true;
                m_cloneDisconnectedEvent.Invoke(new CloneDisconnectedContext(t_clone));
            }
            m_existingClones.Clear();
        }
        public void HaveAllClonesPlayDisappearAnimation()
        {
            foreach (TimeClone t_clone in m_existingClones)
            {
                PlaySingleDisappearAnimation(t_clone);
            }
            foreach (TimeClone t_clone in m_disconnectedClones)
            {
                PlaySingleDisappearAnimation(t_clone);
            }

            void PlaySingleDisappearAnimation(TimeClone clone)
            {
                clone.disappearAnimation.PlayAsUnityTimeAnimation();
                clone.cloneBlink.PlayBlinkAnimationInUnityTime(clone.disappearAnimation.animLength);
            }
        }

        #region Query Functions
        public IReadOnlyList<TimeClone> GetAllExistingClones() => m_existingClones;
        /// <summary>
        /// Returns the time clones active at a given time.
        /// </summary>
        public List<TimeClone> GetClonesActiveAtTime(float time)
        {
            List<TimeClone> t_clones = new List<TimeClone>();
            foreach (TimeClone t_clone in m_existingClones)
            {
                TimeCloneInitData t_cloneData = t_clone.cloneData;
                TimeFrame t_frame = new TimeFrame(t_cloneData.spawnTime, t_cloneData.farthestTime);
                if (t_frame.ContainsTime(time))
                {
                    t_clones.Add(t_clone);
                }
            }
            return t_clones;
        }
        public List<TimeClone> GetClonesActiveAtOrBeforeTime(float time)
        {
            List<TimeClone> t_clones = new List<TimeClone>();
            foreach (TimeClone t_clone in m_existingClones)
            {
                TimeCloneInitData t_cloneData = t_clone.cloneData;
                if (t_cloneData.spawnTime <= time)
                {
                    t_clones.Add(t_clone);
                }
            }
            return t_clones;
        }
        /// <summary>
        /// Returns the amount of time clones active at a given time.
        /// </summary>
        public int GetAmountClonesActiveAtTime(float time)
        {
            int t_activeClonesAm = 0;
            foreach (TimeClone t_clone in m_existingClones)
            {
                TimeCloneInitData t_cloneData = t_clone.cloneData;
                TimeFrame t_frame = new TimeFrame(t_cloneData.spawnTime, t_cloneData.farthestTime);
                if (t_frame.ContainsTime(time))
                {
                    ++t_activeClonesAm;
                }
            }
            return t_activeClonesAm;
        }
        /// <summary>
        /// Returns the amount of time clones active during the specified time frame (inclusively).
        /// </summary>
        /// <param name="startTime">start of TimeFrame (inclusive).</param>
        /// <param name="endTime">end of TimeFrame (inclusive).</param>
        public int GetAmountClonesInTimeFrame(float startTime, float endTime) => GetAmountClonesInTimeFrame(new TimeFrame(startTime, endTime));
        /// <summary>
        /// Returns the amount of time clones active during the specified time frame (inclusively).
        /// </summary>
        public int GetAmountClonesInTimeFrame(TimeFrame timeFrame)
        {
            int t_activeClonesAm = 0;
            foreach (TimeClone t_clone in m_existingClones)
            {
                TimeCloneInitData t_cloneData = t_clone.cloneData;
                TimeFrame t_cloneLifetimeFrame = new TimeFrame(t_cloneData.spawnTime, t_cloneData.farthestTime);
                if (timeFrame.ContainsTime(t_cloneLifetimeFrame.startTime) || timeFrame.ContainsTime(t_cloneLifetimeFrame.endTime) || t_cloneLifetimeFrame.ContainsTime(timeFrame.startTime) || t_cloneLifetimeFrame.ContainsTime(timeFrame.endTime))
                {
                    ++t_activeClonesAm;
                }
            }
            return t_activeClonesAm;
        }
        /// <summary>
        /// Amount of clone charges that the player currently has available.
        /// </summary>
        public int GetCloneChargesAvailable()
        {
            return GetAmountClonesInTimeFrame(0.0f, curTime);
        }
        #endregion Query Functions

        private int TakeNextAvailableCharge()
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(0, m_availableCharges, this);
            #endregion Asserts
            int t_nextCharge = m_availableCharges[0];
            m_availableCharges.RemoveAt(0);
            return t_nextCharge;
        }
        private void ReturnCharge(int chargeIndex)
        {
            if (m_availableCharges.Contains(chargeIndex))
            {
                // Charge already returned
                return;
            }
            m_availableCharges.Add(chargeIndex);
            m_availableCharges.Sort();
        }
    }
}
