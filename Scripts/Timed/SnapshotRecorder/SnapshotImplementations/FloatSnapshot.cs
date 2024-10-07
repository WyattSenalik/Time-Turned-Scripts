using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class FloatSnapshot : ISnapshot<float, FloatSnapshot>
    {
        public float data { get; private set; } = float.NaN;
        public float time { get; private set; } = float.NaN;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public FloatSnapshot(float data, float time, eInterpolationOption interpolationOpt = eInterpolationOption.Linear)
        {
            this.data = data;
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }


        public FloatSnapshot Clone() => Clone(time);
        public FloatSnapshot Clone(float timeForClone)
        {
            return new FloatSnapshot(data, timeForClone, interpolationOpt);
        }
        public FloatSnapshot Interpolate(FloatSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<float, FloatSnapshot>(this, other, targetTime, out FloatSnapshot t_earlierSnap, out FloatSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            float t_interpolatedValue = Interpolate(t_earlierSnap.data, t_earlierSnap.time, t_laterSnap.data, t_laterSnap.time, t_intOpt, targetTime);
            return new FloatSnapshot(t_interpolatedValue, targetTime, t_intOpt);
        }

        public override string ToString()
        {
            return $"data:{data}; time:{time}; interpolationOpt:{interpolationOpt}";
        }

        public static float Interpolate(float earlierVal, float earlierTime, float laterVal, float laterTime, eInterpolationOption intOpt, float targetTime)
        {
            // Validate the data
            if (float.IsNaN(earlierVal))
            {
                if (float.IsNaN(laterVal))
                {
                    // Both snaps are NaN
                    return float.NaN;
                }
                else if (targetTime < laterTime)
                {
                    // Only 1st snap is NaN, and target time is between snaps, so interpolation should be NaN.
                    return float.NaN;
                }
                else
                {
                    // 1st snap is NaN, 2nd snap isn't, and the target time is after the 2nd snap, so can return real data from the later snap 
                    return laterVal;
                }
            }
            else if (float.IsNaN(laterVal))
            {
                if (targetTime < laterTime)
                {
                    // Target time is between snaps, but 2nd snap is NaN, so return real data from the earlier snap.
                    return earlierVal;
                }
                else
                {
                    // Target time is after 2nd snap (which is NaN), so return NaN.
                    return float.NaN;
                }
            }

            // Okay, so neither snap is NaN now, we can handle them normally.
            float t_lerpedData;
            switch (intOpt)
            {
                case eInterpolationOption.Linear:
                {
                    float t = SnapshotHelpers.ConvertTimesToTValue(earlierTime, laterTime, targetTime);
                    t_lerpedData = Mathf.LerpUnclamped(earlierVal, laterVal, t);
                    break;
                }
                case eInterpolationOption.Step:
                {
                    t_lerpedData = targetTime < laterTime ? earlierVal : laterVal;
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(intOpt, nameof(FloatSnapshot.Interpolate));
                    t_lerpedData = earlierVal;
                    break;
                }
            }
            return t_lerpedData;
        }
    }
}