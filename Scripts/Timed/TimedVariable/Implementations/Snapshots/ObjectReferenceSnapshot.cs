// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class ObjectReferenceSnapshot<T> : ISnapshot<ObjectReferenceData<T>, ObjectReferenceSnapshot<T>>
        where T : class
    {
        public ObjectReferenceData<T> data { get; private set; } = null;
        public float time { get; private set; } = float.NaN;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Step;


        public ObjectReferenceSnapshot(T data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = new ObjectReferenceData<T>(data);
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }
        public ObjectReferenceSnapshot(ObjectReferenceData<T> data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = data;
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }


        public ObjectReferenceSnapshot<T> Clone() => Clone(time);
        public ObjectReferenceSnapshot<T> Clone(float timeForClone) => new ObjectReferenceSnapshot<T>(data, timeForClone, interpolationOpt);
        public ObjectReferenceSnapshot<T> Interpolate(ObjectReferenceSnapshot<T> other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<ObjectReferenceData<T>, ObjectReferenceSnapshot<T>>(this, other, targetTime, out ObjectReferenceSnapshot<T> t_earlierSnap, out ObjectReferenceSnapshot<T> t_laterSnap, out eInterpolationOption t_intOpt);

            // Interpolation option is meaningless to ObjectReference. Its always step.
            ObjectReferenceData<T> t_lerpedData = SnapshotHelpers.GetAppropriateStepData<ObjectReferenceData<T>, ObjectReferenceSnapshot<T>>(t_earlierSnap, t_laterSnap, targetTime);
            return new ObjectReferenceSnapshot<T>(t_lerpedData, targetTime, t_intOpt);
        }
    }
}