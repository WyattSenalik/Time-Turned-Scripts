// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// <see cref="TimedVariable{TSnap, TSnapData}"/> for tracking bools.
    /// </summary>
    public sealed class TimedBool : TimedVariable<BoolSnapshot, bool>
    {
        public TimedBool(bool initialData, bool shouldNeverRecord = false, bool isDebugging = false) : base(initialData, shouldNeverRecord) 
        {
            m_isDebugging = isDebugging;
        }
        public TimedBool(IReadOnlySnapshotScrapbook<BoolSnapshot, bool> scrapbookToCopy, bool shouldNeverRecord = false) : base(scrapbookToCopy, shouldNeverRecord) { }

        protected override BoolSnapshot CreateSnapshot(bool data, float time)
        {
            return new BoolSnapshot(data, time, eInterpolationOption.Step);
        }
    }
}