// Original Authors - Wyatt Senalik

namespace Helpers.Bitmasks
{
    /// <summary>
    /// Bit mask using a <see cref="ushort"/>.
    /// </summary>
    public struct BitMask16
    {
        public ushort value => m_mask;

        private ushort m_mask;


        public BitMask16(ushort mask)
        {
            m_mask = mask;
        }


        public static bool operator ==(BitMask16 a, BitMask16 b) => a.value == b.value;
        public static bool operator !=(BitMask16 a, BitMask16 b) => a.value != b.value;
        public static implicit operator ushort(BitMask16 mask) => mask.value;
        public static implicit operator BitMask16(ushort shortVal) => new BitMask16(shortVal);

        public override string ToString() => m_mask.ToString();
        public override bool Equals(object other) => m_mask.Equals(other);
        public override int GetHashCode()
        {
            int hashCode = 1615617206;
            hashCode = hashCode * -472870453 + m_mask.GetHashCode();
            return hashCode;
        }
    }
}
