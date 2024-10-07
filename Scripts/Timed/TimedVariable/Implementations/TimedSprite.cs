using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public sealed class TimedSprite : TimedVariable<SpriteSnapshot, SpriteData>
    {
        public TimedSprite(Sprite initialData, bool shouldNeverRecord = false, bool isDebugging = false) : base(new SpriteData(initialData), shouldNeverRecord)
        {
            m_isDebugging = isDebugging;
        }
        public TimedSprite(SpriteData initialData, bool shouldNeverRecord = false, bool isDebugging = false) : base(initialData, shouldNeverRecord)
        {
            m_isDebugging = isDebugging;
        }
        public TimedSprite(IReadOnlySnapshotScrapbook<SpriteSnapshot, SpriteData> scrapbookToCopy, bool shouldNeverRecord = false) : base(scrapbookToCopy, shouldNeverRecord) { }

        protected override SpriteSnapshot CreateSnapshot(SpriteData data, float time)
        {
            return new SpriteSnapshot(data, time, eInterpolationOption.Step);
        }
    }
}