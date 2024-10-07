// Original Authors - Wyatt Senalik

namespace Helpers.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// If the object is a number.
        /// </summary>
        public static bool IsNumber(this object value)
        {
            return value is byte
                    || value is sbyte
                    || value is decimal
                    || value is double
                    || value is float
                    || value is int
                    || value is uint
                    || value is nint
                    || value is nuint
                    || value is long
                    || value is ulong
                    || value is short
                    || value is ushort;
        }
    }
}