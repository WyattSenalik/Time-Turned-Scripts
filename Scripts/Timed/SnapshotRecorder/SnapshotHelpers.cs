using System;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public static class SnapshotHelpers
    {
        /// <summary>
        /// Figure out an interpolation percentage (t) for the target time given the known times.
        /// 
        /// If target time is greater than both time0 and time1, a value greater than 1 will be returned. If it is lower than both time0 and time1, a value less than 0 will be returned.
        /// </summary>
        public static float ConvertTimesToTValue(float time0, float time1, float targetTime)
        {
            // Determine which time is the earlier one and which is the later.
            float t_earlierTime;
            float t_laterTime;
            if (time0 < time1)
            {
                t_earlierTime = time0;
                t_laterTime = time1;
            }
            else
            {
                t_earlierTime = time1;
                t_laterTime = time0;
            }

            // Interpolation percent. Commonly referred to as t.
            // For interpolation, this value should be in the range [0, 1].
            // For extrapolation, this value should be within the ranges
            // (-infinity, 0) U (1, infinity).
            float t_denom = t_laterTime - t_earlierTime;
            float t = 0;
            if (t_denom != 0)
            {
                t = (targetTime - t_earlierTime) / t_denom;
            }

            return t;
        }

        public static void GetEarlierAndLaterTimes(float time0, float time1, out float earlierTime, out float laterTime)
        {
            if (time0 <= time1)
            {
                earlierTime = time0;
                laterTime = time1;
            }
            else
            {
                earlierTime = time1;
                laterTime = time0;
            }
        }
        public static void GetEarlierAndLaterSnaps<TData, TSnap>(TSnap snap0, TSnap snap1, out TSnap earlierSnap, out TSnap laterSnap)
            where TData : IEquatable<TData>
            where TSnap : ISnapshot<TData, TSnap>
        {
            if (snap0.time < snap1.time)
            {
                earlierSnap = snap0;
                laterSnap = snap1;
            }
            else
            {
                earlierSnap = snap1;
                laterSnap = snap0;
            }
        }
        public static void GatherInterpolationInfo<TData, TSnap>(TSnap snap0, TSnap snap1, float targetTime, out TSnap earlierSnap, out TSnap laterSnap, out eInterpolationOption interpolationOpt)
            where TData : IEquatable<TData>
            where TSnap : ISnapshot<TData, TSnap>
        {
            // Earlier/Later snap
            GetEarlierAndLaterSnaps<TData, TSnap>(snap0, snap1, out earlierSnap, out laterSnap);

            if (targetTime >= earlierSnap.time && targetTime < laterSnap.time)
            {
                interpolationOpt = earlierSnap.interpolationOpt;
            }
            else if (targetTime >= laterSnap.time)
            {
                if (earlierSnap.interpolationOpt == eInterpolationOption.Step)
                {
                    // Regardless of if the later interpolation is linear or step, we don't want to be linearly interpolating if the previous snapshot was a step (since it won'tm turn out well).
                    interpolationOpt = eInterpolationOption.Step;
                }
                else
                {
                    interpolationOpt = laterSnap.interpolationOpt;
                }
            }
            else
            {
                interpolationOpt = earlierSnap.interpolationOpt;
                #region Logs
                //CustomDebug.LogWarning($"TargetTime ({targetTime}) is before than both snaps ({earlierSnap}) and ({laterSnap})");
                #endregion Logs
            }
        }
        public static TData GetAppropriateStepData<TData, TSnap>(TSnap earlierSnap, TSnap laterSnap, float targetTime)
            where TData : IEquatable<TData>
            where TSnap : ISnapshot<TData, TSnap>
        {
            TimeFrame t_snapFrame = new TimeFrame(earlierSnap.time, laterSnap.time);
            if (t_snapFrame.ContainsTime(targetTime, eTimeFrameContainsOption.EndExclusive))
            {
                return earlierSnap.data;
            }
            else if (targetTime >= t_snapFrame.endTime)
            {
                return laterSnap.data;
            }
            else
            {
                #region Logs
                //CustomDebug.LogWarning($"Target time ({targetTime}) is before both snaps ({earlierSnap}) and ({laterSnap})");
                #endregion Logs
                return earlierSnap.data;
            }
        }
    }
}