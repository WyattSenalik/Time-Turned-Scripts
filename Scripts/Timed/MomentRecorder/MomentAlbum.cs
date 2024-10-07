using System.Collections.Generic;

using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Default implementation of <see cref="IMomentAlbum{TMomentSelf}"/>.
    /// </summary>
    public sealed class MomentAlbum<TMomentSelf> : IMomentAlbum<TMomentSelf>
        where TMomentSelf : IMoment<TMomentSelf>
    {
        private const bool IS_DEBUGGING = false;

        public int Count => m_moments.Count;


        private readonly List<TMomentSelf> m_moments = null;


        public MomentAlbum()
        {
            m_moments = new List<TMomentSelf>();
        }
        public MomentAlbum(params TMomentSelf[] existingMoments)
        {
            m_moments = new List<TMomentSelf>(existingMoments);
        }


        public void AddMoment(TMomentSelf moment)
        {
            // First moment, just add it.
            if (m_moments.Count <= 0)
            {
                m_moments.Add(moment);
                //CustomDebug.LogForContainerElements($"", m_moments, IS_DEBUGGING);
                return;
            }
            int t_index = BinarySearch(moment.time);
            if (t_index < 0)
            {
                // If the index is less than 0, then it is the bitwise complement
                // of the returned value is the insertion index.
                m_moments.Insert(~t_index, moment);
            }
            else
            {
                // Another moment exists at the same time.
                m_moments.Insert(t_index, moment);
            }
        }
        public TMomentSelf[] Get(float lowerTime, float upperTime)
        {
            if (m_moments.Count <= 0) { return new TMomentSelf[0]; }
            List<TMomentSelf> t_rtnList = new List<TMomentSelf>();

            int t_lowerIndex = BinarySearch(lowerTime);
            if (t_lowerIndex < 0) { t_lowerIndex = ~t_lowerIndex; }
            int t_upperIndex = BinarySearch(upperTime);
            if (t_upperIndex < 0) { t_upperIndex = ~t_upperIndex; }

            t_lowerIndex = Mathf.Clamp(t_lowerIndex, 0, m_moments.Count - 1);
            t_upperIndex = Mathf.Clamp(t_upperIndex, t_lowerIndex + 1, m_moments.Count);
            for (int i = t_lowerIndex; i < t_upperIndex; ++i)
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(i, m_moments, GetType().Name);
                #endregion Asserts
                TMomentSelf t_mom = m_moments[i];
                if (lowerTime <= t_mom.time && 
                    (lowerTime == upperTime || t_mom.time < upperTime))
                {
                    t_rtnList.Add(t_mom);
                }              
            }
            return t_rtnList.ToArray();
        }
        public float GetLatestTime()
        {
            if (m_moments.Count <= 0) { return -1.0f; }
            return m_moments[m_moments.Count - 1].time;
        }
        public int RemoveMomentsAfter(float time)
        {
            // Can't remove any if there are none.
            if (Count <= 0) { return 0; }

            int t_removeCount = 0;
            int t_curIndex = 0;
            while (t_curIndex < m_moments.Count)
            {
                TMomentSelf t_mom = m_moments[t_curIndex];
                if (t_mom.time > time)
                {
                    m_moments.RemoveAt(t_curIndex);
                    ++t_removeCount;
                }
                else
                {
                    ++t_curIndex;
                }
            }
            return t_removeCount;
        }
        public TMomentSelf[] GetInternalData() => m_moments.ToArray();
        public TMomentSelf GetLatestMoment()
        {
            if (m_moments.Count <= 0)
            {
                return default;
            }
            return m_moments[^1];
        }
        public TMomentSelf GetMomentBeforeOrAtTime(float time)
        {
            if (m_moments.Count <= 0)
            {
                return default;
            }

            int t_index = BinarySearch(time);
            if (t_index < 0)
            {
                if (t_index == -1)
                {
                    t_index = 0;
                }
                else
                {
                    t_index = (~t_index) - 1;
                }
            }
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(t_index, m_moments, this);
            #endregion Asserts
            return m_moments[t_index];
        }

        /// <summary>
        /// Returns the index of the moment with the given time.
        /// If no moment has the given time, instead returns the bitwise
        /// complement of where a moment with the given time should be inserted.
        /// </summary>
        public int BinarySearch(float time)
        {
            //CustomDebug.Log($"Searching for {time}", IS_DEBUGGING);
            if (Count <= 0) { return -1; }
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(0, m_moments, GetType().Name);
            #endregion Asserts
            int t_min = 0;
            int t_max = m_moments.Count - 1;
            while (t_min <= t_max)
            {
                int t_mid = (t_min + t_max) / 2;
                TMomentSelf t_moment = m_moments[t_mid];
                if (time < t_moment.time)
                {
                    t_max = t_mid - 1;
                    //CustomDebug.Log($"{time} < {t_moment.time}. New max={t_max}",IS_DEBUGGING);
                }
                else if (time > t_moment.time)
                {
                    t_min = t_mid + 1;
                    //CustomDebug.Log($"{time} > {t_moment.time}. New min={t_min}", IS_DEBUGGING);
                }
                else // if (time == t_moment.time)
                {
                    //CustomDebug.Log($"{time} = {t_moment.time}. returning {t_min}", IS_DEBUGGING);
                    return t_mid;
                }
            }
            // Most of the returns below (except 0 are meant to be ~).
            // Since we exited the loop, that means min and max
            // are flipped, flip them back.
            (t_min, t_max) = (t_max, t_min);
            // Clamp them to make sure they are in bounds.
            t_min = Mathf.Clamp(t_min, 0, m_moments.Count);
            t_max = Mathf.Clamp(t_max, 0, m_moments.Count);
            // If they are now the same, return it.
            if (t_min == t_max)
            {
                //CustomDebug.Log($"({time}) Clamped between min and max {t_min}", IS_DEBUGGING);
                return ~t_min;
            }
            // Special cases:
            // 1. greater than the greatest
            if (m_moments[m_moments.Count - 1].time <= time)
            {
                //CustomDebug.Log($"({time}) Greater than greatest {m_moments.Count}", IS_DEBUGGING);
                return ~m_moments.Count;
            }
            // 2. less than the least
            if (m_moments[0].time >= time)
            {
                //CustomDebug.Log($"({time}) Less than least 0", IS_DEBUGGING);
                return 0;
            }
            // Check the indices 1 before and after min and max
            int t_minBefore = Mathf.Clamp(t_min - 1, 0, m_moments.Count);
            int t_maxAfter = Mathf.Clamp(t_max + 1, 0, m_moments.Count);
            // Belongs right before min-1
            if (time <= m_moments[t_minBefore].time)
            {
                //CustomDebug.Log($"({time}) Belong right before min-1 ({t_minBefore})", IS_DEBUGGING);
                return ~t_minBefore;
            }
            // Belongs right before min
            if (time <= m_moments[t_min].time)
            {
                //CustomDebug.Log($"({time}) Belong right before min ({t_min})", IS_DEBUGGING);
                return ~t_min;
            }
            // Belongs right before max
            if (time <= m_moments[t_max].time)
            {
                //CustomDebug.Log($"({time}) Belong right before max ({t_max})", IS_DEBUGGING);
                return ~t_max;
            }
            // Belongs right before max+1
            if (time <= m_moments[t_maxAfter].time)
            {
                //CustomDebug.Log($"({time}) Belong right before max+1 ({t_maxAfter})", IS_DEBUGGING);
                return ~t_maxAfter;
            }

            // ?
            //CustomDebug.LogError($"Should not reach here");
            return int.MinValue;
        }
        private bool IsInBounds(int index)
        {
            return index >= 0 && index < m_moments.Count;
        }
    }
}