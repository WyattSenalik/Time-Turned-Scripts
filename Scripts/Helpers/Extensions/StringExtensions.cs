using System.Collections.Generic;

namespace Helpers.Extensions
{
    public static class StringExtensions
    {
        public static List<int> FindAllIndicesOf(this string str, char value)
        {
            List<int> t_foundIndices = new List<int>();
            for (int i = str.IndexOf(value); i >= 0; i = str.IndexOf(value, i + 1))
            {
                // For loop end when i=-1 ('a' not found)
                t_foundIndices.Add(i);
            }
            return t_foundIndices;
        }
        public static List<int> FindAllIndicesOf(this string str, string value)
        {
            List<int> t_foundIndices = new List<int>();
            for (int i = str.IndexOf(value); i >= 0; i = str.IndexOf(value, i + 1))
            {
                // For loop end when i=-1 ('a' not found)
                t_foundIndices.Add(i);
            }
            return t_foundIndices;
        }
    }
}