using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class Vector2Snapshot : ISnapshot<Vector2, Vector2Snapshot>
    {
        public float time => m_time;
        public Vector2 data => m_data;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;

        private readonly float m_time = -1.0f;
        private readonly Vector2 m_data = Vector2.negativeInfinity;


        public Vector2Snapshot(float snapTime, Vector2 snapData, eInterpolationOption interpolationOpt = eInterpolationOption.Linear)
        {
            m_time = snapTime;
            m_data = snapData;
            this.interpolationOpt = interpolationOpt;
        }

        public Vector2Snapshot Clone() => Clone(time);
        public Vector2Snapshot Clone(float timeForClone) => new Vector2Snapshot(timeForClone, data, interpolationOpt);
        public Vector2Snapshot Interpolate(Vector2Snapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<Vector2, Vector2Snapshot>(this, other, targetTime, out Vector2Snapshot t_earlierSnap, out Vector2Snapshot t_laterSnap, out eInterpolationOption t_intOpt);

            Vector2 t_interpolatedVector = Interpolate(t_earlierSnap.data, t_earlierSnap.time, t_laterSnap.data, t_laterSnap.time, t_intOpt, targetTime);

            return new Vector2Snapshot(targetTime, t_interpolatedVector, t_intOpt);
        }

        public override string ToString()
        {
            return $"data:{data}; time:{time}; interpolationOpt:{interpolationOpt}";
        }

        public static Vector2 Interpolate(Vector2 earlierVal, float earlierTime, Vector2 laterVal, float laterTime, eInterpolationOption intOpt, float targetTime)
        {
            // We are going to just let float snapshots handle this.
            float t_interpolatedXValue = FloatSnapshot.Interpolate(earlierVal.x, earlierTime, laterVal.x, laterTime, intOpt, targetTime);
            float t_interpolatedYValue = FloatSnapshot.Interpolate(earlierVal.y, earlierTime, laterVal.y, laterTime, intOpt, targetTime);

            return new Vector2(t_interpolatedXValue, t_interpolatedYValue);
        }
    }
}