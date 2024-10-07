using UnityEngine;

namespace Timed
{
    /// <summary>
    /// <see cref="TimedVariable{TSnap, TSnapData}"/> for tracking Vector2.
    /// </summary>
    public sealed class TimedVector2 : TimedVariable<Vector2Snapshot, Vector2>
    {
        public eInterpolationOption interpolationOption { get; private set; } = eInterpolationOption.Linear;

        public TimedVector2(Vector2 initialData, bool shouldNeverRecord = false) : base(initialData, shouldNeverRecord) { }
        public TimedVector2(Vector2 initialData, eInterpolationOption interpolationOption, bool shouldNeverRecord = false) : base(shouldNeverRecord, false, false)
        {
            this.interpolationOption = interpolationOption;
            m_scrapbook.AddSnapshot(CreateSnapshot(initialData, curTime));
        }
        public TimedVector2(IReadOnlySnapshotScrapbook<Vector2Snapshot, Vector2> scrapbookToCopy, bool shouldNeverRecord = false) : base(scrapbookToCopy, shouldNeverRecord) { }
        public TimedVector2(TimedVector2 varToCopy, bool shouldNeverRecord = false) : this(varToCopy.scrapbook, shouldNeverRecord) { }

        protected override Vector2Snapshot CreateSnapshot(Vector2 data, float time)
        {
            return new Vector2Snapshot(time, data, interpolationOption);
        }
    }
}