// Original Authors - Wyatt Senalik

namespace Helpers.Transforms
{
    // Position = 00000001, Rotation = 00000010, Scale = 00000100,
    // Parent = 00001000, SiblingIndex = 00010000
    public enum eTransformChanges { Position = 1, Rotation = 2, Scale = 4,
        Parent = 8, SiblingIndex = 16 }

    /// <summary>
    /// Bit mask for holding what kind of changes were applied.
    /// <see cref="eTransformChanges"/>.
    /// </summary>
    public struct TransformChangesBitMask
    {
        public int value
        {
            get => m_mask;
            set => m_mask = value;
        }

        private int m_mask;


        public TransformChangesBitMask(int mask = 0)
        {
            m_mask = mask;
        }
        public TransformChangesBitMask(params eTransformChanges[] changes)
        {
            m_mask = 0;
            foreach (eTransformChanges c in changes)
            {
                m_mask |= (int)c;
            }
        }


        public static bool operator ==(TransformChangesBitMask a,
            TransformChangesBitMask b)
        {
            return a.value == b.value;
        }
        public static bool operator !=(TransformChangesBitMask a,
            TransformChangesBitMask b)
        {
            return a.value != b.value;
        }
        public static implicit operator int(TransformChangesBitMask mask)
        {
            return mask.value;
        }
        public static implicit operator TransformChangesBitMask(int byteVal)
        {
            return new TransformChangesBitMask(byteVal);
        }

        public bool IsChangeTo(eTransformChanges transChange)
        {
            int changeInt = (int)transChange;
            return (m_mask & changeInt) == changeInt;
        }
        public void Add(eTransformChanges transChange)
        {
            m_mask |= (int)transChange;
        }
        public void Remove(eTransformChanges transChange)
        {
            m_mask &= ~(int)transChange;
        }

        public override string ToString()
        {
            return m_mask.ToString();
        }
        public override bool Equals(object obj)
        {
            return obj is TransformChangesBitMask mask &&
                   value == mask.value;
        }
        public override int GetHashCode()
        {
            int hashCode = 765153635;
            hashCode = hashCode * -1521134295 + value.GetHashCode();
            return hashCode;
        }
    }
}
