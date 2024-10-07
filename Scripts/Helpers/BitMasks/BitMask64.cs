// Original Authors - Wyatt Senalik

namespace Helpers.Bitmasks
{
    /// <summary>
    /// Bit mask using a <see cref="ulong"/>.
    /// </summary>
    public struct BitMask64
    {
        public ulong value => m_mask;

        private ulong m_mask;


        public BitMask64(ulong mask)
        {
            m_mask = mask;
        }


        public static bool operator ==(BitMask64 a, BitMask64 b) => a.value == b.value;
        public static bool operator !=(BitMask64 a, BitMask64 b) => a.value != b.value;
        public static implicit operator ulong(BitMask64 mask) => mask.value;
        public static implicit operator BitMask64(ulong longVal) => new BitMask64(longVal);

        public override string ToString() => m_mask.ToString();
        public override bool Equals(object other) => m_mask.Equals(other);
        public override int GetHashCode()
        {
            int hashCode = 796925605;
            hashCode = hashCode * -246396502 + m_mask.GetHashCode();
            return hashCode;
        }
    }
}
