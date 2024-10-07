using UnityEngine;

namespace Timed
{ 
    public sealed class Vector2IntSnapshot : ISnapshot<Vector2Int, Vector2IntSnapshot>
    {
        public Vector2Int data { get; private set; }
        public float time { get; private set; }
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public Vector2IntSnapshot(Vector2Int data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = data;
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }


        public Vector2IntSnapshot Clone() => Clone(time);
        public Vector2IntSnapshot Clone(float timeForClone) => new Vector2IntSnapshot(data, timeForClone, interpolationOpt);
        public Vector2IntSnapshot Interpolate(Vector2IntSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<Vector2Int, Vector2IntSnapshot>(this, other, targetTime, out Vector2IntSnapshot t_earlierSnap, out Vector2IntSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            Vector2Int t_lerpedData;
            switch (t_intOpt)
            {
                case eInterpolationOption.Linear:
                {
                    float t = SnapshotHelpers.ConvertTimesToTValue(t_earlierSnap.time, t_laterSnap.time, targetTime);
                    int t_lerpedX = Mathf.RoundToInt(Mathf.LerpUnclamped(t_earlierSnap.data.x, t_laterSnap.data.x, t));
                    int t_lerpedY = Mathf.RoundToInt(Mathf.LerpUnclamped(t_earlierSnap.data.y, t_laterSnap.data.y, t));
                    t_lerpedData = new Vector2Int(t_lerpedX, t_lerpedY);
                    break;
                }
                case eInterpolationOption.Step:
                {
                    t_lerpedData = SnapshotHelpers.GetAppropriateStepData<Vector2Int, Vector2IntSnapshot>(t_earlierSnap, t_laterSnap, targetTime);
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(interpolationOpt, GetType().Name);
                    t_lerpedData = t_earlierSnap.data;
                    break;
                }
            }

            return new Vector2IntSnapshot(t_lerpedData, targetTime, t_intOpt);
        }
    }
}