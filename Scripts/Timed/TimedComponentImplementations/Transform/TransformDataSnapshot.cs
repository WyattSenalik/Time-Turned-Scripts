using UnityEngine;

using Helpers.Transforms;
using Helpers.UnityEnums;
using Helpers.Math;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    /// <summary>
    /// <see cref="ISnapshot{T, TSelf}"/> for storing <see cref="TransformData"/>.
    /// </summary>
    [SerializeField]
    public sealed class TransformDataSnapshot : ISnapshot<TransformData, TransformDataSnapshot>
    {
        private const bool IS_DEBUGGING = false;

        public float time { get; private set;}
        public TransformData data { get; private set; }
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public TransformDataSnapshot(float time, TransformData data, eInterpolationOption interpolationOpt)
        {
            this.time = time;
            this.data = data;
            this.interpolationOpt = interpolationOpt;
        }
        public TransformDataSnapshot(float timeOfData, Transform transform, eRelativeSpace worldOrLocal, eInterpolationOption interpolationOpt):
            this(timeOfData, new TransformData(transform, worldOrLocal == eRelativeSpace.World), interpolationOpt)
        { }

        /// <summary>
        /// Linearly interpolates (Lerps) this snapshot with the given
        /// snapshot to more closesly match the given time.
        /// 
        /// For best results, the two snapshots should be adjacent to each other
        /// and the given time should be between the two snapshots.
        /// </summary>
        /// <param name="other">Snapshot to Lerp with.</param>
        /// <param name="targetTime">Target time in seconds that will be lerped to.
        /// This is NOT the same as t in Vector3.Lerp.</param>
        public TransformDataSnapshot Interpolate(TransformDataSnapshot other,
            float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<TransformData, TransformDataSnapshot>(this, other, targetTime, out TransformDataSnapshot t_earlierSnap, out TransformDataSnapshot t_laterSnap, out eInterpolationOption t_intOpt);
            float t = SnapshotHelpers.ConvertTimesToTValue(t_earlierSnap.time, t_laterSnap.time, targetTime);

            // Position
            // UtilizeVector2Snapshot to make it easier.
            Vector2 t_interpolatedPos = Vector2Snapshot.Interpolate(t_earlierSnap.data.position2D, t_earlierSnap.time, t_laterSnap.data.position2D, t_laterSnap.time, t_intOpt, targetTime);

            // Rotation
            float t_interpolatedRot;
            switch (t_intOpt)
            {
                case eInterpolationOption.Step:
                    t_interpolatedRot = t_earlierSnap.data.angle;
                    break;
                case eInterpolationOption.Linear:
                    t_interpolatedRot = MathHelpers.LerpAngleUnclamped(t_earlierSnap.data.angle, t_laterSnap.data.angle, t);
                    break;
                default:
                    t_interpolatedRot = t_earlierSnap.data.angle;
                    CustomDebug.UnhandledEnum(t_intOpt, nameof(TransformDataSnapshot));
                    break;
            }

            // Scale
            // UtilizeVector2Snapshot to make it easier.
            Vector2 t_interpolatedScale = Vector2Snapshot.Interpolate(t_earlierSnap.data.scale2D, t_earlierSnap.time, t_laterSnap.data.scale2D, t_laterSnap.time, t_intOpt, targetTime);

            TransformData t_interpolatedData = new TransformData(t_interpolatedPos, t_interpolatedRot, t_interpolatedScale);
            return new TransformDataSnapshot(targetTime, t_interpolatedData, t_intOpt);
        }
        public TransformDataSnapshot Clone() => Clone(time);
        public TransformDataSnapshot Clone(float timeForClone)
        {
            TransformData t_newData = new TransformData(data.position, data.angle, data.scale);
            return new TransformDataSnapshot(timeForClone, t_newData, interpolationOpt);
        }


        public override string ToString()
        {
            return $"time:{time}; data:{data}; interpolationOption:{interpolationOpt})";
        }
    }

    public static class TransformDataHelpers
    {
        public static SnapshotScrapbook<TransformDataSnapshot, TransformData> DouglasPeucker(IReadOnlySnapshotScrapbook<TransformDataSnapshot, TransformData> scrapbook, int startIndex, int length, float epsilon)
        {
            // Find the point with the maximum distance
            float t_maxDist = 0.0f;
            int t_indexOfMax = 0;
            int t_endIndex = startIndex + length;

            TransformDataSnapshot t_firstSnapshot = scrapbook.GetSnapshotAtIndex(0);
            for (int i = 1; i < t_endIndex; ++i)
            {
                TransformDataSnapshot t_snapshotAtIndex = scrapbook.GetSnapshotAtIndex(i);
                float t_dist = t_snapshotAtIndex.data.CalculateMaxDistance(t_firstSnapshot.data);
                if (t_dist > t_maxDist)
                {
                    t_maxDist = t_dist;
                    t_indexOfMax = i;
                }
            }

            SnapshotScrapbook<TransformDataSnapshot, TransformData> t_results = new SnapshotScrapbook<TransformDataSnapshot, TransformData>();
            // If max distance is greater than epsilon, recursively simply
            if (t_maxDist > epsilon)
            {
                // Recursive call
                ISnapshotScrapbook<TransformDataSnapshot, TransformData> t_results1 = DouglasPeucker(scrapbook, 0, t_indexOfMax, epsilon);
                ISnapshotScrapbook<TransformDataSnapshot, TransformData> t_results2 = DouglasPeucker(scrapbook, t_indexOfMax, t_endIndex, epsilon);

                // Build the result list
                foreach (TransformDataSnapshot t_snap1 in t_results1.GetInternalData())
                {
                    t_results.AddSnapshot(t_snap1);
                }
                foreach (TransformDataSnapshot t_snap2 in t_results2.GetInternalData())
                {
                    t_results.AddSnapshot(t_snap2);
                }
            }
            // Return the result
            return t_results;
        }
    }
}