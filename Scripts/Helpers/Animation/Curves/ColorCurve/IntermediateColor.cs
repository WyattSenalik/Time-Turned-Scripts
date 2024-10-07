using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation.BetterCurve
{
    /// <summary>
    /// Intermediate color for <see cref="ColorCurve"/>.
    /// </summary>
    [Serializable]
    public struct IntermediateColor
    {
        public Color color => m_color;
        public float value => m_value;

        [SerializeField] private Color m_color;
        [SerializeField, AllowNesting, Range(0.0f, 1.0f)] private float m_value;

        public IntermediateColor(Color color, float value)
        {
            m_color = color;
            m_value = value;
        }
    }
}