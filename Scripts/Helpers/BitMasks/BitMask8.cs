// Original Authors - Wyatt Senalik

namespace Helpers.Bitmasks
{
    /// <summary>
    /// Bit mask using a <see cref="byte"/>.
    /// </summary>
    public struct BitMask8
    {
        public byte value => m_mask;

        private byte m_mask;


        public BitMask8(byte mask)
        {
            m_mask = mask;
        }


        public static bool operator ==(BitMask8 a, BitMask8 b) => a.value == b.value;
        public static bool operator !=(BitMask8 a, BitMask8 b) => a.value != b.value;
        public static implicit operator byte(BitMask8 mask) => mask.value;
        public static implicit operator BitMask8(byte byteVal) => new BitMask8(byteVal);

        public override string ToString() => m_mask.ToString();
        public override bool Equals(object other) => m_mask.Equals(other);
        public override int GetHashCode()
        {
            int hashCode = -630944503;
            hashCode = hashCode * -1755837320 + m_mask.GetHashCode();
            return hashCode;
        }
    }
}
