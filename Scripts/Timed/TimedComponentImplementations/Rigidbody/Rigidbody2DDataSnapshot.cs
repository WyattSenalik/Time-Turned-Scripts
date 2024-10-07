using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public class Rigidbody2DDataSnapshot : ISnapshot<Rigidbody2DData, Rigidbody2DDataSnapshot>
    {
        public float time { get; private set; }
        public Rigidbody2DData data { get; private set; }
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public Rigidbody2DDataSnapshot(float occurTime, Rigidbody2DData rbData, eInterpolationOption interpolationOpt)
        {
            time = occurTime;
            data = rbData;
            this.interpolationOpt = interpolationOpt;

            if (data == null)
            {
                //CustomDebug.LogError($"RB2DDataSnap was given null data");
            }
        }
        public Rigidbody2DDataSnapshot(float occurTime, Vector2 velocity, eInterpolationOption interpolationOpt = eInterpolationOption.Linear)
        {
            time = occurTime;
            data = new Rigidbody2DData(velocity);
            this.interpolationOpt = interpolationOpt;
        }


        public Rigidbody2DDataSnapshot Interpolate(Rigidbody2DDataSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<Rigidbody2DData, Rigidbody2DDataSnapshot>(this, other, targetTime, out Rigidbody2DDataSnapshot t_earlierSnap, out Rigidbody2DDataSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            // Utilize Vector2Snapshots to make interpolation easier.
            Vector2Snapshot t_earlierVelSnap = new Vector2Snapshot(t_earlierSnap.time, t_earlierSnap.data.velocity, t_earlierSnap.interpolationOpt);
            Vector2Snapshot t_laterVelSnap = new Vector2Snapshot(t_laterSnap.time, t_laterSnap.data.velocity, t_laterSnap.interpolationOpt);
            Vector2Snapshot t_interpolatedVelSnap = t_earlierVelSnap.Interpolate(t_laterVelSnap, targetTime);

            Rigidbody2DData t_interpolatedData = new Rigidbody2DData(t_interpolatedVelSnap.data);
            return new Rigidbody2DDataSnapshot(targetTime, t_interpolatedData, t_intOpt);
        }
        public Rigidbody2DDataSnapshot Clone() => new Rigidbody2DDataSnapshot(time, data, interpolationOpt);
        public Rigidbody2DDataSnapshot Clone(float timeForClone) => new Rigidbody2DDataSnapshot(timeForClone, data, interpolationOpt);


        public override string ToString()
        {
            if (data == null)
            {
                //CustomDebug.LogError($"RB2DDataSnap's data is null");
                return "";
            }

            return data.ToString() + $" ({nameof(time)}={time})";
        }
    }
}