// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// <see cref="TimedVariable{TSnap, TSnapData}"/> for tracking floats.
    /// </summary>
    public sealed class TimedFloat : TimedVariable<FloatSnapshot, float>
    {
        public eInterpolationOption interpolationOption { get; private set; } = eInterpolationOption.Linear;

        public TimedFloat(float initialData, bool shouldNeverRecord = false) : base(initialData, shouldNeverRecord) { }
        public TimedFloat(float initialData, eInterpolationOption interpolationOption, bool shouldNeverRecord = false) : base(shouldNeverRecord, false, false)
        {
            this.interpolationOption = interpolationOption;
            m_scrapbook.AddSnapshot(CreateSnapshot(initialData, curTime));
        }

        protected override FloatSnapshot CreateSnapshot(float data, float time)
        {
            return new FloatSnapshot(data, time, interpolationOption);
        }
    }
}