using System;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Atma.Settings;
// Original Authors - Wyatt Senalik

namespace Atma.Translation
{
    [CreateAssetMenu(fileName = "new FontsByLanguage", menuName = "ScriptableObjects/Translation/FontsByLanguage")]
    public sealed class FontsByLanguage : ScriptableObject
    {
        public TMP_FontAsset defaultFont => m_defaultFont;
        public IReadOnlyCollection<FontLanguage> fontLanguagePairs => m_fontLanguagePairs;

        [SerializeField] private TMP_FontAsset m_defaultFont = null;
        [SerializeField] private FontLanguage[] m_fontLanguagePairs = null;


        [Serializable]
        public sealed class FontLanguage
        {
            public eLanguage language => m_language;
            public TMP_FontAsset font => m_font;

            [SerializeField] private eLanguage m_language = eLanguage.EN;
            [SerializeField] private TMP_FontAsset m_font = null;
        }
    }
}