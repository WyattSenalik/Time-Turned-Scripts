// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class TimedObjectReference<T> : TimedVariable<ObjectReferenceSnapshot<T>, ObjectReferenceData<T>>
        where T : class
    {
        public TimedObjectReference(T initialData, bool shouldNeverRecord = false, bool isDebugging = false) : base(new ObjectReferenceData<T>(initialData), shouldNeverRecord)
        {
            m_isDebugging = isDebugging;
        }
        public TimedObjectReference(ObjectReferenceData<T> initialData, bool shouldNeverRecord = false, bool isDebugging = false) : base(initialData, shouldNeverRecord)
        {
            m_isDebugging = isDebugging;
        }
        public TimedObjectReference(IReadOnlySnapshotScrapbook<ObjectReferenceSnapshot<T>, ObjectReferenceData<T>> scrapbookToCopy, bool shouldNeverRecord = false) : base(scrapbookToCopy, shouldNeverRecord) { }

        protected override ObjectReferenceSnapshot<T> CreateSnapshot(ObjectReferenceData<T> data, float time)
        {
            return new ObjectReferenceSnapshot<T>(data, time, eInterpolationOption.Step);
        }
    }
}