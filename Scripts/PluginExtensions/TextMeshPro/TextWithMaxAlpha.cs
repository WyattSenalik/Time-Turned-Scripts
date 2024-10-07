using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace TMPro.Extension
{
    /// <summary>
    /// Serializable TextMeshProUGUI with a max alpha. Useful for coroutines and animations that want to set the alpha of an already transparent image.
    /// <seealso cref="Helpers.UI.ImageWithMaxAlpha"/>.
    /// </summary>
    [Serializable]
    public sealed class TextWithMaxAlpha
    {
        public TextMeshProUGUI text => m_text;
        public float maxAlpha => m_maxAlpha;

        [SerializeField] private TextMeshProUGUI m_text = null;
        [SerializeField, Range(0.0f, 1.0f)] private float m_maxAlpha = 1.0f;
    }
}