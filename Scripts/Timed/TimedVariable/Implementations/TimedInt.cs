// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// <see cref="TimedVariable{TSnap, TSnapData}"/> for tracking ints.
    /// </summary>
    public sealed class TimedInt : TimedVariable<IntSnapshot, int>
    {
        public eInterpolationOption interpolationOption { get; private set; } = eInterpolationOption.Linear;

        public TimedInt(int initialData, eInterpolationOption interpolationOption, bool shouldNeverRecord = false, bool shouldRecordEverySnap = false) : base(shouldNeverRecord, shouldRecordEverySnap, false)
        {
            this.interpolationOption = interpolationOption;
            m_scrapbook.AddSnapshot(CreateSnapshot(initialData, curTime));
        }
        public TimedInt(IReadOnlySnapshotScrapbook<IntSnapshot, int> scrapbookToCopy, bool shouldNeverRecord = false) : base(scrapbookToCopy, shouldNeverRecord) { }
        public TimedInt(IReadOnlySnapshotScrapbook<IntSnapshot, int> scrapbookToCopy, eInterpolationOption interpolationOption, bool shouldNeverRecord = false) : base(scrapbookToCopy, shouldNeverRecord)
        {
            this.interpolationOption = interpolationOption;
        }
        public TimedInt(TimedInt varToCopy, bool shouldNeverRecord = false) : this(varToCopy.scrapbook, shouldNeverRecord) { }

        protected override IntSnapshot CreateSnapshot(int data, float time)
        {
            return new IntSnapshot(data, time, interpolationOption);
        }
    }
}