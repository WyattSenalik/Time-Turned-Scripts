using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public static class TimedHelpers
    {
        /// <summary>
        /// Removes all times in the list that are after the given time.
        /// Assumes the list is sorted in ascending order.
        /// </summary>
        public static void RemoveTimesInListAfterTime(List<float> listOfTimes, float time, bool removeAtTime = false)
        {
            for (int i = listOfTimes.Count - 1; i >= 0; --i)
            {
                if (listOfTimes[i] > time)
                {
                    listOfTimes.RemoveAt(i);
                }
                else if (removeAtTime && listOfTimes[i] == time)
                {
                    listOfTimes.RemoveAt(i);
                }
            }
        }
    }
}