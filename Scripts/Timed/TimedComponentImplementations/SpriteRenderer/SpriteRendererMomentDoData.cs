using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public sealed class SpriteRendererMomentDoData : IEquatable<SpriteRendererMomentDoData>
    {
        public bool enabled => m_enabled;
        public Sprite sprite => m_sprite;
        public bool flipX => m_flipX;
        public int sortingOrder => m_sortingOrder;

        private readonly bool m_enabled = false;
        private readonly Sprite m_sprite = null;
        private readonly bool m_flipX = false;
        private readonly int m_sortingOrder = 0;


        public SpriteRendererMomentDoData(SpriteRenderer sprRend)
        {
            m_enabled = sprRend.enabled;
            m_sprite = sprRend.sprite;
            m_flipX = sprRend.flipX;
            m_sortingOrder = sprRend.sortingOrder;
        }
        public SpriteRendererMomentDoData(bool enabled, Sprite sprite, bool flipX, int sortingOrder)
        {
            m_enabled = enabled;
            m_sprite = sprite;
            m_flipX = flipX;
            m_sortingOrder = sortingOrder;
        }

        public bool Equals(SpriteRendererMomentDoData other)
        {
            if (other == null)
            {
                return false;
            }
            // Special case: If both versions are off, they are equal since they aren't visible anyway.
            if (!enabled && !other.enabled)
            {
                return true;
            }

            return enabled == other.enabled && 
                sprite == other.sprite && 
                flipX == other.flipX && 
                sortingOrder == other.sortingOrder;
        }
    }
}