using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class SpriteSnapshot : ISnapshot<SpriteData, SpriteSnapshot>
    {
        public SpriteData data { get; private set; } = null;
        public float time { get; private set; } = float.NaN;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Step;


        public SpriteSnapshot(Sprite data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = new SpriteData(data);
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }
        public SpriteSnapshot(SpriteData data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = data;
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }


        public SpriteSnapshot Clone() => Clone(time);
        public SpriteSnapshot Clone(float timeForClone) => new SpriteSnapshot(data, timeForClone, interpolationOpt);
        public SpriteSnapshot Interpolate(SpriteSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<SpriteData, SpriteSnapshot>(this, other, targetTime, out SpriteSnapshot t_earlierSnap, out SpriteSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            // Interpolation option is meaningless to Sprites. Its always step.
            SpriteData t_lerpedData = SnapshotHelpers.GetAppropriateStepData<SpriteData, SpriteSnapshot>(t_earlierSnap, t_laterSnap, targetTime);
            return new SpriteSnapshot(t_lerpedData, targetTime, t_intOpt);
        }
    }
}