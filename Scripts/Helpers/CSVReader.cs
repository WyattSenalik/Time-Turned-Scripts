// Original Authors - Wyatt Senalik

namespace Helpers
{
    public static class CSVReader
    {
        public static string QueryTable(string csvFileContent, string leftKey, string topKey)
        {
            // Search for the key in the first column (index 0 of every row).
            string[] t_rows = csvFileContent.Split('\n');

            // Search the 1st row for the horizontal key
            int t_horiKeyIndex = -1;
            string[] t_firstRowElements = t_rows[0].Split(',');
            for (int i = 0; i < t_firstRowElements.Length; ++i)
            {
                string t_keyEle = t_firstRowElements[i].Trim('\r');
                if (t_keyEle.Equals(topKey))
                {
                    t_horiKeyIndex = i;
                    break;
                }
            }
            if (t_horiKeyIndex == -1)
            {
                CustomDebug.LogError($"Key ({topKey}) not found in the 1st row of the table");
                return "";
            }

            // First row is column names, we can skip that.
            for (int i = 1; i < t_rows.Length; ++i)
            {
                string[] t_curRowElements = t_rows[i].Split(',');
                string t_keyAtStartOfRow = t_curRowElements[0].Trim('\r');
                // If the 1st element in the row is the key, we've found the right one.
                if (t_keyAtStartOfRow.Equals(leftKey))
                {
                    if (t_horiKeyIndex < 0 || t_horiKeyIndex >= t_curRowElements.Length)
                    {
                        CustomDebug.LogError($"HoriKeyIndex ({t_horiKeyIndex}) is out of bounds of the curRowElements ({t_curRowElements.Length}). Full row is ({t_rows[i]})");
                    }
                    return t_curRowElements[t_horiKeyIndex].Trim('\r').Replace('|', '\n');
                }
            }
            CustomDebug.LogError($"Key ({leftKey}) not found in the 1st column of the table");
            return "";
        }
    }
}