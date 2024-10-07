using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Scrapbook that holds snapshots in chronological order.
    /// </summary>
    /// <typeparam name="TSnap">Snapshot class.</typeparam>
    /// <typeparam name="TSnapType">Type of data the snapshot class has.</typeparam>
    public sealed class SnapshotScrapbook<TSnap, TSnapType> :
        ISnapshotScrapbook<TSnap, TSnapType>
        where TSnap : ISnapshot<TSnapType, TSnap>
    {
        private const bool IS_DEBUGGING = false;
        private const float CLOSE_ENOUGH_TIME = 0.1f;

        public int Count => m_snapshots != null ? m_snapshots.Count : 0;

        // List of snapshots. Should always be sorted in increasing time order.
        private readonly List<TSnap> m_snapshots = null;

        private int m_prevGetSnapIndex = -1;
        private float m_prevGetSnapTime = -1.0f;


        public SnapshotScrapbook()
        {
            m_snapshots = new List<TSnap>();
        }
        public SnapshotScrapbook(params TSnap[] snaps) : this()
        {
            // Add the given snapshots to the binder
            foreach (TSnap s in snaps)
            {
                AddSnapshot(s.Clone());
            }
        }


        /// <summary>
        /// Adds the given snapshot to the end of the scrapbook.
        /// Must be after the latest snapshot.
        /// </summary>
        public void AddSnapshot(TSnap snapshot)
        {
            if (snapshot == null)
            {
                //CustomDebug.LogError($"Tried to add null snapshot to scrapbook");
                return;
            }
            //#region Asserts
            //float t_latestTime = GetLatestTime();
            //if (t_latestTime >= 0.0f)
            //{
            //    if (snapshot.time <= t_latestTime)
            //    {
            //        //CustomDebug.LogError($"the given snapshot (with time {snapshot.time}) to {nameof(AddSnapshot)} to be at a time later than the all other snapshots ({t_latestTime})");
            //    }
            //}
            //#endregion Asserts
            m_snapshots.Add(snapshot);
        }
        /// <summary>
        /// Gets the snapshot at the current time.
        /// If there is no snapshot at the given time (most likely), 
        /// will return an interpolated snapshot using the snapshots before 
        /// and after the given time.
        /// </summary>
        /// <param name="time">Time to get a snapshot for.</param>
        public TSnap GetSnapshot(float time)
        {
            int t_index;
            if (m_prevGetSnapIndex > 0 && time >= m_prevGetSnapTime - CLOSE_ENOUGH_TIME && time <= m_prevGetSnapTime + CLOSE_ENOUGH_TIME)
            {
                // DO LINEAR SEARCH instead of binary
                if (time == m_prevGetSnapTime)
                {
                    t_index = m_prevGetSnapIndex;
                }
                else
                {
                    int t_increment;
                    if (time > m_prevGetSnapTime)
                    {
                        t_increment = 1;
                    }
                    else
                    {
                        t_increment = -1;
                    }

                    t_index = -1;
                    for (int i = m_prevGetSnapIndex; i >= 0 && i < m_snapshots.Count; i += t_increment)
                    {
                        TSnap t_curVal = m_snapshots[i];
                        if (t_curVal.time == time)
                        {
                            t_index = i;
                            break;
                        }
                        else if (t_curVal.time > time && t_increment > 0)
                        {
                            t_index = i - 1;
                            break;
                        }
                        else if (t_curVal.time < time && t_increment < 0)
                        {
                            t_index = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                t_index = FindClosestSnapshot(time);
                // There are no snapshots.
                if (t_index < 0)
                {
                    //CustomDebug.LogWarning($"{nameof(GetSnapshot)} called when there are no snapshots yet.");
                    return default;
                }
            }
            #region Logs
            //CustomDebug.LogForObject($"Closest snapshot to time {time} had time {m_snapshots[t_index].time} at index {t_index}", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(t_index, m_snapshots, GetType().Name);
            #endregion Asserts
            // Closest snap is always less than the given time.
            TSnap t_closestSnap = m_snapshots[t_index];
            // This means the other snap is just the snap after that one.
            // If other is in bounds, use it to interpolate.
            if (t_index + 1 < m_snapshots.Count)
            {
                #region Logs
                //CustomDebug.Log($"Interpolating for time {time} with closest snap ({t_closestSnap}) and later snap ({m_snapshots[t_index + 1]})", IS_DEBUGGING);
                #endregion Logs
                return t_closestSnap.Interpolate(m_snapshots[t_index + 1], time);
            }
            // If the other index is out of bounds, we should use a previous snap to extrapolate.
            else
            {
                if (t_index - 1 < 0)
                {
                    #region Logs
                    //CustomDebug.Log($"Interpolating for time {time} with only closest snap {t_closestSnap}", IS_DEBUGGING);
                    #endregion Logs
                    // If thats also out of bounds, just use itself to interpolate
                    return t_closestSnap.Interpolate(t_closestSnap, time);
                }
                else
                {
                    #region Logs
                    //CustomDebug.Log($"Interpolating for time {time} with early snap {m_snapshots[t_index - 1]} and closest snap {t_closestSnap}", IS_DEBUGGING);
                    #endregion Logs
                    // Since other snap is earlier than the closest snap, interpolate with the other sinstead of the closest.
                    return m_snapshots[t_index - 1].Interpolate(t_closestSnap, time);
                }
            }
        }
        /// <summary>
        /// Gets the time of the snapshot that takes place latest in time.
        /// 
        /// Returns -1 if there are not any snapshots yet.
        /// </summary>
        public float GetLatestTime()
        {
            int t_count = m_snapshots.Count;
            if (t_count <= 0) { return -1.0f; }
            return m_snapshots[t_count - 1].time;
        }
        public int RemoveSnapshotsAfter(float time, bool includeSnapAtTime=true)
        {
            // There are no snapshots.
            if (m_snapshots.Count <= 0) { return 0; }

            int t_amountRemoved = 0;
            TSnap t_snap = m_snapshots[^1];
            while (t_snap.time > time)
            {
                //CustomDebug.LogForObject($"Removing snapshot with time {t_snap.time}", this, IS_DEBUGGING);
                m_snapshots.RemoveAt(m_snapshots.Count - 1);
                ++t_amountRemoved;

                // No snapshots remain
                if (m_snapshots.Count <= 0)
                {
                    return t_amountRemoved;
                }
                t_snap = m_snapshots[m_snapshots.Count - 1];
            }
            // Now the scrapbook contains snapshots up to (and including)
            // the snapshot at the found index. Its possible that the snapshot
            // is for exactly the sought time.
            t_snap = m_snapshots[m_snapshots.Count - 1];
            if (t_snap.time == time)
            {
                if (includeSnapAtTime)
                {
                    m_snapshots.RemoveAt(m_snapshots.Count - 1);
                    ++t_amountRemoved;
                }
                else
                {
                    // Be extra careful and check that our assumption is not wrong.
                    #region Asserts
                    //CustomDebug.AssertIsTrueForObj(t_snap.time <= time, $"found snapshot {t_snap} at time {t_snap.time} to be before the sought time of {time}.", this);
                    #endregion Asserts
                }
            }            

            return t_amountRemoved;
        }
        public TSnap[] GetAllSnapshotsAfter(float time,
            bool includeInterpolatedAtGivenTime)
        {
            // Get the index directly before the time.
            int t_index = FindClosestSnapshot(time);
            // There are no snapshots.
            if (t_index < 0) { return new TSnap[0]; }

            List<TSnap> t_rtnSnaps = new List<TSnap>();
            // Add the first snapshot as an interpolated one if desired.
            if (includeInterpolatedAtGivenTime)
            {
                t_rtnSnaps.Add(GetSnapshot(time));
            }
            // Add each snapshot after the index directly before the time.
            for (int i = t_index + 1; i < Count; ++i)
            {
                t_rtnSnaps.Add(m_snapshots[i].Clone());
            }
            return t_rtnSnaps.ToArray();
        }
        public void GetSnapshotsBeforeAndAfter(float time, out bool hasBeforeSnap,
            out bool hasAfterSnap, out TSnap beforeSnap, out TSnap afterSnap)
        {
            // Get the index directly before the time.
            int t_index = FindClosestSnapshot(time);
            // There are no snapshots.
            if (t_index < 0)
            {
                hasBeforeSnap = false;
                hasAfterSnap = false;
                beforeSnap = default;
                afterSnap = default;
                return;
            }
            // There is a before snap.
            hasBeforeSnap = true;
            beforeSnap = m_snapshots[t_index];
            // There is no after snap.
            if (t_index + 1 >= Count)
            {
                hasAfterSnap = false;
                afterSnap = default;
                return;
            }
            // There is an after snap.
            hasAfterSnap = true;
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(t_index + 1, m_snapshots, this);
            #endregion Asserts
            afterSnap = m_snapshots[t_index + 1];
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(beforeSnap.time < time, $"the before snap's time ({beforeSnap.time}) to be after {time}", this);
            //CustomDebug.AssertIsTrueForObj(afterSnap.time >= time, $"the after snap's time ({afterSnap.time}) to be after {time}", this);
            #endregion Asserts
        }
        public TSnap GetLatestSnapshot()
        {
            if (Count <= 0)
            {
                return default;
            } 
            return m_snapshots[Count - 1];
        }
        public void RemoveSnapshotAtIndex(int index)
        {
            if (index < 0 && index >= Count)
            {
                //CustomDebug.LogError($"Tried to REMOVE snapshot at index {index} when index needed to be in range [0, {Count - 1}]");
                return;
            }
            m_snapshots.RemoveAt(index);
        }
        public TSnap GetSnapshotAtIndex(int index)
        {
            if (index < 0 && index >= Count)
            {
                //CustomDebug.LogError($"Tried to GET snapshot at index {index} when index needed to be in range [0, {Count - 1}]");
                return default;
            }
            return m_snapshots[index];
        }
        public void InsertSnapshotAtIndex(int index, TSnap snap)
        {
            if (index < 0 && index >= Count)
            {
                //CustomDebug.LogError($"Tried to INSERT snapshot ({snap}) at index {index} when index needed to be in range [0, {Count - 1}]");
                return;
            }

            m_snapshots.Insert(index, snap);
        }
        public void ReplaceDataBefore(float time)
        {
            if (time == 0.0f) { return; }

            if (Count <= 0) { return; }
            TSnap t_snap = GetSnapshot(time);
            float t_earliestTime = GetSnapshotAtIndex(0).time;
            for (int i = Count - 1; i >= 0; --i)
            {
                TSnap t_snapAtIndex = GetSnapshotAtIndex(i);
                if (t_snapAtIndex.time <= time)
                {
                    RemoveSnapshotAtIndex(i);
                }
            }
            TSnap t_copiedForEarliestTime = t_snap.Clone(t_earliestTime);
            InsertSnapshotAtIndex(0, t_copiedForEarliestTime);
            InsertSnapshotAtIndex(1, t_snap);
        }
        public void Clear() => m_snapshots.Clear();



        /// <summary>
        /// Finds the index of the closest snapshot that is less than the given 
        /// time. 
        /// 
        /// Pre Conditons - Assumes <see cref="m_snapshots"/> is sorted in 
        /// increasing time order.
        /// Post Conditions - Returns the index of the snapshot with the nearest 
        /// time to the given time. If the list is empty, returns -1.
        /// </summary>
        private int FindClosestSnapshot(float time)
        {
            // Return -1 if there are no snapshots yet.
            if (m_snapshots.Count <= 0) { return -1; }

            // Use binary search to find the snapshot closest to the given time.
            int t_min = 0;
            int t_max = m_snapshots.Count - 1;
            int t_mid = -1;
            while (t_min <= t_max)
            {
                t_mid = (t_min + t_max) / 2;
                TSnap t_curVal = m_snapshots[t_mid];
                if (time < t_curVal.time)
                {
                    // Go left because list is sorted in DECREASING order.
                    t_max = t_mid - 1;
                }
                else if (time > t_curVal.time)
                {
                    // Go right because list is sorted in DECREASING order.
                    t_min = t_mid + 1;
                }
                else
                {
                    // Happens to be the exact time we are looking for
                    return t_mid;
                }
            }
            // Since we will most likely not find a snapshot at exactly the given
            // time, we use the last mid index to determine the closest snapshot.

            // It is possible the final snapshot is actually greater than the time
            // we want, but the index right below it should be the correct one
            // if that is the case.
            TSnap t_finalVal = m_snapshots[t_mid];
            // If the final time is greater, than we want the value to the left.
            // That value is then the closest value under the given time.
            if (t_finalVal.time > time)
            {
                return Mathf.Max(0, t_mid - 1);
            }
            // Final val is already lower.
            return t_mid;
        }


        public override string ToString()
        {
            string t_str = GetType().Name + " {\n";
            foreach (TSnap snap in m_snapshots)
            {
                t_str += $"({snap});\n";
            }
            t_str += "}";
            return t_str;
        }

        public ICollection<TSnap> GetInternalData() => m_snapshots;
    }
}
