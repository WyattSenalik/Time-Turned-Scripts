using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
// Original Authors - Wyatt Senalik (kinda, I borrowed most the code from
// https://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp)

namespace Helpers.Extensions
{
    /// <summary>
    /// Extensions to convert thins to byte arrays and back.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts a System.Object to a byte array. Try using <see cref="System.BitConverter"/> first.
        /// </summary>
        public static byte[] ToByteArray(this object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
        /// <summary>
        /// Converts a byte[] to a System.Object.
        /// Inteded for use for unpacking a System.Object that was converted to a byte array using <see cref="ByteArrayExtensions.ToByteArray"/>.
        /// Try using <see cref="System.BitConverter"/> first.
        /// </summary>
        public static object ToObject(this byte[] byteArr)
        {
            using MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(byteArr, 0, byteArr.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = binForm.Deserialize(memStream);
            return obj;
        }

        public static byte[] GetBytes(this string value)
        {
            byte[] t_rtnBytes = new byte[value.Length * 2 + 4];
            byte[] t_lengthBytes = BitConverter.GetBytes(value.Length);
            t_rtnBytes[0] = t_lengthBytes[0];
            t_rtnBytes[1] = t_lengthBytes[1];
            t_rtnBytes[2] = t_lengthBytes[2];
            t_rtnBytes[3] = t_lengthBytes[3];

            for (int i = 0; i < value.Length; ++i)
            {
                byte[] t_charBytes = BitConverter.GetBytes(value[i]);
                t_rtnBytes[4 + i * 2] = t_charBytes[0];
                t_rtnBytes[5 + i * 2] = t_charBytes[1];
            }

            return t_rtnBytes;
        }
        public static string ToString(this byte[] value, int startIndex)
        {
            int t_length = BitConverter.ToInt32(value, startIndex);
            StringBuilder t_stringBuilder = new StringBuilder(t_length);
            for (int i = 0; i < t_length; ++i)
            {
                t_stringBuilder.Append(BitConverter.ToChar(value, startIndex + 4 + i * 2));
            }
            return t_stringBuilder.ToString();
        }
        public static string ToString(this byte[] value)
        {
            return ToString(value, 0);
        }
    }
}
