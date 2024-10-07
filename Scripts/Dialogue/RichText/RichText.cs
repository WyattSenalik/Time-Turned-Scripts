using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    public sealed class RichText : ICollection<RichSubText>, IEnumerable<RichSubText>, IList<RichSubText>, IReadOnlyCollection<RichSubText>, IReadOnlyList<RichSubText>, IList
    {
        private const bool IS_DEBUGGING = false;

        public int Count => m_subTexts.Count;
        public bool IsReadOnly => ((ICollection<RichSubText>)m_subTexts).IsReadOnly;
        public bool IsFixedSize => ((IList)m_subTexts).IsFixedSize;
        public bool IsSynchronized => ((ICollection)m_subTexts).IsSynchronized;
        public object SyncRoot => ((ICollection)m_subTexts).SyncRoot;
        public RichSubText this[int index]
        { 
            get => m_subTexts[index]; 
            set => m_subTexts[index] = value;
        }

        object IList.this[int index]
        {
            get => m_subTexts[index];
            set => m_subTexts[index] = (RichSubText)value;
        }

        private readonly List<RichSubText> m_subTexts = new List<RichSubText>();


        public RichText() { }
        public RichText(params RichSubText[] subTexts) 
        {
            m_subTexts.AddRange(subTexts);
        }


        public string GetLiteralText()
        {
            string t_rtnStr = "";
            foreach (RichSubText t_subText in m_subTexts)
            {
                t_rtnStr += t_subText.literalText;
            }
            return t_rtnStr;
        }
        public string GetFormattedText(params eRichTag[] ignoreTags)
        {
            string t_rtnStr = "";
            foreach (RichSubText t_subText in m_subTexts)
            {
                t_rtnStr += t_subText.GetFormattedText(ignoreTags);
            }
            return t_rtnStr;
        }
        /// <summary>
        /// Gets the formatted text up to the length specified.
        /// 
        /// Example: If the literal text is "Hello World!" with no formatting and length is 2, will return "He."
        /// Example: If the literal text is "Hello World!" where the "el" is blue and length is 4, will return "H<color=blue>el</color>l"
        /// </summary>
        public string GetFormattedText(int length, params eRichTag[] ignoreTags)
        {
            // Clamp length to be in bounds.
            int t_maxLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                t_maxLength += t_subText.literalText.Length;
            }
            #region Logs
            //CustomDebug.LogForObject($"Length ({length}) to be clamped (0, {t_maxLength})", this, IS_DEBUGGING);
            #endregion Logs
            length = Mathf.Clamp(length, 0, t_maxLength);

            // Start building the return string to the string.
            string t_rtnText = "";
            int t_remainingLength = length;
            foreach (RichSubText t_subText in m_subTexts)
            {
                // No length remaining, add nothing
                if (t_remainingLength <= 0)
                {
                    #region Logs
                    //CustomDebug.LogForObject($"No length remaining. Returning ({t_rtnText}).", this, IS_DEBUGGING);
                    #endregion Logs
                    return t_rtnText;
                }
                // Length exists beyond this subtext, so add the whole subtext.
                else if (t_subText.literalText.Length <= t_remainingLength)
                {
                    string t_formattedST = t_subText.GetFormattedText(ignoreTags);
                    #region Logs
                    //CustomDebug.LogForObject($"Adding ({t_formattedST}) to ({t_rtnText})", this, IS_DEBUGGING);
                    #endregion Logs
                    t_rtnText += t_formattedST;
                }
                // Length ends somewhere inside the next subtext
                else
                {
                    string t_formattedST = t_subText.GetFormattedText(t_remainingLength, ignoreTags);
                    #region Logs
                    //CustomDebug.LogForObject($"Adding ({t_formattedST}) to ({t_rtnText}). Will return shortly.", this, IS_DEBUGGING);
                    #endregion Logs
                    t_rtnText += t_formattedST;
                    return t_rtnText;
                }

                t_remainingLength -= t_subText.literalText.Length;
            }
            return t_rtnText;
        }
        public string GetFormattedTextSubstring(int startIndex, params eRichTag[] ignoreTags)
        {
            string t_literalText = GetLiteralText();
            // If the starting index is out of bounds, just return blank
            if (startIndex >= t_literalText.Length)
            {
                return "";
            }

            // Start building the return string to the string.
            string t_rtnText = "";
            int t_traversedLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                string t_literalSubText = t_subText.literalText;
                int t_literalSTLength = t_literalSubText.Length;

                // We've passed the starting index, add everything now.
                if (t_traversedLength >= startIndex)
                {
                    t_rtnText += t_subText.GetFormattedText(ignoreTags);
                }
                // The starting character is within the current rich sub text.
                else if (t_traversedLength < startIndex && startIndex < t_traversedLength + t_literalSTLength)
                {
                    t_rtnText += t_subText.GetFormattedTextSubstring(startIndex - t_traversedLength, ignoreTags);
                }

                t_traversedLength += t_literalSTLength;
            }
            return t_rtnText;
        }
        public string GetFormattedTextSubstring(int startIndex, int length, params eRichTag[] ignoreTags)
        {
            string t_literalText = GetLiteralText();
            // If the starting index is out of bounds, just return blank
            if (startIndex >= t_literalText.Length)
            {
                return "";
            }

            // Start building the return string to the string.
            string t_rtnText = "";
            int t_traversedLength = 0;
            int t_rtnTextLiteralLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                string t_literalSubText = t_subText.literalText;
                int t_literalSTLength = t_literalSubText.Length;

                // We've passed the starting index.
                if (t_traversedLength >= startIndex)
                {
                    int t_potentialNewLiteralLength = t_rtnTextLiteralLength + t_literalSTLength;
                    // If the rtn text has enough length now, we are done.
                    if (t_rtnTextLiteralLength >= length)
                    {
                        return t_rtnText;
                    }
                    // If the length of the rtn text (literal) after appending the sub text would be LESS than the desired length, just add the whole formatted text.
                    else if (t_potentialNewLiteralLength <= length)
                    {
                        t_rtnText += t_subText.GetFormattedText(ignoreTags);
                        t_rtnTextLiteralLength = t_potentialNewLiteralLength;
                    }
                    // If the length of the rtn text (literal) after appending the sub text would be GREATER than the desired length, add a sub string of the formatted text.
                    else //if (t_potentialNewLiteralLength > length)
                    {
                        int t_lengthToAdd = length - t_rtnTextLiteralLength;
                        t_rtnText += t_subText.GetFormattedTextSubstring(0, t_lengthToAdd, ignoreTags);
                        t_rtnTextLiteralLength += t_lengthToAdd;
                    }
                }
                // The starting character is within the current rich sub text.
                else if (startIndex < t_traversedLength + t_literalSTLength
                    /*&& t_traversedLength < startIndex Commented out because it is obvious from this being an else*/ )
                {
                    int t_subTextStartIndex = startIndex - t_traversedLength;
                    int t_subTextFullSubstringLength = t_literalSTLength - t_subTextStartIndex;
                    // If the sub text length (literal) (substring from the index we want) is LESS than the length we want, add the whole sub text.
                    if (t_subTextFullSubstringLength < length)
                    {
                        t_rtnText += t_subText.GetFormattedTextSubstring(t_subTextStartIndex, ignoreTags);
                        t_rtnTextLiteralLength += t_subTextFullSubstringLength;
                    }
                    // If the sub text length (literal) (substring from the index we want) is EQUAL to the length we want, return the whole subtext.
                    else if (t_subTextFullSubstringLength == length)
                    {
                        return t_subText.GetFormattedTextSubstring(t_subTextStartIndex, ignoreTags);
                    }
                    // If the sub text length (literal) (substring from the index we want) is GREATER than the length we want, get the appropriate substring and return it.
                    else // if (t_subTextFullSubstringLength > length)
                    {
                       return t_subText.GetFormattedTextSubstring(t_subTextStartIndex, length, ignoreTags);
                    }
                }

                t_traversedLength += t_literalSTLength;
            }
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(t_rtnTextLiteralLength <= length, $"Returning a substring with length ({t_rtnTextLiteralLength}) greater than the requested length ({length}).", this);
            #endregion Asserts
            return t_rtnText;
        }
        public float GetCurrentCharacterDelay(int length)
        {
            // Clamp length to be in bounds.
            int t_maxLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                t_maxLength += t_subText.literalText.Length;
            }
            length = Mathf.Clamp(length, 0, t_maxLength);

            int t_remainingLength = length;
            foreach (RichSubText t_subText in m_subTexts)
            {
                t_remainingLength -= t_subText.literalText.Length;
                // This is the current subtext
                if (t_remainingLength <= 0)
                {
                    return t_subText.characterDelay;
                }
            }
            #region Asserts
            CustomDebug.ThrowAssertionFail($"No character delay was found in rich text ({GetLiteralText()}) with length ({length}).", this);
            #endregion Asserts
            return -1.0f;
        }
        public RichSubText GetRichSubTextAtCharacterIndex(int index)
        {
            string t_literalText = GetLiteralText();
            // If the index is out of bounds, just return null
            if (0 > index || index >= t_literalText.Length )
            {
                #region Logs
                //CustomDebug.LogWarning($"Tried to retrieve a RichSubText from RichText ({GetFormattedText()}) with out of bounds index (index). Returning null instead.");
                #endregion Logs
                return null;
            }

            int t_traversedLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                string t_literalSubText = t_subText.literalText;
                int t_literalSTLength = t_literalSubText.Length;
                if (t_traversedLength <= index && index < t_traversedLength + t_literalSTLength)
                {
                    return t_subText;
                }
                t_traversedLength += t_literalSTLength;
            }
            #region Asserts
            CustomDebug.ThrowAssertionFail($"No RichSubText found in RichText ({GetFormattedText()}) (Literal Length={t_literalText.Length}) at index ({index}).", this);
            #endregion Asserts
            return null;
        }
        /// <summary>
        /// Adds a line break at the given character index.
        /// </summary>
        /// <returns>How many characters were added by adding the line break (might be 1 or 0 if a space was removed).</returns>
        public int InsertNewLine(int index)
        {
            int t_traversedLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                string t_subTextLiteral = t_subText.literalText;
                int t_subTextLength = t_subTextLiteral.Length;
                if (t_traversedLength + t_subTextLength > index)
                {
                    int t_indexInSubText = index - t_traversedLength;
                    string t_endPart = t_subTextLiteral.Substring(t_indexInSubText);
                    int t_amCharsAdded = 1;
                    if (t_endPart.Length > 0 && t_endPart[0] == ' ')
                    {
                        t_endPart = t_endPart.Substring(1);
                        --t_amCharsAdded;
                    }
                    // Insert the new line
                    t_subText.literalText = t_subTextLiteral.Substring(0, t_indexInSubText) + '\n' + t_endPart;
                    return t_amCharsAdded;
                }
                else
                {
                    t_traversedLength += t_subTextLength;
                }
            }
            #region Asserts
            CustomDebug.ThrowAssertionFail($"Insert new line did not insert a new line to rich text ({GetFormattedText()}) when given index ({index}).", this);
            #endregion Asserts
            return 0;
        }
        public int LiteralIndexToFormattedIndex(int literalIndex)
        {
            int t_traversedLiteralLength = 0;
            int t_traversedFormattedLength = 0;
            foreach (RichSubText t_subText in m_subTexts)
            {
                int t_stLiteralLength = t_subText.literalText.Length;
                string t_formattedText = t_subText.GetFormattedText();
                int t_stFormattedLength = t_formattedText.Length;

                // Index is inside this sub text.
                if (literalIndex < t_traversedLiteralLength + t_stLiteralLength)
                {
                    string t_opTags = t_subText.GetOpeningTags();
                    // To figure out the formatted index, we take the previous formatted index and add the opening tags to it. Then we need to add the difference of the traversed literal length and the literal index.
                    return t_stFormattedLength + t_opTags.Length + (literalIndex - t_stLiteralLength);
                }

                t_traversedLiteralLength += t_stLiteralLength;
                t_traversedFormattedLength += t_stFormattedLength;
            }
            return t_traversedFormattedLength - 1;
        }

        public void Add(RichSubText subText) => m_subTexts.Add(subText);
        public void Clear() => m_subTexts.Clear();
        public bool Contains(RichSubText subText) => m_subTexts.Contains(subText);
        public void CopyTo(RichSubText[] array, int arrayIndex) => m_subTexts.CopyTo(array, arrayIndex);
        public bool Remove(RichSubText subText) => m_subTexts.Remove(subText);
        public IEnumerator<RichSubText> GetEnumerator() => m_subTexts.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_subTexts.GetEnumerator();
        public int IndexOf(RichSubText item) => m_subTexts.IndexOf(item);
        public void Insert(int index, RichSubText item) => m_subTexts.Insert(index, item);
        public void RemoveAt(int index) => m_subTexts.RemoveAt(index);
        public int Add(object value) => ((IList)m_subTexts).Add(value);
        public bool Contains(object value) => Contains((RichSubText)value);
        public int IndexOf(object value) => IndexOf((RichSubText)value);
        public void Insert(int index, object value) => Insert(index, (RichSubText)value);
        public void Remove(object value) => Remove((RichSubText)value);
        public void CopyTo(Array array, int index) => CopyTo((RichSubText[])array, index);

        public override string ToString()
        {
            return GetFormattedText();
        }
    }
}