using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Text to be displayed that can appended and removed from easily without having to re-add Rich-Text properties.
    /// </summary>
    public sealed class RichSubText
    {
        public string literalText { get; set; } = "";
        public float characterDelay => m_characterDelay;
        public bool hasColor { get; private set; } = false;
        public Color color { get; private set; } = Color.white;
        public bool hasSize { get; private set; } = false;
        public float percentSize { get; private set; } = float.NaN;
        public bool hasVOffset { get; private set; } = false;
        public float vOffset { get; private set; } = 0.0f;
        public bool isBold { get; private set; } = false;
        public bool isItalic { get; private set; } = false;

        // Contains opening tag and closing tag
        private readonly List<(string, string, eRichTag)> m_tags = new List<(string, string, eRichTag)>();
        // If this text is being typed, the amount to delay after typing.
        private readonly float m_characterDelay = 0;


        public RichSubText() { }
        public RichSubText(string literalText, float characterDelay)
        {
            this.literalText = literalText;
            m_characterDelay = characterDelay;
        }

        public string GetFormattedText(params eRichTag[] ignoreTags)
        {
            // Get all opening tags.
            string t_openingTags = GetOpeningTags(ignoreTags);
            // Get all closing tags.
            string t_endingTags = GetClosingTags(ignoreTags);
            string t_formattedText = t_openingTags + literalText + t_endingTags;
            if (!ignoreTags.Contains(eRichTag.Color))
            {
                return HandleSpriteAtlasText(t_formattedText);
            }
            return t_formattedText;
        }
        /// <summary>
        /// Gets the formatted text up to the length specified.
        /// 
        /// Example: If the literal text is "Hello World!" with no formatting and length is 2, will return "He."
        /// Example: If the literal text is "Hello World!" where the "el" is blue and length is 4, will return "H<color=blue>el</color>l"
        /// </summary>
        public string GetFormattedText(int length, params eRichTag[] ignoreTags)
        {
            // Get all opening tags.
            string t_openingTags = GetOpeningTags(ignoreTags);
            // Get all closing tags.
            string t_endingTags = GetClosingTags(ignoreTags);
            // Get the middle bit (literal text). Clamp the length to not be out of bounds.
            length = Mathf.Clamp(length, 0, literalText.Length);
            string t_middleBit = literalText.Substring(0, length);

            string t_formattedText = t_openingTags + t_middleBit + t_endingTags;
            if (!ignoreTags.Contains(eRichTag.Color))
            {
                return HandleSpriteAtlasText(t_formattedText);
            }
            return t_formattedText;
        }
        public string GetFormattedTextSubstring(int startIndex, params eRichTag[] ignoreTags)
        {
            // Get all opening tags.
            string t_openingTags = GetOpeningTags(ignoreTags);
            // Get all closing tags.
            string t_endingTags = GetClosingTags(ignoreTags);
            // Get the middle bit (literal text).
            string t_middleBit = literalText.Substring(startIndex);

            string t_formattedText = t_openingTags + t_middleBit + t_endingTags;
            if (!ignoreTags.Contains(eRichTag.Color))
            {
                return HandleSpriteAtlasText(t_formattedText);
            }
            return t_formattedText;
        }
        public string GetFormattedTextSubstring(int startIndex, int length, params eRichTag[] ignoreTags)
        {
            // Get all opening tags.
            string t_openingTags = GetOpeningTags(ignoreTags);
            // Get all closing tags.
            string t_endingTags = GetClosingTags(ignoreTags);
            // Get the middle bit (literal text).
            string t_middleBit = literalText.Substring(startIndex, length);

            string t_formattedText = t_openingTags + t_middleBit + t_endingTags;
            if (!ignoreTags.Contains(eRichTag.Color))
            {
                return HandleSpriteAtlasText(t_formattedText);
            }
            return t_formattedText;
        }
        public string GetOpeningTags(params eRichTag[] ignoreTags)
        {
            string t_openingTags = "";
            for (int i = 0; i < m_tags.Count; ++i)
            {
                // Only add the tag if not ignoring it.
                if (!ignoreTags.Contains(m_tags[i].Item3))
                {
                    t_openingTags += m_tags[i].Item1;
                }
            }
            return t_openingTags;
        }
        public string GetClosingTags(params eRichTag[] ignoreTags)
        {
            string t_endingTags = "";
            for (int i = m_tags.Count - 1; i >= 0; --i)
            {
                // Only add the tag if not ignoring it.
                if (!ignoreTags.Contains(m_tags[i].Item3))
                {
                    t_endingTags += m_tags[i].Item2;
                }
            }
            return t_endingTags;
        }
        public void SetColorTag(Color color)
        {
            // Remove the previous color tag.
            if (hasColor)
            {
                RemoveTag(eRichTag.Color);
            }

            hasColor = true;
            this.color = color;
            AddTag(CreateColorTagOP(color), COLOR_TAG_ED, eRichTag.Color);
        }
        public void SetPercentSizeTag(float percentSize)
        {
            // Remove the previous size tag.
            if (hasSize)
            {
                RemoveTag(eRichTag.PercentSize);
            }

            hasSize = true;
            this.percentSize = percentSize;
            AddTag(CreateSizeTagOP(percentSize), SIZE_TAG_ED, eRichTag.PercentSize);
        }
        public void SetVerticalOffsetTag(float vOffset)
        {
            // Remove the previous vertical offset tag.
            if (hasVOffset)
            {
                RemoveTag(eRichTag.VerticalOffset);
            }

            hasVOffset = true;
            this.vOffset = vOffset;
            AddTag(CreateVerticalOffsetTagOP(vOffset), VERTICAL_OFFSET_TAG_ED, eRichTag.VerticalOffset);
        }
        public void SetBoldTag(bool bold)
        {
            // Already set appropriately
            if (bold == isBold) { return; }
            isBold = bold;
            if (isBold)
            {
                AddTag(BOLD_TAG_OP, BOLD_TAG_ED, eRichTag.Bold);
            }
            else
            {
                RemoveTag(eRichTag.Bold);
            }
        }
        public void SetItalicTag(bool italic)
        {
            // Already set appropriately
            if (italic == isItalic) { return; }
            isItalic = italic;
            if (isItalic)
            {
                AddTag(ITALIC_TAG_OP, ITALIC_TAG_ED, eRichTag.Italic);
            }
            else
            {
                RemoveTag(eRichTag.Italic);
            }
        }
        public void ClearAllTags()
        {
            hasColor = false;
            hasSize = false;
            hasVOffset = false;
            isBold = false;
            isItalic = false;
            m_tags.Clear();
        }


        private void AddTag(string openingTag, string closingTag, eRichTag tagType = eRichTag.Custom)
        {
            m_tags.Add((openingTag, closingTag, tagType));
        }
        private void RemoveTag(eRichTag tagTypeToRemove)
        {
            for (int i = 0; i < m_tags.Count; ++i)
            {
                if (m_tags[i].Item3 == tagTypeToRemove)
                {
                    m_tags.RemoveAt(i);
                    break;
                }
            }
        }


        private string HandleSpriteAtlasText(string text)
        {
            if (DoesStringContainsSpriteAtlasText(text, out List<(int, int)> t_startEndIndices))
            {
                // Reverse it so we edit the pairs later first and work backwards, otherwise updating the text would change the indices of the later pairs.
                t_startEndIndices.Reverse();
                foreach ((int t_startIndex, int t_endIndex) in t_startEndIndices)
                {
                    text = $"{text.Substring(0, t_endIndex)} color=\"#{ColorUtility.ToHtmlStringRGBA(color)}\">" + text.Substring(t_endIndex + 1);
                }
            }
            return text;
        }
        private bool DoesStringContainsSpriteAtlasText(string text, out List<(int startIndex, int endIndex)> startEndIndices)
        {
            List<int> t_openIndices = text.FindAllIndicesOf('<');
            List<int> t_closeIndices = text.FindAllIndicesOf('>');

            List<(int openIndex, int closeIndex)> t_pairs = new List<(int, int)>();

            for (int i = 0; i < t_openIndices.Count; ++i)
            {
                int t_curOpenIndex = t_openIndices[i];
                for (int k = 0; k < t_closeIndices.Count; ++k)
                {
                    int t_curCloseIndex = t_closeIndices[k];
                    // Current close is after current open, check to make sure there are no opens between the current open and the current close.
                    if (t_curCloseIndex > t_curOpenIndex)
                    {
                        if (i + 1 >= t_openIndices.Count)
                        {
                            // We are good, no opens between
                            t_pairs.Add((t_curOpenIndex, t_curCloseIndex));
                            break;
                        }
                        else if (t_openIndices[i + 1] > t_curCloseIndex)
                        {
                            // We are good, next open is after cur close
                            t_pairs.Add((t_curOpenIndex, t_curCloseIndex));
                            break;
                        }
                        else
                        {
                            // There is an open between these two, no good.
                            break;
                        }
                    }
                }
            }

            startEndIndices = new List<(int, int)>();
            for (int i = 0; i < t_pairs.Count; ++i)
            {
                (int t_openIndex, int t_closeIndex) = t_pairs[i];
                string t_tag = text.Substring(t_openIndex, t_closeIndex - t_openIndex);
                string t_lowercaseTag = t_tag.ToLower();
                if (t_lowercaseTag.Contains("name") && t_lowercaseTag.Contains("sprite"))
                {
                    startEndIndices.Add((t_openIndex, t_closeIndex));
                }
            }
            if (startEndIndices.Count > 0)
            {
                return true;
            }
            return false;
        }



        public const string COLOR_TAG_OP = COLOR_TAG_OP_BEGIN_PART + ".*" + COLOR_TAG_OP_END_PART;
        public const string COLOR_TAG_OP_BEGIN_PART = "<color=#";
        public const string COLOR_TAG_OP_END_PART = ">";
        public const string COLOR_TAG_ED = "</color>";
        public const string SIZE_TAG_OP = SIZE_TAG_OP_BEGIN_PART + ".*" + SIZE_TAG_OP_END_PART;
        public const string SIZE_TAG_OP_BEGIN_PART = "<size=";
        public const string SIZE_TAG_OP_END_PART = "%>";
        public const string SIZE_TAG_ED = "<size=100%>";
        public const string VERTICAL_OFFSET_TAG_OP = VERTICAL_OFFSET_TAG_OP_BEGIN_PART + "*" + VERTICAL_OFFSET_TAG_OP_END_PART;
        public const string VERTICAL_OFFSET_TAG_OP_BEGIN_PART = "<voffset=";
        public const string VERTICAL_OFFSET_TAG_OP_END_PART = "em>";
        public const string VERTICAL_OFFSET_TAG_ED = "</voffset>";
        public const string BOLD_TAG_OP = "<b>";
        public const string BOLD_TAG_ED = "</b>";
        public const string ITALIC_TAG_OP = "<i>";
        public const string ITALIC_TAG_ED = "</i>";
        public static string CreateColorTagOP(Color color) => COLOR_TAG_OP_BEGIN_PART + ColorUtility.ToHtmlStringRGBA(color) + COLOR_TAG_OP_END_PART;
        public static string CreateSizeTagOP(float percentSize) => SIZE_TAG_OP_BEGIN_PART + percentSize.ToString() + SIZE_TAG_OP_END_PART;
        public static string CreateVerticalOffsetTagOP(float vOffset) => VERTICAL_OFFSET_TAG_OP_BEGIN_PART + vOffset.ToString() + VERTICAL_OFFSET_TAG_OP_END_PART;
    }

    public enum eRichTag { Custom, Color, PercentSize, VerticalOffset, Bold, Italic }
}