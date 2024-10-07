using UnityEngine;

namespace Timed
{
    /// <summary>
    /// <see cref="TimedVariable{TSnap, TSnapData}"/> for tracking Vector2Ints.
    /// </summary>
    public sealed class TimedVector2Int : TimedVariable<Vector2IntSnapshot, Vector2Int>
    {
        public eInterpolationOption interpolationOption { get; private set; } = eInterpolationOption.Linear;

        public TimedVector2Int(Vector2Int initialData, eInterpolationOption interpolationOption, bool shouldNeverRecord = false) : base(shouldNeverRecord, false, false)
        {
            this.interpolationOption = interpolationOption;
            m_scrapbook.AddSnapshot(CreateSnapshot(initialData, curTime));
        }

        protected override Vector2IntSnapshot CreateSnapshot(Vector2Int data, float time)
        {
            return new Vector2IntSnapshot(data, time, interpolationOption);
        }
    }
}