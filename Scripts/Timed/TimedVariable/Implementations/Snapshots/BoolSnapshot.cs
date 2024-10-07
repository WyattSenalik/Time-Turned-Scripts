// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class BoolSnapshot : ISnapshot<bool, BoolSnapshot>
    {
        public bool data { get; private set; } = false;
        public float time { get; private set; } = float.NaN;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public BoolSnapshot(bool data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = data;
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }


        public BoolSnapshot Clone() => Clone(time);
        public BoolSnapshot Clone(float timeForClone) => new BoolSnapshot(data, timeForClone, interpolationOpt);
        public BoolSnapshot Interpolate(BoolSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<bool, BoolSnapshot>(this, other, targetTime, out BoolSnapshot t_earlierSnap, out BoolSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            // Interpolation option is meaningless to bools. Its always step.

            bool t_lerpedData = SnapshotHelpers.GetAppropriateStepData<bool, BoolSnapshot>(t_earlierSnap, t_laterSnap, targetTime);
            return new BoolSnapshot(t_lerpedData, targetTime, t_intOpt);
        }
    }
}