using System;
using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Base class for scripts that want to copy snapshot recorded data from a descendant of the player to a descendant WITH THE SAME NAME (as the player's descendant) of the clone.
    /// </summary>
    /// <typeparam name="TCopyFromBehaviour">SnapshotRecorder the data will be copied from.</typeparam>
    /// <typeparam name="TSnapshot">Snapshot of the SnapshotRecorder.</typeparam>
    /// <typeparam name="TData">Data being stored in the Snapshots.</typeparam>
    [RequireComponent(typeof(TimeCloneDescendant))]
    public abstract class TimeCloneDescendantSnapshotCopier<TCopyFromBehaviour, TSnapshot, TData> : TimedRecorder
        where TCopyFromBehaviour : SnapshotRecorder<TSnapshot, TData>
        where TSnapshot : ISnapshot<TData, TSnapshot>
        where TData : IEquatable<TData>
    {
        private const bool IS_DEBUGGING = false;

        public IReadOnlySnapshotScrapbook<TSnapshot, TData> scrapbookSnapshot => m_scrapbookSnippit;

        private TimeCloneDescendant m_cloneDescendant = null;
        private IReadOnlySnapshotScrapbook<TSnapshot, TData> m_scrapbookSnippit = null;

        /// <summary>For debugging only.</summary>
        [SerializeField, ReadOnly] private int m_snapCount = 0;


        protected override void Awake()
        {
            base.Awake();

            m_cloneDescendant = this.GetComponentSafe<TimeCloneDescendant>();
        }
        protected virtual void Start()
        {
            m_cloneDescendant.clone.onInitialized.ToggleSubscription(Intitialize, true);
        }

        protected virtual void OnDestroy()
        {
            if (m_cloneDescendant != null)
            {
                if (m_cloneDescendant.clone != null)
                {
                    m_cloneDescendant.clone.onInitialized.ToggleSubscription(Intitialize, false);
                }
            }
        }
        private void Intitialize()
        {
            GameObject t_origPlayerObj = m_cloneDescendant.cloneData.originalPlayerObj;
            Transform t_copyFromChildTrans = t_origPlayerObj.transform.FindDescendant(name);
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(t_copyFromChildTrans != null, $"to find an object with name ({name}) as a child of {t_origPlayerObj.name}", this);
            #endregion Asserts
            TCopyFromBehaviour t_copyFromBehav = t_copyFromChildTrans.GetComponentSafe<TCopyFromBehaviour>();
            // Get all the snapshots after this object's spawn time.
            TSnapshot[] t_snapsAfterSpawn = t_copyFromBehav.scrapbook.GetAllSnapshotsAfter(spawnTime, true);
            #region Logs
            //CustomDebug.LogForComponent($"Found {t_snapsAfterSpawn.Length} snapshots after time {spawnTime}. There were {t_copyFromBehav.scrapbook.Count} original snapshots.", this, IS_DEBUGGING);
            #endregion Logs
            // Pack the snapshots into a new scrapbook.
            t_snapsAfterSpawn[^1].interpolationOpt = eInterpolationOption.Step;
            m_scrapbookSnippit = new SnapshotScrapbook<TSnapshot, TData>(t_snapsAfterSpawn);
            m_snapCount = m_scrapbookSnippit.Count;
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);
            #region Logs
            //CustomDebug.LogForComponent(nameof(SetToTime), this, IS_DEBUGGING);
            #endregion Logs
            // If haven't yet initialized
            if (m_scrapbookSnippit == null) { return; }
            // If the time clone has an early disapperance before this time, do nothing
            if (m_cloneDescendant.HasEarlyDisappearanceBeforeOrAtTime(time)) { return; }

            TSnapshot t_curSnap = m_scrapbookSnippit.GetSnapshot(time);
            ApplyData(t_curSnap);
        }


        protected abstract void ApplyData(TSnapshot snapshot);
    }
}