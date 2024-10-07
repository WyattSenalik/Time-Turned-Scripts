using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    /// <summary>
    /// Controls the checking out and returning of numerical IDs.
    /// </summary>
    public sealed class IDLibrary
    {
        private const int STARTING_VALUE = 0;

        private readonly List<int> m_availableIDs = null;
        private int m_numbersUsed = 0;
        //private bool m_isDirty = true;
        //private bool m_areAllIDsReturned = true;


        public IDLibrary(int capacity = 4)
        {
            m_availableIDs = new List<int>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                m_availableIDs.Add(STARTING_VALUE + i);
            }
            m_numbersUsed = capacity;
        }


        /// <summary>
        /// Finds an available ID and returns it.
        /// If no IDs are available, creates a new ID and returns that.
        /// </summary>
        public int CheckoutID()
        {
            if (m_availableIDs.Count > 0)
            {
                int t_lastIndex = m_availableIDs.Count - 1;
                int t_rtnID = m_availableIDs[t_lastIndex];
                m_availableIDs.RemoveAt(t_lastIndex);
                return t_rtnID;
            }
            // Not enough availableIDs, create a new one
            int t_newID = STARTING_VALUE + m_numbersUsed;
            ++m_numbersUsed;

            //m_isDirty = true;

            return t_newID;
        }
        /// <summary>
        /// Returns a checked-out id back into the library so that
        /// it can be checked out again at a later time.
        /// </summary>
        public void ReturnID(int id)
        {
            if (id < STARTING_VALUE || id >= STARTING_VALUE + m_numbersUsed)
            {
                #region Logs
                //CustomDebug.LogWarning($"Invalid id ({id}) returned to library. This library only has ids in range [{m_startingValue}, {m_numbersUsed - 1}].");
                #endregion Logs
                return;
            }

            int t_foundIndex = m_availableIDs.BinarySearch(id);
            // The index is not currently checked out, do nothing
            if (t_foundIndex >= 0) { return; }
            // Index was not found, so use the found index to insert the
            // id into the correct place in the list.
            //
            // The foundIndex is the bitwise complement of the next id
            // larger than the id we want to insert.
            int t_insertIndex = ~t_foundIndex;
            m_availableIDs.Insert(t_insertIndex, id);

            //m_isDirty = true;
        }
        /// <summary>
        /// If all IDs that have been created by this library are not currently
        /// checked-out.
        /// </summary>
        public bool AreAllIDsReturned()
        {
            return m_availableIDs.Count == m_numbersUsed;
        }
    }
}
