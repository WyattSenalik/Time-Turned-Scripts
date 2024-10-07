// Original Authors - Wyatt Senalik

namespace Helpers.Bitmasks
{
    /// <summary>
    /// Bit mask using a <see cref="uint"/>.
    /// </summary>
    public struct BitMask32
    {
        public uint value => m_mask;

        private uint m_mask;


        public BitMask32(uint mask)
        {
            m_mask = mask;
        }


        public static bool operator ==(BitMask32 a, BitMask32 b) => a.value == b.value;
        public static bool operator !=(BitMask32 a, BitMask32 b) => a.value != b.value;
        public static implicit operator uint(BitMask32 mask) => mask.value;
        public static implicit operator BitMask32(uint intVal) => new BitMask32(intVal);

        public override string ToString() => m_mask.ToString();
        public override bool Equals(object other) => m_mask.Equals(other);
        public override int GetHashCode()
        {
            int hashCode = -899058285;
            hashCode = hashCode * 1994786750 + m_mask.GetHashCode();
            return hashCode;
        }
    }
}