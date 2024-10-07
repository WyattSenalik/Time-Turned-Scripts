using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class IntSnapshot : ISnapshot<int, IntSnapshot>
    {
        public int data { get; private set; } = 0;
        public float time { get; private set; } = float.NaN;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public IntSnapshot(int data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = data;
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }


        public IntSnapshot Clone() => Clone(time);
        public IntSnapshot Clone(float timeForClone) => new IntSnapshot(data, timeForClone, interpolationOpt);
        public IntSnapshot Interpolate(IntSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<int, IntSnapshot>(this, other, targetTime, out IntSnapshot t_earlierSnap, out IntSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            int t_lerpedData;
            switch (t_intOpt)
            {
                case eInterpolationOption.Linear: 
                {
                    float t = SnapshotHelpers.ConvertTimesToTValue(t_earlierSnap.time, t_laterSnap.time, targetTime);
                    t_lerpedData = Mathf.RoundToInt(Mathf.LerpUnclamped(t_earlierSnap.data, t_laterSnap.data, t));
                    break;
                }
                case eInterpolationOption.Step:
                {
                    t_lerpedData = SnapshotHelpers.GetAppropriateStepData<int, IntSnapshot>(t_earlierSnap, t_laterSnap, targetTime);
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(interpolationOpt, GetType().Name);
                    t_lerpedData = t_earlierSnap.data;
                    break;
                }
            }

            return new IntSnapshot(t_lerpedData, targetTime, t_intOpt);
        }

        public override string ToString()
        {
            return $"IntSnap: (time={time}, data={data})";
        }
    }
}