using System;
using UnityEngine;
using UnityEngine.UI;
// Original Authors - Wyatt Senalik

namespace Helpers.UI
{
    /// <summary>
    /// Serializable image with a max alpha. Useful for coroutines and animations that want to set the alpha of an already transparent image.
    /// </summary>
    [Serializable]
    public sealed class ImageWithMaxAlpha
    {
        public Image img => m_img;
        public float maxAlpha => m_maxAlpha;

        [SerializeField] private Image m_img = null;
        [SerializeField, Range(0.0f, 1.0f)] private float m_maxAlpha = 1.0f;
    }
}